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
	[AddComponentMenu("UI/UIEffect/UIEffectCapturedImage", 200)]
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
		[Tooltip("Effect factor between 0(no effect) and 1(complete effect).")]
		[FormerlySerializedAs("m_ToneLevel")]
		[SerializeField][Range(0, 1)] float m_EffectFactor = 1;

		[Tooltip("Color effect factor between 0(no effect) and 1(complete effect).")]
		[SerializeField][Range(0, 1)] float m_ColorFactor = 1;

		[Tooltip("How far is the blurring from the graphic.")]
		[FormerlySerializedAs("m_Blur")]
		[SerializeField][Range(0, 1)] float m_BlurFactor = 1;

		[Tooltip("Effect mode.")]
		[FormerlySerializedAs("m_ToneMode")]
		[SerializeField] EffectMode m_EffectMode = EffectMode.None;

		[Tooltip("Color effect mode.")]
		[SerializeField] ColorMode m_ColorMode = ColorMode.Multiply;

		[Tooltip("Blur effect mode.")]
		[SerializeField] BlurMode m_BlurMode = BlurMode.DetailBlur;

		[Tooltip("Color for the color effect.")]
		[SerializeField] Color m_EffectColor = Color.white;

		[Tooltip("Desampling rate of the generated RenderTexture.")]
		[SerializeField] DesamplingRate m_DesamplingRate = DesamplingRate.x1;

		[Tooltip("Desampling rate of reduction buffer to apply effect.")]
		[SerializeField] DesamplingRate m_ReductionRate = DesamplingRate.x1;

		[Tooltip("FilterMode for capturing.")]
		[SerializeField] FilterMode m_FilterMode = FilterMode.Bilinear;

		[Tooltip("Effect material.")]
		[SerializeField] Material m_EffectMaterial;

		[Tooltip("Blur iterations.")]
		[FormerlySerializedAs("m_Iterations")]
		[SerializeField][Range(1, 8)] int m_BlurIterations = 3;

		[Tooltip("Fits graphic size to screen on captured.")]
		[FormerlySerializedAs("m_KeepCanvasSize")]
		[SerializeField] bool m_FitToScreen = true;

		[Tooltip("Capture automatically on enable.")]
		[SerializeField] bool m_CaptureOnEnable = false;

		[Tooltip("Capture immediately.")]
		[SerializeField] bool m_ImmediateCapturing = true;


		//################################
		// Public Members.
		//################################
		/// <summary>
		/// Effect factor between 0(no effect) and 1(complete effect).
		/// </summary>
		[System.Obsolete("Use effectFactor instead (UnityUpgradable) -> effectFactor")]
		public float toneLevel { get { return m_EffectFactor; } set { m_EffectFactor = Mathf.Clamp(value, 0, 1); } }

		/// <summary>
		/// Effect factor between 0(no effect) and 1(complete effect).
		/// </summary>
		public float effectFactor { get { return m_EffectFactor; } set { m_EffectFactor = Mathf.Clamp(value, 0, 1); } }

		/// <summary>
		/// Color effect factor between 0(no effect) and 1(complete effect).
		/// </summary>
		public float colorFactor { get { return m_ColorFactor; } set { m_ColorFactor = Mathf.Clamp(value, 0, 1); } }

		/// <summary>
		/// How far is the blurring from the graphic.
		/// </summary>
		[System.Obsolete("Use blurFactor instead (UnityUpgradable) -> blurFactor")]
		public float blur { get { return m_BlurFactor; } set { m_BlurFactor = Mathf.Clamp(value, 0, 4); } }

		/// <summary>
		/// How far is the blurring from the graphic.
		/// </summary>
		public float blurFactor { get { return m_BlurFactor; } set { m_BlurFactor = Mathf.Clamp(value, 0, 4); } }

		/// <summary>
		/// Tone effect mode.
		/// </summary>
		[System.Obsolete("Use effectMode instead (UnityUpgradable) -> effectMode")]
		public EffectMode toneMode { get { return m_EffectMode; } }

		/// <summary>
		/// Effect mode.
		/// </summary>
		public EffectMode effectMode { get { return m_EffectMode; } }

		/// <summary>
		/// Color effect mode.
		/// </summary>
		public ColorMode colorMode { get { return m_ColorMode; } }

		/// <summary>
		/// Blur effect mode.
		/// </summary>
		public BlurMode blurMode { get { return m_BlurMode; } }

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
		/// FilterMode for capturing.
		/// </summary>
		public FilterMode filterMode { get { return m_FilterMode; } set { m_FilterMode = value; } }

		/// <summary>
		/// Captured texture.
		/// </summary>
		public RenderTexture capturedTexture { get { return _rt; } }

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
		/// Fits graphic size to screen on captured.
		/// </summary>
		public bool fitToScreen { get { return m_FitToScreen; } set { m_FitToScreen = value; } }

		/// <summary>
		/// Target RenderTexture to capture.
		/// </summary>
		[System.Obsolete]
		public RenderTexture targetTexture { get { return null; } set { } }

		/// <summary>
		/// Capture automatically on enable.
		/// </summary>
		public bool captureOnEnable { get { return m_CaptureOnEnable; } set { m_CaptureOnEnable = value; } }

		/// <summary>
		/// Capture immediately.
		/// </summary>
		public bool immediateCapturing { get { return m_ImmediateCapturing; } set { m_ImmediateCapturing = value; } }

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
			if (texture == null || color.a < 1 / 255f || canvasRenderer.GetAlpha() < 1 / 255f)
			{
				vh.Clear();
			}
			else
			{
				base.OnPopulateMesh(vh);
				int count = vh.currentVertCount;
				UIVertex vt = default(UIVertex);
				Color c = new Color(1, 1, 1, color.a);
				for (int i = 0; i < count; i++)
				{
					vh.PopulateUIVertex(ref vt, i);
					vt.color = c;
					vh.SetUIVertex(vt, i);
				}
			}
		}

		/// <summary>
		/// Gets the size of the desampling.
		/// </summary>
		public void GetDesamplingSize(DesamplingRate rate, out int w, out int h)
		{
#if UNITY_EDITOR
			if (!Application.isPlaying)
			{
				var res = UnityEditor.UnityStats.screenRes.Split('x');
				w = int.Parse(res[0]);
				h = int.Parse(res[1]);
			}
			else
#endif
			{
				w = Screen.width;
				h = Screen.height;
			}

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
			// Fit to screen.
			var rootCanvas = canvas.rootCanvas;
			if (m_FitToScreen)
			{
				var rootTransform = rootCanvas.transform as RectTransform;
				var size = rootTransform.rect.size;
				rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, size.x);
				rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, size.y);
				rectTransform.position = rootTransform.position;
			}

			// Cache some ids.
			if (s_CopyId == 0)
			{
				s_CopyId = Shader.PropertyToID("_UIEffectCapturedImage_ScreenCopyId");
				s_EffectId1 = Shader.PropertyToID("_UIEffectCapturedImage_EffectId1");
				s_EffectId2 = Shader.PropertyToID("_UIEffectCapturedImage_EffectId2");

				s_EffectFactorId = Shader.PropertyToID("_EffectFactor");
				s_ColorFactorId = Shader.PropertyToID("_ColorFactor");
				s_CommandBuffer = new CommandBuffer();
			}


			// If size of result RT has changed, release it.
			int w, h;
			GetDesamplingSize(m_DesamplingRate, out w, out h);
			if (_rt && (_rt.width != w || _rt.height != h))
			{
				_Release(ref _rt);
			}

			// Generate RT for result.
			if (_rt == null)
			{
				_rt = RenderTexture.GetTemporary(w, h, 0, RenderTextureFormat.ARGB32, RenderTextureReadWrite.Default);
				_rt.filterMode = m_FilterMode;
				_rt.useMipMap = false;
				_rt.wrapMode = TextureWrapMode.Clamp;
				_rtId = new RenderTargetIdentifier(_rt);
			}
			SetupCommandBuffer();
		}

		void SetupCommandBuffer()
		{
			// Material for effect.
			Material mat = m_EffectMaterial;

			if (s_CommandBuffer == null)
			{
				s_CommandBuffer = new CommandBuffer();
			}

			// [1] Capture from back buffer (back buffer -> copied screen).
			int w, h;
			GetDesamplingSize(DesamplingRate.None, out w, out h);
			s_CommandBuffer.GetTemporaryRT(s_CopyId, w, h, 0, m_FilterMode);
#if UNITY_EDITOR
			s_CommandBuffer.Blit(Resources.FindObjectsOfTypeAll<RenderTexture>().FirstOrDefault(x => x.name == "GameView RT"), s_CopyId);
#else
			s_CommandBuffer.Blit(BuiltinRenderTextureType.BindableTexture, s_CopyId);
#endif

			// Set properties for effect.
			s_CommandBuffer.SetGlobalVector(s_EffectFactorId, new Vector4(m_EffectFactor, 0));
			s_CommandBuffer.SetGlobalVector(s_ColorFactorId, new Vector4(m_EffectColor.r, m_EffectColor.g, m_EffectColor.b, m_EffectColor.a));

			// [2] Apply base effect with reduction buffer (copied screen -> effect1).
			GetDesamplingSize(m_ReductionRate, out w, out h);
			s_CommandBuffer.GetTemporaryRT(s_EffectId1, w, h, 0, m_FilterMode);
			s_CommandBuffer.Blit(s_CopyId, s_EffectId1, mat, 0);
			s_CommandBuffer.ReleaseTemporaryRT(s_CopyId);

			// Iterate blurring operation.
			if (m_BlurMode != BlurMode.None)
			{
				s_CommandBuffer.GetTemporaryRT(s_EffectId2, w, h, 0, m_FilterMode);
				for (int i = 0; i < m_BlurIterations; i++)
				{
					// [3] Apply blurring with reduction buffer (effect1 -> effect2, or effect2 -> effect1).
					s_CommandBuffer.SetGlobalVector(s_EffectFactorId, new Vector4(m_BlurFactor, 0));
					s_CommandBuffer.Blit(s_EffectId1, s_EffectId2, mat, 1);
					s_CommandBuffer.SetGlobalVector(s_EffectFactorId, new Vector4(0, m_BlurFactor));
					s_CommandBuffer.Blit(s_EffectId2, s_EffectId1, mat, 1);
				}
				s_CommandBuffer.ReleaseTemporaryRT(s_EffectId2);
			}

			// [4] Copy to result RT.
			s_CommandBuffer.Blit(s_EffectId1, _rtId);
			s_CommandBuffer.ReleaseTemporaryRT(s_EffectId1);

#if UNITY_EDITOR
			if (!Application.isPlaying)
			{
				Graphics.ExecuteCommandBuffer(s_CommandBuffer);

				UpdateTexture();
				return;
			}
			else
			{
				Graphics.ExecuteCommandBuffer(s_CommandBuffer);
			}
#endif
			if (m_ImmediateCapturing)
			{
				UpdateTexture();
			}
			else
			{
				// Execute command buffer.
				canvas.rootCanvas.GetComponent<CanvasScaler>().StartCoroutine(_CoUpdateTextureOnNextFrame());
			}
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
			// Set parameters as 'Medium'.
			m_BlurIterations = 3;
			m_FilterMode = FilterMode.Bilinear;
			m_DesamplingRate = DesamplingRate.x1;
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
		protected override void OnValidate()
		{
			base.OnValidate();
			UnityEditor.EditorApplication.delayCall += () => UpdateMaterial(false);
		}

		/// <summary>
		/// Updates the material.
		/// </summary>
		/// <param name="ignoreInPlayMode">If set to <c>true</c> ignore in play mode.</param>
		protected void UpdateMaterial(bool ignoreInPlayMode)
		{
			if (!this || ignoreInPlayMode && Application.isPlaying)
			{
				return;
			}

			var mat = MaterialResolver.GetOrGenerateMaterialVariant(Shader.Find(shaderName), m_EffectMode, m_ColorMode, m_BlurMode);
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
		RenderTexture _rt;
		RenderTargetIdentifier _rtId;

		static int s_CopyId;
		static int s_EffectId1;
		static int s_EffectId2;
		static int s_EffectFactorId;
		static int s_ColorFactorId;
		static CommandBuffer s_CommandBuffer;

		/// <summary>
		/// Release genarated objects.
		/// </summary>
		/// <param name="releaseRT">If set to <c>true</c> release cached RenderTexture.</param>
		void _Release(bool releaseRT)
		{
			if (releaseRT)
			{
				texture = null;
				_Release(ref _rt);
			}

			if (s_CommandBuffer != null)
			{
				s_CommandBuffer.Clear();

				if (releaseRT)
				{
					s_CommandBuffer.Release();
					s_CommandBuffer = null;
				}
			}
		}

		[System.Diagnostics.Conditional("UNITY_EDITOR")]
		void _SetDirty()
		{
#if UNITY_EDITOR
			if (!Application.isPlaying)
			{
				UnityEditor.EditorUtility.SetDirty(this);
			}
#endif
		}

		void _Release(ref RenderTexture obj)
		{
			if (obj)
			{
				RenderTexture.ReleaseTemporary (obj);
				obj = null;
			}
		}

		/// <summary>
		/// Set texture on next frame.
		/// </summary>
		IEnumerator _CoUpdateTextureOnNextFrame()
		{
			yield return new WaitForEndOfFrame();
			UpdateTexture();
		}

		void UpdateTexture()
		{
#if !UNITY_EDITOR
			// Execute command buffer.
			Graphics.ExecuteCommandBuffer (s_CommandBuffer);
#endif
			_Release(false);
			texture = capturedTexture;
			_SetDirty();
		}

	}
}