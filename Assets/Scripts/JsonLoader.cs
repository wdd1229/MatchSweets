using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.Networking;

public class JsonLoader : Singleton<JsonLoader>
{
    // 泛型加载方法
    public IEnumerator LoadJsonData<T>(string relativePath, Action<T> onComplete,
                                       Action<string> onError = null) where T : class
    {
        string fullPath = "";
        string jsonString = null;

#if UNITY_BYTEDANCE_MINIGAME 
        // 抖音小游戏平台处理
        //fullPath = FormatPath(Application.streamingAssetsPath, relativePath);
        //    Debug.Log($"抖音路径: {fullPath}");
            
        //    // 使用抖音特有API调用方式[^1]
        //    yield return StartCoroutine(LoadForByteDance(fullPath, 
        //        result => jsonString = result,
        //        error => HandleError($"抖音加载失败: {error}", onError)));


        fullPath = FormatPath(Application.streamingAssetsPath, relativePath);
        yield return StartCoroutine(LoadViaUnityWebRequest(fullPath,
            result => jsonString = result,
            error => HandleError($"加载失败: {error}", onError)));
#else
        // 其他平台通用处理
        fullPath = FormatPath(Application.streamingAssetsPath, relativePath);
//#if UNITY_WEBGL
//        if (!fullPath.StartsWith("http"))
//        {
//            fullPath = "file://" + fullPath;
//        }
//#endif

        yield return StartCoroutine(LoadViaUnityWebRequest(fullPath,
            result => jsonString = result,
            error => HandleError($"加载失败: {error}", onError)));
#endif

        // 解析JSON数据
        if (!string.IsNullOrEmpty(jsonString))
        {
            ParseJsonData<T>(jsonString, onComplete, onError);
        }
    }

    // 路径格式化方法（复用）
    private string FormatPath(string basePath, string relativePath)
    {
        return Path.Combine(basePath, relativePath).Replace('\\', '/');
    }

    // 通用加载方法（支持所有平台）
    private IEnumerator LoadViaUnityWebRequest(string path,
        Action<string> onSuccess, Action<string> onError)
    {
        using (UnityWebRequest request = UnityWebRequest.Get(path))
        {
            request.downloadHandler = new DownloadHandlerBuffer();
            yield return request.SendWebRequest();

            if (request.result != UnityWebRequest.Result.Success)
            {
                onError?.Invoke($"{request.error} (路径: {path})");
                yield break;
            }
            onSuccess?.Invoke(request.downloadHandler.text);
        }
    }

    // 抖音小游戏平台专用加载方法（保持不变）
    private IEnumerator LoadForByteDance(string filePath, Action<string> onSuccess, Action<string> onError)
    {
#if UNITY_BYTEDANCE_MINIGAME
            using (UnityWebRequest request = UnityWebRequest.Get(filePath))
            {
                request.downloadHandler = new DownloadHandlerBuffer();
                yield return request.SendWebRequest();
                
                if (request.result != UnityWebRequest.Result.Success)
                {
                    onError?.Invoke($"{request.error} (字节跳动平台)");
                    yield break;
                }
                onSuccess?.Invoke(request.downloadHandler.text);
            }
#else
        onError?.Invoke("非字节跳动平台");
        yield break;
#endif
    }

    // 泛型JSON解析方法
    private void ParseJsonData<T>(string jsonString, Action<T> onComplete,
                                 Action<string> onError) where T : class
    {
        try
        {
            T data = JsonUtility.FromJson<T>(jsonString);
            if (data == null) throw new Exception("反序列化返回null");
            onComplete?.Invoke(data);
        }
        catch (Exception ex)
        {
            HandleError($"JSON解析失败: {ex.Message}", onError);
        }
    }

    // 错误处理方法（保持不变）
    private void HandleError(string errorMessage, Action<string> onError)
    {
        Debug.LogError(errorMessage);
        onError?.Invoke(errorMessage);
    }

    // JSON解析通用方法
    private void ParseJsonData(string jsonString, Action<string> onComplete, Action<string> onError)
    {
        if (string.IsNullOrEmpty(jsonString))
        {
            HandleError("JSON数据为空", onError);
            return;
        }

        try
        {
            //LevelList data = JsonConvert.DeserializeObject<LevelList>(jsonString);
            onComplete?.Invoke(jsonString);
        }
        catch (Exception ex)
        {
            HandleError($"JSON解析错误: {ex.Message}", onError);
        }
    }

    
    //LoadJson<AllScoreData>.SaveJsonToFile("ScoreData", allScoreData);

    //public void SaveJsonToFile<T>(string fileName, T data)
    //{
    //    // 构建完整路径
    //    string filePath = Path.Combine(Application.streamingAssetsPath, fileName);

    //    filePath = filePath.Replace(@"\", "/");

    //    // 将对象序列化为JSON字符串
    //    string jsonData = JsonUtility.ToJson(data, true); // 第二个参数为是否格式化（美观打印）

    //    // 写入文件
    //    using (StreamWriter writer = new StreamWriter(filePath))
    //    {
    //        writer.Write(jsonData);
    //    }

    //    Debug.Log("文件已写入: " + filePath+"   data:"+jsonData);
    //}
}
