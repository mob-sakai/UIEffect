using System;
using Coffee.UIEffectInternal;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;

namespace Coffee.UIEffects
{
    [Icon("Packages/com.coffee.ui-effect/Editor/UIEffectIconIcon.png")]
    [ExecuteAlways]
    [RequireComponent(typeof(UIEffectBase))]
    public class UIEffectTweener : MonoBehaviour
    {
        [Serializable]
        public class TweenerEvent : UnityEvent<float>
        {
        }

        [Flags]
        public enum CullingMask
        {
            Tone = 1 << 0,
            Color = 1 << 1,
            Sampling = 1 << 2,
            Transition = 1 << 3,
            GradiationOffset = 1 << 5,
            GradiationRotation = 1 << 6,
            EdgeShiny = 1 << 8,
            Event = 1 << 31
        }

        public enum UpdateMode
        {
            Normal,
            Unscaled,
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

        public enum PlayOnEnable
        {
            None,
            Forward,
            Reverse,
            KeepDirection
        }

        [Tooltip("The culling mask of the tween.")]
        [SerializeField]
        private CullingMask m_CullingMask = CullingMask.Tone | CullingMask.Color | CullingMask.Sampling |
                                            CullingMask.Transition | CullingMask.GradiationOffset |
                                            CullingMask.GradiationRotation | CullingMask.EdgeShiny;

        [Tooltip("The direction of the tween.")]
        [SerializeField]
        private Direction m_Direction = Direction.Forward;

        [Tooltip("The curve to tween the properties.")]
        [SerializeField]
        private AnimationCurve m_Curve = AnimationCurve.Linear(0, 0, 1, 1);

        [SerializeField]
        private bool m_SeparateReverseCurve;

        [Tooltip("The curve to tween the properties.")]
        [SerializeField]
        private AnimationCurve m_ReverseCurve = AnimationCurve.Linear(0, 0, 1, 1);

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

        [FormerlySerializedAs("m_ResetTimeOnEnable")]
        [Tooltip("Play the tween when the component is enabled.")]
        [SerializeField]
        private PlayOnEnable m_PlayOnEnable = PlayOnEnable.Forward;

        [Tooltip("Reset the tweening time when the component is enabled.")]
        [SerializeField]
        private bool m_ResetTimeOnEnable = true;

        [Tooltip("The wrap mode of the tween.\n" +
                 "  Once: Clamp the tween value (not loop).\n" +
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

        [Tooltip("Event to invoke when the tween has completed.")]
        [SerializeField]
        private UnityEvent m_OnComplete = new UnityEvent();

        [Tooltip("Event to invoke when the rate was changed.")]
        [SerializeField]
        private TweenerEvent m_OnChangedRate = new TweenerEvent();

        private bool _isPaused;
        private float _rate = -1;
        private float _time;
        private UIEffectBase _target;

        /// <summary>
        /// The target UIMaterialPropertyInjector to tween.
        /// </summary>
        private UIEffectBase target => _target ? _target : _target = GetComponent<UIEffectBase>();

        /// <summary>
        /// The culling mask of the tween.
        /// </summary>
        public CullingMask cullingMask
        {
            get => m_CullingMask;
            set => m_CullingMask = value;
        }

        /// <summary>
        /// The direction of the tween.
        /// </summary>
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
            private set
            {
                value = Mathf.Clamp01(value);
                if (Mathf.Approximately(_rate, value)) return;

                _rate = value;

                if (!target || cullingMask == 0) return;

                var currentCurve = curve;
                if (separateReverseCurve)
                {
                    switch (wrapMode)
                    {
                        case WrapMode.Once:
                        case WrapMode.Loop:
                            if (direction == Direction.Reverse)
                            {
                                currentCurve = reverseCurve;
                            }

                            break;
                        case WrapMode.PingPongOnce:
                        case WrapMode.PingPongLoop:
                            if (delay + duration + interval <= _time)
                            {
                                currentCurve = reverseCurve;
                            }

                            break;
                    }
                }

                var evaluatedRate = currentCurve.Evaluate(_rate);
                target.SetRate(evaluatedRate, cullingMask);

                if (0 != (cullingMask & CullingMask.Event))
                {
                    onChangedRate.Invoke(evaluatedRate);
                }
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
                if (wrapMode == WrapMode.Once || wrapMode == WrapMode.PingPongOnce)
                {
                    return Mathf.Clamp(_time, 0, totalTime);
                }

                return Mathf.Repeat(_time, totalTime);
            }
        }

        /// <summary>
        /// The total time of the tween. (read only)
        /// </summary>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
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

        /// <summary>
        /// Play the tween when the component is enabled.
        /// </summary>
        public PlayOnEnable playOnEnable
        {
            get => m_PlayOnEnable;
            set => m_PlayOnEnable = value;
        }

        /// <summary>
        /// Reset the tweening time when the component is enabled..
        /// </summary>
        public bool resetTimeOnEnable
        {
            get => m_ResetTimeOnEnable;
            set => m_ResetTimeOnEnable = value;
        }

        /// <summary>
        /// The wrap mode of the tween.
        /// <p>Once: Clamp the tween value (not loop).</p>
        /// <p>Loop: Loop the tween value.</p>
        /// <p>PingPongOnce: PingPong the tween value (not loop).</p>
        /// <p>PingPong: PingPong the tween value.</p>
        /// </summary>
        public WrapMode wrapMode
        {
            get => m_WrapMode;
            set => m_WrapMode = value;
        }

        /// <summary>
        /// Specifies how to get delta time.
        /// <p>Normal: Use `Time.deltaTime`.</p>
        /// <p>Unscaled: Use `Time.unscaledDeltaTime`.</p>
        /// <p>Manual: Not updated automatically and update manually with `UpdateTime` or `SetTime` method.</p>
        /// </summary>
        public UpdateMode updateMode
        {
            get => m_UpdateMode;
            set => m_UpdateMode = value;
        }

        /// <summary>
        /// The curve to tween the properties.
        /// </summary>
        public AnimationCurve curve
        {
            get => m_Curve;
            set => m_Curve = value;
        }

        public bool separateReverseCurve
        {
            get => m_SeparateReverseCurve;
            set => m_SeparateReverseCurve = value;
        }

        public AnimationCurve reverseCurve
        {
            get => m_ReverseCurve;
            set => m_ReverseCurve = value;
        }

        /// <summary>
        /// Event to invoke when the tween has completed.
        /// </summary>
        public UnityEvent onComplete => m_OnComplete;

        /// <summary>
        /// Event to invoke when the rate was changed.
        /// </summary>
        public TweenerEvent onChangedRate => m_OnChangedRate;

        /// <summary>
        /// Is the tween playing?
        /// </summary>
        public bool isTweening
        {
            get
            {
                if (_isPaused) return false;
                if (wrapMode == WrapMode.Loop || wrapMode == WrapMode.PingPongLoop) return true;

                return direction == Direction.Forward
                    ? _time < totalTime
                    : 0 < _time;
            }
        }

        /// <summary>
        /// Is the tween paused?
        /// </summary>
        public bool isPaused => _isPaused;

        /// <summary>
        /// Is the tween delaying?
        /// </summary>
        public bool isDelaying => _time < delay;

        private void OnEnable()
        {
            _isPaused = true;

#if UNITY_EDITOR
            if (!Application.isPlaying) return;
#endif

            switch (playOnEnable)
            {
                case PlayOnEnable.KeepDirection:
                    Play(resetTimeOnEnable);
                    break;
                case PlayOnEnable.Forward:
                    PlayForward(resetTimeOnEnable);
                    break;
                case PlayOnEnable.Reverse:
                    PlayReverse(resetTimeOnEnable);
                    break;
            }
        }

        private void OnDisable()
        {
            _isPaused = true;
        }

        private void Update()
        {
#if UNITY_EDITOR
            if (!Application.isPlaying) return;
#endif
            if (!isTweening) return;

            var deltaTime = m_UpdateMode == UpdateMode.Unscaled
                ? Time.unscaledDeltaTime
                : Time.deltaTime;
            UpdateTime(direction == Direction.Forward ? deltaTime : -deltaTime);
        }

        public void Play(bool resetTime)
        {
            if (resetTime)
            {
                ResetTime(direction);
            }

            Play();
        }

        public void Play()
        {
            _isPaused = false;

            if (!isTweening)
            {
                m_OnComplete.Invoke();
            }
        }

        public void PlayForward(bool resetTime)
        {
            if (resetTime)
            {
                ResetTime(Direction.Forward);
            }

            PlayForward();
        }

        public void PlayForward()
        {
            direction = Direction.Forward;
            _isPaused = false;

            if (!isTweening)
            {
                m_OnComplete.Invoke();
            }
        }

        public void PlayReverse(bool resetTime)
        {
            if (resetTime)
            {
                ResetTime(Direction.Reverse);
            }

            PlayReverse();
        }

        public void PlayReverse()
        {
            direction = Direction.Reverse;
            _isPaused = false;

            if (!isTweening)
            {
                m_OnComplete.Invoke();
            }
        }

        public void Stop()
        {
            _isPaused = true;
            ResetTime();
        }

        public void SetPause(bool pause)
        {
            _isPaused = pause;
        }

        public void ResetTime()
        {
            SetTime(0);
        }

        public void ResetTime(Direction dir)
        {
            if (dir == Direction.Forward)
            {
                SetTime(0);
            }
            else
            {
                SetTime(totalTime - 0.0001f);
            }
        }

        [Obsolete(
            "UIEffectTweener.Restart has been deprecated. Use UIEffectTweener.ResetTime instead (UnityUpgradable) -> ResetTime")]
        public void Restart()
        {
            ResetTime();
        }

        public void SetTime(float sec)
        {
            _time = 0;
            UpdateTime(sec);
        }

        public void UpdateTime(float deltaSec)
        {
            var prevTweening = isTweening;
            var isLoop = wrapMode == WrapMode.Loop || wrapMode == WrapMode.PingPongLoop;
            _time += deltaSec;
            if (isLoop)
            {
                if (_time < 0)
                {
                    _time = Mathf.Repeat(_time, totalTime);
                }
                else if (delay < _time)
                {
                    _time = Mathf.Repeat(_time - delay, totalTime - delay) + delay;
                }
                else if (deltaSec < 0 && delay <= _time - deltaSec)
                {
                    _time = Mathf.Repeat(_time - delay, totalTime - delay) + delay;
                }
            }
            else
            {
                _time = Mathf.Clamp(_time, 0, totalTime);
            }

            var t = _time - delay;
            if (t <= 0 && 0 <= _time)
            {
                rate = 0;

                if (prevTweening && !isTweening)
                {
                    m_OnComplete.Invoke();
                }

                return;
            }

            switch (wrapMode)
            {
                case WrapMode.Once:
                    t = Mathf.Clamp(t, 0, duration);
                    _time = t + delay;
                    break;
                case WrapMode.Loop:
                    t = Mathf.Repeat(t, duration + interval);
                    _time = t + delay;
                    break;
                case WrapMode.PingPongOnce:
                    t = Mathf.Clamp(t, 0, duration * 2 + interval);
                    _time = t + delay;
                    t = Mathf.PingPong(t, duration + interval * 0.5f);
                    break;
                case WrapMode.PingPongLoop:
                    t = Mathf.Repeat(t, (duration + interval) * 2);
                    _time = t + delay;
                    t = t < duration * 2 + interval
                        ? Mathf.PingPong(t, duration + interval * 0.5f)
                        : 0;
                    break;
            }

            rate = Mathf.Clamp(t, 0, duration) / duration;

            if (prevTweening && !isTweening)
            {
                m_OnComplete.Invoke();
            }
        }
    }
}
