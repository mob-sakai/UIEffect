//#define LOG_PROGRESS

using UnityEngine;
using System;
using System.Collections.Generic;

namespace Coffee.UIEffects
{
    [Serializable]
    public class EffectPlayer
    {
        #region [Variables]
        /// <summary>
        /// Gets or sets a value indicating whether is playing.
        /// </summary>
        [Header("Effect Player")] [Tooltip("Playing.")]
        public bool play = false;

        /// <summary>
        /// Gets or sets the delay before looping.
        /// </summary>
        [Tooltip("Initial play delay.")] [Range(0f, 10f)]
        public float initialPlayDelay = 0;

        /// <summary>
        /// Gets or sets the duration.
        /// </summary>
        [Tooltip("Duration.")] [Range(0.01f, 10f)]
        public float duration = 1;

        /// <summary>
        /// Gets or sets a value indicating whether can loop.
        /// </summary>
        [Tooltip("Loop.")] public bool loop = false;

        /// <summary>
        /// Gets or sets the delay before looping.
        /// </summary>
        [Tooltip("Delay before looping.")] [Range(0f, 10f)]
        public float loopDelay = 0;

        /// <summary>
        /// Gets or sets the update mode.
        /// </summary>
        [Tooltip("Update mode")] public AnimatorUpdateMode updateMode = AnimatorUpdateMode.Normal;

        // Private variables

        static List<Action> s_UpdateActions;

        float _delayBeforeContinuing;
        float _timePassed;
        Action<float> _callback;

        #endregion [Variables]

        #region OnEnable()
        /// <summary>
        /// Register player.
        /// </summary>
        public void OnEnable( Action<float> callback = null )
        {
            // Register update function on canvas
            if( s_UpdateActions == null )
            {
#if LOG_PROGRESS
                Debug.Log( $"<color=cyan><b>REGISTER ON CANVAS</b> {_delayBeforeContinuing:0.#} / {_timePassed:0.#} / {_callback} / {play}</color>" );
#endif

                s_UpdateActions = new List<Action>();
                Canvas.willRenderCanvases += () =>      // This will happen on each canvas render
                {
                    var count = s_UpdateActions.Count;
                    for( int i = 0; i < count; i++ )
                    {
                        s_UpdateActions[i].Invoke();
                    }
                };
            }

            s_UpdateActions.Add( OnWillRenderCanvases );

            _delayBeforeContinuing = initialPlayDelay;
            _timePassed = 0;

            if( callback != null ) _callback = callback;

#if LOG_PROGRESS
            Debug.Log( $"<color=cyan><b>ENABLED</b> {_delayBeforeContinuing:0.#} / {_timePassed:0.#}</color>" );
#endif
        }
        #endregion OnEnable()

        #region OnDisable()
        /// <summary>
        /// Unregister player.
        /// </summary>
        public void OnDisable()
        {
            _callback = null;
            if( s_UpdateActions != null ) s_UpdateActions.Remove( OnWillRenderCanvases );

#if LOG_PROGRESS
            Debug.Log( $"<color=cyan><b>DISABLED</b> {_delayBeforeContinuing:0.#} / {_timePassed:0.#}</color>" );
#endif
        }
        #endregion OnDisable()

        #region [API] Play()
        /// <summary>
        /// Start playing.
        /// </summary>
        public void Play( bool reset, Action<float> callback = null )
        {
            if( reset ) {
                _delayBeforeContinuing = initialPlayDelay;
                _timePassed = 0;
            }

            play = true;

            if( callback != null ) {
                _callback = callback;
            }

            var perc = Mathf.Clamp01( _timePassed / duration );
            _callback?.Invoke( perc );

#if LOG_PROGRESS
            Debug.Log( $"<color=cyan><b>PLAY</b> {_delayBeforeContinuing:0.#} / {_timePassed:0.#} / {perc:0.##} / {_callback}</color>" );
#endif
        }
        #endregion [API] Play()

        #region [API] Stop()
        /// <summary>
        /// Stop playing.
        /// </summary>
        public void Stop( bool reset )
        {
#if LOG_PROGRESS
            Debug.Log( $"<color=cyan><b>STOP</b> {_delayBeforeContinuing:0.#} / {_timePassed:0.#} / {_callback}</color>" );
#endif

            if( reset )
            {
                _timePassed = 0;
                _delayBeforeContinuing = initialPlayDelay;
                _callback?.Invoke( 0 );
            }

            play = false;
        }
        #endregion [API] Stop()


        #region OnWillRenderCanvases() - update each frame
        void OnWillRenderCanvases()
        {
#if LOG_PROGRESS
            Debug.Log( $"<color=cyan><b>UPDATE</b> {_delayBeforeContinuing:0.#} / {_timePassed:0.#} / {_callback} / Playing: {play}</color>" );
#endif

            if( !play || !Application.isPlaying || _callback == null ) {
                return;
            }


            var dTime = updateMode == AnimatorUpdateMode.UnscaledTime
                ? Time.unscaledDeltaTime
                : Time.deltaTime;

            if( _delayBeforeContinuing > 0 ) {
                _delayBeforeContinuing -= dTime;
                return;
            }

            _timePassed += dTime;

            var perc = Mathf.Clamp01( _timePassed / duration );
            if( perc >= 1 )     // Finished with animation loop
            {
                play = loop;    // Play again, if we're in loop

                _timePassed = 0;
                _delayBeforeContinuing = loop ? loopDelay : 0;
            }

            _callback( perc );
        }
    }
    #endregion OnWillRenderCanvases() - update each frame
}
