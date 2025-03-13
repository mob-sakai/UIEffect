using Coffee.UIEffectInternal;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace Coffee.UIEffects
{
    [DisallowMultipleComponent]
    [RequireComponent(typeof(Graphic))]
    public class UIFlip : UIBehaviour, IMeshModifier
    {
        [Tooltip("Flip horizontally.")]
        [SerializeField]
        private bool m_Horizontal = false;

        [FormerlySerializedAs("m_Veritical")]
        [Tooltip("Flip vertically.")]
        [SerializeField]
        private bool m_Vertical = false;

        private Graphic _graphic;
        public Graphic graphic => _graphic ? _graphic : _graphic = GetComponent<Graphic>();

        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="Coffee.UIEffects.UIFlip"/> should be flipped horizontally.
        /// </summary>
        /// <value><c>true</c> if be flipped horizontally; otherwise, <c>false</c>.</value>
        public bool horizontal
        {
            get => m_Horizontal;
            set
            {
                if (m_Horizontal == value) return;
                m_Horizontal = value;
                SetVerticesDirty();
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="Coffee.UIEffects.UIFlip"/> should be flipped vertically.
        /// </summary>
        /// <value><c>true</c> if be flipped horizontally; otherwise, <c>false</c>.</value>
        public bool vertical
        {
            get => m_Vertical;
            set
            {
                if (m_Vertical == value) return;
                m_Vertical = value;
                SetVerticesDirty();
            }
        }

        protected override void OnEnable()
        {
            SetVerticesDirty();
        }

        protected override void OnDisable()
        {
            SetVerticesDirty();
        }

        public void SetVerticesDirty()
        {
            if (graphic)
            {
                graphic.SetVerticesDirty();
                GraphicProxy.Find(graphic).SetVerticesDirty(graphic, enabled);
                Misc.QueuePlayerLoopUpdate();
            }
        }

        public void ModifyMesh(Mesh mesh)
        {
        }

        public void ModifyMesh(VertexHelper vh)
        {
            if (!isActiveAndEnabled || !graphic || !graphic.IsActive() || (!horizontal && !vertical)) return;

            var count = vh.currentVertCount;
            var vt = default(UIVertex);
            for (var i = 0; i < count; i++)
            {
                vh.PopulateUIVertex(ref vt, i);
                var pos = vt.position;
                vt.position = new Vector3(m_Horizontal ? -pos.x : pos.x, m_Vertical ? -pos.y : pos.y);
                vh.SetUIVertex(vt, i);
            }
        }

#if UNITY_EDITOR
        protected override void OnValidate()
        {
            SetVerticesDirty();
        }
#endif
    }
}
