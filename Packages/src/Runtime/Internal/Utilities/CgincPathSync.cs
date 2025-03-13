#if UNITY_EDITOR
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEngine;

namespace Coffee.UIEffectInternal
{
    public class CgincPathSync : AssetPostprocessor
    {
        private static string[] s_ShaderNames = null;
        private static string s_CgincPattern = null;

        public static void RegisterShaders(string cgincPattern, string[] shaderNames)
        {
            s_CgincPattern = cgincPattern;
            s_ShaderNames = shaderNames;
            UpdateTmpCgincPath();
        }

        private static void OnPostprocessAllAssets(string[] _, string[] __, string[] ___, string[] ____)
        {
            UpdateTmpCgincPath();
        }

        private static void UpdateTmpCgincPath()
        {
            if (s_CgincPattern == null || s_ShaderNames == null || s_ShaderNames.Length == 0) return;

            AssetDatabase.StartAssetEditing();
            string[] cgincs = null;
            foreach (var shaderName in s_ShaderNames)
            {
                var shader = Shader.Find(shaderName);
                if (!shader) continue;

                var hasCgincError = ShaderUtil.GetShaderMessages(shader)
                    .Any(x => Regex.IsMatch(x.message, $"Couldn't open include file.*{s_CgincPattern}"));
                if (!hasCgincError) continue;

                cgincs = cgincs ?? AssetDatabase.FindAssets("t:ShaderInclude")
                    .Select(AssetDatabase.GUIDToAssetPath)
                    .Where(x => Regex.IsMatch(x, s_CgincPattern))
                    .ToArray();

                UpdateTmpCgincPath(shader, cgincs);
            }

            AssetDatabase.StopAssetEditing();
        }

        private static void UpdateTmpCgincPath(Shader shader, string[] cgincs)
        {
            var path = AssetDatabase.GetAssetPath(shader);
            var text = File.ReadAllText(path);
            var hash = text.GetHashCode();

            foreach (var cginc in cgincs)
            {
                var file = Path.GetFileName(cginc);
                text = Regex.Replace(text, $"#include.*\"[^\"]*{file}\"", $"#include \"{cginc}\"",
                    RegexOptions.Multiline);
            }

            if (hash == text.GetHashCode()) return;

            Debug.Log($"Update Shader: '{path}'");
            File.WriteAllText(path, text, Encoding.UTF8);
            AssetDatabase.ImportAsset(path);
        }
    }
}
#endif
