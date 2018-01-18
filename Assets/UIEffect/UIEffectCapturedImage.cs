using System.Collections;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace UnityEngine.UI
{
	/// <summary>
	/// UIEffectCapturedImage
	/// </summary>
	public class UIEffectCapturedImage : RawImage
#if UNITY_EDITOR
		, ISerializationCallbackReceiver
#endif
	{
		public const string shaderName = "UI/Hidden/UI-EffectCapture";

		/// <summary>
		/// Desampling rate.
		/// </summary>
		public enum DesamplingRate
		{
			None = 0,
			x1 = 1,
			x2 = 2,
			x4 = 4,
			x8 = 8,
		}

		/// <summary>
		/// Tone effect level between 0(no effect) and 1(complete effect).
		/// </summary>
		public float toneLevel { get { return m_ToneLevel; } set { m_ToneLevel = Mathf.Clamp(value, 0, 1); } }

		[SerializeField]
		[Range(0, 1)]
		float m_ToneLevel = 1;

		/// <summary>
		/// How far is the blurring from the graphic.
		/// </summary>
		public float blur { get { return m_Blur; } set { m_Blur = Mathf.Clamp(value, 0, 4); } }

		[SerializeField]
		[Range(0, 2)]
		float m_Blur = 0;

		/// <summary>
		/// Tone effect mode.
		/// </summary>
		public UIEffect.ToneMode toneMode { get { return m_ToneMode; } set { m_ToneMode = value; } }

		[SerializeField]
		UIEffect.ToneMode m_ToneMode;

		/// <summary>
		/// Color effect mode.
		/// </summary>
		public UIEffect.ColorMode colorMode { get { return m_ColorMode; } set { m_ColorMode = value; } }

		[SerializeField]
		UIEffect.ColorMode m_ColorMode;

		/// <summary>
		/// Blur effect mode.
		/// </summary>
		public UIEffect.BlurMode blurMode { get { return m_BlurMode; } set { m_BlurMode = value; } }

		[SerializeField]
		UIEffect.BlurMode m_BlurMode;

		/// <summary>
		/// Color for the color effect.
		/// </summary>
		public Color effectColor { get { return m_EffectColor; } set { m_EffectColor = value; } }

		[SerializeField]
		Color m_EffectColor = Color.white;

		/// <summary>
		/// Desampling rate of the generated RenderTexture.
		/// </summary>
		public DesamplingRate desamplingRate { get { return m_DesamplingRate; } set { m_DesamplingRate = value; } }

		[SerializeField]
		DesamplingRate m_DesamplingRate;

		/// <summary>
		/// Desampling rate of reduction buffer to apply effect.
		/// </summary>
		public DesamplingRate reductionRate { get { return m_ReductionRate; } set { m_ReductionRate = value; } }

		[SerializeField]
		DesamplingRate m_ReductionRate;

		/// <summary>
		/// FilterMode for capture.
		/// </summary>
		public FilterMode filterMode { get { return m_FilterMode; } set { m_FilterMode = value; } }

		[SerializeField]
		FilterMode m_FilterMode = FilterMode.Bilinear;

		/// <summary>
		/// Effect material.
		/// </summary>
		public virtual Material effectMaterial { get { return m_EffectMaterial; } }

		[SerializeField] Material m_EffectMaterial;

		/// <summary>
		/// Captured texture.
		/// </summary>
		public RenderTexture capturedTexture { get { return _rt; } }

		/// <summary>
		/// Gets the size of the desampling.
		/// </summary>
		public void GetDesamplingSize(DesamplingRate rate, out int w, out int h)
		{
			var cam = canvas.worldCamera ?? Camera.main;
			h = cam.pixelHeight;
			w = cam.pixelWidth;
			if (rate != DesamplingRate.None)
			{
				h = Mathf.ClosestPowerOfTwo(h / (int)rate);
				w = Mathf.ClosestPowerOfTwo(w / (int)rate);
			}
		}

		/// <summary>
		/// Capture rendering result.
		/// </summary>
		public void Capture()
		{
			// Camera for command buffer.
			_camera = canvas.worldCamera ?? Camera.main;

			// Cache id for RT.
			if (s_CopyId == 0)
			{
				s_CopyId = Shader.PropertyToID("_ScreenCopy");
				s_EffectId = Shader.PropertyToID("_Effect");
			}

			// If size of generated result RT has changed, relese it.
			int w, h;
			GetDesamplingSize(m_DesamplingRate, out w, out h);
			if (_rt && (_rt.width != w || _rt.height != h))
			{
				_rtToRelease = _rt;
				_rt = null;
			}

			// Generate result RT.
			if (_rt == null)
			{
				_rt = new RenderTexture(w, h, 0, RenderTextureFormat.ARGB32, RenderTextureReadWrite.Default);
				_rt.filterMode = m_FilterMode;
				_rt.useMipMap = false;
				_rt.wrapMode = TextureWrapMode.Clamp;
				_rt.hideFlags = HideFlags.HideAndDontSave;
			}

			// Create command buffer.
			if (_buffer == null)
			{
				var rtId = new RenderTargetIdentifier(_rt);

				// Material for effect.
				Material mat = effectMaterial;

				_buffer = new CommandBuffer();
				_buffer.name =
					_rt.name =
						mat ? mat.name : "noeffect";

				// Copy to temporary RT.
				_buffer.GetTemporaryRT(s_CopyId, -1, -1, 0, FilterMode.Bilinear);
				_buffer.Blit(BuiltinRenderTextureType.CurrentActive, s_CopyId);

				// Set properties.
				if(toneMode == UIEffect.ToneMode.Hue)
					_buffer.SetGlobalVector("_EffectFactor", new Vector4(Mathf.Cos(toneLevel* Mathf.PI * 2), Mathf.Sin(toneLevel * Mathf.PI * 2), blur / w));
				else
					_buffer.SetGlobalVector("_EffectFactor", new Vector4(toneLevel, 0, blur / w));

				_buffer.SetGlobalVector("_ColorFactor", new Vector4(effectColor.r, effectColor.g, effectColor.b, effectColor.a));

				// Blit without effect.
				if (!mat)
				{
					_buffer.Blit(s_CopyId, rtId);
					_buffer.ReleaseTemporaryRT(s_CopyId);
				}
				// Blit with reduction buffer.
				else if (m_DesamplingRate < m_ReductionRate)
				{
					GetDesamplingSize(m_ReductionRate, out w, out h);
					_buffer.GetTemporaryRT(s_EffectId, w, h, 0, FilterMode.Bilinear);
					_buffer.Blit(s_CopyId, s_EffectId, mat);
					_buffer.ReleaseTemporaryRT(s_CopyId);
					_buffer.Blit(s_EffectId, rtId);
					_buffer.ReleaseTemporaryRT(s_EffectId);
				}
				// Blit without reduction buffer.
				else
				{
					_buffer.Blit(s_CopyId, rtId, mat);
					_buffer.ReleaseTemporaryRT(s_CopyId);
				}
			}

			// Add command buffer to camera.
			_camera.AddCommandBuffer(kCameraEvent, _buffer);

			// StartCoroutine by CanvasScaler.
#if UNITY_5_4_OR_NEWER
			canvas.rootCanvas.GetComponent<CanvasScaler>().StartCoroutine(_CoUpdateTextureOnNextFrame());
#else
			canvas.GetComponentInParent<CanvasScaler>().StartCoroutine(_CoUpdateTextureOnNextFrame());
#endif
		}

		public void Release()
		{
			_Release(true);
		}

		/// <summary>
		/// This function is called when the MonoBehaviour will be destroyed.
		/// </summary>
		protected override void OnDestroy()
		{
			_Release(true);
			base.OnDestroy();
		}


		#if UNITY_EDITOR
		public void OnBeforeSerialize()
		{
		}

		public void OnAfterDeserialize()
		{
			EditorApplication.delayCall += () =>
			{
				var old = m_EffectMaterial;
				m_EffectMaterial = UIEffect.GetMaterial(Shader.Find(shaderName), toneMode, colorMode, blurMode);
				if (old != m_EffectMaterial)
				{
					EditorUtility.SetDirty(this);
					EditorApplication.delayCall +=AssetDatabase.SaveAssets;
				}
			};
		}
		#endif

		/// <summary>
		/// Callback function when a UI element needs to generate vertices.
		/// </summary>
		protected override void OnPopulateMesh(VertexHelper vh)
		{
			// When not displaying, clear vertex.
			if (texture == null || effectColor.a < 1 / 255f || canvasRenderer.GetAlpha() < 1 / 255f)
				vh.Clear();
			else
				base.OnPopulateMesh(vh);
		}

		const CameraEvent kCameraEvent = CameraEvent.AfterEverything;
		Camera _camera;
		RenderTexture _rt;
		RenderTexture _rtToRelease;
		CommandBuffer _buffer;

		static int s_CopyId;
		static int s_EffectId;

		/// <summary>
		/// Release genarated objects.
		/// </summary>
		void _Release(bool releaseRT)
		{
			if (releaseRT)
			{
				texture = null;

				if (_rt != null)
				{
					_rt.Release();
					_rt = null;
				}
			}

			if (_buffer != null)
			{
				if (_camera != null)
					_camera.RemoveCommandBuffer(kCameraEvent, _buffer);
				_buffer.Release();
				_buffer = null;
			}

			if (_rtToRelease)
			{
				_rtToRelease.Release();
				_rtToRelease = null;
			}
		}

		/// <summary>
		/// Set texture on next frame.
		/// </summary>
		IEnumerator _CoUpdateTextureOnNextFrame()
		{
			yield return new WaitForEndOfFrame();

			_Release(false);
			texture = _rt;
		}
	}
}