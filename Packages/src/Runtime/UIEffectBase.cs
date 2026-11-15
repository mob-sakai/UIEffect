using System;
using System.Runtime.CompilerServices;
using Coffee.UIEffectInternal;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Profiling;
using UnityEngine.UI;
using UnityEditor;
#if TIMELINE_ENABLE
using UnityEngine.Timeline;
#endif

[assembly: InternalsVisibleTo("UIEffect")]
[assembly: InternalsVisibleTo("Coffee.UIEffect.Editor")]

namespace Coffee.UIEffects
{
    [ExecuteAlways]
    [DisallowMultipleComponent]
    public abstract class UIEffectBase : UIBehaviour, IMeshModifier, IMaterialModifier, ICanvasRaycastFilter
#if TIMELINE_ENABLE
        , ITimeControl
#endif
    {
        private static readonly InternalObjectPool<UIEffectContext> s_ContextPool =
            new InternalObjectPool<UIEffectContext>(() => new UIEffectContext(), x => true, x => x.Reset());

        private Graphic _graphic;
        private Material _material;
        private UIEffectContext _context;
        private Action _onBeforeCanvasRebuild;
        private Action _onAfterCanvasRebuild;
        private bool _canModifyMesh;
        private Matrix4x4 _prevTransformHash;
        private Canvas _canvas;
        private bool _canvasCached = false;
        private bool _isContextDirty;

        public Material effectMaterial => _material;
        public Graphic graphic => _graphic != null ? _graphic : _graphic = GetComponent<Graphic>();
        public virtual uint effectId => (uint)GetHashCode();
        public virtual float actualSamplingScale => 1;
        public virtual bool canModifyShape => true;

        protected Canvas canvas
        {
            get
            {
                if (_canvasCached) return _canvas;

                _canvasCached = true;
                _canvas = GetComponentInParent<Canvas>();
                if (_canvas == null || !_canvas.isActiveAndEnabled)
                {
                    _canvas = null;
                    _canvasCached = false;
                }

                return _canvas;
            }
        }

        public virtual UIEffectContext context
        {
            get
            {
                if (_context == null)
                {
                    _context = s_ContextPool.Rent();
                    UpdateContext(_context);
                }

                return _context;
            }
        }

        protected Material GetCurrentMaterial()
        {
            if (!isActiveAndEnabled) return null;

            var g = graphic;
            if (g == null) return null;

            var canvasRenderer = g.canvasRenderer;
            if (canvasRenderer == null || canvasRenderer.materialCount == 0) return null;

            return canvasRenderer.GetMaterial();
        }

        public virtual RectTransform transitionRoot => transform as RectTransform;

        protected override void OnEnable()
        {
            _canModifyMesh = true;
            _onBeforeCanvasRebuild ??= OnBeforeCanvasRebuild;
            _onAfterCanvasRebuild ??= OnAfterCanvasRebuild;
            UIExtraCallbacks.onBeforeCanvasRebuild += _onBeforeCanvasRebuild;
            UIExtraCallbacks.onAfterCanvasRebuild += _onAfterCanvasRebuild;
            UpdateContext(context);
            SetMaterialDirty();
            SetVerticesDirty();
        }

        protected override void OnDisable()
        {
            if (_onBeforeCanvasRebuild != null)
            {
                UIExtraCallbacks.onBeforeCanvasRebuild -= _onBeforeCanvasRebuild;
            }

            if (_onAfterCanvasRebuild != null)
            {
                UIExtraCallbacks.onAfterCanvasRebuild -= _onAfterCanvasRebuild;
            }

            MaterialRepository.Release(ref _material);
            SetMaterialDirty();
            SetVerticesDirty();
        }

        protected override void OnDestroy()
        {
            _onBeforeCanvasRebuild = null;
            _graphic = null;
            s_ContextPool.Return(ref _context);
        }

        private void OnBeforeCanvasRebuild()
        {
            // If the mesh is not modified in the previous frame, try to modify it if possible.
            if (!_canModifyMesh && CanModifyMesh())
            {
                _canModifyMesh = true;
                SetVerticesDirty();
            }
        }

        private void OnAfterCanvasRebuild()
        {
            var g = graphic;
            var c = canvas;
            var ctx = context;
            if (_material == null || g == null || c == null || ctx == null) return;

            if (_isContextDirty)
            {
                _isContextDirty = false;
                ApplyContextToMaterial(GetCurrentMaterial());
            }

            if (!ctx.useViewMatrix) return;

            var material = GetCurrentMaterial();
            var root = transitionRoot;
            var rootCanvas = c.rootCanvas;
            if (material == null || root == null || rootCanvas == null) return;

            ctx.UpdateViewMatrix(material, root, rootCanvas);
        }

        protected override void OnCanvasHierarchyChanged()
        {
            _canvasCached = false;
            _canvas = null;
        }

        public void ModifyMesh(Mesh mesh)
        {
        }

        public virtual void ModifyMesh(VertexHelper vh)
        {
            if (!isActiveAndEnabled || vh == null) return;

            var g = graphic;
            var ctx = context;
            var root = transitionRoot;
            if (g == null || ctx == null || root == null) return;

            _canModifyMesh = CanModifyMesh();
            if (_canModifyMesh)
            {
                ctx.ModifyMesh(g, canvas, root, vh, canModifyShape);
            }
        }

        private bool CanModifyMesh()
        {
            if (graphic == null || !graphic.isActiveAndEnabled) return false;
            if (!isActiveAndEnabled) return false;

            // The transitionRoot is same as the transform => true.
            var root = transitionRoot;
            if (root == null) return false;
            if (transform == root) return true;

            // The transitionRoot is not visible => false.
            var scale1 = root.lossyScale;
            var scale2 = transform.lossyScale;
            return !Mathf.Approximately(scale1.x * scale1.y * scale2.x * scale2.y, 0);
        }

        public virtual Material GetModifiedMaterial(Material baseMaterial)
        {
            if (baseMaterial == null || !isActiveAndEnabled || context == null || !context.willModifyMaterial)
            {
                MaterialRepository.Release(ref _material);
                return baseMaterial;
            }

            Profiler.BeginSample("(UIE)[UIEffect] GetModifiedMaterial");
            var samplingScaleId = (uint)(Mathf.InverseLerp(0.01f, 100, actualSamplingScale) * uint.MaxValue);
            var rootId = (uint)(transitionRoot != null ? transitionRoot.GetHashCode() : 0);
            var hash = new Hash128((uint)baseMaterial.GetHashCode(), effectId, samplingScaleId, rootId);
            if (!MaterialRepository.Valid(hash, _material))
            {
                Profiler.BeginSample("(UIE)[UIEffect] GetModifiedMaterial > Get or create material");
                MaterialRepository.Get(hash, ref _material, x => new Material(x)
                {
                    shader = UIEffectProjectSettings.shaderRegistry.FindOptionalShader(x.shader,
                        "(UIEffect)",
                        "Hidden/{0} (UIEffect)",
                        "Hidden/UI/Default (UIEffect)"),
                    hideFlags = HideFlags.HideAndDontSave
                }, baseMaterial);
                Profiler.EndSample();
            }

            _material.CopyPropertiesFromMaterial(baseMaterial);
            ApplyContextToMaterial(_material);
            Profiler.EndSample();
            return _material;
        }

#if UNITY_EDITOR
        protected override void OnValidate()
        {
            UpdateContext(context);
            SetVerticesDirty();
            SetMaterialDirty();
        }

        protected override void Reset()
        {
            OnValidate();
        }

        internal virtual void SetEnablePreviewIfSelected(GameObject[] selection)
        {
        }
#endif

        protected override void OnRectTransformDimensionsChange()
        {
            if (!isActiveAndEnabled) return;
            SetVerticesDirty();
        }

        protected override void OnDidApplyAnimationProperties()
        {
            UpdateContext(context);
            SetVerticesDirty();
            SetMaterialDirty();
        }

        public virtual void SetVerticesDirty()
        {
            var g = graphic;
            if (g == null) return;

            g.SetVerticesDirty();
            GraphicProxy.Find(g)?.SetVerticesDirty(g, enabled);
            Misc.QueuePlayerLoopUpdate();
        }

        public virtual void SetMaterialDirty()
        {
            var g = graphic;
            if (g == null) return;

            g.SetMaterialDirty();
            SetMaterialContextDirty();
            Misc.QueuePlayerLoopUpdate();
        }

        public virtual void SetMaterialContextDirty()
        {
            _isContextDirty = true;
        }

        internal void ReleaseMaterial()
        {
            MaterialRepository.Release(ref _material);
        }

        internal abstract void UpdateContext(UIEffectContext c);

        public virtual void ApplyContextToMaterial(Material material)
        {
            if (!isActiveAndEnabled || context == null || material == null) return;

            _isContextDirty = false;
            context.ApplyToMaterial(material, actualSamplingScale);

#if UNITY_EDITOR
            Profiler.BeginSample("(Editor/UIE)[UIEffect] ApplyContextToMaterial Post Process");
            if (!Application.isPlaying)
            {
                SetEnablePreviewIfSelected(Selection.gameObjects);
                Misc.QueuePlayerLoopUpdate();
            }

            UIEffectProjectSettings.shaderRegistry.RegisterVariant(material, "UI > UIEffect");
            Profiler.EndSample();
#endif
        }

        public abstract void SetRate(float rate, UIEffectTweener.CullingMask cullingMask);
        public abstract bool IsRaycastLocationValid(Vector2 sp, Camera eventCamera);

        public void SetTime(double _)
        {
        }

        public void OnControlTimeStart()
        {
            if (this != null)
            {
                enabled = true;
            }
        }

        public void OnControlTimeStop()
        {
            if (this != null)
            {
                enabled = false;
            }
        }
    }
}
