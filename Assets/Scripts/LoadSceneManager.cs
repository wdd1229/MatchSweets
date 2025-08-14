using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadSceneManager : MonoBehaviour
{
    private static LoadSceneManager instance;

    //��ʹ��ʱ�ж��Ƿ�Ϊ�� ���򴴽�Ԥ�ƣ����ڽű�
    public static LoadSceneManager Instance
    {
        get
        {
            if (instance == null)
            {
                GameObject obj = new GameObject(typeof(LoadSceneManager).Name);
                instance = obj.AddComponent<LoadSceneManager>();
                DontDestroyOnLoad(obj); // ��֤�糡������
            }
            return instance;
        }
    }
    /// <summary>
    /// ͬ�����س���
    /// </summary>
    /// <param name="sceneName">����Ϊ����ĳ�������</param>
    public void LoadScene(string sceneName)
    {
        if (SceneManager.GetSceneByName(sceneName) != null)
        {
            SceneManager.LoadScene(sceneName);
        }
        else
        {
            Debug.Log("��ǰ���������ڣ�������Դ�����ƻ���Build Settings");
        }
    }
    /// <summary>
    /// �첽���س���
    /// </summary>
    /// <param name="sceneName">Ҫ���صĳ�������</param>
    /// <param name="onComplete">������ɺ�Ļص�</param>
    public void LoadSceneAsync(string sceneName, System.Action onComplete = null)
    {
        StartCoroutine(LoadSceneAsyncCoroutine(sceneName, onComplete));
    }
    private IEnumerator LoadSceneAsyncCoroutine(string sceneName, System.Action onComplete)
    {
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneName);

        while (!asyncLoad.isDone)
        {
            float progress = Mathf.Clamp01(asyncLoad.progress / 0.9f);
            Debug.Log($"���ؽ���: {progress * 100}%");
            yield return null;
        }

        onComplete?.Invoke();
    }
}
