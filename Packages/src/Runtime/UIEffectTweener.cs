using System;
using UnityEngine;

namespace Coffee.UIEffects
{
    [RequireComponent(typeof(UIEffectBase))]
    public class UIEffectTweener : MonoBehaviour
    {
        [Flags]
        public enum CullingMask
        {
            Tone = 1 << 0,
            Color = 1 << 1,
            Sampling = 1 << 2,
            Transition = 1 << 3
        }

        public enum UpdateMode
        {
            Normal,
            Unscaled,
            Manual
        }

        public enum StartMode
        {
            Automatic,
            Manual
        }

        public enum WrapMode
        {
            Once,
            Loop,
            PingPongOnce,
            PingPongLoop
        }

        public enum Direction
        {
            Forward,
            Reverse
        }

        [Tooltip("The culling mask of the tween.")]
        [SerializeField]
        private CullingMask m_CullingMask =
            CullingMask.Tone | CullingMask.Color | CullingMask.Sampling | CullingMask.Transition;

        [Tooltip("The direction of the tween.")]
        [SerializeField]
        private Direction m_Direction = Direction.Forward;

        [Tooltip("The curve to tween the properties.")]
        [SerializeField]
        private AnimationCurve m_Curve = AnimationCurve.Linear(0, 0, 1, 1);

        [Tooltip("The delay in seconds before the tween starts.")]
        [SerializeField]
        [Range(0f, 10)]
        private float m_Delay;

        [Tooltip("The duration in seconds of the tween.")]
        [SerializeField]
        [Range(0.05f, 10)]
        private float m_Duration = 1;

        [Tooltip("The interval in seconds between each loop.")]
        [SerializeField]
        [Range(0f, 10)]
        private float m_Interval;

        [Tooltip("Whether to restart the tween when enabled.")]
        [SerializeField]
        private bool m_RestartOnEnable = true;

        [Tooltip("The wrap mode of the tween.\n" +
                 "  Clamp: Clamp the tween value (not loop).\n" +
                 "  Loop: Loop the tween value.\n" +
                 "  PingPongOnce: PingPong the tween value (not loop).\n" +
                 "  PingPong: PingPong the tween value.")]
        [SerializeField]
        private WrapMode m_WrapMode = WrapMode.Loop;

        [Tooltip("Specifies how to get delta time.\n" +
                 "  Normal: Use `Time.deltaTime`.\n" +
                 "  Unscaled: Use `Time.unscaledDeltaTime`.\n" +
                 "  Manual: Not updated automatically and update manually with `UpdateTime` or `SetTime` method.")]
        [SerializeField]
        private UpdateMode m_UpdateMode = UpdateMode.Normal;

        [Tooltip("Specifies how the effect tweener will start.\n" +
                 "  Automatic: Plays the tween automatically when it starts.\n" +
                 "  Manual: Waits for the first `Play()` call to start.")]
        [SerializeField]
        private StartMode m_StartMode = StartMode.Automatic;

        public bool _isAwaitingStart;
        private float _rate;
        private float _time;
        private UIEffectBase _target;

        /// <summary>
        /// The target UIMaterialPropertyInjector to tween.
        /// </summary>
        private UIEffectBase target => _target ? _target : _target = GetComponent<UIEffectBase>();

        public CullingMask cullingMask
        {
            get => m_CullingMask;
            set => m_CullingMask = value;
        }

        public Direction direction
        {
            get => m_Direction;
            set => m_Direction = value;
        }

        /// <summary>
        /// The rate of the tween.
        /// </summary>
        public float rate
        {
            get => _rate;
            set
            {
                value = Mathf.Clamp01(value);
                if (Mathf.Approximately(_rate, value)) return;

                _rate = value;

                if (!target || cullingMask == 0) return;
                var evaluatedRate = m_Curve.Evaluate(_rate);
                target.SetRate(evaluatedRate, cullingMask);
            }
        }

        /// <summary>
        /// The duration in seconds of the tween.
        /// </summary>
        public float duration
        {
            get => m_Duration;
            set => m_Duration = Mathf.Max(0.001f, value);
        }

        /// <summary>
        /// The delay in seconds before the tween starts.
        /// </summary>
        public float delay
        {
            get => m_Delay;
            set => m_Delay = Mathf.Max(0, value);
        }

        /// <summary>
        /// The interval in seconds between each loop.
        /// </summary>
        public float interval
        {
            get => m_Interval;
            set => m_Interval = Mathf.Max(0, value);
        }

        /// <summary>
        /// The current time of the tween.
        /// </summary>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        public float time
        {
            get
            {
                if (_time < delay) return _time;
                var t = _time - delay;
                switch (wrapMode)
                {
                    case WrapMode.Once:
                    case WrapMode.PingPongOnce:
                        return Mathf.Clamp(t, 0, totalTime - delay) + delay;
                    case WrapMode.Loop:
                    case WrapMode.PingPongLoop:
                        return Mathf.Repeat(t, totalTime - delay) + delay;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
        }

        public float totalTime
        {
            get
            {
                switch (wrapMode)
                {
                    case WrapMode.Once: return delay + duration;
                    case WrapMode.Loop: return delay + duration + interval;
                    case WrapMode.PingPongOnce: return delay + duration * 2 + interval;
                    case WrapMode.PingPongLoop: return delay + duration * 2 + interval * 2;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
        }

        public bool restartOnEnable
        {
            get => m_RestartOnEnable;
            set => m_RestartOnEnable = value;
        }

        public WrapMode wrapMode
        {
            get => m_WrapMode;
            set => m_WrapMode = value;
        }

        public UpdateMode updateMode
        {
            get => m_UpdateMode;
            set => m_UpdateMode = value;
        }

        public StartMode startMode
        {
            get => m_StartMode;
            set => m_StartMode = value;
        }

        public AnimationCurve curve
        {
            get => m_Curve;
            set => m_Curve = value;
        }

        private void Awake()
        {
            _isAwaitingStart = m_StartMode == StartMode.Manual;
        }

        private void Update()
        {
            if (m_StartMode == StartMode.Manual && _isAwaitingStart)
            {
                return;
            }

            float deltaTime = m_UpdateMode == UpdateMode.Unscaled ? Time.unscaledDeltaTime : Time.deltaTime;
            UpdateTime(deltaTime);
        }

        private void OnEnable()
        {
            if (m_RestartOnEnable)
            {
                Restart();
            }
        }

        public void Play()
        {
            _isAwaitingStart = false;
            Restart();
        }

        public void Stop()
        {
            _isAwaitingStart = true;
            Restart();
        }

        public void Restart()
        {
            SetTime(0);
        }

        public void SetTime(float sec)
        {
            _time = 0;
            UpdateTime(sec);
        }

        public void UpdateTime(float deltaSec)
        {
            rate = UpdateTime_Internal(Mathf.Max(0, deltaSec)) / duration;
        }

        private float UpdateTime_Internal(float delta)
        {
            _time += direction == Direction.Forward ? delta : -delta;
            if (_time < delay) return 0;

            var t = _time - delay;
            switch (wrapMode)
            {
                case WrapMode.Loop:
                    t = Mathf.Repeat(t, duration + interval);
                    break;
                case WrapMode.PingPongOnce:
                    t = Mathf.Clamp(t, 0, duration * 2 + interval);
                    t = Mathf.PingPong(t, duration + interval * 0.5f);
                    break;
                case WrapMode.PingPongLoop:
                    t = Mathf.Repeat(t, (duration + interval) * 2);
                    t = t < duration * 2 + interval
                        ? Mathf.PingPong(t, duration + interval * 0.5f)
                        : 0;
                    break;
            }

            return Mathf.Clamp(t, 0, duration);
        }
    }
}
