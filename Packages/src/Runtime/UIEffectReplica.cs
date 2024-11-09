using Coffee.UIEffectInternal;
using UnityEngine;

namespace Coffee.UIEffects
{
    public class UIEffectReplica : UIEffectBase
    {
        [SerializeField] private UIEffect m_Target;
        [SerializeField] private bool m_UseTargetTransform = true;

        private UIEffect _currentTarget;
        private Matrix4x4 _prevTransformHash;

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

        public override uint effectId => target ? target.effectId : 0;
        public override UIEffectContext context => target && target.isActiveAndEnabled ? target.context : null;

        public override RectTransform transitionRoot => useTargetTransform && target
            ? target.transitionRoot
            : transform as RectTransform;

        protected override void OnEnable()
        {
            RefreshTarget(target);
            UIExtraCallbacks.onBeforeCanvasRebuild += SetVerticesDirtyIfTransformChanged;
            CheckTransform();
            base.OnEnable();
        }

        protected override void OnDisable()
        {
            RefreshTarget(null);
            UIExtraCallbacks.onBeforeCanvasRebuild -= SetVerticesDirtyIfTransformChanged;
            base.OnDisable();
        }

#if UNITY_EDITOR
        protected override void OnValidate()
        {
            RefreshTarget(target);
            base.OnValidate();
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

        protected override void UpdateContext(UIEffectContext c)
        {
        }

        public override void ApplyContextToMaterial()
        {
            if (!isActiveAndEnabled || !target || !target.isActiveAndEnabled) return;

            base.ApplyContextToMaterial();
        }

        public override void SetRate(float rate, UIEffectTweener.CullingMask mask)
        {
        }

        public override bool IsRaycastLocationValid(Vector2 sp, Camera eventCamera)
        {
            return !target || target.IsRaycastLocationValid(sp, eventCamera);
        }

        private bool CheckTransform()
        {
            return useTargetTransform
                   && target
                   && transform.HasChanged(target.transform, ref _prevTransformHash,
                       UIEffectProjectSettings.sensitivity);
        }

        private void SetVerticesDirtyIfTransformChanged()
        {
            if (CheckTransform())
            {
                SetVerticesDirty();
            }
        }
    }
}
