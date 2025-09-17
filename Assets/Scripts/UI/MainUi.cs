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
        startBtn.onClick.AddListener(GameStart);
        

    }

   
    void Start()
    {

        //closeBtn.onClick.AddListener(() => {
        //    Debug.Log("Unity message ExitMiniProgram");
        //    TT.ExitMiniProgram();
        //});
    }

    void GameStart()
    {
        //SDKManager.Instance.StopRecord();


        //LoadSceneManager.Instance.LoadScene("Game");
        gameObject.SetActive(false);

        gameUI.SetActive(true);
        GameManager.Instance.GameStart();
    }


}
