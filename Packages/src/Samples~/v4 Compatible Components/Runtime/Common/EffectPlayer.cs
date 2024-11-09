using UnityEngine;
using System;
using Coffee.UIEffectInternal;

namespace Coffee.UIEffects
{
    /// <summary>
    /// Effect player.
    /// </summary>
    [Serializable]
    public class EffectPlayer
    {
        //################################
        // Public Members.
        //################################
        /// <summary>
        /// Gets or sets a value indicating whether is playing.
        /// </summary>
        [Header("Effect Player")]
        [Tooltip("Playing.")]
        public bool play = false;

        /// <summary>
        /// Gets or sets the delay before looping.
        /// </summary>
        [Tooltip("Initial play delay.")]
        [Range(0f, 10f)]
        public float initialPlayDelay = 0;

        /// <summary>
        /// Gets or sets the duration.
        /// </summary>
        [Tooltip("Duration.")]
        [Range(0.01f, 10f)]
        public float duration = 1;

        /// <summary>
        /// Gets or sets a value indicating whether can loop.
        /// </summary>
        [Tooltip("Loop.")]
        public bool loop = false;

        /// <summary>
        /// Gets or sets the delay before looping.
        /// </summary>
        [Tooltip("Delay before looping.")]
        [Range(0f, 10f)]
        public float loopDelay = 0;

        /// <summary>
        /// Gets or sets the update mode.
        /// </summary>
        [Tooltip("Update mode")]
        public AnimatorUpdateMode updateMode = AnimatorUpdateMode.Normal;

        private UIEffectTweener _tweener;
        private UIEffectBase _target;

        /// <summary>
        /// Register player.
        /// </summary>
        public void OnEnable()
        {
            if (play)
            {
                Play(true);
            }
        }

        /// <summary>
        /// Unregister player.
        /// </summary>
        public void OnDisable()
        {
            Stop(false);
        }

        /// <summary>
        /// Start playing.
        /// </summary>
        public void Play(bool reset)
        {
            play = true;
            SetupProperty();
            if (reset && _tweener)
            {
                _tweener.direction = UIEffectTweener.Direction.Forward;
                _tweener.SetTime(0);
            }
        }

        public void PlayReverse(bool reset)
        {
            play = true;
            SetupProperty();
            if (reset && _tweener)
            {
                _tweener.direction = UIEffectTweener.Direction.Reverse;
                _tweener.SetTime(_tweener.totalTime);
            }
        }

        /// <summary>
        /// Stop playing.
        /// </summary>
        public void Stop(bool reset)
        {
            play = false;
            if (reset && _tweener)
            {
                SetupProperty();
                _tweener.SetTime(0);
            }
        }

        public void Attach(UIEffectBase target)
        {
            _target = target;
            SetupProperty();
        }

        private void SetupProperty()
        {
            if (!_target) return;
            _tweener = _target.GetOrAddComponent<UIEffectTweener>();
            _tweener.enabled = play;
            _tweener.duration = duration;
            _tweener.wrapMode = loop ? UIEffectTweener.WrapMode.Loop : UIEffectTweener.WrapMode.Once;
            _tweener.interval = loopDelay;
            _tweener.delay = initialPlayDelay;
            _tweener.updateMode = updateMode == AnimatorUpdateMode.UnscaledTime
                ? UIEffectTweener.UpdateMode.Unscaled
                : UIEffectTweener.UpdateMode.Normal;
        }
    }
}
