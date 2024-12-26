using System;
using System.Collections.Generic;
using UnityEngine;

namespace Coffee.UIEffects
{
    internal static class UIVertexUtil
    {
        public static Func<UIVertex, UIVertex, UIVertex, float, UIVertex> onLerpVertex;

        public static void ExpandCapacity(List<UIVertex> verts, int multiplier)
        {
            var capacity = Mathf.NextPowerOfTwo(verts.Count * multiplier);
            if (verts.Capacity < capacity)
            {
                verts.Capacity = capacity;
            }
        }

        public static void Expand(List<UIVertex> verts, int start, int bundleSize, Vector2 expandSize, Rect bounds)
        {
            if (expandSize == Vector2.zero) return;

            // Quad (6 vertices)
            for (var j = 0; j < bundleSize; j += 6)
            {
                var size = default(Vector3);
                var extendPos = default(Vector3);
                var extendUV = default(Vector3);
                var posLB = verts[start + j + 1].position;
                var posRT = verts[start + j + 4].position;
                var willExpand = bundleSize == 6 // Text or simple quad
                                 || !bounds.Contains(posLB)
                                 || !bounds.Contains(posRT); // Outer 9-sliced quad
                if (willExpand)
                {
                    var uvLB = verts[start + j + 1].uv0;
                    var uvRT = verts[start + j + 4].uv0;
                    var posCenter = (posLB + posRT) / 2;
                    var uvCenter = (uvLB + uvRT) / 2;
                    size = posLB - posRT;
                    size.x = 1 + expandSize.x / Mathf.Abs(size.x);
                    size.y = 1 + expandSize.y / Mathf.Abs(size.y);
                    size.z = 1;
                    extendPos = posCenter - Vector3.Scale(size, posCenter);
                    extendUV = uvCenter - Vector4.Scale(size, uvCenter);
                }

                // Set vertex position, uv, uvMask and local normalized position.
                for (var k = 0; k < 6; k++)
                {
                    var vt = verts[start + j + k];
                    var pos = vt.position;
                    var uv0 = vt.uv0;

                    // Expand edge vertex
                    if (willExpand)
                    {
                        if (pos.x < bounds.xMin || bounds.xMax < pos.x)
                        {
                            pos.x = pos.x * size.x + extendPos.x;
                            uv0.x = uv0.x * size.x + extendUV.x;
                        }

                        if (pos.y < bounds.yMin || bounds.yMax < pos.y)
                        {
                            pos.y = pos.y * size.y + extendPos.y;
                            uv0.y = uv0.y * size.y + extendUV.y;
                        }
                    }

                    vt.position = pos;
                    vt.uv0.x = uv0.x;
                    vt.uv0.y = uv0.y;
                    verts[start + j + k] = vt;
                }
            }
        }

        public static UIVertex VertexLerp(UIVertex a, UIVertex b, float t)
        {
            var vt = new UIVertex();
            vt.position = Vector3.Lerp(a.position, b.position, t);
            vt.normal = Vector3.Lerp(a.normal, b.normal, t);
            vt.tangent = Vector4.Lerp(a.tangent, b.tangent, t);
            vt.color = Color.Lerp(a.color, b.color, t);
            vt.uv0 = Vector4.Lerp(a.uv0, b.uv0, t);
            vt.uv1 = Vector4.Lerp(a.uv1, b.uv1, t);
            vt.uv2 = Vector4.Lerp(a.uv2, b.uv2, t);

            if (onLerpVertex != null)
            {
                vt = onLerpVertex(vt, a, b, t);
            }

            return vt;
        }

        public static void GetBounds(List<UIVertex> verts, int start, int bundleSize, out Rect posBounds,
            out Rect uvBounds)
        {
            var minPos = new Vector2(float.MaxValue, float.MaxValue);
            var maxPos = new Vector2(float.MinValue, float.MinValue);
            var minUV = new Vector2(float.MaxValue, float.MaxValue);
            var maxUV = new Vector2(float.MinValue, float.MinValue);
            for (var i = start; i < start + bundleSize; i++)
            {
                var vt = verts[i];
                UpdateMinMax(ref minPos, ref maxPos, vt.position);
                UpdateMinMax(ref minUV, ref maxUV, vt.uv0);
            }

            // Shrink coordinate to avoid uv edge
            posBounds = new Rect(minPos.x + 0.001f, minPos.y + 0.001f,
                maxPos.x - minPos.x - 0.002f, maxPos.y - minPos.y - 0.002f);
            uvBounds = new Rect(minUV.x, minUV.y, maxUV.x - minUV.x, maxUV.y - minUV.y);

            return;

            static void UpdateMinMax(ref Vector2 min, ref Vector2 max, Vector2 value)
            {
                if (value.x < min.x) min.x = value.x; // Left
                if (max.x < value.x) max.x = value.x; // Right
                if (value.y < min.y) min.y = value.y; // Bottom
                if (max.y < value.y) max.y = value.y; // Top
            }
        }

        public static void GetQuad(List<UIVertex> verts, int i, out UIVertex lb, out UIVertex lt, out UIVertex rt,
            out UIVertex rb)
        {
            lb = verts[i];
            lt = verts[i + 1];
            rt = verts[i + 2];
            rb = verts[i + 4];
        }

        public static void SetQuad(List<UIVertex> verts, int i, UIVertex lb, UIVertex lt, UIVertex rt, UIVertex rb)
        {
            verts[i] = verts[i + 5] = lb;
            verts[i + 1] = lt;
            verts[i + 2] = verts[i + 3] = rt;
            verts[i + 4] = rb;
        }

        public static void SplitHorizontal(List<UIVertex> verts, UIVertex lb, UIVertex lt, ref UIVertex rb,
            ref UIVertex rt, Rect rect, List<float> times)
        {
            if (times == null || times.Count == 0) return;

            var min = Mathf.InverseLerp(rect.xMin, rect.xMax, lb.position.x);
            for (var i = 0; i < times.Count; i++)
            {
                var time = times[i];
                if (time <= 0 || 1 <= time) continue;

                var max = Mathf.InverseLerp(rect.xMin, rect.xMax, rt.position.x);
                if (time <= min || max <= time) continue;

                SplitHorizontal(verts, lb, lt, ref rb, ref rt, Mathf.InverseLerp(min, max, time));
            }
        }

        public static void SplitHorizontal(List<UIVertex> verts, UIVertex lb, UIVertex lt, ref UIVertex rb,
            ref UIVertex rt, float time)
        {
            var mb = VertexLerp(lb, rb, time);
            var mt = VertexLerp(lt, rt, time);
            verts.Add(mb);
            verts.Add(mt);
            verts.Add(rt);
            verts.Add(rt);
            verts.Add(rb);
            verts.Add(mb);
            rt = mt;
            rb = mb;
        }

        public static void SplitVertical(List<UIVertex> verts, UIVertex lb, ref UIVertex lt, UIVertex rb,
            ref UIVertex rt, Rect rect, List<float> times)
        {
            if (times == null || times.Count == 0) return;

            var min = Mathf.InverseLerp(rect.yMin, rect.yMax, lb.position.y);
            for (var i = 0; i < times.Count; i++)
            {
                var time = times[i];
                var max = Mathf.InverseLerp(rect.yMin, rect.yMax, rt.position.y);
                if (time <= min || max <= time) continue;

                SplitVertical(verts, lb, ref lt, rb, ref rt, Mathf.InverseLerp(min, max, time));
            }
        }

        public static void SplitVertical(List<UIVertex> verts, UIVertex lb, ref UIVertex lt, UIVertex rb,
            ref UIVertex rt, float time)
        {
            var lm = VertexLerp(lb, lt, time);
            var rm = VertexLerp(rb, rt, time);
            verts.Add(lm);
            verts.Add(lt);
            verts.Add(rt);
            verts.Add(rt);
            verts.Add(rm);
            verts.Add(lm);
            lt = lm;
            rt = rm;
        }
    }
}
