#if UNITEXT_ENABLE
using System;
using Coffee.UIEffectInternal;
using LightSide;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

namespace Coffee.UIEffects
{
    internal sealed class UniTextProxy : GraphicProxy
    {
        /// <summary>
        /// Check if the graphic is valid for this proxy.
        /// </summary>
        protected override bool IsValid(Graphic graphic)
        {
            if (!graphic) return false;
            if (graphic is UniTextProxyRenderer sub && sub.material != EmojiFont.Material) return true;

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
            graphic.SetVerticesDirty();
        }

        private static readonly Func<UIVertex, UIVertex, UIVertex, float, UIVertex> s_OnLerpVertex = null;

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

#if UNITY_EDITOR
        [InitializeOnLoadMethod]
#else
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
#endif
        private static void InitializeOnLoad()
        {
            Register(new UniTextProxy());
            UniText.OnChanged -= ModifyMesh;
            UniText.OnChanged += ModifyMesh;
        }

        private static void ModifyMesh(UniText uniText)
        {
            if (!uniText.TryGetComponent<UIEffectBase>(out var effect)) return;

            var renderData = uniText.RenderData;
            var subMeshRenderers = uniText.SubMeshRenderers;
            for (var i = 0; i < renderData.Count; i++)
            {
                var r = subMeshRenderers[i].renderer;
                if (!r) continue;

                if (!r.TryGetComponent<UIEffectReplica>(out var replica))
                {
                    replica = r.gameObject.AddComponent<UIEffectReplica>();
                }

                if (!r.TryGetComponent<UniTextProxyRenderer>(out var proxyRenderer))
                {
                    proxyRenderer = r.gameObject.AddComponent<UniTextProxyRenderer>();
                }

                var data = renderData[i];
                replica.SetTarget(effect);
                proxyRenderer.Rebuild(data.mesh, data.material, data.texture);
            }

            Misc.QueuePlayerLoopUpdate();
        }
    }
}
#endif
