using UnityEngine;
using System;
using System.Collections.Generic;

namespace Coffee.UIExtensions
{
	/// <summary>
	/// Effect Runner.
	/// </summary>
	[Serializable]
	public class EffectRunner
	{
		//################################
		// Public Members.
		//################################
		/// <summary>
		/// Gets or sets a value indicating whether is running.
		/// </summary>
		[Tooltip("Running.")]
		public bool running;

		/// <summary>
		/// Gets or sets a value indicating whether can loop.
		/// </summary>
		[Tooltip("Loop.")]
		public bool loop;

		/// <summary>
		/// Gets or sets the duration.
		/// </summary>
		[Tooltip("Duration.")]
		public float duration;

		/// <summary>
		/// Gets or sets the delay before looping.
		/// </summary>
		[Tooltip("Delay before looping.")]
		public float loopDelay;

		/// <summary>
		/// Gets or sets the update mode.
		/// </summary>
		[Tooltip("Update mode")]
		public AnimatorUpdateMode updateMode;

		static List<Action> s_UpdateActions;

		public void OnEnable(Action<float> callback = null)
		{

			if (s_UpdateActions == null)
			{
				s_UpdateActions = new List<Action>();
				Canvas.willRenderCanvases += () =>
				{
					var count = s_UpdateActions.Count;
					for (int i = 0; i < count; i++)
					{
						s_UpdateActions[i].Invoke();
					}
				};
			}
			s_UpdateActions.Add(OnWillRenderCanvases);

			_time = 0;
			_callback = callback;
		}

		/// <summary>
		/// Unregister runner.
		/// </summary>
		public void OnDisable()
		{
			_callback = null;
			s_UpdateActions.Remove(OnWillRenderCanvases);
		}

		/// <summary>
		/// Play runner.
		/// </summary>
		public void Play(Action<float> callback = null)
		{
			_time = 0;
			running = true;
			if(callback != null)
			{
				_callback = callback;
			}
		}

		//################################
		// Private Members.
		//################################
		float _time = 0;
		Action<float> _callback;

		void OnWillRenderCanvases()
		{
			if (!running || !Application.isPlaying || _callback == null)
			{
				return;
			}

			_time += updateMode == AnimatorUpdateMode.UnscaledTime
				? Time.unscaledDeltaTime
				: Time.deltaTime;
			var current = _time / duration;

			if (duration <= _time)
			{
				running = loop;
				_time = loop ? -loopDelay : 0;
			}
			_callback(current);
		}
	}
}