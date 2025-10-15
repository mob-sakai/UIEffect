#if TMP_ENABLE
using System;
using System.Collections.Generic;
using Coffee.UIEffectInternal;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Object = UnityEngine.Object;

namespace Coffee.UIEffects
{
    internal sealed class TmpProxy : GraphicProxy
    {
        /// <summary>
        /// Check if the graphic is valid for this proxy.
        /// </summary>
        protected override bool IsValid(Graphic graphic)
        {
            if (!graphic) return false;
            if (graphic is TextMeshProUGUI) return true;
            if (graphic is TMP_SubMeshUI sub)
            {
                // If TMP_SubMeshUI is used for text
                return !sub.spriteAsset
                       || sub.sharedMaterial != sub.spriteAsset.material;
            }

            return false;
        }

        /// <summary>
        /// Check if the graphic is a text.
        /// </summary>
        public override bool IsText(Graphic graphic)
        {
            return true;
        }

        /// <summary>
        /// Called before modifying the mesh.
        /// </summary>
        public override void OnPreModifyMesh(Graphic graphic, Canvas canvas)
        {
            UIVertexUtil.onLerpVertex = s_OnLerpVertex;
            ShadowUtil.onMarkAsShadow = s_OnMarkAsShadow;
            UIEffectContext.onModifyVertex = s_OnModifyVertex;

            if (canvas)
            {
                canvas.additionalShaderChannels |= AdditionalCanvasShaderChannels.TexCoord2;
            }
        }

        public override void SetVerticesDirty(Graphic graphic, bool enabled)
        {
            if (graphic is TextMeshProUGUI textMeshProUGUI && textMeshProUGUI.isActiveAndEnabled)
            {
                if (enabled)
                {
                    s_ChangedInstances.Add(textMeshProUGUI);
                }
                else if (0 < textMeshProUGUI.textInfo?.meshInfo?.Length
                         && 0 < textMeshProUGUI.textInfo.meshInfo[0].vertexCount)
                {
                    textMeshProUGUI.UpdateVertexData();
                }
            }
        }

        private static readonly Func<UIVertex, UIVertex, UIVertex, float, UIVertex> s_OnLerpVertex =
#if UNITY_2023_2_OR_NEWER
            null;
#else
            (vt, a, b, t) =>
            {
                // TMP 3.x uses packed UV
                vt.uv1.x = PackUV(Vector2.Lerp(UnpackUV(a.uv1.x), UnpackUV(b.uv1.x), t));
                return vt;

                static float PackUV(Vector2 input)
                {
                    return (int)(input.x * 511) * 4096 + (int)(input.y * 511);
                }

                static Vector2 UnpackUV(float input)
                {
                    return new Vector2(input / 4096 / 511, input % 4096 / 511);
                }
            };
#endif

        private static readonly Func<UIVertex, float, UIVertex> s_OnMarkAsShadow =
            (vt, s) =>
            {
                vt.uv2.x -= s;
                return vt;
            };

        private static readonly Func<UIVertex, Rect, UIVertex> s_OnModifyVertex =
            (vt, uvMask) =>
            {
                vt.uv2 = new Vector4(uvMask.xMin, uvMask.yMin, uvMask.xMax, uvMask.yMax);
                return vt;
            };

        private static Mesh s_Mesh;
        private static readonly VertexHelper s_VertexHelper = new VertexHelper();
        private static readonly HashSet<TextMeshProUGUI> s_ChangedInstances = new HashSet<TextMeshProUGUI>();
        private static readonly HashSet<TextMeshProUGUI> s_RegisteredInstances = new HashSet<TextMeshProUGUI>();
        private static readonly Dictionary<int, float> s_SdfScaleCache = new Dictionary<int, float>();

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void RuntimeInitializeOnLoadMethod()
        {
            s_ChangedInstances.Clear();
            s_RegisteredInstances.Clear();
            s_SdfScaleCache.Clear();
        }

#if UNITY_EDITOR
        [InitializeOnLoadMethod]
#else
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
#endif
        private static void InitializeOnLoad()
        {
            Register(new TmpProxy());

            // When the text is changed, add it to the changed list
            TMPro_EventManager.TEXT_CHANGED_EVENT.Add(obj =>
            {
                if (obj is TextMeshProUGUI textMeshProUGUI
                    && textMeshProUGUI.isActiveAndEnabled
                    // fix: `TmpProxy` causes loss of modifications from other `IMeshModifiers` (#371)
                    && textMeshProUGUI.TryGetComponent<UIEffectBase>(out var _))
                {
                    s_ChangedInstances.Add(textMeshProUGUI);
                }
            });

            UIExtraCallbacks.onBeforeCanvasRebuild += () =>
            {
                // Check SDF scale (lossyScale.y) change
                var toRemove = InternalListPool<TextMeshProUGUI>.Rent();
                foreach (var textMeshProUGUI in s_RegisteredInstances)
                {
                    if (textMeshProUGUI
                        && textMeshProUGUI.isActiveAndEnabled
                        && !textMeshProUGUI.isTextObjectScaleStatic)
                    {
                        var id = textMeshProUGUI.GetHashCode();
                        var lossyScaleY = textMeshProUGUI.transform.lossyScale.y;

                        // If the scale has changed, add to the changed list
                        if (s_SdfScaleCache.TryGetValue(id, out var prev) && !Mathf.Approximately(prev, lossyScaleY))
                        {
                            s_ChangedInstances.Add(textMeshProUGUI);
                        }

                        // Update the scale cache
                        s_SdfScaleCache[id] = lossyScaleY;
                    }
                    // Remove destroyed objects later
                    else if (!ReferenceEquals(textMeshProUGUI, null))
                    {
                        toRemove.Add(textMeshProUGUI);
                    }
                }

                // Remove destroyed objects
                foreach (var textMeshProUGUI in toRemove)
                {
                    s_SdfScaleCache.Remove(textMeshProUGUI.GetHashCode());
                    s_RegisteredInstances.Remove(textMeshProUGUI);
                }

                InternalListPool<TextMeshProUGUI>.Return(ref toRemove);
            };

            // Modify the changed TMP mesh
            UIExtraCallbacks.onAfterCanvasRebuild += () =>
            {
                foreach (var textMeshProUGUI in s_ChangedInstances)
                {
                    if (!textMeshProUGUI || !textMeshProUGUI.isActiveAndEnabled) continue;
                    if (!textMeshProUGUI.TryGetComponent<IMeshModifier>(out var _)) continue;

                    s_RegisteredInstances.Add(textMeshProUGUI);
                    ModifyMesh(textMeshProUGUI);
                }

                s_ChangedInstances.Clear();
            };

#if UNITY_EDITOR
            UnityEditor.SceneManagement.EditorSceneManager.sceneSaved += _ =>
            {
                foreach (var textMeshProUGUI in s_RegisteredInstances)
                {
                    if (!textMeshProUGUI || !textMeshProUGUI.isActiveAndEnabled) continue;
                    if (textMeshProUGUI.TryGetComponent<UIEffectBase>(out var effect) && effect.isActiveAndEnabled)
                    {
                        effect.ReleaseMaterial();
                        effect.SetMaterialDirty();
                    }
                }

                Misc.QueuePlayerLoopUpdate();
            };
#endif
        }

        private static void ModifyMesh(TextMeshProUGUI textMeshProUGUI)
        {
            if (!s_Mesh)
            {
                s_Mesh = new Mesh();
                s_Mesh.MarkDynamic();
            }

            var subMeshes = InternalListPool<TMP_SubMeshUI>.Rent();
            var modifiers = InternalListPool<IMeshModifier>.Rent();
            var subModifiers = InternalListPool<IMeshModifier>.Rent();
            textMeshProUGUI.TryGetComponent<UIEffectBase>(out var effect);
            textMeshProUGUI.GetComponentsInChildren(subMeshes, 1);
            textMeshProUGUI.GetComponents(modifiers);

            for (var i = 0; i < textMeshProUGUI.textInfo.meshInfo.Length; i++)
            {
                var meshInfo = textMeshProUGUI.textInfo.meshInfo[i];
                if (meshInfo.vertexCount == 0)
                {
                    // Clear mesh if no vertices.
                    s_Mesh.Clear(false);
                    if (i == 0)
                    {
                        textMeshProUGUI.canvasRenderer.SetMesh(s_Mesh);
                    }
                    else
                    {
                        var subMeshUI = GetSubMeshUI(subMeshes, meshInfo.material, i - 1);
                        if (subMeshUI)
                        {
                            subMeshUI.canvasRenderer.SetMesh(s_Mesh);
                        }
                    }

                    continue;
                }

                s_VertexHelper.Clear();
                meshInfo.mesh.CopyTo(s_VertexHelper);
                if (i == 0)
                {
                    foreach (var modifier in modifiers)
                    {
                        modifier.ModifyMesh(s_VertexHelper);
                    }

                    s_VertexHelper.FillMesh(s_Mesh);
                    textMeshProUGUI.canvasRenderer.SetMesh(s_Mesh);
                }
                else if (i - 1 < subMeshes.Count)
                {
                    foreach (var modifier in modifiers)
                    {
                        if (modifier is UIEffectBase) continue;
                        modifier.ModifyMesh(s_VertexHelper);
                    }

                    var subMeshUI = GetSubMeshUI(subMeshes, meshInfo.material, i - 1);
                    if (!subMeshUI) break;

                    // fix: TMP Text sprite submesh lost when changing fontMaterial (#368)
                    if (!subMeshUI.TryGetComponent<UIEffectReplica>(out var replica))
                    {
                        replica = subMeshUI.gameObject.AddComponent<UIEffectReplica>();
                        textMeshProUGUI.RegisterDirtyMaterialCallback(() =>
                        {
                            if (subMeshUI) subMeshUI.SetMaterialDirty();
                        });
                    }

                    if (effect is UIEffect pEffect && pEffect.isActiveAndEnabled)
                    {
                        replica.target = pEffect;
                        replica.useTargetTransform = true;
                        replica.customRoot = null;
                    }
                    else if (effect is UIEffectReplica pReplica && pReplica.isActiveAndEnabled)
                    {
                        replica.target = pReplica.target;
                        replica.preset = pReplica.preset;
                        replica.useTargetTransform = pReplica.useTargetTransform;
                        replica.customRoot = pReplica.customRoot;
                    }
                    else
                    {
                        replica.target = null;
                        replica.customRoot = null;
                    }

                    subMeshUI.GetComponents(subModifiers);
                    foreach (var modifier in subModifiers)
                    {
                        modifier.ModifyMesh(s_VertexHelper);
                    }

                    s_VertexHelper.FillMesh(s_Mesh);
                    subMeshUI.canvasRenderer.SetMesh(s_Mesh);
                }
                else
                {
                    break;
                }
            }

            InternalListPool<TMP_SubMeshUI>.Return(ref subMeshes);
            InternalListPool<IMeshModifier>.Return(ref modifiers);
            InternalListPool<IMeshModifier>.Return(ref subModifiers);
            s_Mesh.Clear(false);
            Misc.QueuePlayerLoopUpdate();
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
    }
}
#endif
