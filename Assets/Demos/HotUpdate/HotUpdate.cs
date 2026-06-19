using System.Collections;
using Coffee.UIEffects;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.SceneManagement;

public class HotUpdate : MonoBehaviour
{
    private IEnumerator Start()
    {
        const string k_SettingsAddress = "Assets/ProjectSettings/UIEffectProjectSettings.asset";
        yield return Addressables.LoadAssetAsync<UIEffectProjectSettings>(k_SettingsAddress);

#if UNITY_2022_2_OR_NEWER
        while (UIEffectProjectSettings.shaderVariantCollection.WarmUpProgressively(5) == false) yield return null;
#else
        UIEffectProjectSettings.shaderVariantCollection.WarmUp();
#endif

        yield return SceneManager.LoadSceneAsync(gameObject.scene.buildIndex + 1, LoadSceneMode.Single);
    }
}
