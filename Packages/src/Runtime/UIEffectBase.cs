using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Coffee.UIEffectInternal;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Profiling;
using UnityEngine.UI;
using UnityEditor;
#if TMP_ENABLE
using TMPro;
#endif

[assembly: InternalsVisibleTo("UIEffect")]
[assembly: InternalsVisibleTo("Coffee.UIEffect.Editor")]

namespace Coffee.UIEffects
{
    [ExecuteAlways]
    [DisallowMultipleComponent]
    public abstract class UIEffectBase : UIBehaviour, IMeshModifier, IMaterialModifier, ICanvasRaycastFilter
    {
        private static readonly VertexHelper s_VertexHelper = new VertexHelper();
        private static Mesh s_Mesh;

        private static readonly InternalObjectPool<UIEffectContext> s_ContextPool =
            new InternalObjectPool<UIEffectContext>(() => new UIEffectContext(), x => true, x => x.Reset());

        private Graphic _graphic;
        private Material _material;
        private UIEffectContext _context;

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

        public virtual RectTransform transitionRoot => transform as RectTransform;

        protected override void OnEnable()
        {
#if TMP_ENABLE
            if (graphic is TextMeshProUGUI)
            {
                _prevLossyScaleY = transform.lossyScale.y;
                Canvas.willRenderCanvases += CheckSDFScaleForTMP;
                UIExtraCallbacks.onScreenSizeChanged += SetVerticesDirtyForTMP;
            }
#endif

            UpdateContext(context);
            SetMaterialDirty();
            SetVerticesDirty();
        }

        protected override void OnDisable()
        {
#if TMP_ENABLE
            Canvas.willRenderCanvases -= CheckSDFScaleForTMP;
            UIExtraCallbacks.onScreenSizeChanged -= SetVerticesDirtyForTMP;
#endif

            MaterialRepository.Release(ref _material);
            SetMaterialDirty();
            SetVerticesDirty();
        }

        protected override void OnDestroy()
        {
            s_ContextPool.Return(ref _context);
        }

        public void ModifyMesh(Mesh mesh)
        {
        }

        public virtual void ModifyMesh(VertexHelper vh)
        {
            if (!isActiveAndEnabled || context == null) return;

            context.ModifyMesh(graphic, transitionRoot, vh, canModifyShape);
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
            var hash = new Hash128((uint)baseMaterial.GetInstanceID(), effectId, samplingScaleId, 0);
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

            ApplyContextToMaterial();
            Profiler.EndSample();
            return _material;
        }

#if UNITY_EDITOR
        protected override void OnValidate()
        {
            UpdateContext(context);
            ApplyContextToMaterial();
            SetVerticesDirty();
            SetMaterialDirty();

            if (!EditorApplication.isPlaying)
            {
                EditorApplication.QueuePlayerLoopUpdate();
            }
        }

        protected override void Reset()
        {
            OnValidate();
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
            ApplyContextToMaterial();
        }

        public virtual void SetVerticesDirty()
        {
#if TMP_ENABLE
            if (graphic is TextMeshProUGUI textMeshProUGUI && textMeshProUGUI.isActiveAndEnabled)
            {
                if (isActiveAndEnabled)
                {
                    OnTMPChanged(textMeshProUGUI);
                }
                else if (0 < textMeshProUGUI.textInfo?.meshInfo?.Length
                         && 0 < textMeshProUGUI.textInfo.meshInfo[0].vertexCount)
                {
                    textMeshProUGUI.UpdateVertexData();
                }
            }
            else
#endif
            if (graphic)
            {
                graphic.SetVerticesDirty();
            }
        }

        public virtual void SetMaterialDirty()
        {
            if (graphic)
            {
                graphic.SetMaterialDirty();
            }
        }

        protected abstract void UpdateContext(UIEffectContext c);

        public virtual void ApplyContextToMaterial()
        {
            if (!isActiveAndEnabled || context == null) return;

            context.ApplyToMaterial(_material, actualSamplingScale);

#if UNITY_EDITOR
            UIEffectProjectSettings.shaderRegistry.RegisterVariant(_material, "UI > UIEffect");
            if (!EditorApplication.isPlaying)
            {
                EditorApplication.QueuePlayerLoopUpdate();
            }
#endif
        }

#if TMP_ENABLE
#if UNITY_EDITOR
        [InitializeOnLoadMethod]
#else
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
#endif
        private static void InitializeOnLoad()
        {
            TMPro_EventManager.TEXT_CHANGED_EVENT.Add(obj =>
            {
                if (obj is TextMeshProUGUI textMeshProUGUI)
                {
                    OnTMPChanged(textMeshProUGUI);
                }
            });
        }

        private static void OnTMPChanged(TextMeshProUGUI textMeshProUGUI)
        {
            if (!textMeshProUGUI.TryGetComponent<UIEffectBase>(out var effect)) return;
            if (!effect || !effect.isActiveAndEnabled) return;

            if (!s_Mesh)
            {
                s_Mesh = new Mesh();
                s_Mesh.MarkDynamic();
            }

            var target = effect is UIEffect uiEffect
                ? uiEffect
                : effect is UIEffectReplica parentReplica
                    ? parentReplica.target
                    : null;
            var subMeshes = InternalListPool<TMP_SubMeshUI>.Rent();
            var modifiers = InternalListPool<IMeshModifier>.Rent();
            textMeshProUGUI.GetComponentsInChildren(subMeshes, 1);
            for (var i = 0; i < textMeshProUGUI.textInfo.meshInfo.Length; i++)
            {
                var meshInfo = textMeshProUGUI.textInfo.meshInfo[i];
                s_VertexHelper.Clear();
                meshInfo.mesh.CopyTo(s_VertexHelper, meshInfo.vertexCount, meshInfo.vertexCount * 6 / 4);
                if (i == 0)
                {
                    textMeshProUGUI.GetComponents(modifiers);
                    foreach (var modifier in modifiers)
                    {
                        modifier.ModifyMesh(s_VertexHelper);
                    }

                    s_VertexHelper.FillMesh(s_Mesh);
                    textMeshProUGUI.canvasRenderer.SetMesh(s_Mesh);
                }
                else if (i - 1 < subMeshes.Count)
                {
                    var subMeshUI = GetSubMeshUI(subMeshes, meshInfo.material, i - 1);
                    if (!target || !subMeshUI) break;

                    var replica = subMeshUI.GetOrAddComponent<UIEffectReplica>();
                    replica.target = target;

                    subMeshUI.GetComponents(modifiers);
                    foreach (var modifier in modifiers)
                    {
                        modifier.ModifyMesh(s_VertexHelper);
                    }

                    s_VertexHelper.FillMesh(s_Mesh);
                    replica.ApplyContextToMaterial();
                    subMeshUI.canvasRenderer.SetMesh(s_Mesh);
                }
                else
                {
                    break;
                }
            }

            InternalListPool<IMeshModifier>.Return(ref modifiers);
            InternalListPool<TMP_SubMeshUI>.Return(ref subMeshes);
            s_Mesh.Clear(false);
        }

        private static TMP_SubMeshUI GetSubMeshUI(List<TMP_SubMeshUI> subMeshes, Material material, int start)
        {
            var count = subMeshes.Count;
            for (var j = 0; j < count; j++)
            {
                var s = subMeshes[(j + start + count) % count];
                if (s.sharedMaterial == material) return s;
            }

            return null;
        }

        private void SetVerticesDirtyForTMP()
        {
            if (graphic && graphic.isActiveAndEnabled)
            {
                graphic.SetVerticesDirty();
            }
        }

        private void CheckSDFScaleForTMP()
        {
            var lossyScaleY = transform.lossyScale.y;
            if (Mathf.Approximately(_prevLossyScaleY, lossyScaleY)) return;

            _prevLossyScaleY = lossyScaleY;
            if (graphic is TextMeshProUGUI textMeshProUGUI && graphic.isActiveAndEnabled)
            {
                OnTMPChanged(textMeshProUGUI);
            }
        }

        private float _prevLossyScaleY;
#endif

        public abstract void SetRate(float rate, UIEffectTweener.CullingMask cullingMask);
        public abstract bool IsRaycastLocationValid(Vector2 sp, Camera eventCamera);
    }
}
