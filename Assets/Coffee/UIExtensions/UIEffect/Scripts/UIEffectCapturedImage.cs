using System.Collections;
using System.Linq;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.Rendering;
using UnityEngine.UI;

namespace Coffee.UIExtensions
{
	/// <summary>
	/// UIEffectCapturedImage
	/// </summary>
	public class UIEffectCapturedImage : RawImage
#if UNITY_EDITOR
		, ISerializationCallbackReceiver
#endif
	{

		//################################
		// Constant or Static Members.
		//################################
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


		//################################
		// Serialize Members.
		//################################
		[SerializeField][Range(0, 1)] float m_ToneLevel = 1;
		[SerializeField][Range(0, 1)] float m_Blur = 0;
		[SerializeField] ToneMode m_ToneMode;
		[SerializeField] ColorMode m_ColorMode;
		[SerializeField] BlurMode m_BlurMode;
		[SerializeField] Color m_EffectColor = Color.white;
		[SerializeField] DesamplingRate m_DesamplingRate;
		[SerializeField] DesamplingRate m_ReductionRate;
		[SerializeField] FilterMode m_FilterMode = FilterMode.Bilinear;
		[SerializeField] Material m_EffectMaterial;
		[FormerlySerializedAs("m_Iterations")]
		[SerializeField][Range(1, 8)] int m_BlurIterations = 1;
		[FormerlySerializedAs("m_KeepCanvasSize")]
		[SerializeField] bool m_FitToScreen = true;
		[SerializeField] RenderTexture m_TargetTexture;
		[SerializeField] bool m_CaptureOnEnable = false;


		//################################
		// Public Members.
		//################################
		/// <summary>
		/// Tone effect level between 0(no effect) and 1(complete effect).
		/// </summary>
		public float toneLevel { get { return m_ToneLevel; } set { m_ToneLevel = Mathf.Clamp(value, 0, 1); } }

		/// <summary>
		/// How far is the blurring from the graphic.
		/// </summary>
		public float blur { get { return m_Blur; } set { m_Blur = Mathf.Clamp(value, 0, 4); } }

		/// <summary>
		/// Tone effect mode.
		/// </summary>
		public ToneMode toneMode { get { return m_ToneMode; } set { m_ToneMode = value; } }

		/// <summary>
		/// Color effect mode.
		/// </summary>
		public ColorMode colorMode { get { return m_ColorMode; } set { m_ColorMode = value; } }

		/// <summary>
		/// Blur effect mode.
		/// </summary>
		public BlurMode blurMode { get { return m_BlurMode; } set { m_BlurMode = value; } }

		/// <summary>
		/// Color for the color effect.
		/// </summary>
		public Color effectColor { get { return m_EffectColor; } set { m_EffectColor = value; } }

		/// <summary>
		/// Effect material.
		/// </summary>
		public virtual Material effectMaterial { get { return m_EffectMaterial; } }

		/// <summary>
		/// Desampling rate of the generated RenderTexture.
		/// </summary>
		public DesamplingRate desamplingRate { get { return m_DesamplingRate; } set { m_DesamplingRate = value; } }

		/// <summary>
		/// Desampling rate of reduction buffer to apply effect.
		/// </summary>
		public DesamplingRate reductionRate { get { return m_ReductionRate; } set { m_ReductionRate = value; } }

		/// <summary>
		/// FilterMode for capture.
		/// </summary>
		public FilterMode filterMode { get { return m_FilterMode; } set { m_FilterMode = value; } }

		/// <summary>
		/// Captured texture.
		/// </summary>
		public RenderTexture capturedTexture { get { return m_TargetTexture ? m_TargetTexture : _rt; } }

		/// <summary>
		/// Blur iterations.
		/// </summary>
		[System.Obsolete("Use blurIterations instead (UnityUpgradable) -> blurIterations")]
		public int iterations { get { return m_BlurIterations; } set { m_BlurIterations = value; } }

		/// <summary>
		/// Blur iterations.
		/// </summary>
		public int blurIterations { get { return m_BlurIterations; } set { m_BlurIterations = value; } }

		/// <summary>
		/// Fits graphic size to screen.
		/// </summary>
		[System.Obsolete("Use fitToScreen instead (UnityUpgradable) -> fitToScreen")]
		public bool keepCanvasSize { get { return m_FitToScreen; } set { m_FitToScreen = value; } }

		/// <summary>
		/// Fits graphic size to screen.
		/// </summary>
		public bool fitToScreen { get { return m_FitToScreen; } set { m_FitToScreen = value; } }

		/// <summary>
		/// Target RenderTexture to capture.
		/// </summary>
		public RenderTexture targetTexture { get { return m_TargetTexture; } set { m_TargetTexture = value; } }

		/// <summary>
		/// This function is called when the object becomes enabled and active.
		/// </summary>
		protected override void OnEnable()
		{
			base.OnEnable();
			if (m_CaptureOnEnable && Application.isPlaying)
			{
				Capture();
			}
		}

		protected override void OnDisable()
		{
			base.OnDisable();
			if (m_CaptureOnEnable && Application.isPlaying)
			{
				_Release(false);
				texture = null;
			}
		}

		/// <summary>
		/// This function is called when the MonoBehaviour will be destroyed.
		/// </summary>
		protected override void OnDestroy()
		{
			Release();
			base.OnDestroy();
		}

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

		/// <summary>
		/// Gets the size of the desampling.
		/// </summary>
		public void GetDesamplingSize(DesamplingRate rate, out int w, out int h)
		{
			var cam = canvas.worldCamera ?? Camera.main;
			h = cam.pixelHeight;
			w = cam.pixelWidth;
			if (rate == DesamplingRate.None)
				return;

			float aspect = (float)w / h;
			if (w < h)
			{
				h = Mathf.ClosestPowerOfTwo(h / (int)rate);
				w = Mathf.CeilToInt(h * aspect);
			}
			else
			{
				w = Mathf.ClosestPowerOfTwo(w / (int)rate);
				h = Mathf.CeilToInt(w / aspect);
			}
		}

		/// <summary>
		/// Capture rendering result.
		/// </summary>
		public void Capture()
		{
			var rootCanvas = canvas.rootCanvas;
			if (m_FitToScreen)
			{
				var rootTransform = rootCanvas.transform as RectTransform;
				var size = rootTransform.rect.size;
				rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, size.x);
				rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, size.y);
				rectTransform.position = rootTransform.position;
			}

			// Camera for command buffer.
			_camera = canvas.worldCamera ?? Camera.main;

			// Cache id for RT.
			if (s_CopyId == 0)
			{
				s_CopyId = Shader.PropertyToID("_UIEffectCapturedImage_ScreenCopyId");
				s_EffectId1 = Shader.PropertyToID("_UIEffectCapturedImage_EffectId1");
				s_EffectId2 = Shader.PropertyToID("_UIEffectCapturedImage_EffectId2");

				s_EffectFactorId = Shader.PropertyToID("_EffectFactor");
				s_ColorFactorId = Shader.PropertyToID("_ColorFactor");
			}

			// If size of generated result RT has changed, relese it.
			int w, h;
			GetDesamplingSize(m_DesamplingRate, out w, out h);
			if (m_TargetTexture)
			{
				_rtToRelease = _rt;
			}
			else
			{
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
			}
			SetupCommandBuffer();
		}

		void SetupCommandBuffer()
		{
			if (_buffer != null)
			{
				return;
			}

			bool isOverlay = canvas.rootCanvas.renderMode == RenderMode.ScreenSpaceOverlay;
			int w, h;
			GetDesamplingSize(m_DesamplingRate, out w, out h);

			var rtId = new RenderTargetIdentifier(m_TargetTexture ? m_TargetTexture : _rt);

			// Material for effect.
			Material mat = effectMaterial;

			_buffer = new CommandBuffer();
			_buffer.name = mat ? mat.name : "noeffect";
			if (_rt)
			{
				_rt.name = _buffer.name;
			}

			// Copy to temporary RT.
			_buffer.GetTemporaryRT(s_CopyId, -1, -1, 0, m_FilterMode);

			if (isOverlay)
			{
				if (!s_renderedResult)
				{
					s_renderedResult = new Texture2D(Screen.width, Screen.height, TextureFormat.ARGB32, false, false);
					s_renderedResult.filterMode = FilterMode.Point;
				}
				else if (s_renderedResult.width != Screen.width || s_renderedResult.height != Screen.height)
				{
					s_renderedResult.Resize(Screen.width, Screen.height, TextureFormat.ARGB32, false);
				}

#if UNITY_EDITOR
				if (!Application.isPlaying)
				{
					RenderTexture.active = Resources.FindObjectsOfTypeAll<RenderTexture>().FirstOrDefault(x=>x.name == "GameView RT");
					s_renderedResult.ReadPixels(new Rect(0, 0, s_renderedResult.width, s_renderedResult.height), 0, 0);
					s_renderedResult.Apply(false, false);
					RenderTexture.active = null;
				}
#endif

				_buffer.Blit(new RenderTargetIdentifier(s_renderedResult), s_CopyId);
			}
			else
			{
				_buffer.Blit(BuiltinRenderTextureType.CurrentActive, s_CopyId);
			}

			// Set properties.
			_buffer.SetGlobalVector(s_EffectFactorId, new Vector4(toneLevel, 0));
			_buffer.SetGlobalVector(s_ColorFactorId, new Vector4(effectColor.r, effectColor.g, effectColor.b, effectColor.a));

			// Blit without effect.
			if (!mat)
			{
				_buffer.Blit(s_CopyId, rtId);
				_buffer.ReleaseTemporaryRT(s_CopyId);
			}
			// Blit with effect.
			else
			{
				GetDesamplingSize(m_ReductionRate, out w, out h);
				_buffer.GetTemporaryRT(s_EffectId1, w, h, 0, m_FilterMode);

				// Apply base effect (copied screen -> effect1).
				_buffer.Blit(s_CopyId, s_EffectId1, mat, 0);
				_buffer.ReleaseTemporaryRT(s_CopyId);

				// Iterate the operation.
				if (m_BlurMode != BlurMode.None)
				{
					_buffer.GetTemporaryRT(s_EffectId2, w, h, 0, m_FilterMode);
					for (int i = 0; i < m_BlurIterations; i++)
					{
						// Apply effect (effect1 -> effect2, or effect2 -> effect1).
						_buffer.SetGlobalVector(s_EffectFactorId, new Vector4(blur, 0));
						_buffer.Blit(s_EffectId1, s_EffectId2, mat, 1);
						_buffer.SetGlobalVector(s_EffectFactorId, new Vector4(0, blur));
						_buffer.Blit(s_EffectId2, s_EffectId1, mat, 1);
					}
					_buffer.ReleaseTemporaryRT(s_EffectId2);
				}

				_buffer.Blit(s_EffectId1, rtId);
				_buffer.ReleaseTemporaryRT(s_EffectId1);
			}

#if UNITY_EDITOR
			// Start rendering for editor mode.
			if (!Application.isPlaying)
			{
				_cameraEvent = CameraEvent.AfterEverything;
				_camera.AddCommandBuffer(_cameraEvent, _buffer);
				texture = null;
				_SetDirty();

				UnityEditor.EditorApplication.delayCall += () =>
					{
						_Release(false);
						texture = capturedTexture;
						_SetDirty();
					};
				return;
			}
#endif

			// Start rendering coroutine by CanvasScaler.
			var scaler = canvas.rootCanvas.GetComponent<CanvasScaler>();
			scaler.StartCoroutine(
				isOverlay
				? _CoUpdateTextureOnNextFrameOverlay()
				: _CoUpdateTextureOnNextFrame()
			);
		}

		/// <summary>
		/// Release captured image.
		/// </summary>
		public void Release()
		{
			_Release(true);
			texture = null;
			_SetDirty();
		}

#if UNITY_EDITOR
		protected override void Reset()
		{
			// Set parameters as 'Detail'.
			m_BlurIterations = 5;
			m_FilterMode = FilterMode.Bilinear;
			m_DesamplingRate = DesamplingRate.None;
			m_ReductionRate = DesamplingRate.x1;
			base.Reset();
		}

		/// <summary>
		/// Raises the before serialize event.
		/// </summary>
		public void OnBeforeSerialize()
		{
		}

		/// <summary>
		/// Raises the after deserialize event.
		/// </summary>
		public void OnAfterDeserialize()
		{
			UnityEditor.EditorApplication.delayCall += () => UpdateMaterial(true);
		}

		/// <summary>
		/// Raises the validate event.
		/// </summary>
		protected override void OnValidate ()
		{
			base.OnValidate ();
			UnityEditor.EditorApplication.delayCall += () => UpdateMaterial(false);
		}

		/// <summary>
		/// Updates the material.
		/// </summary>
		/// <param name="ignoreInPlayMode">If set to <c>true</c> ignore in play mode.</param>
		protected void UpdateMaterial(bool ignoreInPlayMode)
		{
			if(!this || ignoreInPlayMode && Application.isPlaying)
			{
				return;
			}

			var mat =  MaterialResolver.GetOrGenerateMaterialVariant(Shader.Find(shaderName), m_ToneMode, m_ColorMode, m_BlurMode);
			if (m_EffectMaterial != mat)
			{
				material = null;
				m_EffectMaterial = mat;
				_SetDirty();
			}
		}
#endif



		//################################
		// Private Members.
		//################################
		CameraEvent _cameraEvent = CameraEvent.AfterEverything;
		Camera _camera;
		RenderTexture _rt;
		RenderTexture _rtToRelease;
		CommandBuffer _buffer;

		public Texture2D s_renderedResult;
		static int s_CopyId;
		static int s_EffectId1;
		static int s_EffectId2;
		static int s_EffectFactorId;
		static int s_ColorFactorId;

		/// <summary>
		/// Release genarated objects.
		/// </summary>
		/// <param name="releaseRT">If set to <c>true</c> release cached RenderTexture.</param>
		void _Release(bool releaseRT)
		{
			if (releaseRT || m_TargetTexture)
			{
				texture = null;
				_Release(ref _rt);
			}

			if (_buffer != null)
			{
				if (_camera != null)
					_camera.RemoveCommandBuffer(_cameraEvent, _buffer);
				_buffer.Release();
				_buffer = null;
			}

			_Release(ref _rtToRelease);
		}

		[System.Diagnostics.Conditional("UNITY_EDITOR")]
		void _SetDirty()
		{
#if UNITY_EDITOR
			if(Application.isPlaying)
			{
				UnityEditor.EditorUtility.SetDirty(this);
			}
#endif
		}

		void _Release(ref RenderTexture obj)
		{
			if (obj)
			{
				obj.Release();
#if UNITY_EDITOR
				if(!Application.isPlaying)
					DestroyImmediate(obj);
				else
#endif
					Destroy(obj);
				obj = null;
			}
		}

		/// <summary>
		/// Set texture on next frame.
		/// </summary>
		IEnumerator _CoUpdateTextureOnNextFrame()
		{
			// Add command buffer to camera.
			_cameraEvent = CameraEvent.AfterEverything;
			_camera.AddCommandBuffer(_cameraEvent, _buffer);
			yield return new WaitForEndOfFrame();

			_Release(false);
			texture = m_TargetTexture ? m_TargetTexture : _rt;
		}

		/// <summary>
		/// Set texture on next frame (for overlay).
		/// </summary>
		IEnumerator _CoUpdateTextureOnNextFrameOverlay()
		{
			// Add command buffer to camera.
			yield return new WaitForEndOfFrame();
			s_renderedResult.ReadPixels(new Rect(0, 0, s_renderedResult.width, s_renderedResult.height), 0, 0);
			s_renderedResult.Apply(false, false);

			_cameraEvent = CameraEvent.BeforeForwardOpaque;
			_camera.AddCommandBuffer(_cameraEvent, _buffer);
			texture = m_TargetTexture ? m_TargetTexture : _rt;

			yield return new WaitForEndOfFrame();
			_Release(false);
		}
	}
}