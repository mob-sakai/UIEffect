#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using System.Linq;
using UnityEditor;
using UnityEditor.PackageManager.UI;
using PackageInfo = UnityEditor.PackageManager.PackageInfo;

namespace Coffee.UIEffectInternal
{
    internal static class ShaderSampleImporter
    {
        private static (string shaderName, string sampleName, string version)[] s_Samples;
        private static (string guid, string fileName)[] s_DeprecatedShaders;
        private static readonly Dictionary<string, string> s_SampleNames = new Dictionary<string, string>();
        private static readonly Dictionary<string, string> s_ShaderAliases = new Dictionary<string, string>();

        public static void RegisterShaderSamples((string shaderName, string sampleName, string version)[] samples)
        {
            if (IsBatchOrBuilding()) return;

            // Collect sample names.
            s_Samples = samples;
            foreach (var (shaderName, sampleName, _) in samples)
            {
                s_SampleNames[shaderName] = sampleName;
            }
        }

        public static void RegisterShaderAliases((string, string)[] aliases)
        {
            foreach (var (from, to) in aliases)
            {
                s_ShaderAliases[from] = to;
            }
        }

        public static void RegisterDeprecatedShaders((string, string)[] deprecatedShaders)
        {
            if (IsBatchOrBuilding()) return;

            s_DeprecatedShaders = deprecatedShaders;
        }

        /// <summary>
        /// Import the sample containing the requested shader.
        /// If choice 'Import' is selected, the sample is imported.
        /// If choice 'Skip in this session' is selected, the sample is skipped in this session.
        /// </summary>
        public static bool ImportShaderIfSelected(string shaderName)
        {
            if (IsBatchOrBuilding()) return false;

            if (s_ShaderAliases.TryGetValue(shaderName, out var alias))
            {
                shaderName = alias;
            }

            // Find sample name.
            if (s_SampleNames.TryGetValue(shaderName, out var sampleName))
            {
                var message = $"Import the sample '{sampleName}' to use the shader '{shaderName}'.";
                return ImportSampleIfSelected(sampleName, message);
            }

            return false;
        }

        private static bool ImportSampleIfSelected(string sampleName, string message)
        {
            if (IsBatchOrBuilding()) return false;
            if (!s_SampleNames.ContainsValue(sampleName)) return false;

            // Find package info.
            var pInfo = PackageInfo.FindForAssembly(typeof(ShaderSampleImporter).Assembly);
            if (pInfo == null) return false;

            // Find sample. If not found (resolvedPath == null), skip.
            var sample = Sample.FindByPackage(pInfo.name, pInfo.version)
                .FirstOrDefault(x => x.displayName == sampleName);
            if (sample.resolvedPath == null) return false;

            // Import the sample if selected.
            if (!string.IsNullOrEmpty(message))
            {
                var selected = EditorUtility.DisplayDialog($"Import {sampleName}", message, "Import", "Cancel");
                if (!selected) return false;
            }

            // Remove the imported sample name from the list.
            foreach (var shaderName in s_SampleNames.Keys.ToArray())
            {
                if (s_SampleNames[shaderName] != sampleName) continue;
                s_SampleNames.Remove(shaderName);
                DeleteShader(shaderName);
            }

            // Import the sample in the next frame.
            EditorApplication.delayCall += () => sample.Import(Sample.ImportOptions.OverridePreviousImports);
            return true;
        }

        private static string GetVersion(string shaderName)
        {
            if (string.IsNullOrEmpty(shaderName)) return null;

            var shader = Shader.Find(shaderName);
            if (!shader) return null;

            var path = AssetDatabase.GetAssetPath(shader);
            if (string.IsNullOrEmpty(path)) return null;

            return AssetImporter.GetAtPath(path).userData;
        }

        private static void DeleteShader(string shaderName)
        {
            var shader = Shader.Find(shaderName);
            if (shader)
            {
                AssetDatabase.DeleteAsset(AssetDatabase.GetAssetPath(shader));
            }
        }

        private static bool IsBatchOrBuilding()
        {
            return Application.isBatchMode || BuildPipeline.isBuildingPlayer;
        }

        public static void Update()
        {
            if (s_Samples == null || IsBatchOrBuilding()) return;

            // Find package info.
            var pInfo = PackageInfo.FindForAssembly(typeof(ShaderSampleImporter).Assembly);
            if (pInfo == null) return;

            // Find sample. If not found (resolvedPath == null), skip.
            var sample = Sample.FindByPackage(pInfo.name, pInfo.version).FirstOrDefault();
            if (sample.resolvedPath == null) return;

            // Update the sample.
            foreach (var (shaderName, sampleName, version) in s_Samples)
            {
                if (string.IsNullOrEmpty(version)) continue;
                if (!s_SampleNames.ContainsValue(sampleName)) continue;

                // If the shader exist and the version is different, add the sample to the update list.
                var currentVersion = GetVersion(shaderName);
                if (currentVersion != null && currentVersion != version && ImportSampleIfSelected(sampleName, null))
                {
                    Debug.Log($"Updating '{sampleName}' to use the package '{pInfo.displayName} v{pInfo.version}'.");
                }
            }

            // Remove deprecated shaders.
            foreach (var (guid, fileName) in s_DeprecatedShaders)
            {
                var path = AssetDatabase.GUIDToAssetPath(guid);
                if (!string.IsNullOrEmpty(path) && Path.GetFileName(path) == fileName)
                {
                    AssetDatabase.DeleteAsset(path);
                }
            }

            s_Samples = null;
        }
    }
}
#endif
