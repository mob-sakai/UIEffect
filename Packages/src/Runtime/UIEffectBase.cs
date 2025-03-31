using System;
using System.Runtime.CompilerServices;
using Coffee.UIEffectInternal;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Profiling;
using UnityEngine.UI;
using UnityEditor;

[assembly: InternalsVisibleTo("UIEffect")]
[assembly: InternalsVisibleTo("Coffee.UIEffect.Editor")]

namespace Coffee.UIEffects
{
    [ExecuteAlways]
    [DisallowMultipleComponent]
    public abstract class UIEffectBase : UIBehaviour, IMeshModifier, IMaterialModifier, ICanvasRaycastFilter
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

        public Material effectMaterial => _material;
        public Graphic graphic => _graphic ? _graphic : _graphic = GetComponent<Graphic>();
        public virtual uint effectId => (uint)GetInstanceID();
        public virtual float actualSamplingScale => 1;
        public virtual bool canModifyShape => true;

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
            if (!g) return null;

            var canvasRenderer = g.canvasRenderer;
            if (!canvasRenderer || canvasRenderer.materialCount == 0) return null;

            return canvasRenderer.GetMaterial();
        }

        public virtual RectTransform transitionRoot => transform as RectTransform;

        public virtual Flip flip
        {
            get => 0;
            set { }
        }

        protected override void OnEnable()
        {
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

        protected virtual void OnBeforeCanvasRebuild()
        {
            if (!_canModifyMesh && CanModifyMesh())
            {
                SetVerticesDirty();
            }
        }

        protected virtual void OnAfterCanvasRebuild()
        {
            if (!_material || !graphic || context == null) return;

            context.UpdateViewMatrix(GetCurrentMaterial(), transitionRoot, graphic.canvas.rootCanvas, flip);
        }

        public void ModifyMesh(Mesh mesh)
        {
        }

        public virtual void ModifyMesh(VertexHelper vh)
        {
            if (!isActiveAndEnabled || context == null) return;

            _canModifyMesh = CanModifyMesh();
            if (_canModifyMesh)
            {
                context.ModifyMesh(graphic, transitionRoot, vh, canModifyShape, flip);
            }
        }

        private bool CanModifyMesh()
        {
            // The transitionRoot is same as the transform => true.
            var root = transitionRoot;
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
            var rootId = (uint)(transitionRoot ? transitionRoot.GetInstanceID() : 0);
            var hash = new Hash128((uint)baseMaterial.GetInstanceID(), effectId, samplingScaleId, rootId);
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
            if (graphic)
            {
                graphic.SetVerticesDirty();
                GraphicProxy.Find(graphic).SetVerticesDirty(graphic, enabled);
                Misc.QueuePlayerLoopUpdate();
            }
        }

        public virtual void SetMaterialDirty()
        {
            if (graphic)
            {
                graphic.SetMaterialDirty();
                Misc.QueuePlayerLoopUpdate();
            }
        }

        internal void ReleaseMaterial()
        {
            MaterialRepository.Release(ref _material);
        }

        internal abstract void UpdateContext(UIEffectContext c);

        public virtual void ApplyContextToMaterial(Material material)
        {
            if (!isActiveAndEnabled || context == null || !material) return;

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
    }
}
