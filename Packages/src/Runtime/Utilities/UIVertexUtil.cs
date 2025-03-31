using System;
using System.Collections.Generic;
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
    }
}
