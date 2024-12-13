using UnityEngine;

namespace Coffee.UIEffectInternal
{
    public enum TransformSensitivity
    {
        Low,
        Medium,
        High
    }

    /// <summary>
    /// Extension methods for Transform class.
    /// </summary>
    internal static class TransformExtensionsForTransformSensitivity
    {
        private const float k_DefaultEpsilon = 1f / (2 ^ 8);

        /// <summary>
        /// Check if a transform has changed.
        /// </summary>
        public static bool HasChanged(this Transform self, ref Matrix4x4 prev, TransformSensitivity sensitivity)
        {
            return self.HasChanged_Internal(null, ref prev, Convert(sensitivity));
        }

        /// <summary>
        /// Check if a transform has changed.
        /// </summary>
        public static bool HasChanged(this Transform self, Transform baseTransform, ref Matrix4x4 prev,
            TransformSensitivity sensitivity)
        {
            return self.HasChanged_Internal(baseTransform, ref prev, Convert(sensitivity));
        }

        private static float Convert(TransformSensitivity self)
        {
            switch (self)
            {
                case TransformSensitivity.Low: return 1f / (1 << 4);
                case TransformSensitivity.Medium: return 1f / (1 << 8);
                case TransformSensitivity.High: return 1f / (1 << 12);
                default: return 1f / (1 << (int)self);
            }
        }

        private static bool HasChanged_Internal(this Transform self, Transform baseTransform, ref Matrix4x4 prev,
            float epsilon)
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
    }
}
