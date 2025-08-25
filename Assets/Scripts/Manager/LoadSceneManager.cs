using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadSceneManager : MonoBehaviour
{
    private static LoadSceneManager instance;

    //在使用时判断是否为空 否则创建预制，挂在脚本
    public static LoadSceneManager Instance
    {
        get
        {
            if (instance == null)
            {
                GameObject obj = new GameObject(typeof(LoadSceneManager).Name);
                instance = obj.AddComponent<LoadSceneManager>();
                DontDestroyOnLoad(obj); // 保证跨场景存在
            }
            return instance;
        }
    }
    /// <summary>
    /// 同步加载场景
    /// </summary>
    /// <param name="sceneName">参数为传入的场景名称</param>
    public void LoadScene(string sceneName)
    {
        if (SceneManager.GetSceneByName(sceneName) != null)
        {
            SceneManager.LoadScene(sceneName);
        }
        else
        {
            Debug.Log("当前场景不存在，请检查资源及名称或检查Build Settings");
        }
    }
    /// <summary>
    /// 异步加载场景
    /// </summary>
    /// <param name="sceneName">要加载的场景名称</param>
    /// <param name="onComplete">加载完成后的回调</param>
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
            Debug.Log($"加载进度: {progress * 100}%");
            yield return null;
        }

        onComplete?.Invoke();
    }
}
