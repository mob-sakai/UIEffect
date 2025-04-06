using System;
using UnityEditor.Rendering.BuiltIn.ShaderGraph;
using UnityEditor.ShaderGraph;

namespace UnityEditor.Rendering.Universal.ShaderGraph
{
    internal class UniversalUIEffectSubTarget : UniversalCanvasSubTarget
    {
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
            .Add(BuiltInUIEffectSubTarget.s_UIEffectHlsl, IncludeLocation.Postgraph);

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
