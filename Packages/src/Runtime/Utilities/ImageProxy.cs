using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

namespace Coffee.UIEffects
{
    internal class ImageProxy : GraphicProxy
    {
#if UNITY_EDITOR
        [InitializeOnLoadMethod]
#endif
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void InitializeOnLoad()
        {
            Register(new ImageProxy());
        }

        /// <summary>
        /// Check if the graphic is valid for this proxy.
        /// </summary>
        protected override bool IsValid(Graphic graphic)
        {
            if (!graphic) return false;
            if (graphic is Image) return true;
            return false;
        }

        /// <summary>
        /// Check if the graphic is a text.
        /// </summary>
        public override bool IsText(Graphic graphic)
        {
            return false;
        }

        public override Vector4 ModifyExpandSize(Graphic graphic, Vector2 expandSize)
        {
            var image = graphic as Image;
            var ret = new Vector4(expandSize.x, expandSize.y, expandSize.x, expandSize.y);

            if (image
                && image.type == Image.Type.Filled
                && image.fillMethod == Image.FillMethod.Radial360
                && image.fillAmount <= 0.5f)
            {
                ret[(image.fillOrigin + (image.fillClockwise ? 2 : 0)) % 4] = 0;

                if (image.fillAmount <= 0.25f)
                {
                    ret[(image.fillOrigin + 3) % 4] = 0;
                }
            }

            return ret;
        }
    }
}
