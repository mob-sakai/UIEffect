using System;
using System.Collections.Generic;
using Coffee.UIEffectInternal;
using UnityEngine;
using UnityEngine.UI;

namespace Coffee.UIEffects
{
    internal static class UIVertexUtil
    {
        public static Func<UIVertex, UIVertex, UIVertex, float, UIVertex> onLerpVertex;

        public static void Flip(List<UIVertex> verts, bool horizontal, bool vertical)
        {
            var count = verts.Count;
            for (var i = 0; i < count; i++)
            {
                var vt = verts[i];
                var pos = vt.position;
                vt.position = new Vector3(horizontal ? -pos.x : pos.x, vertical ? -pos.y : pos.y);
                verts[i] = vt;
            }
        }

        public static void Flip(VertexHelper vh, bool horizontal, bool vertical)
        {
            var count = vh.currentVertCount;
            var vt = default(UIVertex);
            for (var i = 0; i < count; i++)
            {
                vh.PopulateUIVertex(ref vt, i);
                var pos = vt.position;
                vt.position = new Vector3(horizontal ? -pos.x : pos.x, vertical ? -pos.y : pos.y);
                vh.SetUIVertex(vt, i);
            }
        }

        public static void ExpandCapacity(List<UIVertex> verts, int multiplier)
        {
            var capacity = Mathf.NextPowerOfTwo(verts.Count * multiplier);
            if (verts.Capacity < capacity)
            {
                verts.Capacity = capacity;
            }
        }

        public static void Expand(List<UIVertex> verts, int start, int bundleSize, Vector4 expandSize, Rect bounds)
        {
            if (expandSize == Vector4.zero) return;

            for (var j = 0; j < bundleSize; j += 3)
            {
                if (bounds.Contains(verts[start + j + 0].position)
                    && bounds.Contains(verts[start + j + 1].position)
                    && bounds.Contains(verts[start + j + 2].position))
                {
                    continue;
                }

                GetBounds(verts, start + j, 3, out var posBounds, out var uvBounds);
                var posCenter = (Vector4)posBounds.center;
                posCenter.z = posCenter.x;
                posCenter.w = posCenter.y;
                var uvCenter = (Vector4)uvBounds.center;
                uvCenter.z = uvCenter.x;
                uvCenter.w = uvCenter.y;
                var size = (Vector4)posBounds.size;
                size.z = 1 + expandSize.z / Mathf.Abs(size.x);
                size.w = 1 + expandSize.w / Mathf.Abs(size.y);
                size.x = 1 + expandSize.x / Mathf.Abs(size.x);
                size.y = 1 + expandSize.y / Mathf.Abs(size.y);
                var extendPos = posCenter - Vector4.Scale(size, posCenter);
                var extendUV = uvCenter - Vector4.Scale(size, uvCenter);

                // Set vertex position, uv, uvMask and local normalized position.
                for (var k = 0; k < 3; k++)
                {
                    var vt = verts[start + j + k];
                    var pos = vt.position;
                    var uv0 = vt.uv0;

                    // Expand edge vertex
                    if (pos.x < bounds.xMin)
                    {
                        pos.x = pos.x * size.x + extendPos.x;
                        uv0.x = uv0.x * size.x + extendUV.x;
                    }
                    else if (bounds.xMax < pos.x)
                    {
                        pos.x = pos.x * size.z + extendPos.z;
                        uv0.x = uv0.x * size.z + extendUV.z;
                    }

                    if (pos.y < bounds.yMin)
                    {
                        pos.y = pos.y * size.y + extendPos.y;
                        uv0.y = uv0.y * size.y + extendUV.y;
                    }
                    else if (bounds.yMax < pos.y)
                    {
                        pos.y = pos.y * size.w + extendPos.w;
                        uv0.y = uv0.y * size.w + extendUV.w;
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
            var vt = new UIVertex
            {
                position = Vector3.Lerp(a.position, b.position, t),
                normal = Vector3.Lerp(a.normal, b.normal, t),
                tangent = Vector4.Lerp(a.tangent, b.tangent, t),
                color = Color.Lerp(a.color, b.color, t),
                uv0 = Vector4.Lerp(a.uv0, b.uv0, t),
                uv1 = Vector4.Lerp(a.uv1, b.uv1, t),
                uv2 = Vector4.Lerp(a.uv2, b.uv2, t)
            };

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

        public static void SplitHorizontalFast(List<UIVertex> verts, List<float> times, UIVertex lb, UIVertex lt,
            ref UIVertex rb, ref UIVertex rt, Rect rect, Matrix4x4 m)
        {
            if (times == null || times.Count == 0) return;

            var min = Mathf.InverseLerp(rect.xMin, rect.xMax, m.MultiplyPoint3x4(lb.position).x);
            for (var i = 0; i < times.Count; i++)
            {
                var time = times[i];
                if (time <= 0 || 1 <= time) continue;

                var max = Mathf.InverseLerp(rect.xMin, rect.xMax, m.MultiplyPoint3x4(rt.position).x);
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

        public static void SplitVerticalFast(List<UIVertex> verts, List<float> times, UIVertex lb, ref UIVertex lt,
            UIVertex rb, ref UIVertex rt, Rect rect, Matrix4x4 m)
        {
            if (times == null || times.Count == 0) return;

            var min = Mathf.InverseLerp(rect.yMin, rect.yMax, m.MultiplyPoint3x4(lb.position).y);
            for (var i = 0; i < times.Count; i++)
            {
                var time = times[i];
                var max = Mathf.InverseLerp(rect.yMin, rect.yMax, m.MultiplyPoint3x4(rt.position).y);
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

        public static void SplitAngle(List<UIVertex> verts, List<float> times, Rect rect, Matrix4x4 m)
        {
            if (times == null || times.Count == 0) return;

            for (var i = 0; i < times.Count; i++)
            {
                var x = Mathf.Lerp(rect.xMin, rect.xMax, times[i]);
                Split(verts, new Vector2(x, 0), new Vector2(0, 1), m);
            }

            return;

            static void Split(List<UIVertex> verts, Vector2 p, Vector2 dir, Matrix4x4 m)
            {
                // Each triangle
                var intersections = InternalListPool<UIVertex>.Rent();
                var count = verts.Count;
                for (var i = 0; i < count; i += 3)
                {
                    intersections.Clear();
                    var n = -1;
                    // Each edge
                    for (var j = 0; j < 3; j++)
                    {
                        var start = verts[i + j];
                        var end = verts[i + (j + 1) % 3];

                        if (DoesIntersect(start, end, p, dir, m, out var intersection))
                        {
                            intersections.Add(intersection);
                        }
                        else
                        {
                            n = j;
                        }
                    }

                    if (intersections.Count == 2)
                    {
                        var vt0 = verts[i + (n + 2) % 3];
                        var vt1 = verts[i + (n + 0) % 3];
                        var vt2 = verts[i + (n + 1) % 3];
                        var it0 = intersections[0];
                        var it1 = intersections[1];
                        var it2 = intersections[(n + 1) % 2];
                        verts[i + 0] = vt0;
                        verts[i + 1] = it0;
                        verts[i + 2] = it1;
                        verts.Add(it2);
                        verts.Add(vt1);
                        verts.Add(vt2);
                        verts.Add(vt2);
                        verts.Add(it0);
                        verts.Add(it1);
                    }
                }

                InternalListPool<UIVertex>.Return(ref intersections);
            }

            static bool DoesIntersect(UIVertex start, UIVertex end, Vector2 p, Vector2 dir, Matrix4x4 m,
                out UIVertex intersection)
            {
                intersection = default;
                var startPosition = (Vector2)m.MultiplyPoint3x4(start.position);
                var endPosition = (Vector2)m.MultiplyPoint3x4(end.position);
                var segDir = endPosition - startPosition;
                var cross = Vector3.Cross(segDir, dir);

                // Parallel.
                if (cross.magnitude < Mathf.Epsilon)
                {
                    return false; // 線と線分が平行
                }

                var diff = p - startPosition;
                var determinant = Vector3.Dot(cross, cross);
                var t = Vector3.Dot(Vector3.Cross(diff, dir), cross) / determinant;

                if (0 <= t && t <= 1)
                {
                    intersection = VertexLerp(start, end, t);
                    return true;
                }

                return false;
            }
        }
    }
}
