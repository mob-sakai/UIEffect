using Coffee.UIEffectInternal;
using UnityEngine;

namespace Coffee.UIEffects
{
    [Icon("Packages/com.coffee.ui-effect/Editor/UIEffectIconIcon.png")]
    public class UIEffectReplica : UIEffectBase
    {
        [SerializeField] private UIEffect m_Target;
        [SerializeField] private bool m_UseTargetTransform = true;

        [PowerRange(0.01f, 100, 10f)]
        [SerializeField]
        protected float m_SamplingScale = 1f;

        [SerializeField]
        protected bool m_AllowToModifyMeshShape = true;

        [SerializeField]
        protected RectTransform m_CustomRoot = null;

        private UIEffect _currentTarget;

        public UIEffect target
        {
            get => m_Target;
            set
            {
                if (m_Target == value) return;
                m_Target = value;
                RefreshTarget(m_Target);
                SetVerticesDirty();
                SetMaterialDirty();
            }
        }

        public bool useTargetTransform
        {
            get => m_UseTargetTransform;
            set
            {
                if (m_UseTargetTransform == value) return;
                m_UseTargetTransform = value;
                SetVerticesDirty();
            }
        }

        public RectTransform customRoot
        {
            get => m_CustomRoot;
            set
            {
                if (m_CustomRoot == value) return;
                m_CustomRoot = value;
                SetVerticesDirty();
            }
        }

        public float samplingScale
        {
            get => m_SamplingScale;
            set
            {
                value = Mathf.Clamp(value, 0.01f, 100);
                if (Mathf.Approximately(m_SamplingScale, value)) return;
                m_SamplingScale = value;
                SetMaterialDirty();
            }
        }

        public bool allowToModifyMeshShape
        {
            get => m_AllowToModifyMeshShape;
            set
            {
                if (m_AllowToModifyMeshShape == value) return;
                m_AllowToModifyMeshShape = value;
                SetVerticesDirty();
            }
        }

        public override Flip flip => target ? target.flip : 0;

        public override float actualSamplingScale => Mathf.Clamp(m_SamplingScale, 0.01f, 100);

        public override bool canModifyShape => m_AllowToModifyMeshShape;

        public override uint effectId => target ? target.effectId : 0;

        public override UIEffectContext context
        {
            get
            {
                if (!target) return null;
                if (!isTargetInScene) return base.context;

                return target.isActiveAndEnabled ? target.context : null;
            }
        }

        public override RectTransform transitionRoot => useTargetTransform && isTargetInScene
            ? target.transitionRoot
            : m_CustomRoot
                ? m_CustomRoot
                : transform as RectTransform;

        private bool isTargetInScene => target && target.gameObject.scene.IsValid();

        protected override void OnEnable()
        {
            RefreshTarget(target);
            base.OnEnable();
        }

        protected override void OnDisable()
        {
            RefreshTarget(null);
            base.OnDisable();
        }

        protected override void OnDestroy()
        {
            _currentTarget = null;
            base.OnDestroy();
        }

#if UNITY_EDITOR
        protected override void OnValidate()
        {
            RefreshTarget(target);
            base.OnValidate();
        }

        internal override void SetEnablePreviewIfSelected(GameObject[] selection)
        {
            if (!isTargetInScene) return;
            target.SetEnablePreviewIfSelected(selection);
        }
#endif

        private void RefreshTarget(UIEffect newTarget)
        {
            if (_currentTarget == newTarget) return;
            if (_currentTarget)
            {
                _currentTarget.replicas.Remove(this);
            }

            if (newTarget)
            {
                _currentTarget = newTarget;
                if (isTargetInScene)
                {
                    _currentTarget.replicas.Add(this);
                }
            }
            else
            {
                _currentTarget = null;
            }
        }

        internal override void UpdateContext(UIEffectContext c)
        {
            if (target && !isTargetInScene)
            {
                target.UpdateContext(c);
            }
        }

        public override void ApplyContextToMaterial(Material material)
        {
            if (!isActiveAndEnabled || !target) return;
            if (isTargetInScene && !target.isActiveAndEnabled) return;

            base.ApplyContextToMaterial(material);
        }

        public override void SetRate(float rate, UIEffectTweener.CullingMask mask)
        {
        }

        public override bool IsRaycastLocationValid(Vector2 sp, Camera eventCamera)
        {
            if (!isActiveAndEnabled || !isTargetInScene) return true;

            return target.IsRaycastLocationValid(sp, eventCamera);
        }
    }
}
