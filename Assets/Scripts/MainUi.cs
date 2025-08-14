using System.Collections;
using System.Collections.Generic;
using TTSDK;
using UnityEngine;
using UnityEngine.UI;

public class MainUi : MonoBehaviour
{
    private Button startBtn;

    private Button closeBtn;

    private void Awake()
    {
        startBtn =transform.Find("Btn_Start").GetComponent<Button>();
        closeBtn = GameObject.Find("CloseBtn").transform.GetComponent<Button>();
    }

    void Start()
    {
        TT.InitSDK((code, env) =>
        {
            Debug.Log("Unity message init sdk callback");
            Debug.Log("Unity message code: " + code);
            Debug.Log("Unity message HostEnum: " + env.m_HostEnum);
            Debug.Log("Unity message AppId: " + env.GameAppId);
        });

        Debug.LogError("是否在TT Container真机环境下：" + TT.InContainerEnv);
        startBtn.onClick.AddListener(GameStart);
        closeBtn.onClick.AddListener(() => {
            Debug.Log("Unity message ExitMiniProgram");
            TT.ExitMiniProgram();
        });
    }

    void GameStart()
    {
        //LoadSceneManager.Instance.LoadScene("Game");
        gameObject.SetActive(false);
    }


}
