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
		public bool running;

		/// <summary>
		/// Gets or sets a value indicating whether can loop.
		/// </summary>
		public bool loop;

		/// <summary>
		/// Gets or sets the duration.
		/// </summary>
		public float duration;

		/// <summary>
		/// Gets or sets the delay before looping.
		/// </summary>
		public float loopDelay;

		/// <summary>
		/// Gets or sets the update mode.
		/// </summary>
		public AnimatorUpdateMode updateMode;

		static List<Action> s_UpdateActions;

		public void OnEnable(Action<float> callback)
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
			this._callback = callback;
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
		public void Play()
		{
			_time = 0;
			running = true;
		}

		//################################
		// Private Members.
		//################################
		float _time = 0;
		Action<float> _callback;

		void OnWillRenderCanvases()
		{
			if (!running || !Application.isPlaying)
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