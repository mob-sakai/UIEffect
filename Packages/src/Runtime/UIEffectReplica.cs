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
        protected Flip m_Flip = 0;

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

        public override Flip flip
        {
            get => m_Flip;
            set
            {
                if (m_Flip == value) return;
                m_Flip = value;
                SetVerticesDirty();
            }
        }

        public override float actualSamplingScale => Mathf.Clamp(m_SamplingScale, 0.01f, 100);

        public override bool canModifyShape => m_AllowToModifyMeshShape;

        public override uint effectId => target ? target.effectId : 0;
        public override UIEffectContext context => target && target.isActiveAndEnabled ? target.context : null;

        public override RectTransform transitionRoot => useTargetTransform && target
            ? target.transitionRoot
            : transform as RectTransform;

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
            if (!target) return;
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

            _currentTarget = newTarget;
            if (_currentTarget)
            {
                _currentTarget.replicas.Add(this);
            }
        }

        protected override void OnBeforeCanvasRebuild()
        {
            base.OnBeforeCanvasRebuild();

            if (useTargetTransform && target && CheckTransformChangedWith(target.transform))
            {
                SetVerticesDirty();
            }
        }

        protected override void UpdateContext(UIEffectContext c)
        {
        }

        public override void ApplyContextToMaterial(Material material)
        {
            if (!isActiveAndEnabled || !target || !target.isActiveAndEnabled) return;

            base.ApplyContextToMaterial(material);
        }

        public override void SetRate(float rate, UIEffectTweener.CullingMask mask)
        {
        }

        public override bool IsRaycastLocationValid(Vector2 sp, Camera eventCamera)
        {
            return !target || target.IsRaycastLocationValid(sp, eventCamera);
        }
    }
}
