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
    internal class TmpProxy : GraphicProxy
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
        public override void OnPreModifyMesh(Graphic graphic)
        {
            UIVertexUtil.onLerpVertex = s_OnLerpVertex;
            ShadowUtil.onMarkAsShadow = s_OnMarkAsShadow;
            UIEffectContext.onModifyVertex = s_OnModifyVertex;

            var canvas = graphic.canvas;
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
                    OnChangeText(textMeshProUGUI);
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

        private static readonly Func<UIVertex, UIVertex> s_OnMarkAsShadow =
            vt =>
            {
                vt.uv2.x -= 2;
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

#if UNITY_EDITOR
        [InitializeOnLoadMethod]
#endif
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void InitializeOnLoad()
        {
            s_ChangedInstances.Clear();
            s_RegisteredInstances.Clear();
            s_SdfScaleCache.Clear();
            Register(new TmpProxy());

            // When the text is changed, add it to the changed list
            TMPro_EventManager.TEXT_CHANGED_EVENT.Add(obj =>
            {
                if (obj is TextMeshProUGUI textMeshProUGUI && textMeshProUGUI.isActiveAndEnabled)
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
                    if (textMeshProUGUI && textMeshProUGUI.isActiveAndEnabled)
                    {
                        var id = textMeshProUGUI.GetHashCode();
                        var lossyScaleY = textMeshProUGUI.transform.lossyScale.y;

                        // If the scale has changed, add to the changed list
                        if (s_SdfScaleCache.TryGetValue(id, out var prev) && !Mathf.Approximately(prev, lossyScaleY))
                        {
                            OnChangeText(textMeshProUGUI);
                        }

                        // Update the scale cache
                        s_SdfScaleCache[id] = textMeshProUGUI.transform.lossyScale.y;
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

        private static void OnChangeText(Object obj)
        {
            if (obj is TextMeshProUGUI textMeshProUGUI && textMeshProUGUI.isActiveAndEnabled)
            {
                s_ChangedInstances.Add(textMeshProUGUI);
            }
        }

        private static void ModifyMesh(TextMeshProUGUI textMeshProUGUI)
        {
            if (!s_Mesh)
            {
                s_Mesh = new Mesh();
                s_Mesh.MarkDynamic();
            }

            s_RegisteredInstances.Add(textMeshProUGUI);
            var effect = textMeshProUGUI.GetComponent<UIEffectBase>();
            var subMeshes = InternalListPool<TMP_SubMeshUI>.Rent();
            var modifiers = InternalListPool<IMeshModifier>.Rent();
            var subModifiers = InternalListPool<IMeshModifier>.Rent();
            textMeshProUGUI.GetComponentsInChildren(subMeshes, 1);
            textMeshProUGUI.GetComponents(modifiers);

            for (var i = 0; i < textMeshProUGUI.textInfo.meshInfo.Length; i++)
            {
                var meshInfo = textMeshProUGUI.textInfo.meshInfo[i];
                s_VertexHelper.Clear();
                meshInfo.mesh.CopyTo(s_VertexHelper, meshInfo.vertexCount, meshInfo.vertexCount * 6 / 4);
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

                    var replica = subMeshUI.GetOrAddComponent<UIEffectReplica>();
                    if (effect is UIEffect pEffect && pEffect.isActiveAndEnabled)
                    {
                        replica.target = pEffect;
                        replica.useTargetTransform = true;
                        replica.customRoot = null;
                        replica.flip = pEffect.flip;
                    }
                    else if (effect is UIEffectReplica pReplica && pReplica.isActiveAndEnabled)
                    {
                        replica.target = pReplica.target;
                        replica.useTargetTransform = pReplica.useTargetTransform;
                        replica.customRoot = pReplica.customRoot;
                        replica.flip = pReplica.flip;
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
