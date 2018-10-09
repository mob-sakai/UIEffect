using System.Collections;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEditor;
using UnityEngine;

namespace Coffee.UIExtensions.Editors
{
	/// <summary>
	/// Remove deprecated files in old .unitypackage, after compiling.
	/// </summary>
	public class DeprecatedRemover
	{
		/// <summary>
		/// GUIDs of deprecated files.
		/// </summary>
		static readonly List<string> DeprecatedFiles = new List<string>()
		{
			"156b57fee6ef941958e66a129ce387e2",	// UICustomEffect.cs
			"a4961e148a8cd4fe0b84dddc2741894a",	// UICustomEffectEditor.cs
			"7b1ed09bdf5e54042b5cd1fbe69361bf",	// MaterialBundle.cs
		};


		#if UNITY_EDITOR
		[UnityEditor.InitializeOnLoadMethod]
		static void RemoveFiles()
		{
			// The deprecated file path that exists.
			var files = DeprecatedFiles.Select(x => AssetDatabase.GUIDToAssetPath(x))
				.Where(x => File.Exists(x))
				.ToArray();
			
			if (files.Any())
			{
				StringBuilder sb = new StringBuilder();
				sb.AppendFormat("<b><color=orange>[{0}]</color></b> {1} files have been removed.\n", typeof(DeprecatedRemover).Name, files.Length);

				foreach (var path in files)
				{
					AssetDatabase.DeleteAsset(path);
					sb.AppendFormat("  - {0}\n", path);
				}

				AssetDatabase.Refresh();
				UnityEngine.Debug.Log(sb);
			}
		}
		#endif
	}
}