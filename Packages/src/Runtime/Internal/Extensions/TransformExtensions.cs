using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Profiling;

namespace Coffee.UIEffectInternal
{
    /// <summary>
    /// Extension methods for Transform class.
    /// </summary>
    internal static class TransformExtensions
    {
        private const float k_DefaultEpsilon = 1f / (2 ^ 8);
        private static readonly Vector3[] s_Corners = new Vector3[4];

        /// <summary>
        /// Compare the hierarchy index of one transform with another transform.
        /// </summary>
        public static int CompareHierarchyIndex(this Transform self, Transform other, Transform stopAt)
        {
            if (self == other) return 0;

            Profiler.BeginSample("(COF)[TransformExt] CompareHierarchyIndex > GetTransforms");
            var lTrs = self.GetTransforms(stopAt, ListPool<Transform>.Rent());
            var rTrs = other.GetTransforms(stopAt, ListPool<Transform>.Rent());
            Profiler.EndSample();

            Profiler.BeginSample("(COF)[TransformExt] CompareHierarchyIndex > Calc");
            var loop = Mathf.Max(lTrs.Count, rTrs.Count);
            var result = 0;
            for (var i = 0; i < loop; ++i)
            {
                var selfIndex = 0 <= lTrs.Count - i - 1 ? lTrs[lTrs.Count - i - 1].GetSiblingIndex() : -1;
                var otherIndex = 0 <= rTrs.Count - i - 1 ? rTrs[rTrs.Count - i - 1].GetSiblingIndex() : -1;
                if (selfIndex == otherIndex) continue;

                result = selfIndex - otherIndex;
                break;
            }

            Profiler.EndSample();

            Profiler.BeginSample("(COF)[TransformExt] CompareHierarchyIndex > Return");
            ListPool<Transform>.Return(ref lTrs);
            ListPool<Transform>.Return(ref rTrs);
            Profiler.EndSample();

            return result;
        }

        private static List<Transform> GetTransforms(this Transform self, Transform stopAt, List<Transform> results)
        {
            results.Clear();
            while (self != stopAt)
            {
                results.Add(self);
                self = self.parent;
            }

            return results;
        }

        /// <summary>
        /// Check if a transform has changed.
        /// </summary>
        public static bool HasChanged(this Transform self, ref Matrix4x4 prev, float epsilon = k_DefaultEpsilon)
        {
            return self.HasChanged(null, ref prev, epsilon);
        }

        /// <summary>
        /// Check if a transform has changed.
        /// </summary>
        public static bool HasChanged(this Transform self, Transform baseTransform, ref Matrix4x4 prev,
            float epsilon = k_DefaultEpsilon)
        {
            if (!self) return false;

            var hash = baseTransform ? baseTransform.GetHashCode() : 0;
            if (FrameCache.TryGet(self, nameof(HasChanged), hash, out bool result)) return result;

            var matrix = baseTransform
                ? baseTransform.worldToLocalMatrix * self.localToWorldMatrix
                : self.localToWorldMatrix;
            var current = matrix * Matrix4x4.Scale(Vector3.one * 10000);
            result = !Approximately(current, prev, epsilon);
            FrameCache.Set(self, nameof(HasChanged), hash, result);
            if (result)
            {
                prev = current;
            }

            return result;
        }

        private static bool Approximately(Matrix4x4 self, Matrix4x4 other, float epsilon = k_DefaultEpsilon)
        {
            for (var i = 0; i < 16; i++)
            {
                if (epsilon < Mathf.Abs(self[i] - other[i]))
                {
                    return false;
                }
            }

            return true;
        }

        public static Bounds GetRelativeBounds(this Transform self, Transform child)
        {
            if (!self || !child)
            {
                return new Bounds(Vector3.zero, Vector3.zero);
            }

            var list = ListPool<RectTransform>.Rent();
            child.GetComponentsInChildren(false, list);
            if (list.Count == 0)
            {
                ListPool<RectTransform>.Return(ref list);
                return new Bounds(Vector3.zero, Vector3.zero);
            }

            var max = new Vector3(float.MaxValue, float.MaxValue, float.MaxValue);
            var min = new Vector3(float.MinValue, float.MinValue, float.MinValue);
            var worldToLocalMatrix = self.worldToLocalMatrix;
            for (var i = 0; i < list.Count; i++)
            {
                list[i].GetWorldCorners(s_Corners);
                for (var j = 0; j < 4; j++)
                {
                    var lhs = worldToLocalMatrix.MultiplyPoint3x4(s_Corners[j]);
                    max = Vector3.Min(lhs, max);
                    min = Vector3.Max(lhs, min);
                }
            }

            ListPool<RectTransform>.Return(ref list);

            var rectTransformBounds = new Bounds(max, Vector3.zero);
            rectTransformBounds.Encapsulate(min);
            return rectTransformBounds;
        }
    }
}
