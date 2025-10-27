using System.Linq;
using Coffee.UIEffectInternal;
using UnityEngine;

namespace Coffee.UIEffects
{
    [Icon("Packages/com.coffee.ui-effect/Editor/UIEffectIconIcon.png")]
    public class UIEffectReplica : UIEffectBase, ISerializationCallbackReceiver
    {
        [SerializeField] private UIEffect m_Target;
        [SerializeField] private UIEffectPreset m_Preset;
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
                m_Preset = null;
                RefreshTarget(m_Target);
                SetVerticesDirty();
                SetMaterialDirty();
            }
        }

        public UIEffectPreset preset
        {
            get => m_Preset;
            set
            {
                if (m_Preset == value) return;
                m_Preset = value;
                RefreshTarget(null);
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

        public override float actualSamplingScale => Mathf.Clamp(m_SamplingScale, 0.01f, 100);

        public override bool canModifyShape => m_AllowToModifyMeshShape;

        public override uint effectId => target
            ? target.effectId
            : preset
                ? (uint)preset.GetInstanceID()
                : (uint)GetInstanceID();


        public override UIEffectContext context
        {
            get
            {
                // Following the preset/preset-target.
                if (preset || (target && !isTargetInScene)) return base.context;

                // Following the instantiated target.
                return target && target.isActiveAndEnabled && isTargetInScene
                    ? target.context
                    : null;
            }
        }

        public override RectTransform transitionRoot
        {
            get
            {
                if (useTargetTransform)
                {
                    if (preset && !m_CustomRoot && graphic && canvas)
                    {
                        return canvas.transform as RectTransform;
                    }

                    if (isTargetInScene)
                    {
                        return target.transitionRoot;
                    }
                }

                return m_CustomRoot ? m_CustomRoot : transform as RectTransform;
            }
        }

        private bool isTargetInScene => target && target.gameObject.scene.IsValid();

        protected override void OnEnable()
        {
            if (!preset)
            {
                RefreshTarget(target);
            }

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

            if (preset || (target && !isTargetInScene))
            {
                context?.SetGradationDirty();
                context?.SetTransitionGradationDirty();
            }

            base.OnValidate();
        }

        internal override void SetEnablePreviewIfSelected(GameObject[] selection)
        {
            // Following the instantiated target.
            if (isTargetInScene)
            {
                target.SetEnablePreviewIfSelected(selection);
            }
            // Following the preset/preset-target.
            else
            {
                context?.SetEnablePreview(selection.Contains(gameObject), effectMaterial);
            }
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

        internal override void UpdateContext(UIEffectContext dst)
        {
            // Following the preset.
            if (preset)
            {
                preset.UpdateContext(dst);
            }
            // Following the preset-target.
            else if (target && !isTargetInScene)
            {
                target.UpdateContext(dst);
            }
        }

        public override void ApplyContextToMaterial(Material material)
        {
            if (!isActiveAndEnabled && !preset && !target) return;

            // Following the preset/preset-target.
            if (preset || (target && !isTargetInScene))
            {
                base.ApplyContextToMaterial(material);
            }
            // Following the instantiated target.
            else if (isTargetInScene && target.isActiveAndEnabled)
            {
                base.ApplyContextToMaterial(material);
            }
        }

        public override void SetRate(float rate, UIEffectTweener.CullingMask mask)
        {
        }

        public override bool IsRaycastLocationValid(Vector2 sp, Camera eventCamera)
        {
            if (!isActiveAndEnabled || !isTargetInScene) return true;

            return target.IsRaycastLocationValid(sp, eventCamera);
        }


        public void OnBeforeSerialize()
        {
        }

        public void OnAfterDeserialize()
        {
            if (m_Preset)
            {
                m_Target = null;
            }
            else if (m_Target)
            {
                m_Preset = null;
            }
        }
    }
}
