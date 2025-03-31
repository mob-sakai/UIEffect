using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Coffee.UIEffects
{
    public class GraphicProxy
    {
        private static readonly List<GraphicProxy> s_Proxies = new List<GraphicProxy>()
        {
            new GraphicProxy()
        };

        public static void Register(GraphicProxy proxy)
        {
            // Register only once.
            foreach (var p in s_Proxies)
            {
                if (p.GetType() == proxy.GetType()) return;
            }

            s_Proxies.Add(proxy);
        }

        public static GraphicProxy Find(Graphic graphic)
        {
            if (!graphic) return null;

            for (var i = s_Proxies.Count - 1; i >= 0; i--)
            {
                var p = s_Proxies[i];
                if (p.IsValid(graphic)) return p;
            }

            return null;
        }

        /// <summary>
        /// Check if the graphic is valid for this proxy.
        /// </summary>
        protected virtual bool IsValid(Graphic graphic)
        {
            return graphic;
        }

        /// <summary>
        /// Check if the graphic is a text.
        /// </summary>
        public virtual bool IsText(Graphic graphic)
        {
            return graphic is Text;
        }

        /// <summary>
        /// Called before modifying the mesh.
        /// </summary>
        public virtual void OnPreModifyMesh(Graphic graphic)
        {
            UIVertexUtil.onLerpVertex = s_OnLerpVertex;
            ShadowUtil.onMarkAsShadow = s_OnMarkAsShadow;
            UIEffectContext.onModifyVertex = s_OnModifyVertex;

            var canvas = graphic.canvas;
            if (canvas)
            {
                canvas.additionalShaderChannels |= AdditionalCanvasShaderChannels.TexCoord1;
            }
        }

        public virtual　void SetVerticesDirty(Graphic graphic, bool enabled)
        {
        }

        public virtual Vector4 ModifyExpandSize(Graphic graphic, Vector4 expandSize)
        {
            return expandSize;
        }

        private static readonly Func<UIVertex, UIVertex, UIVertex, float, UIVertex> s_OnLerpVertex =
            null;

        private static readonly Func<UIVertex, UIVertex> s_OnMarkAsShadow =
            vt =>
            {
                vt.uv1.x -= 2;
                return vt;
            };

        private static readonly Func<UIVertex, Rect, UIVertex> s_OnModifyVertex =
            (vt, uvMask) =>
            {
                vt.uv1 = new Vector4(uvMask.xMin, uvMask.yMin, uvMask.xMax, uvMask.yMax);
                return vt;
            };
    }
}
