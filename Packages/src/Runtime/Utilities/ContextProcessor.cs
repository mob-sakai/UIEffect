using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
#if TMP_ENABLE
using TMPro;
#endif


namespace Coffee.UIEffects
{
    public class ContextProcessor
    {
        private static readonly List<ContextProcessor> s_Processors = new List<ContextProcessor>()
        {
            new ContextProcessor(),
#if TMP_ENABLE
            new TmpContextProcessor(),
#endif
        };

        public static void RegisterProcessor(ContextProcessor processor)
        {
            // Register only once.
            foreach (var p in s_Processors)
            {
                if (p.GetType() == processor.GetType()) return;
            }

            s_Processors.Add(processor);
        }

        public static ContextProcessor FindProcessor(Graphic graphic)
        {
            for (var i = s_Processors.Count - 1; i >= 0; i--)
            {
                var p = s_Processors[i];
                if (p.IsValid(graphic)) return p;
            }

            return null;
        }

        /// <summary>
        /// Check if the graphic is valid for this processor.
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
        }

        private static readonly Func<UIVertex, UIVertex, UIVertex, float, UIVertex> s_OnLerpVertex =
            null;

        private static readonly Func<UIVertex, UIVertex> s_OnMarkAsShadow =
            vt =>
            {
                vt.uv0.z -= 8;
                vt.uv0.w -= 8;
                return vt;
            };

        private static readonly Func<UIVertex, Rect, float, float, UIVertex> s_OnModifyVertex =
            (vt, uvMask, normalizedX, normalizedY) =>
            {
                vt.uv0.z = normalizedX;
                vt.uv0.w = normalizedY;
                vt.uv1 = new Vector4(uvMask.xMin, uvMask.yMin, uvMask.xMax, uvMask.yMax);
                return vt;
            };
    }

#if TMP_ENABLE
    internal class TmpContextProcessor : ContextProcessor
    {
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
                vt.uv1.z -= 8;
                vt.uv1.w -= 8;
                return vt;
            };

        private static readonly Func<UIVertex, Rect, float, float, UIVertex> s_OnModifyVertex =
            (vt, uvMask, normalizedX, normalizedY) =>
            {
                vt.uv1.z = normalizedX;
                vt.uv1.w = normalizedY;
                vt.uv2 = new Vector4(uvMask.xMin, uvMask.yMin, uvMask.xMax, uvMask.yMax);
                return vt;
            };
    }
#endif
}
