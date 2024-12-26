using System.Collections.Generic;
using UnityEngine;

namespace Coffee.UIEffects
{
    public static class ShadowUtil
    {
        public static void DoShadow(ShadowMode mode, List<UIVertex> verts, Vector2 distance, int iteration, float fade)
        {
            distance = new Vector2(Mathf.Clamp(distance.x, -600, 600), Mathf.Clamp(distance.y, -600, 600));
            var count = verts.Count;
            var start = 0;
            var end = count;
            var d = Vector2.zero;
            var a = 1f;
            for (var i = 0; i < iteration; i++)
            {
                d += distance / (i + 1);
                a *= fade;
                ApplyShadow(verts, ref start, ref end, d, mode, a);
            }
        }

        public static void DoMirror(List<UIVertex> verts, Vector2 distance, float scale, float fade, RectTransform root)
        {
            distance = new Vector2(Mathf.Clamp(distance.x, -600, 600), Mathf.Clamp(distance.y, -600, 600));
            var count = verts.Count;
            var rect2 = root.rect;
            var pivot = root.pivot.y;
            var height = rect2.height;
            var rate = distance.x;
            var offset = distance.y - (scale + 1) * pivot * height;
            var range = new Vector2(rect2.yMin, rect2.yMax);
            ApplyMirror(verts, count, rate, range, scale, offset, fade);
        }

        private static void ApplyMirror(List<UIVertex> verts, int count, float rate, Vector2 range, float scale,
            float offset, float alpha)
        {
            rate = Mathf.Clamp01(rate);
            var start = 0;
            var end = count;
            ApplyShadowZeroAlloc(verts, ref start, ref end, 0, 0, alpha);

            for (var i = 0; i < count; i += 6)
            {
                var lb = verts[i];
                var lbRate = Mathf.InverseLerp(range.x, range.y, lb.position.y);
                var lt = verts[i + 1];
                var ltRate = Mathf.InverseLerp(range.x, range.y, lt.position.y);
                var rt = verts[i + 2];
                var rtRate = Mathf.InverseLerp(range.x, range.y, rt.position.y);
                var rb = verts[i + 4];
                var rbRate = Mathf.InverseLerp(range.x, range.y, rb.position.y);

                lb.color.a = (byte)(Mathf.InverseLerp(rate, 0, lbRate) * lb.color.a);
                lt.color.a = (byte)(Mathf.InverseLerp(rate, 0, ltRate) * lt.color.a);
                rt.color.a = (byte)(Mathf.InverseLerp(rate, 0, rtRate) * rt.color.a);
                rb.color.a = (byte)(Mathf.InverseLerp(rate, 0, rbRate) * rb.color.a);

                if (lbRate < rate && rate < ltRate)
                {
                    var t = (rate - lbRate) / (ltRate - lbRate);
                    lt.position = Vector3.Lerp(lb.position, lt.position, t);
                    lt.uv0 = Vector4.Lerp(lb.uv0, lt.uv0, t);
                    lt.uv1 = Vector4.Lerp(lb.uv1, lt.uv1, t);
                    lt.uv2 = Vector4.Lerp(lb.uv2, lt.uv2, t);
                    lt.color = Color.Lerp(lb.color, lt.color, t);
                }

                if (rbRate < rate && rate < rtRate)
                {
                    var t = (rate - rbRate) / (rtRate - rbRate);
                    rt.position = Vector3.Lerp(rb.position, rt.position, t);
                    rt.uv0 = Vector4.Lerp(rb.uv0, rt.uv0, t);
                    rt.uv1 = Vector4.Lerp(rb.uv1, rt.uv1, t);
                    rt.uv2 = Vector4.Lerp(rb.uv2, rt.uv2, t);
                    rt.color = Color.Lerp(rb.color, rt.color, t);
                }


                lb.position.y = -lb.position.y * scale + offset;
                lt.position.y = -lt.position.y * scale + offset;
                rt.position.y = -rt.position.y * scale + offset;
                rb.position.y = -rb.position.y * scale + offset;

                verts[i] = verts[i + 5] = lb;
                verts[i + 1] = lt;
                verts[i + 2] = verts[i + 3] = rt;
                verts[i + 4] = rb;
            }
        }

        /// <summary>
        /// Append shadow vertices.
        /// * It is similar to Shadow component implementation.
        /// </summary>
        private static void ApplyShadow(List<UIVertex> verts, ref int start, ref int end, Vector2 distance,
            ShadowMode mode, float alpha)
        {
            if (mode == ShadowMode.None) return;

            var x = distance.x;
            var y = distance.y;
            switch (mode)
            {
                case ShadowMode.Shadow:
                    ApplyShadowZeroAlloc(verts, ref start, ref end, x, y, alpha);
                    break;
                case ShadowMode.Shadow3:
                    ApplyShadowZeroAlloc(verts, ref start, ref end, x, y, alpha);
                    ApplyShadowZeroAlloc(verts, ref start, ref end, x, 0, alpha);
                    ApplyShadowZeroAlloc(verts, ref start, ref end, 0, y, alpha);
                    break;
                case ShadowMode.Outline:
                    ApplyShadowZeroAlloc(verts, ref start, ref end, x, y, alpha);
                    ApplyShadowZeroAlloc(verts, ref start, ref end, x, -y, alpha);
                    ApplyShadowZeroAlloc(verts, ref start, ref end, -x, y, alpha);
                    ApplyShadowZeroAlloc(verts, ref start, ref end, -x, -y, alpha);
                    break;
                case ShadowMode.Outline8:
                    ApplyShadowZeroAlloc(verts, ref start, ref end, x, y, alpha);
                    ApplyShadowZeroAlloc(verts, ref start, ref end, x, -y, alpha);
                    ApplyShadowZeroAlloc(verts, ref start, ref end, -x, y, alpha);
                    ApplyShadowZeroAlloc(verts, ref start, ref end, -x, -y, alpha);
                    ApplyShadowZeroAlloc(verts, ref start, ref end, x, 0, alpha);
                    ApplyShadowZeroAlloc(verts, ref start, ref end, 0, y, alpha);
                    ApplyShadowZeroAlloc(verts, ref start, ref end, -x, 0, alpha);
                    ApplyShadowZeroAlloc(verts, ref start, ref end, 0, -y, alpha);
                    break;
            }
        }

        /// <summary>
        /// Append shadow vertices.
        /// * It is similar to Shadow component implementation.
        /// </summary>
        private static void ApplyShadowZeroAlloc(List<UIVertex> verts, ref int start, ref int end, float x, float y,
            float alpha)
        {
            var count = end - start;
            for (var i = 0; i < count; i++)
            {
                // The original vertices is pushed backward.
                verts.Add(verts[end - count + i]);

                // Set shadow vertex.
                var vt = verts[start + i];
                vt.position.x += x;
                vt.position.y += y;
                vt.color.a = (byte)(alpha * vt.color.a);
                verts[start + i] = vt;
            }

            // Update next shadow offset.
            start = end;
            end = verts.Count;
        }
    }
}
