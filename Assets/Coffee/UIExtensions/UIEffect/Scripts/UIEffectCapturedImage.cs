using System.Collections;
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
		[SerializeField] bool m_KeepCanvasSize = true;
		[SerializeField] RenderTexture m_TargetTexture;


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
		/// Fits graphic size to the root canvas.
		/// </summary>
		public bool keepCanvasSize { get { return m_KeepCanvasSize; } set { m_KeepCanvasSize = value; } }

		/// <summary>
		/// Target RenderTexture to capture.
		/// </summary>
		public RenderTexture targetTexture { get { return m_TargetTexture; } set { m_TargetTexture = value; } }

		/// <summary>
		/// This function is called when the MonoBehaviour will be destroyed.
		/// </summary>
		protected override void OnDestroy()
		{
			_Release(true);
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

			// Create command buffer.
			if (_buffer == null)
			{
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
				_buffer.Blit(BuiltinRenderTextureType.CurrentActive, s_CopyId);

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
					if(m_BlurMode != BlurMode.None)
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
			}

			// Add command buffer to camera.
			_camera.AddCommandBuffer(kCameraEvent, _buffer);

			// StartCoroutine by CanvasScaler.
			var rootCanvas = canvas.rootCanvas;
			var scaler = rootCanvas.GetComponent<CanvasScaler>();
			scaler.StartCoroutine(_CoUpdateTextureOnNextFrame());
			if (m_KeepCanvasSize)
			{
				var rootTransform = rootCanvas.transform as RectTransform;
				var size = rootTransform.rect.size;
				rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, size.x);
				rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, size.y);
				rectTransform.position = rootTransform.position;
			}
		}

		/// <summary>
		/// Release captured image.
		/// </summary>
		public void Release()
		{
			_Release(true);
		}

#if UNITY_EDITOR
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
				UnityEditor.EditorUtility.SetDirty(this);
			}
		}
#endif



		//################################
		// Private Members.
		//################################
		const CameraEvent kCameraEvent = CameraEvent.AfterEverything;
		Camera _camera;
		RenderTexture _rt;
		RenderTexture _rtToRelease;
		CommandBuffer _buffer;

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
			texture = m_TargetTexture ? m_TargetTexture : _rt;
		}
	}
}