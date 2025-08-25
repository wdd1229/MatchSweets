using System.Collections;
using System.Collections.Generic;
using TTSDK;
using UnityEngine;
using UnityEngine.UI;

public class MainUi : MonoBehaviour
{
    private Button startBtn;

    private Button closeBtn;

    private GameObject gameUI;

    private Transform Canvas;

    private void Awake()
    {
        Canvas = GameObject.Find("Canvas").transform;
        startBtn =transform.Find("Btn_Start").GetComponent<Button>();
        //closeBtn = Canvas.Find("CloseBtn").GetComponent<Button>();
        gameUI = Canvas.Find("GameUI").gameObject;
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
        //closeBtn.onClick.AddListener(() => {
        //    Debug.Log("Unity message ExitMiniProgram");
        //    TT.ExitMiniProgram();
        //});
    }

    void GameStart()
    {
        //LoadSceneManager.Instance.LoadScene("Game");
        gameObject.SetActive(false);

        gameUI.SetActive(true);
        GameManager.Instance.GameStart();
    }


}
