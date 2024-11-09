using UnityEngine.Rendering;
using static Coffee.UIEffects.SamplingFilter;
using static Coffee.UIEffects.ToneFilter;
using static Coffee.UIEffects.TransitionFilter;

namespace Coffee.UIEffects
{
    internal static class EnumConverter
    {
        public static UITransitionEffect.EffectMode Convert(this TransitionFilter mode)
        {
            switch (mode)
            {
                case Fade: return UITransitionEffect.EffectMode.Fade;
                case Cutoff: return UITransitionEffect.EffectMode.Cutoff;
                case Dissolve: return UITransitionEffect.EffectMode.Dissolve;
                default:
                    return UITransitionEffect.EffectMode.None;
            }
        }

        public static TransitionFilter Convert(this UITransitionEffect.EffectMode mode)
        {
            switch (mode)
            {
                case UITransitionEffect.EffectMode.Fade: return Fade;
                case UITransitionEffect.EffectMode.Cutoff: return Cutoff;
                case UITransitionEffect.EffectMode.Dissolve: return Dissolve;
                default: return TransitionFilter.None;
            }
        }

        public static ColorFilter Convert(this ColorMode mode)
        {
            switch (mode)
            {
                case ColorMode.Add: return ColorFilter.Additive;
                case ColorMode.Subtract: return ColorFilter.Subtractive;
                case ColorMode.Fill: return ColorFilter.Replace;
                default: return ColorFilter.None;
            }
        }

        public static ColorMode Convert(this ColorFilter filter)
        {
            switch (filter)
            {
                case ColorFilter.Additive: return ColorMode.Add;
                case ColorFilter.Subtractive: return ColorMode.Subtract;
                case ColorFilter.Replace: return ColorMode.Fill;
                default: return ColorMode.Multiply;
            }
        }

        public static (ToneFilter, SamplingFilter) Convert(this EffectMode mode)
        {
            switch (mode)
            {
                case EffectMode.Grayscale: return (Grayscale, SamplingFilter.None);
                case EffectMode.Sepia: return (Sepia, SamplingFilter.None);
                case EffectMode.Nega: return (Negative, SamplingFilter.None);
                case EffectMode.Pixel: return (ToneFilter.None, Pixelation);
                default: return (ToneFilter.None, SamplingFilter.None);
            }
        }

        public static (ToneFilter, SamplingFilter) Convert(this (EffectMode effectMode, BlurMode blurMode) mode)
        {
            switch (mode.effectMode)
            {
                case EffectMode.Grayscale: return (Grayscale, mode.blurMode.Convert());
                case EffectMode.Sepia: return (Sepia, mode.blurMode.Convert());
                case EffectMode.Nega: return (Negative, mode.blurMode.Convert());
                case EffectMode.Pixel: return (ToneFilter.None, Pixelation);
                default: return (ToneFilter.None, mode.blurMode.Convert());
            }
        }

        public static EffectMode Convert(this (ToneFilter, SamplingFilter) filter)
        {
            if (filter.Item2 == Pixelation)
            {
                return EffectMode.Pixel;
            }

            switch (filter.Item1)
            {
                case Grayscale: return EffectMode.Grayscale;
                case Sepia: return EffectMode.Sepia;
                case Negative: return EffectMode.Nega;
                default: return EffectMode.None;
            }
        }

        public static BlurMode Convert(this SamplingFilter filter)
        {
            switch (filter)
            {
                case BlurFast: return BlurMode.FastBlur;
                case BlurDetail: return BlurMode.DetailBlur;
                default: return BlurMode.None;
            }
        }

        public static SamplingFilter Convert(this BlurMode mode)
        {
            switch (mode)
            {
                case BlurMode.FastBlur: return BlurFast;
                case BlurMode.MediumBlur: return BlurDetail;
                case BlurMode.DetailBlur: return BlurDetail;
                default: return SamplingFilter.None;
            }
        }

        public static ShadowMode Convert(this ShadowStyle style)
        {
            switch (style)
            {
                case ShadowStyle.Shadow: return ShadowMode.Shadow;
                case ShadowStyle.Outline: return ShadowMode.Outline;
                case ShadowStyle.Outline8: return ShadowMode.Outline8;
                case ShadowStyle.Shadow3: return ShadowMode.Shadow3;
                default: return ShadowMode.None;
            }
        }


        // public static (BlendMode, BlendMode) Convert(this (BlendType type, BlendMode src, BlendMode dst) self)
        // {
        //     return self.type switch
        //     {
        //         BlendType.AlphaBlend => (BlendMode.One, BlendMode.OneMinusSrcAlpha),
        //         BlendType.Multiply => (BlendMode.DstColor, BlendMode.OneMinusSrcAlpha),
        //         BlendType.Additive => (BlendMode.One, BlendMode.One),
        //         BlendType.SoftAdditive => (BlendMode.OneMinusDstColor, BlendMode.One),
        //         BlendType.MultiplyAdditive => (BlendMode.DstColor, BlendMode.One),
        //         _ => (self.src, self.dst)
        //     };
        // }
        //
        // public static BlendType Convert(this (BlendMode src, BlendMode dst) self)
        // {
        //     return self switch
        //     {
        //         (BlendMode.One, BlendMode.OneMinusSrcAlpha) => BlendType.AlphaBlend,
        //         (BlendMode.DstColor, BlendMode.OneMinusSrcAlpha) => BlendType.Multiply,
        //         (BlendMode.One, BlendMode.One) => BlendType.Additive,
        //         (BlendMode.OneMinusDstColor, BlendMode.One) => BlendType.SoftAdditive,
        //         (BlendMode.DstColor, BlendMode.One) => BlendType.MultiplyAdditive,
        //         _ => BlendType.Custom
        //     };
        // }
    }
}
