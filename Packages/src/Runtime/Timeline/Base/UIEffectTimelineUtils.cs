#if TIMELINE_ENABLE
using System;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

namespace Coffee.UIEffects.Timeline
{
    [AttributeUsage(AttributeTargets.Class)]
    public class FloatClipUsageAttribute : PropertyAttribute
    {
        public readonly float min;
        public readonly float max;
        public readonly float defaultValue;

        public FloatClipUsageAttribute(float min, float max, float defaultValue = 0f)
        {
            this.min = min;
            this.max = max;
            this.defaultValue = defaultValue;
        }
    }

    [AttributeUsage(AttributeTargets.Class)]
    public class ColorClipUsageAttribute : PropertyAttribute
    {
        public readonly bool alpha;

        public ColorClipUsageAttribute(bool alpha)
        {
            this.alpha = alpha;
        }
    }

    public interface IGetValue<out T>
    {
        T Get(float time);
    }

    [TrackColor(0.92f, 0.54f, 0.17f)]
    [TrackBindingType(typeof(UIEffect), TrackBindingFlags.AllowCreateComponent)]
    public abstract class UIEffectTrack<T> : TrackAsset
        where T : PlayableBehaviour, new()
    {
        protected abstract string fieldName { get; }

        public override Playable CreateTrackMixer(PlayableGraph graph, GameObject go, int inputCount)
        {
            var playable = ScriptPlayable<T>.Create(graph, inputCount);

            foreach (var timelineClip in GetClips())
            {
                if (timelineClip.asset is UIEffectClip clip)
                {
                    clip.timelineClip = timelineClip;
                }
            }

            return playable;
        }

        public override void GatherProperties(PlayableDirector director, IPropertyCollector driver)
        {
            var trackBinding = director.GetGenericBinding(this) as UIEffect;
            if (!trackBinding) return;

            driver.AddFromName<UIEffect>(trackBinding.gameObject, fieldName);
            base.GatherProperties(director, driver);
        }
    }

    public abstract class UIEffectClip : PlayableAsset, ITimelineClipAsset
    {
        public TimelineClip timelineClip { get; set; }
        public ClipCaps clipCaps => ClipCaps.Blending | ClipCaps.Extrapolation;

        private void OnValidate()
        {

        }
    }

    public abstract class UIEffectClip<T> : UIEffectClip
        where T : UIEffectBehaviour, new()
    {
        [NotKeyable]
        [SerializeField]
        public T m_Data = new T();

        public override Playable CreatePlayable(PlayableGraph graph, GameObject owner)
        {
            var playable = ScriptPlayable<T>.Create(graph, m_Data);
            var behaviour = playable.GetBehaviour();
            behaviour.clip = this;
            return playable;
        }
    }

    [Serializable]
    public abstract class UIEffectBehaviour : PlayableBehaviour
    {
        public bool m_Tween;
        public AnimationCurve m_Curve = AnimationCurve.Linear(0, 0, 1, 1);
        public UIEffectClip clip { get; set; }
    }

    public abstract class UIEffectMixerBehaviour<T, TBehavior> : PlayableBehaviour
        where TBehavior : UIEffectBehaviour, IGetValue<T>, new()
    {
        private T _defaultValue;
        protected abstract T currentValue { get; set; }
        protected UIEffect effect { get; private set; }

        public override void ProcessFrame(Playable playable, FrameData info, object playerData)
        {
            InitializeIfNeeded(playerData as UIEffect);
            if (!effect) return;

            var inputCount = playable.GetInputCount();
            var value = default(T);
            var totalWeight = GetTotalWeight(playable);

            for (var i = 0; i < inputCount; i++)
            {
                var weight = playable.GetInputWeight(i);
                if (weight <= 0) continue;

                var playableInput = (ScriptPlayable<TBehavior>)playable.GetInput(i);
                var input = playableInput.GetBehaviour();
                var normalizedTime = (float)(playableInput.GetTime() / input.clip.timelineClip.duration);

                value = Add(value, input.Get(normalizedTime), weight / totalWeight);
                totalWeight += weight;
            }

            currentValue = Lerp(_defaultValue, value, totalWeight);
        }

        public override void OnPlayableDestroy(Playable playable)
        {
            if (effect)
            {
                currentValue = _defaultValue;
            }
        }

        private static float GetTotalWeight(Playable playable)
        {
            var totalWeight = 0f;
            var inputCount = playable.GetInputCount();
            for (var i = 0; i < inputCount; i++)
            {
                totalWeight += playable.GetInputWeight(i);
            }

            return totalWeight;
        }

        private void InitializeIfNeeded(UIEffect newEffect)
        {
            if (effect == newEffect) return;

            if (effect)
            {
                currentValue = _defaultValue;
            }

            effect = newEffect;
            _defaultValue = newEffect ? currentValue : default;
        }

        protected abstract T Add(T current, T value, float weight);

        protected abstract T Lerp(T defaultValue, T value, float weight);
    }
}

#endif
