using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Coffee.UIEffects
{
    [DisallowMultipleComponent]
    [AddComponentMenu("UI/UIEffects/UIFlip", 102)]
    public class UIFlip : BaseMeshEffect
    {
        [Tooltip("Flip horizontally.")] [SerializeField]
        private bool m_Horizontal = false;

        [Tooltip("Flip vertically.")] [SerializeField]
        private bool m_Veritical = false;

        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="Coffee.UIEffects.UIFlip"/> should be flipped horizontally.
        /// </summary>
        /// <value><c>true</c> if be flipped horizontally; otherwise, <c>false</c>.</value>
        public bool horizontal
        {
            get { return m_Horizontal; }
            set
            {
                if (m_Horizontal == value) return;
                m_Horizontal = value;
                SetEffectParamsDirty();
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="Coffee.UIEffects.UIFlip"/> should be flipped vertically.
        /// </summary>
        /// <value><c>true</c> if be flipped horizontally; otherwise, <c>false</c>.</value>
        public bool vertical
        {
            get { return m_Veritical; }
            set
            {
                if (m_Veritical == value) return;
                m_Veritical = value;
                SetEffectParamsDirty();
            }
        }

        /// <summary>
        /// Call used to modify mesh.
        /// </summary>
        /// <param name="vh">VertexHelper.</param>
        public override void ModifyMesh(VertexHelper vh, Graphic graphic)
        {
            if (!isActiveAndEnabled) return;

            var pivot = rectTransform.pivot;
            var rect = rectTransform.rect;
            var width = rect.size.x;
            var height = rect.size.y;
            var vt = default(UIVertex);
            for (var i = 0; i < vh.currentVertCount; i++)
            {
                vh.PopulateUIVertex(ref vt, i);
                var pos = vt.position;
                if (horizontal)
                {
                    // pivot = 0.5, x = -x;
                    // pivot = 0, x = width - x;
                    // pivot = 1, x = -width - x;
                    pos.x = (0.5f - pivot.x) * 2 * width - pos.x;
                }

                if (vertical)
                {
                    pos.y = (0.5f - pivot.y) * 2 * height - pos.y;
                }
                vt.position = pos;
                vh.SetUIVertex(vt, i);
            }
        }
    }
}
