using System;
using System.Collections.Generic;
using UnityEngine;

namespace Coffee.UIEffects
{
    public static class GradientUtil
    {
        private static readonly List<float> s_KeyTimes = new List<float>();

        public static void DoGradient(GradationMode mode, List<UIVertex> verts, Color a, Color b, Gradient gradient,
            float offset, float scale, Rect rect)
        {
            switch (mode)
            {
                case GradationMode.Horizontal:
                    DoHorizontalGradient(verts, a, b, offset, scale, rect);
                    break;
                case GradationMode.Vertical:
                    DoVerticalGradient(verts, a, b, offset, scale, rect);
                    break;
                case GradationMode.DiagonalLeftTopToRightBottom:
                    DoDiagonalGradientLeftTopToRightBottom(verts, a, b, offset, scale, rect);
                    break;
                case GradationMode.DiagonalRightTopToLeftBottom:
                    DoDiagonalGradientRightTopToLeftBottom(verts, a, b, offset, scale, rect);
                    break;
                case GradationMode.RadialFast:
                    DoRadialGradient(verts, a, b, offset, scale, rect, 4);
                    break;
                case GradationMode.RadialDetail:
                    DoRadialGradient(verts, a, b, offset, scale, rect, 12);
                    break;
                case GradationMode.HorizontalGradient:
                    DoHorizontalGradient(verts, gradient, offset, scale, rect);
                    break;
                case GradationMode.VerticalGradient:
                    DoVerticalGradient(verts, gradient, offset, scale, rect);
                    break;
            }
        }

        private static void DoHorizontalGradient(List<UIVertex> verts, Color a, Color b, float offset, float scale,
            Rect rect)
        {
            DoGradient(verts, a, b, rect,
                (x, y) => Mathf.Clamp01(offset + (x - 0.5f) * scale + 0.5f));
        }

        private static void DoHorizontalGradient(List<UIVertex> verts, Gradient gradient, float offset, float scale,
            Rect rect)
        {
            GetKeyTimes(gradient, s_KeyTimes, offset, scale);
            var count = verts.Count;
            for (var i = 0; i < verts.Count; i += 6)
            {
                GetBundle(verts, i, out var lb, out var lt, out var rt, out var rb);
                if (i + 5 < count)
                {
                    SplitHorizontal(verts, lb, lt, ref rb, ref rt, rect, s_KeyTimes);
                }

                lb.color *= EvaluateHorizontal(gradient, rect, lb.position.x + 0.0001f, offset, scale);
                lt.color *= EvaluateHorizontal(gradient, rect, lt.position.x + 0.0001f, offset, scale);
                rt.color *= EvaluateHorizontal(gradient, rect, rt.position.x - 0.0001f, offset, scale);
                rb.color *= EvaluateHorizontal(gradient, rect, rb.position.x - 0.0001f, offset, scale);
                SetBundle(verts, i, lb, lt, rt, rb);
            }

            return;

            static Color EvaluateHorizontal(Gradient gradient, Rect rect, float t, float offset, float scale)
            {
                t = Mathf.InverseLerp(rect.xMin, rect.xMax, t);
                t = Mathf.Repeat(t * scale - 1 + scale - scale * offset, 1);
                return gradient.Evaluate(t);
            }
        }

        private static void DoVerticalGradient(List<UIVertex> verts, Color a, Color b, float offset, float scale,
            Rect rect)
        {
            DoGradient(verts, a, b, rect,
                (x, y) => Mathf.Clamp01(offset + (0.5f - y) * scale + 0.5f));
        }

        private static void DoVerticalGradient(List<UIVertex> verts, Gradient gradient, float offset, float scale,
            Rect rect)
        {
            GetKeyTimes(gradient, s_KeyTimes, offset, scale);
            var count = verts.Count;
            for (var i = 0; i < verts.Count; i += 6)
            {
                GetBundle(verts, i, out var lb, out var lt, out var rt, out var rb);
                if (i + 5 < count)
                {
                    SplitVertical(verts, lb, ref lt, rb, ref rt, rect, s_KeyTimes);
                }

                lb.color *= EvaluateVertical(gradient, rect, lb.position.y + 0.0001f, offset, scale);
                lt.color *= EvaluateVertical(gradient, rect, lt.position.y + 0.0001f, offset, scale);
                rt.color *= EvaluateVertical(gradient, rect, rt.position.y - 0.0001f, offset, scale);
                rb.color *= EvaluateVertical(gradient, rect, rb.position.y - 0.0001f, offset, scale);
                SetBundle(verts, i, lb, lt, rt, rb);
            }

            return;

            static Color EvaluateVertical(Gradient gradient, Rect rect, float t, float offset, float scale)
            {
                t = Mathf.InverseLerp(rect.yMin, rect.yMax, t);
                t = Mathf.Repeat(t * scale - 1 + scale - scale * offset, 1);
                return gradient.Evaluate(t);
            }
        }

        private static void DoDiagonalGradientLeftTopToRightBottom(List<UIVertex> verts, Color a, Color b, float offset,
            float scale,
            Rect rect)
        {
            DoGradient(verts, a, b, rect,
                (x, y) => Mathf.Clamp01(0.5f * scale + 0.5f + offset - (1 - x + y) * scale / 2));
        }

        private static void DoDiagonalGradientRightTopToLeftBottom(List<UIVertex> verts, Color a, Color b, float offset,
            float scale,
            Rect rect)
        {
            DoGradient(verts, a, b, rect,
                (x, y) => Mathf.Clamp01(0.5f * scale + 0.5f + offset - (x + y) * scale / 2));
        }

        private static void DoRadialGradient(List<UIVertex> verts, Color a, Color b, float offset, float scale,
            Rect rect, int divide)
        {
            var count = verts.Count;
            for (var i = 0; i < count; i += 6)
            {
                GetBundle(verts, i, out var lb, out var lt, out var rt, out var rb);
                for (var j = 1; j < divide; j++)
                {
                    SplitHorizontal(verts, lb, lt, ref rb, ref rt, rect, 1f - (float)j / divide);
                }

                SetBundle(verts, i, lb, lt, rt, rb);
            }

            count = verts.Count;
            for (var i = 0; i < verts.Count; i += 6)
            {
                GetBundle(verts, i, out var lb, out var lt, out var rt, out var rb);

                if (i + 5 < count)
                {
                    for (var j = 1; j < divide; j++)
                    {
                        SplitVertical(verts, lb, ref lt, rb, ref rt, rect, 1f - (float)j / divide);
                    }
                }

                lb.color *= DoGradient(lb, a, b, rect, Rad);
                lt.color *= DoGradient(lt, a, b, rect, Rad);
                rt.color *= DoGradient(rt, a, b, rect, Rad);
                rb.color *= DoGradient(rb, a, b, rect, Rad);
                SetBundle(verts, i, lb, lt, rt, rb);
                continue;

                float Rad(float x, float y)
                {
                    x -= 0.5f;
                    y -= 0.5f;
                    return Mathf.Clamp01(offset + Mathf.Sqrt(x * x + y * y) * 2 * scale);
                }
            }
        }

        private static void DoGradient(List<UIVertex> verts, Color a, Color b, Rect rect,
            Func<float, float, float> getTime)
        {
            for (var i = 0; i < verts.Count; i++)
            {
                var vt = verts[i];
                vt.color *= DoGradient(vt, a, b, rect, getTime);
                verts[i] = vt;
            }
        }

        private static Color DoGradient(UIVertex vt, Color a, Color b, Rect rect, Func<float, float, float> getTime)
        {
            var x = Mathf.InverseLerp(rect.xMin, rect.xMax, vt.position.x);
            var y = Mathf.InverseLerp(rect.yMin, rect.yMax, vt.position.y);
            return Color.Lerp(a, b, getTime(x, y));
        }

        private static void SplitHorizontal(List<UIVertex> verts, UIVertex lb, UIVertex lt, ref UIVertex rb,
            ref UIVertex rt, Rect rect, float t)
        {
            var min = Mathf.InverseLerp(rect.xMin, rect.xMax, lb.position.x);
            var max = Mathf.InverseLerp(rect.xMin, rect.xMax, rt.position.x);
            SplitHorizontal(verts, lb, lt, ref rb, ref rt, min, max, t);
        }

        private static void SplitHorizontal(List<UIVertex> verts, UIVertex lb, UIVertex lt, ref UIVertex rb,
            ref UIVertex rt, Rect rect, List<float> keyTimes)
        {
            if (keyTimes == null || keyTimes.Count == 0) return;

            var min = Mathf.InverseLerp(rect.xMin, rect.xMax, lb.position.x);
            foreach (var keyTime in keyTimes)
            {
                if (keyTime <= 0 || 1 <= keyTime) continue;

                var max = Mathf.InverseLerp(rect.xMin, rect.xMax, rt.position.x);
                SplitHorizontal(verts, lb, lt, ref rb, ref rt, min, max, keyTime);
            }
        }

        private static void SplitHorizontal(List<UIVertex> verts, UIVertex lb, UIVertex lt, ref UIVertex rb,
            ref UIVertex rt, float min, float max, float t)
        {
            if (t <= min || max <= t) return;

            t = Mathf.InverseLerp(min, max, t);
            var mb = VertexLerp(lb, rb, t);
            var mt = VertexLerp(lt, rt, t);
            verts.Add(mb);
            verts.Add(mt);
            verts.Add(rt);
            verts.Add(rt);
            verts.Add(rb);
            verts.Add(mb);
            rt = mt;
            rb = mb;
        }

        private static void SplitVertical(List<UIVertex> verts, UIVertex lb, ref UIVertex lt, UIVertex rb,
            ref UIVertex rt, Rect rect, float t)
        {
            var min = Mathf.InverseLerp(rect.yMin, rect.yMax, lb.position.y);
            var max = Mathf.InverseLerp(rect.yMin, rect.yMax, rt.position.y);
            SplitVertical(verts, lb, ref lt, rb, ref rt, min, max, t);
        }

        private static void SplitVertical(List<UIVertex> verts, UIVertex lb, ref UIVertex lt, UIVertex rb,
            ref UIVertex rt, Rect rect, List<float> keyTimes)
        {
            var min = Mathf.InverseLerp(rect.yMin, rect.yMax, lb.position.y);
            foreach (var keyTime in keyTimes)
            {
                var max = Mathf.InverseLerp(rect.yMin, rect.yMax, rt.position.y);
                SplitVertical(verts, lb, ref lt, rb, ref rt, min, max, keyTime);
            }
        }

        private static void SplitVertical(List<UIVertex> verts, UIVertex lb, ref UIVertex lt, UIVertex rb,
            ref UIVertex rt, float min, float max, float t)
        {
            if (t <= min || max <= t) return;

            t = Mathf.InverseLerp(min, max, t);
            var lm = VertexLerp(lb, lt, t);
            var rm = VertexLerp(rb, rt, t);
            verts.Add(lm);
            verts.Add(lt);
            verts.Add(rt);
            verts.Add(rt);
            verts.Add(rm);
            verts.Add(lm);
            lt = lm;
            rt = rm;
        }

        private static void GetKeyTimes(Gradient gradient, List<float> results, float offset, float scale)
        {
            results.Clear();
            var colors = gradient.colorKeys;
            for (var i = 0; i < colors.Length; i++)
            {
                AddKeyTime(results, colors[i].time, offset, scale);
            }

            var alphas = gradient.alphaKeys;
            for (var i = 0; i < alphas.Length; i++)
            {
                AddKeyTime(results, alphas[i].time, offset, scale);
            }

            AddKeyTime(results, 0, offset, scale);
            AddKeyTime(results, 1, offset, scale);
            results.Sort((a, b) => b.CompareTo(a));
        }

        private static void AddKeyTime(List<float> results, float value, float offset, float scale)
        {
            scale = 1f / scale;
            value = value * scale + offset - 1;
            while (value < 1f)
            {
                if (0 < value && results.FindIndex(x => Mathf.Abs(x - value) < 0.001f) < 0)
                {
                    results.Add(value);
                }

                value += scale;
            }
        }

        private static void GetBundle(List<UIVertex> verts, int i, out UIVertex lb, out UIVertex lt, out UIVertex rt,
            out UIVertex rb)
        {
            lb = verts[i];
            lt = verts[i + 1];
            rt = verts[i + 2];
            rb = verts[i + 4];
        }

        private static void SetBundle(List<UIVertex> verts, int i, UIVertex lb, UIVertex lt, UIVertex rt, UIVertex rb)
        {
            verts[i] = verts[i + 5] = lb;
            verts[i + 1] = lt;
            verts[i + 2] = verts[i + 3] = rt;
            verts[i + 4] = rb;
        }

        private static UIVertex VertexLerp(UIVertex a, UIVertex b, float t)
        {
            var vt = new UIVertex();
            vt.position = Vector3.Lerp(a.position, b.position, t);
            vt.normal = Vector3.Lerp(a.normal, b.normal, t);
            vt.tangent = Vector4.Lerp(a.tangent, b.tangent, t);
            vt.color = Color.Lerp(a.color, b.color, t);
            vt.uv0 = Vector4.Lerp(a.uv0, b.uv0, t);
            vt.uv1 = Vector4.Lerp(a.uv1, b.uv1, t);
            vt.uv2 = Vector4.Lerp(a.uv2, b.uv2, t);
            return vt;
        }
    }
}
