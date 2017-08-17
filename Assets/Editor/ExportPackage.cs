using UnityEditor;

namespace UnityEditor.UI
{
	public static class ExportPackage
	{
		const string kPackageName = "UIEffect.unitypackage";
		static readonly string[] kAssetPathes = {
			"Assets/UIEffect",
		};

		[MenuItem ("Export Package/" + kPackageName)]
		[InitializeOnLoadMethod]
		static void Export ()
		{
			if (EditorApplication.isPlayingOrWillChangePlaymode)
				return;
			
			AssetDatabase.ExportPackage (kAssetPathes, kPackageName, ExportPackageOptions.Recurse | ExportPackageOptions.Default);
			UnityEngine.Debug.Log ("Export successfully : " + kPackageName);
		}
	}
}