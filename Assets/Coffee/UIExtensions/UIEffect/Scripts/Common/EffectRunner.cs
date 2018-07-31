using UnityEngine;
using System;

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
		public bool running;// { get; set; }

		/// <summary>
		/// Gets or sets a value indicating whether can loop.
		/// </summary>
		public bool loop;//  { get; set; }

		/// <summary>
		/// Gets or sets the duration.
		/// </summary>
		public float duration;//  { get; set; }

		/// <summary>
		/// Gets or sets the delay before looping.
		/// </summary>
		public float loopDelay;//  { get; set; }

		/// <summary>
		/// Gets or sets the update mode.
		/// </summary>
		public AnimatorUpdateMode updateMode;//  { get; set; }

//		/// <summary>
//		/// Register runner.
//		/// </summary>
//		public void Register(bool running,float duration, AnimatorUpdateMode updateMode, bool loop, float loopDelay, Action<float> callback)
//		{
//			Canvas.willRenderCanvases += OnWillRenderCanvases;
//
//			_time = 0;
//			this._callback = callback;
//			this.loop = loop;
//			this.running = running;
//			this.duration = duration;
//			this.loopDelay = loopDelay;
//			this.updateMode = updateMode;
//		}

		public void OnEnable(Action<float> callback)
		{
			Canvas.willRenderCanvases -= OnWillRenderCanvases;
			Canvas.willRenderCanvases += OnWillRenderCanvases;

			_time = 0;
			this._callback = callback;
		}

//		/// <summary>
//		/// Unregister runner.
//		/// </summary>
//		public void Unregister()
//		{
//			_callback = null;
//			Canvas.willRenderCanvases -= OnWillRenderCanvases;
//		}

		/// <summary>
		/// Unregister runner.
		/// </summary>
		public void OnDisable()
		{
			_callback = null;
			Canvas.willRenderCanvases -= OnWillRenderCanvases;
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