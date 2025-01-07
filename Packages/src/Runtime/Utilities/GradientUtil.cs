using System.Collections.Generic;
using UnityEngine;

namespace Coffee.UIEffects
{
    internal static class GradientUtil
    {
        public static void DoHorizontalGradient(List<UIVertex> verts, Color a, Color b, float offset, float scale,
            Rect rect, Matrix4x4 m)
        {
            for (var i = 0; i < verts.Count; i++)
            {
                var vt = verts[i];
                var x = Mathf.InverseLerp(rect.xMin, rect.xMax, m.MultiplyPoint3x4(vt.position).x);
                var t = Mathf.Clamp01(offset + (x - 0.5f) * scale + 0.5f);
                vt.color *= Color.Lerp(a, b, t);
                verts[i] = vt;
            }
        }

        public static void DoHorizontalGradient(List<UIVertex> verts, Gradient gradient, List<float> splitTimes,
            float offset, float scale, Rect rect, Matrix4x4 m)
        {
            var count = verts.Count;
            for (var i = 0; i < verts.Count; i += 6)
            {
                UIVertexUtil.GetQuad(verts, i, out var lb, out var lt, out var rt, out var rb);
                if (i + 5 < count)
                {
                    UIVertexUtil.SplitHorizontalFast(verts, splitTimes, lb, lt, ref rb, ref rt, rect, m);
                }

                lb.color *= Evaluate(gradient, rect, m.MultiplyPoint3x4(lb.position).x + 0.0005f, offset, scale);
                lt.color *= Evaluate(gradient, rect, m.MultiplyPoint3x4(lt.position).x + 0.0005f, offset, scale);
                rt.color *= Evaluate(gradient, rect, m.MultiplyPoint3x4(rt.position).x - 0.0005f, offset, scale);
                rb.color *= Evaluate(gradient, rect, m.MultiplyPoint3x4(rb.position).x - 0.0005f, offset, scale);
                UIVertexUtil.SetQuad(verts, i, lb, lt, rt, rb);
            }

            return;

            static Color Evaluate(Gradient gradient, Rect rect, float t, float offset, float scale)
            {
                t = Mathf.InverseLerp(rect.xMin, rect.xMax, t); // 0-1
                t = Mathf.Repeat((t - offset * scale - (1 - scale) / 2) / scale, 1); // Revert
                return gradient.Evaluate(t);
            }
        }

        public static void DoHorizontalGradientFixed(List<UIVertex> verts, Gradient gradient, float offset, float scale,
            Rect rect, Matrix4x4 m)
        {
            var count = verts.Count;
            for (var i = 0; i < count; i += 3)
            {
                var vt0 = verts[i + 0];
                var vt1 = verts[i + 1];
                var vt2 = verts[i + 2];
                var center = m.MultiplyPoint3x4((vt0.position + vt1.position + vt2.position) / 3).x;
                vt0.color *= Evaluate(gradient, rect, center, offset, scale);
                vt1.color *= Evaluate(gradient, rect, center, offset, scale);
                vt2.color *= Evaluate(gradient, rect, center, offset, scale);
                verts[i + 0] = vt0;
                verts[i + 1] = vt1;
                verts[i + 2] = vt2;
            }

            return;

            static Color Evaluate(Gradient gradient, Rect rect, float t, float offset, float scale)
            {
                t = Mathf.InverseLerp(rect.xMin, rect.xMax, t); // 0-1
                t = Mathf.Repeat((t - offset * scale - (1 - scale) / 2) / scale, 1); // Revert
                return gradient.Evaluate(t);
            }
        }

        public static void DoHorizontalGradient(List<UIVertex> verts, Gradient gradient, float offset, float scale,
            Rect rect, Matrix4x4 m)
        {
            var count = verts.Count;
            for (var i = 0; i < count; i++)
            {
                var vt = verts[i];
                vt.color *= Evaluate(gradient, rect, m.MultiplyPoint3x4(vt.position).x, offset, scale);
                verts[i] = vt;
            }

            return;

            static Color Evaluate(Gradient gradient, Rect rect, float t, float offset, float scale)
            {
                t = Mathf.InverseLerp(rect.xMin, rect.xMax, t); // 0-1
                t = Mathf.Repeat((t - offset * scale - (1 - scale) / 2) / scale, 1); // Revert
                return gradient.Evaluate(t);
            }
        }

        public static void DoVerticalGradient(List<UIVertex> verts, Color a, Color b, float offset, float scale,
            Rect rect, Matrix4x4 m)
        {
            for (var i = 0; i < verts.Count; i++)
            {
                var vt = verts[i];
                var y = Mathf.InverseLerp(rect.yMin, rect.yMax, m.MultiplyPoint3x4(vt.position).y);
                var t = Mathf.Clamp01(offset + (0.5f - y) * scale + 0.5f);
                vt.color *= Color.Lerp(a, b, t);
                verts[i] = vt;
            }
        }

        public static void DoVerticalGradient(List<UIVertex> verts, Gradient gradient, List<float> splitTimes,
            float offset, float scale, Rect rect, Matrix4x4 m)
        {
            var count = verts.Count;
            for (var i = 0; i < verts.Count; i += 6)
            {
                UIVertexUtil.GetQuad(verts, i, out var lb, out var lt, out var rt, out var rb);
                if (i + 5 < count)
                {
                    UIVertexUtil.SplitVerticalFast(verts, splitTimes, lb, ref lt, rb, ref rt, rect, m);
                }

                lb.color *= Evaluate(gradient, rect, m.MultiplyPoint3x4(lb.position).y + 0.0005f, offset, scale);
                lt.color *= Evaluate(gradient, rect, m.MultiplyPoint3x4(lt.position).y - 0.0005f, offset, scale);
                rt.color *= Evaluate(gradient, rect, m.MultiplyPoint3x4(rt.position).y - 0.0005f, offset, scale);
                rb.color *= Evaluate(gradient, rect, m.MultiplyPoint3x4(rb.position).y + 0.0005f, offset, scale);
                UIVertexUtil.SetQuad(verts, i, lb, lt, rt, rb);
            }

            return;

            static Color Evaluate(Gradient gradient, Rect rect, float t, float offset, float scale)
            {
                t = Mathf.InverseLerp(rect.yMin, rect.yMax, t); // 0-1
                t = Mathf.Repeat((t - offset * scale - (1 - scale) / 2) / scale, 1); // Revert
                return gradient.Evaluate(t);
            }
        }

        public static void DoVerticalGradient(List<UIVertex> verts, Gradient gradient, float offset, float scale,
            Rect rect, Matrix4x4 m)
        {
            var count = verts.Count;
            for (var i = 0; i < count; i++)
            {
                var vt = verts[i];
                vt.color *= Evaluate(gradient, rect, m.MultiplyPoint3x4(vt.position).y, offset, scale);
                verts[i] = vt;
            }

            return;

            static Color Evaluate(Gradient gradient, Rect rect, float t, float offset, float scale)
            {
                t = Mathf.InverseLerp(rect.yMin, rect.yMax, t); // 0-1
                t = Mathf.Repeat((t - offset * scale - (1 - scale) / 2) / scale, 1); // Revert
                return gradient.Evaluate(t);
            }
        }

        public static void DoDiagonalGradientToRightBottom(List<UIVertex> verts, Color a, Color b, float offset,
            float scale, Rect rect, Matrix4x4 m)
        {
            for (var i = 0; i < verts.Count; i++)
            {
                var vt = verts[i];
                var x = Mathf.InverseLerp(rect.xMin, rect.xMax, m.MultiplyPoint3x4(vt.position).x);
                var y = Mathf.InverseLerp(rect.yMin, rect.yMax, m.MultiplyPoint3x4(vt.position).y);
                var t = Mathf.Clamp01(0.5f * scale + 0.5f + offset - (1 - x + y) * scale / 2);
                vt.color *= Color.Lerp(a, b, t);
                verts[i] = vt;
            }
        }

        public static void DoDiagonalGradientToLeftBottom(List<UIVertex> verts, Color a, Color b, float offset,
            float scale, Rect rect, Matrix4x4 m)
        {
            for (var i = 0; i < verts.Count; i++)
            {
                var vt = verts[i];
                var x = Mathf.InverseLerp(rect.xMin, rect.xMax, m.MultiplyPoint3x4(vt.position).x);
                var y = Mathf.InverseLerp(rect.yMin, rect.yMax, m.MultiplyPoint3x4(vt.position).y);
                var t = Mathf.Clamp01(0.5f * scale + 0.5f + offset - (x + y) * scale / 2);
                vt.color *= Color.Lerp(a, b, t);
                verts[i] = vt;
            }
        }

        public static void DoRadialGradient(List<UIVertex> verts, Color a, Color b, float offset, float scale,
            Rect rect, Matrix4x4 m, int divide)
        {
            var count = verts.Count;
            for (var i = 0; i < count; i += 6)
            {
                UIVertexUtil.GetQuad(verts, i, out var lb, out var lt, out var rt, out var rb);
                for (var j = 1; j < divide; j++)
                {
                    var min = Mathf.InverseLerp(rect.xMin, rect.xMax, m.MultiplyPoint3x4(lb.position).x);
                    var max = Mathf.InverseLerp(rect.xMin, rect.xMax, m.MultiplyPoint3x4(rt.position).x);
                    var time = 1f - (float)j / divide;
                    if (time <= min || max <= time) continue;

                    UIVertexUtil.SplitHorizontal(verts, lb, lt, ref rb, ref rt, Mathf.InverseLerp(min, max, time));
                }

                UIVertexUtil.SetQuad(verts, i, lb, lt, rt, rb);
            }

            count = verts.Count;
            for (var i = 0; i < verts.Count; i += 6)
            {
                UIVertexUtil.GetQuad(verts, i, out var lb, out var lt, out var rt, out var rb);

                if (i + 5 < count)
                {
                    for (var j = 1; j < divide; j++)
                    {
                        var min = Mathf.InverseLerp(rect.yMin, rect.yMax, m.MultiplyPoint3x4(lb.position).y);
                        var max = Mathf.InverseLerp(rect.yMin, rect.yMax, m.MultiplyPoint3x4(rt.position).y);
                        var time = 1f - (float)j / divide;
                        if (time <= min || max <= time) continue;

                        UIVertexUtil.SplitVertical(verts, lb, ref lt, rb, ref rt, Mathf.InverseLerp(min, max, time));
                    }
                }

                lb.color *= DoGradient(lb, a, b, rect, m, offset, scale);
                lt.color *= DoGradient(lt, a, b, rect, m, offset, scale);
                rt.color *= DoGradient(rt, a, b, rect, m, offset, scale);
                rb.color *= DoGradient(rb, a, b, rect, m, offset, scale);
                UIVertexUtil.SetQuad(verts, i, lb, lt, rt, rb);
            }

            return;

            static Color DoGradient(UIVertex vt, Color a, Color b, Rect rect, Matrix4x4 m, float offset, float scale)
            {
                var x = Mathf.InverseLerp(rect.xMin, rect.xMax, m.MultiplyPoint3x4(vt.position).x) - 0.5f;
                var y = Mathf.InverseLerp(rect.yMin, rect.yMax, m.MultiplyPoint3x4(vt.position).y) - 0.5f;
                var t = Mathf.Clamp01(offset + Mathf.Sqrt(x * x + y * y) * 2 * scale);
                return Color.Lerp(a, b, t);
            }
        }

        public static void DoAngleGradient(List<UIVertex> verts, Gradient gradient, List<float> splitTimes,
            float offset, float scale, Rect rect, Matrix4x4 m)
        {
            UIVertexUtil.SplitAngle(verts, splitTimes, rect, m);
            if (gradient.mode == GradientMode.Fixed)
            {
                DoHorizontalGradientFixed(verts, gradient, offset, scale, rect, m);
            }
            else
            {
                DoHorizontalGradient(verts, gradient, offset, scale, rect, m);
            }
        }

        public static void GetKeyTimes(Gradient gradient, List<float> results)
        {
            results.Clear();
            var colors = gradient.colorKeys;
            for (var i = 0; i < colors.Length; i++)
            {
                AddKeyTime(results, colors[i].time);
            }

            var alphas = gradient.alphaKeys;
            for (var i = 0; i < alphas.Length; i++)
            {
                AddKeyTime(results, alphas[i].time);
            }

            AddKeyTime(results, 0);
            AddKeyTime(results, 1);
            results.Sort((a, b) => b.CompareTo(a));

            return;

            static void AddKeyTime(List<float> results, float time)
            {
                if (!Contains(results, time))
                {
                    results.Add(time);
                }
            }

            static bool Contains(List<float> list, float v)
            {
                for (var i = 0; i < list.Count; i++)
                {
                    if (Mathf.Abs(list[i] - v) < 0.001f) return true;
                }

                return false;
            }
        }

        public static void SplitKeyTimes(List<float> keyTimes, List<float> results, float offset, float scale)
        {
            results.Clear();
            for (var i = 0; i < keyTimes.Count; i++)
            {
                AddSplitTime(results, keyTimes[i], offset, scale);
            }

            results.Sort((a, b) => b.CompareTo(a));

            return;

            static void AddSplitTime(List<float> results, float time, float offset, float scale)
            {
                time = time * scale + offset * scale + (1 - scale) / 2;

                while (0 < time)
                {
                    time -= scale;
                }

                while (time <= 1f)
                {
                    if (0 <= time && !Contains(results, time))
                    {
                        results.Add(time);
                    }

                    time += scale;
                }
            }

            static bool Contains(List<float> list, float v)
            {
                for (var i = 0; i < list.Count; i++)
                {
                    if (Mathf.Abs(list[i] - v) < 0.001f) return true;
                }

                return false;
            }
        }
    }
}
