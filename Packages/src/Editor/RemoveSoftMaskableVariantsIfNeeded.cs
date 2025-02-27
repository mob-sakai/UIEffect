using System.Collections.Generic;
using UnityEditor.Build;
using UnityEditor.Rendering;
using UnityEngine;
using UnityEngine.Rendering;
using PackageInfo = UnityEditor.PackageManager.PackageInfo;

namespace Coffee.UIEffects.Editors
{
    internal class RemoveSoftMaskableVariantsIfNeeded : IPreprocessShaders
    {
        public int callbackOrder => 1;

        // Remove the 'SOFTMASKABLE' variants if 'SoftMaskForUGUI' package is not installed.
        public void OnProcessShader(Shader shader, ShaderSnippetData snippet, IList<ShaderCompilerData> data)
        {
            // If the shader is not for UIEffect: skip.
            if (!shader.name.EndsWith(" (UIEffect)")) return;

            // If the 'SoftMaskForUGUI' package is installed: skip.
            if (PackageInfo.FindForAssetPath("Packages/com.coffee.softmask-for-ugui/package.json") != null) return;

            // Remove the 'SOFTMASKABLE' variants.
            var softMaskable = new ShaderKeyword(shader, "SOFTMASKABLE");
            for (var i = data.Count - 1; i >= 0; i--)
            {
                var d = data[i];
                if (d.shaderKeywordSet.IsEnabled(softMaskable))
                {
                    data.RemoveAt(i);
                }
            }
        }
    }
}
