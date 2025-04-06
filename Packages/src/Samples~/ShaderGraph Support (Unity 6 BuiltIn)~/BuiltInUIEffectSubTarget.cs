using System;
using System.Linq;
using System.Reflection;
using UnityEditor.ShaderGraph;
using UnityEngine;
using BlendMode = UnityEngine.Rendering.BlendMode;

namespace UnityEditor.Rendering.BuiltIn.ShaderGraph
{
    internal class BuiltInUIEffectSubTarget : BuiltInCanvasSubTarget
    {
        // UIEffectForShaderGraph.hlsl
        internal static string s_UIEffectHlsl => AssetDatabase.GUIDToAssetPath("5109b2ad9b8be459592190a5678464e3");

        public BuiltInUIEffectSubTarget() { displayName = "Canvas (UIEffect)"; }

        [MenuItem("Assets/Create/Shader Graph/BuiltIn/Canvas Shader Graph (UIEffect)", priority = 1000)]
        private static void CreateUnlitGraph()
        {
            var target = (BuiltInTarget)Activator.CreateInstance(typeof(BuiltInTarget));
            target.TrySetActiveSubTarget(typeof(BuiltInUIEffectSubTarget));
            target.CreateGraphAsset();
        }

        protected override IncludeCollection postgraphIncludes => base.postgraphIncludes
            .Add(s_UIEffectHlsl, IncludeLocation.Postgraph); // Include UIEffectForShaderGraph.hlsl on postgraph

        /// <summary>
        /// Modify pass descriptor for UIEffect.
        /// </summary>
        public static PassDescriptor ModifyUIPassDescriptor(PassDescriptor descriptor)
        {
            descriptor.keywords
                .AddKeywordForUIEffect("TONE", "s_ToneKeywords") // add tone keywords
                .AddKeywordForUIEffect("COLOR_FILTER") // add color filter keywords
                .AddKeywordForUIEffect("SAMPLING", "s_SamplingKeywords") // add sampling keywords
                .AddKeywordForUIEffect("TRANSITION", "s_TransitionKeywords") // add transition keywords
                .AddKeywordForUIEffect("EDGE", "s_EdgeKeywords") // add edge keywords
                .AddKeywordForUIEffect("DETAIL", "s_DetailKeywords") // add detail keywords
                .AddKeywordForUIEffect("TARGET", "s_TargetKeywords") // add target keywords
                .AddKeywordForUIEffect("GRADATION", "s_GradationKeywords") // add gradation keywords
                .AddKeywordForUIEffect("UIEFFECT_EDITOR", KeywordScope.Global) // add UIEffect editor keywords
                .AddKeywordForUIEffect("SOFTMASK_EDITOR", KeywordScope.Global) // add softmask editor keywords
                .AddKeywordForUIEffect("SOFTMASKABLE"); // add softmaskable keywords

            descriptor.requiredFields
                .Add(StructFields.Varyings.texCoord2); // add texCoord2 for UIEffect (uvMask)

            descriptor.renderStates = descriptor.renderStates
                .RemoveIf(x => x.descriptor.type == RenderStateType.Blend) // remove default blend state
                .Add(RenderState.Blend("Blend [_SrcBlend] [_DstBlend]")); // add custom blend state

            descriptor.pragmas = descriptor.pragmas
                .RemoveIf(x =>
                    x.value.Contains(" vertex ") || x.value.Contains(" fragment ")) // remove default vertex/fragment
                .Add(Pragma.Vertex("vert_override")) // add custom vertex function
                .Add(Pragma.Fragment("frag_override")); // add custom fragment function

            return descriptor;
        }

        /// <summary>
        /// Modify shader properties for UIEffect.
        /// </summary>
        public static void ModifyShaderProperties(PropertyCollector collector)
        {
            collector.AddFloatProperty("_SrcBlend", (int)BlendMode.One);
            collector.AddFloatProperty("_DstBlend", (int)BlendMode.OneMinusSrcAlpha);
        }

        public override PassDescriptor GenerateUIPassDescriptor(bool isSRP)
        {
            return ModifyUIPassDescriptor(base.GenerateUIPassDescriptor(isSRP));
        }

        public override void CollectShaderProperties(PropertyCollector collector, GenerationMode generationMode)
        {
            base.CollectShaderProperties(collector, generationMode);
            ModifyShaderProperties(collector);
        }
    }

    internal static class ShaderGraphExtensions
    {
        public static RenderStateCollection RemoveIf(this RenderStateCollection self,
            Predicate<RenderStateCollection.Item> predicate)
        {
            var result = new RenderStateCollection();
            foreach (var item in self.Where(x => !predicate(x)))
            {
                result.Add(item.descriptor, item.fieldConditions);
            }

            return result;
        }

        public static PragmaCollection RemoveIf(this PragmaCollection self,
            Predicate<PragmaCollection.Item> predicate)
        {
            var result = new PragmaCollection();
            foreach (var item in self.Where(x => !predicate(x)))
            {
                result.Add(item.descriptor, item.fieldConditions);
            }

            return result;
        }

        /// <summary>
        /// Creates a boolean type keyword descriptor.
        /// </summary>
        public static KeywordCollection AddKeywordForUIEffect(this KeywordCollection self,
            string name, KeywordScope scope = KeywordScope.Local)
        {
            return self.Add(new KeywordDescriptor()
            {
                displayName = name,
                referenceName = name,
                type = KeywordType.Boolean,
                definition = KeywordDefinition.ShaderFeature,
                stages = KeywordShaderStage.Fragment,
                scope = scope
            });
        }

        /// <summary>
        /// Creates an enum type keyword descriptor based on the fields of the UIEffectContext class.
        /// </summary>
        public static KeywordCollection AddKeywordForUIEffect(this KeywordCollection self,
            string name, string fieldName, KeywordScope scope = KeywordScope.Local)
        {
            var prefix = name + "_";
            var keywords = Type.GetType("Coffee.UIEffects.UIEffectContext, Coffee.UIEffect")
                .GetField(fieldName, BindingFlags.Static | BindingFlags.NonPublic)
                .GetValue(null) as string[];

            return self.Add(new KeywordDescriptor()
            {
                displayName = name,
                referenceName = name,
                type = KeywordType.Enum,
                definition = KeywordDefinition.ShaderFeature,
                stages = KeywordShaderStage.Fragment,
                scope = scope,
                entries = keywords
                    .Select(x => x.Replace(prefix, ""))
                    .Select(x => new KeywordEntry(x, x))
                    .ToArray()
            });
        }

        public static void CreateGraphAsset(this Target self)
        {
            var name = $"New Shader Graph (UIEffect).{ShaderGraphImporter.Extension}";
            var graphItem = ScriptableObject.CreateInstance<NewGraphAction>();
            graphItem.targets = new[] { self };
            graphItem.blocks = new[]
            {
                BlockFields.SurfaceDescription.BaseColor,
                BlockFields.SurfaceDescription.Emission,
                BlockFields.SurfaceDescription.Alpha,
                BlockFields.SurfaceDescription.AlphaClipThreshold
            };
            ProjectWindowUtil.StartNameEditingIfProjectWindowExists(0, graphItem, name, null, null);
        }
    }
}
