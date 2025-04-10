using System;
using System.Collections.Generic;
using Coffee.UIEffectInternal;
using UnityEngine;
using UnityEngine.Profiling;
using UnityEngine.Rendering;
using UnityEngine.UI;

namespace Coffee.UIEffects
{
    public class UIEffectContext
    {
        private static readonly UIEffectContext s_DefaultContext = new UIEffectContext();
        private static readonly List<UIVertex> s_WorkingVertices = new List<UIVertex>(1024 * 8);
        private static readonly int s_SrcBlend = Shader.PropertyToID("_SrcBlend");
        private static readonly int s_DstBlend = Shader.PropertyToID("_DstBlend");
        private static readonly int s_ToneIntensity = Shader.PropertyToID("_ToneIntensity");
        private static readonly int s_ToneParams = Shader.PropertyToID("_ToneParams");
        private static readonly int s_ColorFilter = Shader.PropertyToID("_ColorFilter");
        private static readonly int s_ColorValue = Shader.PropertyToID("_ColorValue");
        private static readonly int s_ColorIntensity = Shader.PropertyToID("_ColorIntensity");
        private static readonly int s_ColorGlow = Shader.PropertyToID("_ColorGlow");
        private static readonly int s_SamplingIntensity = Shader.PropertyToID("_SamplingIntensity");
        private static readonly int s_SamplingWidth = Shader.PropertyToID("_SamplingWidth");
        private static readonly int s_SamplingScale = Shader.PropertyToID("_SamplingScale");
        private static readonly int s_TransitionRate = Shader.PropertyToID("_TransitionRate");
        private static readonly int s_TransitionReverse = Shader.PropertyToID("_TransitionReverse");
        private static readonly int s_TransitionTex = Shader.PropertyToID("_TransitionTex");
        private static readonly int s_TransitionTex_ST = Shader.PropertyToID("_TransitionTex_ST");
        private static readonly int s_TransitionTex_Speed = Shader.PropertyToID("_TransitionTex_Speed");
        private static readonly int s_TransitionWidth = Shader.PropertyToID("_TransitionWidth");
        private static readonly int s_TransitionSoftness = Shader.PropertyToID("_TransitionSoftness");
        private static readonly int s_TransitionRange = Shader.PropertyToID("_TransitionRange");
        private static readonly int s_TransitionColorFilter = Shader.PropertyToID("_TransitionColorFilter");
        private static readonly int s_TransitionColor = Shader.PropertyToID("_TransitionColor");
        private static readonly int s_TransitionColorGlow = Shader.PropertyToID("_TransitionColorGlow");
        private static readonly int s_TransitionPatternReverse = Shader.PropertyToID("_TransitionPatternReverse");
        private static readonly int s_TransitionAutoPlaySpeed = Shader.PropertyToID("_TransitionAutoPlaySpeed");
        private static readonly int s_TargetColor = Shader.PropertyToID("_TargetColor");
        private static readonly int s_TargetRange = Shader.PropertyToID("_TargetRange");
        private static readonly int s_TargetSoftness = Shader.PropertyToID("_TargetSoftness");
        private static readonly int s_ShadowColorFilter = Shader.PropertyToID("_ShadowColorFilter");
        private static readonly int s_ShadowColor = Shader.PropertyToID("_ShadowColor");
        private static readonly int s_ShadowBlurIntensity = Shader.PropertyToID("_ShadowBlurIntensity");
        private static readonly int s_ShadowColorGlow = Shader.PropertyToID("_ShadowColorGlow");
        private static readonly int s_EdgeWidth = Shader.PropertyToID("_EdgeWidth");
        private static readonly int s_EdgeColorFilter = Shader.PropertyToID("_EdgeColorFilter");
        private static readonly int s_EdgeColor = Shader.PropertyToID("_EdgeColor");
        private static readonly int s_EdgeColorGlow = Shader.PropertyToID("_EdgeColorGlow");
        private static readonly int s_EdgeShinyAutoPlaySpeed = Shader.PropertyToID("_EdgeShinyAutoPlaySpeed");
        private static readonly int s_EdgeShinyRate = Shader.PropertyToID("_EdgeShinyRate");
        private static readonly int s_EdgeShinyWidth = Shader.PropertyToID("_EdgeShinyWidth");
        private static readonly int s_PatternArea = Shader.PropertyToID("_PatternArea");
        private static readonly int s_DetailIntensity = Shader.PropertyToID("_DetailIntensity");
        private static readonly int s_DetailThreshold = Shader.PropertyToID("_DetailThreshold");
        private static readonly int s_DetailTex = Shader.PropertyToID("_DetailTex");
        private static readonly int s_DetailTex_ST = Shader.PropertyToID("_DetailTex_ST");
        private static readonly int s_DetailTex_Speed = Shader.PropertyToID("_DetailTex_Speed");
        private static readonly int s_GradationIntensity = Shader.PropertyToID("_GradationIntensity");
        private static readonly int s_GradationColorFilter = Shader.PropertyToID("_GradationColorFilter");
        private static readonly int s_GradationColor1 = Shader.PropertyToID("_GradationColor1");
        private static readonly int s_GradationColor2 = Shader.PropertyToID("_GradationColor2");
        private static readonly int s_GradationColor3 = Shader.PropertyToID("_GradationColor3");
        private static readonly int s_GradationColor4 = Shader.PropertyToID("_GradationColor4");
        private static readonly int s_GradationTex = Shader.PropertyToID("_GradationTex");
        private static readonly int s_GradationTex_ST = Shader.PropertyToID("_GradationTex_ST");
        private static readonly int s_RootViewMatrix = Shader.PropertyToID("_RootViewMatrix");
        private static readonly int s_GradViewMatrix = Shader.PropertyToID("_GradViewMatrix");
        private static readonly int s_CanvasToWorldMatrix = Shader.PropertyToID("_CanvasToWorldMatrix");

        private static readonly string[] s_ToneKeywords =
        {
            "",
            "TONE_GRAYSCALE",
            "TONE_SEPIA",
            "TONE_NEGATIVE",
            "TONE_RETRO",
            "TONE_POSTERIZE"
        };

        private static readonly string[] s_ColorKeywords =
        {
            "",
            "COLOR_FILTER"
        };

        private static readonly string[] s_SamplingKeywords =
        {
            "",
            "SAMPLING_BLUR_FAST",
            "SAMPLING_BLUR_MEDIUM",
            "SAMPLING_BLUR_DETAIL",
            "SAMPLING_PIXELATION",
            "SAMPLING_RGB_SHIFT",
            "SAMPLING_EDGE_LUMINANCE",
            "SAMPLING_EDGE_ALPHA"
        };

        private static readonly string[] s_TransitionKeywords =
        {
            "",
            "TRANSITION_FADE",
            "TRANSITION_CUTOFF",
            "TRANSITION_DISSOLVE",
            "TRANSITION_SHINY",
            "TRANSITION_MASK",
            "TRANSITION_MELT",
            "TRANSITION_BURN",
            "TRANSITION_PATTERN"
        };

        private static readonly string[] s_TargetKeywords =
        {
            "",
            "TARGET_HUE",
            "TARGET_LUMINANCE"
        };

        private static readonly string[] s_EdgeKeywords =
        {
            "",
            "EDGE_PLAIN",
            "EDGE_SHINY"
        };

        private static readonly string[] s_DetailKeywords =
        {
            "",
            "DETAIL_MASKING",
            "DETAIL_MULTIPLY",
            "DETAIL_ADDITIVE",
            "DETAIL_REPLACE",
            "DETAIL_MULTIPLY_ADDITIVE"
        };

        private static readonly string[] s_GradationKeywords =
        {
            "",
            "GRADATION_GRADIENT",
            "GRADATION_RADIAL",
            "GRADATION_COLOR2",
            "GRADATION_COLOR4"
        };

        private static readonly Vector2[][] s_ShadowVectors = new[]
        {
            Array.Empty<Vector2>(), // None
            new[] { Vector2.one }, // Shadow
            new[] { Vector2.one, Vector2.right, Vector2.up }, // Shadow3
            new[] { Vector2.one, -Vector2.one, new Vector2(1, -1), new Vector2(-1, 1) }, // Outline
            new[]
            {
                Vector2.one, -Vector2.one, new Vector2(1, -1), new Vector2(-1, 1),
                Vector2.right, Vector2.up, Vector2.left, Vector2.down
            } // Outline8
        };

        public static Func<UIVertex, Rect, UIVertex> onModifyVertex;
        public ToneFilter toneFilter = ToneFilter.None;
        public float toneIntensity = 1;
        public Vector4 toneParams = new Vector4(0, 0, 0, 0);

        public ColorFilter colorFilter = ColorFilter.None;
        public float colorIntensity = 1;
        public Color color = Color.white;
        public bool colorGlow;

        public SamplingFilter samplingFilter = SamplingFilter.None;
        public float samplingIntensity = 0.5f;
        public float samplingWidth;

        public TransitionFilter transitionFilter = TransitionFilter.None;
        public float transitionRate = 0.5f;
        public bool transitionReverse;
        public Texture transitionTex;
        public Vector2 transitionTexScale = new Vector2(1, 1);
        public Vector2 transitionTexOffset = new Vector2(0, 0);
        public Vector2 transitionTexSpeed = new Vector2(0, 0);
        public float transitionRotation = 0;
        public bool transitionKeepAspectRatio = true;
        public float transitionWidth = 0.2f;
        public float transitionSoftness = 0.2f;
        public MinMax01 transitionRange = new MinMax01(0, 1);
        public ColorFilter transitionColorFilter = ColorFilter.MultiplyAdditive;
        public Color transitionColor = new Color(0f, 0.5f, 1.0f, 1.0f);
        public bool transitionColorGlow;
        public bool transitionPatternReverse;
        public float transitionAutoPlaySpeed;

        public TargetMode targetMode = TargetMode.None;
        public Color targetColor = Color.white;
        public float targetRange = 1;
        public float targetSoftness = 0.5f;

        public BlendMode srcBlendMode = BlendMode.One;
        public BlendMode dstBlendMode = BlendMode.OneMinusSrcAlpha;

        public ShadowMode shadowMode = ShadowMode.None;
        public Vector2 shadowDistance = new Vector2(1f, -1f);
        public int shadowIteration = 1;
        public float shadowFade = 0.9f;
        public float shadowMirrorScale = 0.5f;
        public float shadowBlurIntensity;
        public ColorFilter shadowColorFilter;
        public Color shadowColor;
        public bool shadowColorGlow;

        public EdgeMode edgeMode;
        public float edgeShinyRate;
        public float edgeWidth;
        public ColorFilter edgeColorFilter;
        public Color edgeColor;
        public bool edgeColorGlow;
        public float edgeShinyWidth;
        public float edgeShinyAutoPlaySpeed;
        public PatternArea patternArea;

        public GradationMode gradationMode;
        public float gradationIntensity;
        public GradationColorFilter gradationColorFilter;
        public Color gradationColor1;
        public Color gradationColor2;
        public Color gradationColor3;
        public Color gradationColor4;
        public Texture2D gradationTex;
        public float gradationOffset;
        public float gradationScale;
        public float gradationRotation;
        private List<float> _keyTimes;

        public DetailFilter detailFilter;
        public float detailIntensity;
        public MinMax01 detailThreshold;
        public Texture detailTex;
        public Vector2 detailTexScale = new Vector2(1, 1);
        public Vector2 detailTexOffset = new Vector2(0, 0);
        public Vector2 detailTexSpeed = new Vector2(0, 0);

        public bool willModifyMaterial => toneFilter != ToneFilter.None
                                          || colorFilter != ColorFilter.None
                                          || samplingFilter != SamplingFilter.None
                                          || transitionFilter != TransitionFilter.None
                                          || srcBlendMode != BlendMode.One
                                          || dstBlendMode != BlendMode.OneMinusSrcAlpha
                                          || shadowMode != ShadowMode.None
                                          || edgeMode != EdgeMode.None
                                          || detailFilter != DetailFilter.None
                                          || gradationMode != GradationMode.None;

        public bool willModifyVertex => willModifyMaterial;

        public void Reset()
        {
            InternalListPool<float>.Return(ref _keyTimes);
            CopyFrom(s_DefaultContext);
        }

        private void CopyFrom(UIEffectContext preset)
        {
            toneFilter = preset.toneFilter;
            toneIntensity = preset.toneIntensity;
            toneParams = preset.toneParams;

            colorFilter = preset.colorFilter;
            color = preset.color;
            colorIntensity = preset.colorIntensity;
            colorGlow = preset.colorGlow;

            samplingFilter = preset.samplingFilter;
            samplingIntensity = preset.samplingIntensity;
            samplingWidth = preset.samplingWidth;

            transitionFilter = preset.transitionFilter;
            transitionRate = preset.transitionRate;
            transitionReverse = preset.transitionReverse;
            transitionTex = preset.transitionTex;
            transitionTexScale = preset.transitionTexScale;
            transitionTexOffset = preset.transitionTexOffset;
            transitionTexSpeed = preset.transitionTexSpeed;
            transitionKeepAspectRatio = preset.transitionKeepAspectRatio;
            transitionRotation = preset.transitionRotation;
            transitionWidth = preset.transitionWidth;
            transitionSoftness = preset.transitionSoftness;
            transitionRange = preset.transitionRange;
            transitionColor = preset.transitionColor;
            transitionColorFilter = preset.transitionColorFilter;
            transitionColorGlow = preset.transitionColorGlow;
            transitionPatternReverse = preset.transitionPatternReverse;
            transitionAutoPlaySpeed = preset.transitionAutoPlaySpeed;

            targetMode = preset.targetMode;
            targetColor = preset.targetColor;
            targetRange = preset.targetRange;
            targetSoftness = preset.targetSoftness;

            srcBlendMode = preset.srcBlendMode;
            dstBlendMode = preset.dstBlendMode;

            shadowMode = preset.shadowMode;
            shadowDistance = preset.shadowDistance;
            shadowIteration = preset.shadowIteration;
            shadowFade = preset.shadowFade;
            shadowMirrorScale = preset.shadowMirrorScale;
            shadowBlurIntensity = preset.shadowBlurIntensity;
            shadowColorFilter = preset.shadowColorFilter;
            shadowColor = preset.shadowColor;
            shadowColorGlow = preset.shadowColorGlow;

            edgeMode = preset.edgeMode;
            edgeShinyRate = preset.edgeShinyRate;
            edgeWidth = preset.edgeWidth;
            edgeColorFilter = preset.edgeColorFilter;
            edgeColor = preset.edgeColor;
            edgeColorGlow = preset.edgeColorGlow;
            edgeShinyAutoPlaySpeed = preset.edgeShinyAutoPlaySpeed;
            edgeShinyWidth = preset.edgeShinyWidth;
            patternArea = preset.patternArea;

            gradationMode = preset.gradationMode;
            gradationIntensity = preset.gradationIntensity;
            gradationColorFilter = preset.gradationColorFilter;
            gradationColor1 = preset.gradationColor1;
            gradationColor2 = preset.gradationColor2;
            gradationColor3 = preset.gradationColor3;
            gradationColor4 = preset.gradationColor4;
            gradationTex = preset.gradationTex;
            gradationScale = preset.gradationScale;
            gradationOffset = preset.gradationOffset;
            gradationRotation = preset.gradationRotation;

            detailFilter = preset.detailFilter;
            detailIntensity = preset.detailIntensity;
            detailThreshold = preset.detailThreshold;
            detailTex = preset.detailTex;
            detailTexScale = preset.detailTexScale;
            detailTexOffset = preset.detailTexOffset;
            detailTexSpeed = preset.detailTexSpeed;
        }

        public void SetGradationDirty()
        {
            InternalListPool<float>.Return(ref _keyTimes);
        }

        public void ApplyToMaterial(Material material, float actualSamplingScale = 1f)
        {
            if (!material) return;

            Profiler.BeginSample("(UIE)[UIEffect] GetModifiedMaterial");

            material.SetInt(s_SrcBlend, (int)srcBlendMode);
            material.SetInt(s_DstBlend, (int)dstBlendMode);

            material.SetFloat(s_ToneIntensity, Mathf.Clamp01(toneIntensity));
            material.SetVector(s_ToneParams, toneParams);

            material.SetInt(s_ColorFilter, (int)colorFilter);
            material.SetColor(s_ColorValue, color);
            material.SetFloat(s_ColorIntensity, Mathf.Clamp01(colorIntensity));
            material.SetInt(s_ColorGlow, colorGlow ? 1 : 0);

            material.SetFloat(s_SamplingIntensity, Mathf.Clamp01(samplingIntensity));
            material.SetFloat(s_SamplingWidth, samplingWidth);
            material.SetFloat(s_SamplingScale, actualSamplingScale);

            material.SetFloat(s_TransitionRate, Mathf.Clamp01(transitionRate));
            material.SetInt(s_TransitionReverse, transitionReverse ? 1 : 0);
            material.SetTexture(s_TransitionTex, transitionFilter != 0 ? transitionTex : null);
            material.SetVector(s_TransitionTex_ST,
                new Vector4(transitionTexScale.x, transitionTexScale.y,
                    transitionTexOffset.x, transitionTexOffset.y));
            material.SetVector(s_TransitionTex_Speed, transitionTexSpeed);
            material.SetFloat(s_TransitionWidth, Mathf.Clamp01(transitionWidth));
            material.SetFloat(s_TransitionSoftness, Mathf.Clamp01(transitionSoftness));
            material.SetVector(s_TransitionRange, new Vector2(transitionRange.min, transitionRange.max));
            material.SetInt(s_TransitionColorFilter, (int)transitionColorFilter);
            material.SetColor(s_TransitionColor, transitionColor);
            material.SetInt(s_TransitionColorGlow, transitionColorGlow ? 1 : 0);
            material.SetInt(s_TransitionPatternReverse, transitionPatternReverse ? 1 : 0);
            material.SetFloat(s_TransitionAutoPlaySpeed, transitionAutoPlaySpeed);

            material.SetColor(s_TargetColor, targetColor);
            material.SetFloat(s_TargetRange, Mathf.Clamp01(targetRange));
            material.SetFloat(s_TargetSoftness, Mathf.Clamp01(targetSoftness));

            switch (samplingFilter)
            {
                case SamplingFilter.BlurFast:
                case SamplingFilter.BlurMedium:
                case SamplingFilter.BlurDetail:
                    material.SetFloat(s_ShadowBlurIntensity, Mathf.Clamp01(shadowBlurIntensity));
                    break;
                default:
                    material.SetFloat(s_ShadowBlurIntensity, Mathf.Clamp01(samplingIntensity));
                    break;
            }

            material.SetInt(s_ShadowColorFilter, (int)shadowColorFilter);
            material.SetColor(s_ShadowColor, shadowColor);
            material.SetInt(s_ShadowColorGlow, shadowColorGlow ? 1 : 0);

            material.SetFloat(s_EdgeWidth, Mathf.Clamp01(edgeWidth));
            material.SetInt(s_EdgeColorFilter, (int)edgeColorFilter);
            material.SetColor(s_EdgeColor, edgeColor);
            material.SetInt(s_EdgeColorGlow, edgeColorGlow ? 1 : 0);
            material.SetFloat(s_EdgeShinyRate, Mathf.Clamp01(edgeShinyRate));
            material.SetFloat(s_EdgeShinyWidth, Mathf.Clamp01(edgeShinyWidth));
            material.SetFloat(s_EdgeShinyAutoPlaySpeed, edgeShinyAutoPlaySpeed);
            material.SetInt(s_PatternArea, edgeMode != EdgeMode.None ? (int)patternArea : 0);

            material.SetTexture(s_GradationTex, gradationMode != 0 ? gradationTex : null);
            material.SetVector(s_GradationTex_ST, GetGradationScaleAndOffset());

            material.SetFloat(s_GradationIntensity, gradationIntensity);
            material.SetInt(s_GradationColorFilter, (int)gradationColorFilter);
            material.SetColor(s_GradationColor1, gradationColor1);
            material.SetColor(s_GradationColor2, gradationColor2);
            material.SetColor(s_GradationColor3, gradationColor3);
            material.SetColor(s_GradationColor4, gradationColor4);

            material.SetFloat(s_DetailIntensity, Mathf.Clamp01(detailIntensity));
            material.SetVector(s_DetailThreshold, new Vector2(detailThreshold.min, detailThreshold.max));
            material.SetTexture(s_DetailTex, detailFilter != 0 ? detailTex : null);
            material.SetVector(s_DetailTex_ST,
                new Vector4(detailTexScale.x, detailTexScale.y,
                    detailTexOffset.x, detailTexOffset.y));
            material.SetVector(s_DetailTex_Speed, detailTexSpeed);

            SetKeyword(material, s_ToneKeywords, (int)toneFilter);
            SetKeyword(material, s_ColorKeywords, colorFilter != 0 || shadowMode != 0 ? 1 : 0);
            SetKeyword(material, s_SamplingKeywords, (int)samplingFilter);
            SetKeyword(material, s_TransitionKeywords, (int)transitionFilter);
            SetKeyword(material, s_EdgeKeywords, (int)edgeMode);
            SetKeyword(material, s_DetailKeywords, (int)detailFilter);
            SetKeyword(material, s_TargetKeywords, (int)targetMode);

            switch (gradationMode)
            {
                case GradationMode.None:
                    SetKeyword(material, s_GradationKeywords, 0);
                    break;
                case GradationMode.HorizontalGradient:
                case GradationMode.VerticalGradient:
                case GradationMode.AngleGradient:
                    SetKeyword(material, s_GradationKeywords, 1);
                    break;
                case GradationMode.RadialFast:
                case GradationMode.RadialDetail:
                    SetKeyword(material, s_GradationKeywords, 2);
                    break;
                case GradationMode.Horizontal:
                case GradationMode.Vertical:
                case GradationMode.DiagonalToRightBottom:
                case GradationMode.DiagonalToLeftBottom:
                case GradationMode.Angle:
                    SetKeyword(material, s_GradationKeywords, 3);
                    break;
                case GradationMode.Diagonal:
                    SetKeyword(material, s_GradationKeywords, 4);
                    break;
            }

            Profiler.EndSample();
        }

        public void SetEnablePreview(bool enable, Material material)
        {
            if (!material) return;
            material.SetVector(s_TransitionTex_Speed, enable ? (Vector4)transitionTexSpeed : Vector4.zero);
            material.SetVector(s_DetailTex_Speed, enable ? (Vector4)detailTexSpeed : Vector4.zero);
            material.SetFloat(s_TransitionAutoPlaySpeed, enable ? transitionAutoPlaySpeed : 0);
            material.SetFloat(s_EdgeShinyAutoPlaySpeed, enable ? edgeShinyAutoPlaySpeed : 0);
        }

        public void UpdateViewMatrix(Material material, RectTransform transitionRoot, Canvas canvas, Flip flip)
        {
            if (!material) return;

            var size = transitionRoot.rect.size;
            var scale = new Vector3(1f / size.x, 1f / size.y, 1f);
            if (transitionKeepAspectRatio && !Mathf.Approximately(scale.x, scale.y))
            {
                scale.x = scale.y = Mathf.Min(scale.x, scale.y);
            }

            if (0 != (flip & Flip.Effect))
            {
                scale.x = 0 != (flip & Flip.Horizontal) ? -scale.x : scale.x;
                scale.y = 0 != (flip & Flip.Vertical) ? -scale.y : scale.y;
            }

            var pivot = new Vector2(0.5f, 0.5f);
            var offset = (transitionRoot.pivot - pivot) * size;
            var w2LMat = Matrix4x4.Translate(offset) * transitionRoot.worldToLocalMatrix;

            var rootRotation = Quaternion.Euler(0, 0, transitionRotation);
            var rootScale = 1f / GetMultiplier(transitionRotation);
            material.SetMatrix(s_RootViewMatrix, Matrix4x4.TRS(pivot, rootRotation, scale * rootScale) * w2LMat);

            var gradRotation = Quaternion.Euler(0, 0, GetGradationRotation());
            var gradScale = 1f / GetMultiplier(GetGradationRotation());
            material.SetMatrix(s_GradViewMatrix, Matrix4x4.TRS(pivot, gradRotation, scale * gradScale) * w2LMat);

            if (canvas.renderMode == RenderMode.ScreenSpaceOverlay || !canvas.worldCamera)
            {
                material.SetMatrix(s_CanvasToWorldMatrix, canvas.transform.localToWorldMatrix);
            }
            else
            {
                material.SetMatrix(s_CanvasToWorldMatrix, Matrix4x4.identity);
            }
        }

        private Vector4 GetGradationScaleAndOffset()
        {
            var gScale = 1 / gradationScale;
            switch (gradationMode)
            {
                case GradationMode.HorizontalGradient:
                case GradationMode.VerticalGradient:
                case GradationMode.AngleGradient:
                    return new Vector4(gScale, 1, -0.5f * (gScale + 1) - gradationOffset, 0);
                case GradationMode.RadialFast:
                case GradationMode.RadialDetail:
                {
                    return new Vector4(gScale, 1, gradationOffset, 0);
                }
                default:
                    return new Vector4(gScale, 1, gradationOffset * (gScale + 1) / 2 - gScale * 0.5f + 0.5f, 0);
            }
        }

        private float GetGradationRotation()
        {
            switch (gradationMode)
            {
                case GradationMode.DiagonalToLeftBottom:
                    return 135;
                case GradationMode.DiagonalToRightBottom:
                    return 45;
                case GradationMode.Vertical:
                    return 90;
                case GradationMode.VerticalGradient:
                    return -90;
                case GradationMode.Angle:
                case GradationMode.AngleGradient:
                    return gradationRotation;
                default:
                    return 0;
            }
        }

        private static float GetMultiplier(float deg)
        {
            var rad = Mathf.Deg2Rad * deg;
            var sin = Mathf.Sin(rad);
            var cos = Mathf.Cos(rad);
            return Mathf.Max(Mathf.Abs(cos - sin), Mathf.Abs(cos + sin));
        }

        private static void SetKeyword(Material material, string[] keywords, int index)
        {
            for (var i = 0; i < keywords.Length; i++)
            {
                if (i != index)
                {
                    material.DisableKeyword(keywords[i]);
                }
                else if (!string.IsNullOrEmpty(keywords[i]))
                {
                    material.EnableKeyword(keywords[i]);
                }
            }
        }

        public void ModifyMesh(Graphic graphic, RectTransform transitionRoot, VertexHelper vh,
            bool canModifyShape, Flip flip)
        {
            // Apply flip (without effect).
            ApplyFlipWithoutEffect(vh, flip);

            var count = vh.currentIndexCount;
            if (!willModifyVertex || count == 0) return;

            var effectProxy = GraphicProxy.Find(graphic);
            var isText = effectProxy.IsText(graphic);
            effectProxy.OnPreModifyMesh(graphic);

            var verts = s_WorkingVertices;
            var expandSize = effectProxy.ModifyExpandSize(graphic, GetExpandSize(canModifyShape));

            // Get the rectangle to calculate the normalized position.
            vh.GetUIVertexStream(verts);
            var bundleSize = isText ? 6 : count;

            // Modify vertices for each bundled-quad.
            for (var i = 0; i < count; i += bundleSize)
            {
                // min/max for bundled-quad
                UIVertexUtil.GetBounds(verts, i, bundleSize, out var bounds, out var uvMask);
                UIVertexUtil.Expand(verts, i, bundleSize, expandSize, bounds);
                for (var j = 0; j < bundleSize; j++)
                {
                    var vt = verts[i + j];
                    if (onModifyVertex != null)
                    {
                        vt = onModifyVertex(vt, uvMask);
                    }
                    else
                    {
                        vt.uv1 = new Vector4(uvMask.xMin, uvMask.yMin, uvMask.xMax, uvMask.yMax);
                    }

                    verts[i + j] = vt;
                }
            }

            // Apply flip (with effect).
            ApplyFlipWithEffect(verts, flip);

            // Apply shadow.
            ApplyShadow(verts, transitionRoot, flip);

            vh.Clear();
            vh.AddUIVertexTriangleStream(verts);
        }

        private void ApplyShadow(List<UIVertex> verts, RectTransform transitionRoot, Flip flip)
        {
            var distance = shadowDistance;
            if (distance == Vector2.zero) return;

            if ((flip & Flip.Shadow) != 0)
            {
                distance.x = (flip & Flip.Horizontal) != 0 ? -distance.x : distance.x;
                distance.y = (flip & Flip.Vertical) != 0 ? -distance.y : distance.y;
            }

            switch (shadowMode)
            {
                case ShadowMode.Shadow:
                case ShadowMode.Shadow3:
                case ShadowMode.Outline:
                case ShadowMode.Outline8:
                    ShadowUtil.DoShadow(verts, s_ShadowVectors[(int)shadowMode], distance, shadowIteration,
                        shadowFade);
                    break;
                case ShadowMode.Mirror:
                    ShadowUtil.DoMirror(verts, distance, shadowMirrorScale, shadowFade, transitionRoot);
                    break;
            }
        }

        private static void ApplyFlipWithoutEffect(VertexHelper vh, Flip flip)
        {
            if ((flip & Flip.Effect) != 0) return;

            var flipHorizontal = 0 != (flip & Flip.Horizontal);
            var flipVertical = 0 != (flip & Flip.Vertical);
            if (!flipHorizontal && !flipVertical) return;

            UIVertexUtil.Flip(vh, flipHorizontal, flipVertical);
        }

        private static void ApplyFlipWithEffect(List<UIVertex> verts, Flip flip)
        {
            if ((flip & Flip.Effect) == 0) return;

            var flipHorizontal = 0 != (flip & Flip.Horizontal);
            var flipVertical = 0 != (flip & Flip.Vertical);
            if (!flipHorizontal && !flipVertical) return;

            UIVertexUtil.Flip(verts, flipHorizontal, flipVertical);
        }

        private Vector4 GetExpandSize(bool canModifyShape)
        {
            if (!canModifyShape) return Vector4.zero;

            var expandSize = Vector4.zero;
            switch (samplingFilter)
            {
                case SamplingFilter.BlurFast:
                    expandSize += Vector4.one * 10;
                    break;
                case SamplingFilter.BlurMedium:
                    expandSize += Vector4.one * 15;
                    break;
                case SamplingFilter.BlurDetail:
                    expandSize += Vector4.one * 20;
                    break;
                case SamplingFilter.RgbShift:
                    expandSize.x += 40;
                    expandSize.z += 40;
                    break;
            }

            switch (transitionFilter)
            {
                case TransitionFilter.Melt:
                    expandSize.y += 40;
                    break;
                case TransitionFilter.Burn:
                    expandSize.w += 40;
                    break;
            }

            return expandSize;
        }
    }
}
