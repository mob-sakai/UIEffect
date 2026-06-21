using System.Collections;
using System.Text;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class HotUpdate : MonoBehaviour
{
    [SerializeField]
    private string m_Address;

    [SerializeField]
    private Text m_Log;

    [SerializeField]
    private bool m_ExecuteOnStart;

    private readonly StringBuilder _sb = new StringBuilder(1024);

    private Coroutine _co;

    private void Start()
    {
        if (m_ExecuteOnStart)
            Execute();
    }

    private void AddLog(string message)
    {
        Debug.Log(message);
        _sb.AppendLine(message);
        if (m_Log)
            m_Log.text = _sb.ToString();
    }

    public void Execute()
    {
        if (string.IsNullOrEmpty(m_Address))
        {
            AddLog($"*** Address is empty. Please set the address of the asset to load.");
            return;
        }

        if (_co != null)
        {
            AddLog($"*** Execution is already in progress. Please wait until it completes.");
            return;
        }
        _co = StartCoroutine(Co_Execute());
    }

    public void LoadNextScene()
    {
        SceneManager.LoadScene(gameObject.scene.buildIndex + 1, LoadSceneMode.Single);
    }

    private IEnumerator Co_Execute()
    {
        AddLog($"########################################");
        AddLog($"Hot update starting...");
        AddLog($"########################################");
        AddLog("");

        AddLog($"Loading '{m_Address}'...");
        var handle = Addressables.LoadAssetAsync<Object>(m_Address);

        while (handle.IsDone == false)
        {
            yield return new WaitForSeconds(0.1f);
            AddLog($"  Progress: {handle.PercentComplete:P}");
        }

        if (handle.Status == AsyncOperationStatus.Succeeded)
        {
            AddLog($"  Loaded '{m_Address}' successfully.");
        }
        else
        {
            AddLog($"  *** Failed to load '{m_Address}'.");
            AddLog($"  *** {handle.OperationException.Message}");
            yield break;
        }

        AddLog("");
        AddLog($"Hot-Update complete !");
        AddLog("");

        AddLog($"  -> Load next scene in: 3");
        yield return new WaitForSeconds(1f);
        AddLog($"  -> Load next scene in: 2");
        yield return new WaitForSeconds(1f);
        AddLog($"  -> Load next scene in: 1");
        yield return new WaitForSeconds(1f);

        LoadNextScene();
    }
}
