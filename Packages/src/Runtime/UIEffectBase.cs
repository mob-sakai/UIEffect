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
        private static readonly Dictionary<int, string> s_ShaderNameCache = new Dictionary<int, string>();

        private static readonly ObjectPool<UIEffectContext> s_ContextPool =
            new ObjectPool<UIEffectContext>(() => new UIEffectContext(), x => true, x => x.Reset());

        private Graphic _graphic;
        private Material _material;
        private UIEffectContext _context;

        public Graphic graphic => _graphic ? _graphic : _graphic = GetComponent<Graphic>();
        public virtual uint effectId => (uint)GetInstanceID();

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
            UpdateContext(context);
            SetMaterialDirty();
            SetVerticesDirty();
        }

        protected override void OnDisable()
        {
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

            context.ModifyMesh(graphic, transitionRoot, vh);
        }

        public virtual Material GetModifiedMaterial(Material baseMaterial)
        {
            if (baseMaterial == null || !isActiveAndEnabled || context == null || !context.willModifyMaterial)
            {
                MaterialRepository.Release(ref _material);
                return baseMaterial;
            }

            Profiler.BeginSample("(UIE)[UIEffect] GetModifiedMaterial");
            var hash = new Hash128((uint)baseMaterial.GetInstanceID(), effectId, 0, 0);
            if (!MaterialRepository.Valid(hash, _material))
            {
                Profiler.BeginSample("(UIE)[UIEffect] GetModifiedMaterial > Get or create material");
                MaterialRepository.Get(hash, ref _material, x => new Material(x)
                {
                    shader = FindShader(x),
                    hideFlags = HideFlags.HideAndDontSave
                }, baseMaterial);
                Profiler.EndSample();
            }

            ApplyContextToMaterial();
            Profiler.EndSample();
            return _material;
        }


        private static Shader FindShader(Material material)
        {
            var shader = material.shader;
            var hash = shader.GetInstanceID();
            if (!s_ShaderNameCache.TryGetValue(hash, out var shaderName))
            {
                shaderName = shader.name;
                if (!shaderName.Contains("(UIEffect)"))
                {
                    shaderName = $"Hidden/{shaderName} (UIEffect)";
                }

                var uiEffectShader = Shader.Find(shaderName);
                if (!uiEffectShader)
                {
                    shaderName = "Hidden/UI/Default (UIEffect)";
                    uiEffectShader = Shader.Find(shaderName);
                }

                s_ShaderNameCache[hash] = shaderName;
                return uiEffectShader;
            }

            return Shader.Find(shaderName);
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
                    var canvas = graphic.canvas;
                    if (canvas && graphic.isActiveAndEnabled)
                    {
                        canvas.additionalShaderChannels |= AdditionalCanvasShaderChannels.TexCoord2;
                    }
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
                var canvas = graphic.canvas;
                if (canvas && graphic.isActiveAndEnabled)
                {
                    canvas.additionalShaderChannels |= AdditionalCanvasShaderChannels.TexCoord1;
                }
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

            context.ApplyToMaterial(_material);

#if UNITY_EDITOR
            UIEffectProjectSettings.RegisterVariant(_material);
            if (!EditorApplication.isPlaying)
            {
                EditorApplication.QueuePlayerLoopUpdate();
            }
#endif
        }

#if TMP_ENABLE
#if UNITY_EDITOR
        private class MyAllPostprocessor : AssetPostprocessor
        {
            private static void OnPostprocessAllAssets(string[] _, string[] __, string[] ___, string[] ____)
            {
                if (Application.isBatchMode || BuildPipeline.isBuildingPlayer) return;

                s_ShaderNameCache.Clear();
#if UNITY_2021_3 || UNITY_2022_2_OR_NEWER
                foreach (var effect in FindObjectsByType<UIEffectBase>(FindObjectsSortMode.None))
#else
                foreach (var effect in FindObjectsOfType<UIEffectBase>())
#endif
                {
                    if (!effect.isActiveAndEnabled) continue;
                    if (!(effect.graphic is TextMeshProUGUI tmp) || !tmp.isActiveAndEnabled) continue;
                    effect.SetMaterialDirty();
                }

                EditorApplication.QueuePlayerLoopUpdate();
            }
        }
#endif

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
            var subMeshes = ListPool<TMP_SubMeshUI>.Rent();
            var modifiers = ListPool<IMeshModifier>.Rent();
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
                    var subMeshUI = GetSubMeshUI(subMeshes, meshInfo.material);
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

            ListPool<IMeshModifier>.Return(ref modifiers);
            ListPool<TMP_SubMeshUI>.Return(ref subMeshes);
            s_Mesh.Clear(false);
        }

        private static TMP_SubMeshUI GetSubMeshUI(List<TMP_SubMeshUI> subMeshes, Material material)
        {
            for (var j = 0; j < subMeshes.Count; j++)
            {
                if (subMeshes[j].sharedMaterial == material) return subMeshes[j];
            }

            return null;
        }
#endif

        public abstract void SetRate(float rate, UIEffectTweener.CullingMask cullingMask);
        public abstract bool IsRaycastLocationValid(Vector2 sp, Camera eventCamera);
    }
}
