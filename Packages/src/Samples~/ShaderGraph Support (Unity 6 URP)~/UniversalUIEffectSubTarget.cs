using System;
using UnityEditor.Rendering.BuiltIn.ShaderGraph;
using UnityEditor.ShaderGraph;

namespace UnityEditor.Rendering.Universal.ShaderGraph
{
    internal class UniversalUIEffectSubTarget : UniversalCanvasSubTarget
    {
        // UIEffectForShaderGraph.hlsl
        private static GUID s_UIEffectCgincGuid => new GUID("2dd0ed35b98b345218d9aa8ac5a3ad6b");
        private static GUID s_UIEffectHlslGuid => new GUID("5109b2ad9b8be459592190a5678464e3");
        private static string s_UIEffectHlsl => AssetDatabase.GUIDToAssetPath(s_UIEffectHlslGuid.ToString());

        [MenuItem("Assets/Create/Shader Graph/URP/Canvas Shader Graph (UIEffect)", priority = 1000)]
        private static void CreateUnlitGraph()
        {
            var target = (UniversalTarget)Activator.CreateInstance(typeof(UniversalTarget));
            target.TrySetActiveSubTarget(typeof(BuiltInUIEffectSubTarget));
            target.TrySetActiveSubTarget(typeof(UniversalUIEffectSubTarget));
            target.CreateGraphAsset();
        }

        public UniversalUIEffectSubTarget() { displayName = "Canvas (UIEffect)"; }

        protected override IncludeCollection postgraphIncludes => base.postgraphIncludes
            .Add(s_UIEffectHlsl, IncludeLocation.Postgraph);

        public override void Setup(ref TargetSetupContext context)
        {
            base.Setup(ref context);
            context.AddAssetDependency(s_UIEffectHlslGuid, AssetCollection.Flags.SourceDependency);
            context.AddAssetDependency(s_UIEffectCgincGuid, AssetCollection.Flags.SourceDependency);
        }

        public override PassDescriptor GenerateUIPassDescriptor(bool isSRP)
        {
            return BuiltInUIEffectSubTarget.ModifyUIPassDescriptor(base.GenerateUIPassDescriptor(isSRP));
        }

        public override void CollectShaderProperties(PropertyCollector collector, GenerationMode generationMode)
        {
            base.CollectShaderProperties(collector, generationMode);
            BuiltInUIEffectSubTarget.ModifyShaderProperties(collector);
        }
    }
}
