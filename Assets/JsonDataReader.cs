using System.IO;
using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using UnityEngine.UI;

public class JsonDataReader : MonoBehaviour
{
    public string jsonFileName1 = "Data/LevelData.json";
    public string jsonFileName2 = "Data/ScoreData.json";

    public Text text1;
    public Text text2;

    void Start()
    {
        StartCoroutine(LoadJsonFiles());
    }

    IEnumerator LoadJsonFiles()
    {
        // 读取第一个JSON文件
        string filePath1 = Path.Combine(Application.streamingAssetsPath, jsonFileName1);
        string jsonData1 = "";

        if (filePath1.Contains("://"))
        {
            UnityWebRequest www1 = UnityWebRequest.Get(filePath1);
            yield return www1.SendWebRequest();

            if (www1.result == UnityWebRequest.Result.Success)
            {
                jsonData1 = www1.downloadHandler.text;
            }
            else
            {
                Debug.LogError("Failed to load " + jsonFileName1 + ": " + www1.error);
            }
        }
        else
        {
            jsonData1 = File.ReadAllText(filePath1);
        }

        // 解析第一个JSON数据
        if (!string.IsNullOrEmpty(jsonData1))
        {
            // 这里可以根据JSON的结构创建对应的类来解析数据
            // 示例：假设JSON是一个简单的对象
            // MyData data1 = JsonUtility.FromJson<MyData>(jsonData1);
            text1.gameObject.SetActive(true);
            Debug.Log("Loaded data from " + jsonFileName1 + ": " + jsonData1);
        }

        // 读取第二个JSON文件
        string filePath2 = Path.Combine(Application.streamingAssetsPath, jsonFileName2);
        string jsonData2 = "";

        if (filePath2.Contains("://"))
        {
            UnityWebRequest www2 = UnityWebRequest.Get(filePath2);
            yield return www2.SendWebRequest();

            if (www2.result == UnityWebRequest.Result.Success)
            {
                jsonData2 = www2.downloadHandler.text;
            }
            else
            {
                Debug.LogError("Failed to load " + jsonFileName2 + ": " + www2.error);
            }
        }
        else
        {
            jsonData2 = File.ReadAllText(filePath2);
        }

        // 解析第二个JSON数据
        if (!string.IsNullOrEmpty(jsonData2))
        {
            // 示例：假设JSON是一个简单的对象
            // MyData data2 = JsonUtility.FromJson<MyData>(jsonData2);
            text2.gameObject.SetActive(true);
            Debug.Log("Loaded data from " + jsonFileName2 + ": " + jsonData2);
        }
    }
}

// 示例JSON数据类
[System.Serializable]
public class MyData
{
    public string key;
    public int value;
}
