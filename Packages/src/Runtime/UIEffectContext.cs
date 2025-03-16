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
        private static readonly int s_ShadowColor = Shader.PropertyToID("_ShadowColor");
        private static readonly int s_ShadowBlurIntensity = Shader.PropertyToID("_ShadowBlurIntensity");
        private static readonly int s_ShadowColorGlow = Shader.PropertyToID("_ShadowColorGlow");
        private static readonly int s_EdgeWidth = Shader.PropertyToID("_EdgeWidth");
        private static readonly int s_EdgeColor = Shader.PropertyToID("_EdgeColor");
        private static readonly int s_EdgeColorGlow = Shader.PropertyToID("_EdgeColorGlow");
        private static readonly int s_EdgeShinyAutoPlaySpeed = Shader.PropertyToID("_EdgeShinyAutoPlaySpeed");
        private static readonly int s_EdgeShinyRate = Shader.PropertyToID("_EdgeShinyRate");
        private static readonly int s_EdgeShinyWidth = Shader.PropertyToID("_EdgeShinyWidth");
        private static readonly int s_PatternArea = Shader.PropertyToID("_PatternArea");

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
            "COLOR_MULTIPLY",
            "COLOR_ADDITIVE",
            "COLOR_SUBTRACTIVE",
            "COLOR_REPLACE",
            "COLOR_MULTIPLY_LUMINANCE",
            "COLOR_MULTIPLY_ADDITIVE",
            "COLOR_HSV_MODIFIER",
            "COLOR_CONTRAST"
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

        private static readonly string[] s_TransitionColorKeywords =
        {
            "",
            "TRANSITION_COLOR_MULTIPLY",
            "TRANSITION_COLOR_ADDITIVE",
            "TRANSITION_COLOR_SUBTRACTIVE",
            "TRANSITION_COLOR_REPLACE",
            "TRANSITION_COLOR_MULTIPLY_LUMINANCE",
            "TRANSITION_COLOR_MULTIPLY_ADDITIVE",
            "TRANSITION_COLOR_HSV_MODIFIER",
            "TRANSITION_COLOR_CONTRAST"
        };

        private static readonly string[] s_TargetKeywords =
        {
            "",
            "TARGET_HUE",
            "TARGET_LUMINANCE"
        };

        private static readonly string[] s_ShadowColorKeywords =
        {
            "",
            "SHADOW_COLOR_MULTIPLY",
            "SHADOW_COLOR_ADDITIVE",
            "SHADOW_COLOR_SUBTRACTIVE",
            "SHADOW_COLOR_REPLACE",
            "SHADOW_COLOR_MULTIPLY_LUMINANCE",
            "SHADOW_COLOR_MULTIPLY_ADDITIVE",
            "SHADOW_COLOR_HSV_MODIFIER",
            "SHADOW_COLOR_CONTRAST"
        };

        private static readonly string[] s_EdgeKeywords =
        {
            "",
            "EDGE_PLAIN",
            "EDGE_SHINY"
        };

        private static readonly string[] s_EdgeColorKeywords =
        {
            "",
            "EDGE_COLOR_MULTIPLY",
            "EDGE_COLOR_ADDITIVE",
            "EDGE_COLOR_SUBTRACTIVE",
            "EDGE_COLOR_REPLACE",
            "EDGE_COLOR_MULTIPLY_LUMINANCE",
            "EDGE_COLOR_MULTIPLY_ADDITIVE",
            "EDGE_COLOR_HSV_MODIFIER",
            "EDGE_COLOR_CONTRAST"
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

        public static Func<UIVertex, Rect, float, float, UIVertex> onModifyVertex;
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
        public Color gradationColor1;
        public Color gradationColor2;
        public Color gradationColor3;
        public Color gradationColor4;
        public Gradient gradationGradient;
        public float gradationOffset;
        public float gradationScale;
        public float gradationRotation;
        private List<float> _keyTimes;

        public bool willModifyMaterial => toneFilter != ToneFilter.None
                                          || colorFilter != ColorFilter.None
                                          || samplingFilter != SamplingFilter.None
                                          || transitionFilter != TransitionFilter.None
                                          || srcBlendMode != BlendMode.One
                                          || dstBlendMode != BlendMode.OneMinusSrcAlpha
                                          || shadowMode != ShadowMode.None
                                          || edgeMode != EdgeMode.None;

        public bool willModifyVertex => willModifyMaterial
                                        || gradationMode != GradationMode.None;

        public void Reset()
        {
            InternalListPool<float>.Return(ref _keyTimes);
            CopyFrom(s_DefaultContext);
        }

        public void CopyFrom(UIEffectContext preset)
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
            gradationColor1 = preset.gradationColor1;
            gradationColor2 = preset.gradationColor2;
            gradationColor3 = preset.gradationColor3;
            gradationColor4 = preset.gradationColor4;
            gradationGradient = preset.gradationGradient;
            gradationOffset = preset.gradationOffset;
            gradationScale = preset.gradationScale;
            gradationRotation = preset.gradationRotation;
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

            material.SetColor(s_ColorValue, color);
            material.SetFloat(s_ColorIntensity, Mathf.Clamp01(colorIntensity));
            material.SetInt(s_ColorGlow, colorGlow ? 1 : 0);

            material.SetFloat(s_SamplingIntensity, Mathf.Clamp01(samplingIntensity));
            material.SetFloat(s_SamplingWidth, samplingWidth);
            material.SetFloat(s_SamplingScale, actualSamplingScale);

            material.SetFloat(s_TransitionRate, Mathf.Clamp01(transitionRate));
            material.SetInt(s_TransitionReverse, transitionReverse ? 1 : 0);
            material.SetTexture(s_TransitionTex, transitionTex);
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

            material.SetColor(s_ShadowColor, shadowColor);
            material.SetInt(s_ShadowColorGlow, shadowColorGlow ? 1 : 0);

            material.SetFloat(s_EdgeWidth, Mathf.Clamp01(edgeWidth));
            material.SetColor(s_EdgeColor, edgeColor);
            material.SetInt(s_EdgeColorGlow, edgeColorGlow ? 1 : 0);
            material.SetFloat(s_EdgeShinyRate, Mathf.Clamp01(edgeShinyRate));
            material.SetFloat(s_EdgeShinyWidth, Mathf.Clamp01(edgeShinyWidth));
            material.SetFloat(s_EdgeShinyAutoPlaySpeed, edgeShinyAutoPlaySpeed);
            material.SetInt(s_PatternArea, edgeMode != EdgeMode.None ? (int)patternArea : 0);

            SetKeyword(material, s_ToneKeywords, (int)toneFilter);
            SetKeyword(material, s_ColorKeywords, (int)colorFilter);
            SetKeyword(material, s_SamplingKeywords, (int)samplingFilter);
            SetKeyword(material, s_TransitionKeywords, (int)transitionFilter);
            switch (transitionFilter)
            {
                case TransitionFilter.None:
                case TransitionFilter.Fade:
                case TransitionFilter.Cutoff:
                    SetKeyword(material, s_TransitionColorKeywords, (int)ColorFilter.None);
                    break;
                default:
                    SetKeyword(material, s_TransitionColorKeywords, (int)transitionColorFilter);
                    break;
            }

            switch (shadowMode)
            {
                case ShadowMode.None:
                    SetKeyword(material, s_ShadowColorKeywords, (int)ColorFilter.None);
                    break;
                default:
                    SetKeyword(material, s_ShadowColorKeywords, (int)shadowColorFilter);
                    break;
            }

            SetKeyword(material, s_EdgeKeywords, (int)edgeMode);

            switch (edgeMode)
            {
                case EdgeMode.None:
                    SetKeyword(material, s_EdgeColorKeywords, (int)ColorFilter.None);
                    break;
                default:
                    SetKeyword(material, s_EdgeColorKeywords, (int)edgeColorFilter);
                    break;
            }


            SetKeyword(material, s_TargetKeywords, (int)targetMode);

            Profiler.EndSample();
        }

        public void SetEnablePreview(bool enable, Material material)
        {
            if (!material) return;
            material.SetVector(s_TransitionTex_Speed, enable ? (Vector4)transitionTexSpeed : Vector4.zero);
            material.SetFloat(s_TransitionAutoPlaySpeed, enable ? transitionAutoPlaySpeed : 0);
            material.SetFloat(s_EdgeShinyAutoPlaySpeed, enable ? edgeShinyAutoPlaySpeed : 0);
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
            var rectMatrix = Matrix4x4.identity;
            var rot = Matrix4x4.Rotate(Quaternion.Euler(0, 0, transitionRotation));
            var v1 = rot.MultiplyPoint3x4(new Vector3(1, 1, 0));
            var multiplier = Mathf.Max(Mathf.Abs(v1.x), Mathf.Abs(v1.y));

            Rect rect;
            if (transitionRoot)
            {
                rect = transitionRoot.rect;
                if (transitionRoot != graphic.transform)
                {
                    rectMatrix = transitionRoot.worldToLocalMatrix
                                 * graphic.transform.localToWorldMatrix;
                }

                rot *= Matrix4x4.Scale(new Vector3(1 / multiplier, 1 / multiplier, 1))
                       * rectMatrix;
            }
            else
            {
                rect = graphic.rectTransform.rect;
                rot *= Matrix4x4.Scale(new Vector3(multiplier, multiplier, 1));
            }

            if (transitionKeepAspectRatio && transitionTex)
            {
                var center = rect.center;
                var aspectRatio = (float)transitionTex.width / transitionTex.height;
                if (rect.width < rect.height)
                {
                    rect.width = rect.height * aspectRatio;
                }
                else
                {
                    rect.height = rect.width / aspectRatio;
                }

                rect.center = center;
            }

            // Modify vertices for each bundled-quad.
            for (var i = 0; i < count; i += bundleSize)
            {
                // min/max for bundled-quad
                UIVertexUtil.GetBounds(verts, i, bundleSize, out var bounds, out var uvMask);
                UIVertexUtil.Expand(verts, i, bundleSize, expandSize, bounds);
                for (var j = 0; j < bundleSize; j++)
                {
                    var vt = verts[i + j];
                    ModifyVertex(ref vt, vt.position, vt.uv0, uvMask, rect, rot);
                    verts[i + j] = vt;
                }
            }

            // Apply gradation.
            ApplyGradation(verts, transitionRoot.rect, rectMatrix, canModifyShape);

            // Apply flip (with effect).
            ApplyFlipWithEffect(verts, flip);

            // Apply shadow.
            ApplyShadow(verts, transitionRoot, flip);

            vh.Clear();
            vh.AddUIVertexTriangleStream(verts);
        }

        private void ApplyGradation(List<UIVertex> verts, Rect rect, Matrix4x4 m, bool canModifyShape)
        {
            if (gradationMode == GradationMode.None) return;

            var a = gradationColor1;
            var b = gradationColor2;
            var offset = gradationOffset;
            var scale = gradationScale;
            var grad = gradationGradient;
            var rot = gradationRotation;
            switch (gradationMode)
            {
                case GradationMode.Horizontal:
                    GradientUtil.DoHorizontalGradient(verts, a, b, offset, scale, rect, m);
                    break;
                case GradationMode.Vertical:
                    GradientUtil.DoVerticalGradient(verts, a, b, offset, scale, rect, m);
                    break;
                case GradationMode.Diagonal:
                    GradientUtil.DoDiagonal(verts, a, b, gradationColor3, gradationColor4, offset, scale, rect, m);
                    break;
                case GradationMode.DiagonalToRightBottom:
                    GradientUtil.DoDiagonalGradientToRightBottom(verts, a, b, offset, scale, rect, m);
                    break;
                case GradationMode.DiagonalToLeftBottom:
                    GradientUtil.DoDiagonalGradientToLeftBottom(verts, a, b, offset, scale, rect, m);
                    break;
                case GradationMode.RadialFast:
                    GradientUtil.DoRadialGradient(verts, a, b, offset, scale, rect, m, 4);
                    break;
                case GradationMode.RadialDetail:
                    GradientUtil.DoRadialGradient(verts, a, b, offset, scale, rect, m, 12);
                    break;
                case GradationMode.Angle:
                    rect = GradientUtil.RotateRectAsNormalized(rect, rot);
                    m = Matrix4x4.Rotate(Quaternion.Euler(0, 0, rot)) * m;
                    GradientUtil.DoHorizontalGradient(verts, a, b, offset, scale, rect, m);
                    break;
                case GradationMode.HorizontalGradient:
                {
                    if (_keyTimes == null)
                    {
                        _keyTimes = InternalListPool<float>.Rent();
                        GradientUtil.GetKeyTimes(grad, _keyTimes);
                    }

                    if (canModifyShape)
                    {
                        var splitTimes = InternalListPool<float>.Rent();
                        GradientUtil.SplitKeyTimes(_keyTimes, splitTimes, offset, scale);
                        GradientUtil.DoHorizontalGradient(verts, grad, splitTimes, offset, scale, rect, m);
                        InternalListPool<float>.Return(ref splitTimes);
                    }
                    else
                    {
                        GradientUtil.DoHorizontalGradient(verts, grad, offset, scale, rect, m);
                    }

                    break;
                }
                case GradationMode.VerticalGradient:
                {
                    if (_keyTimes == null)
                    {
                        _keyTimes = InternalListPool<float>.Rent();
                        GradientUtil.GetKeyTimes(grad, _keyTimes);
                    }

                    if (canModifyShape)
                    {
                        var splitTimes = InternalListPool<float>.Rent();
                        GradientUtil.SplitKeyTimes(_keyTimes, splitTimes, offset, scale);
                        GradientUtil.DoVerticalGradient(verts, grad, splitTimes, offset, scale, rect, m);
                        InternalListPool<float>.Return(ref splitTimes);
                    }
                    else
                    {
                        GradientUtil.DoVerticalGradient(verts, grad, offset, scale, rect, m);
                    }

                    break;
                }
                case GradationMode.AngleGradient:
                {
                    if (_keyTimes == null)
                    {
                        _keyTimes = InternalListPool<float>.Rent();
                        GradientUtil.GetKeyTimes(grad, _keyTimes);
                    }

                    rect = GradientUtil.RotateRectAsNormalized(rect, rot);
                    m = Matrix4x4.Rotate(Quaternion.Euler(0, 0, rot)) * m;
                    if (canModifyShape)
                    {
                        var splitTimes = InternalListPool<float>.Rent();
                        GradientUtil.SplitKeyTimes(_keyTimes, splitTimes, offset, scale);
                        GradientUtil.DoAngleGradient(verts, grad, splitTimes, offset, scale, rect, m);
                        InternalListPool<float>.Return(ref splitTimes);
                    }
                    else
                    {
                        GradientUtil.DoHorizontalGradient(verts, grad, offset, scale, rect, m);
                    }

                    break;
                }
            }
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

        private Vector2 GetExpandSize(bool canModifyShape)
        {
            if (!canModifyShape) return Vector2.zero;

            var expandSize = Vector2.zero;
            switch (samplingFilter)
            {
                case SamplingFilter.BlurFast:
                    expandSize.x += 10;
                    expandSize.y += 10;
                    break;
                case SamplingFilter.BlurMedium:
                    expandSize.x += 15;
                    expandSize.y += 15;
                    break;
                case SamplingFilter.BlurDetail:
                    expandSize.x += 20;
                    expandSize.y += 20;
                    break;
                case SamplingFilter.RgbShift:
                    expandSize.x += 40;
                    expandSize.y += 40;
                    break;
            }

            switch (transitionFilter)
            {
                case TransitionFilter.Melt:
                case TransitionFilter.Burn:
                    expandSize.y += 40;
                    break;
            }

            return expandSize;
        }

        private static void ModifyVertex(ref UIVertex vt, Vector2 pos, Vector2 uv, Rect uvMask,
            Rect rect, Matrix4x4 m)
        {
            pos = m.MultiplyPoint3x4(pos);
            var normalizedX = Mathf.InverseLerp(rect.xMin, rect.xMax, pos.x);
            var normalizedY = Mathf.InverseLerp(rect.yMin, rect.yMax, pos.y);

            if (onModifyVertex != null)
            {
                vt = onModifyVertex(vt, uvMask, normalizedX, normalizedY);
            }
            else
            {
                vt.uv0.z = normalizedX;
                vt.uv0.w = normalizedY;
                vt.uv1 = new Vector4(uvMask.xMin, uvMask.yMin, uvMask.xMax, uvMask.yMax);
            }
        }
    }
}
