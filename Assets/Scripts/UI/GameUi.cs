using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameUi : MonoBehaviour
{
    private Button resetStart;

    private Button aiStart;

    private Transform Canvas;

    private Text specialNum;
    private Text scoreNum;
    private void Awake()
    {
        Canvas = GameObject.Find("Canvas").transform;

        specialNum = transform.Find("SpecialText").GetComponent<Text>();
        scoreNum = transform.Find("ScoreText").GetComponent<Text>();

        resetStart = transform.Find("ResetStart").GetComponent<Button>();
        aiStart = transform.Find("AIStart").GetComponent<Button>();

    }

    private void Start()
    {
        resetStart.onClick.AddListener(ResetStart);
        resetStart.interactable = false;
        aiStart.interactable = false;
        aiStart.onClick.AddListener(AiStart);
    }

    private void AiStart()
    {
        GameManager.Instance.SetAiState(true);
        SetResetBtnState(false);
    }

    private void ResetStart()
    {
        Debug.Log("ResetStart");
        SetResetBtnState(false);
        GameManager.Instance.SetAiState(false);
        GameManager.Instance.gridManager.TriggerExplosion();
        //StartCoroutine(GameManager.Instance.gridManager.GameReset(GameLevelManager.Instance.GetCurLevel()));
        StartCoroutine(ResetGrid());
    }

    IEnumerator ResetGrid()
    {
        Debug.LogError("ResetGrid");
        yield return new WaitForSeconds(1.5f);
        StartCoroutine(GameManager.Instance.gridManager.GameReset(GameLevelManager.Instance.GetCurLevel()));
    }

    public void RefreshSpecial(int num)
    {
        specialNum.text = num.ToString();
        Debug.Log($"RefreshSpecial");
    }

    public void SetResetBtnState(bool state)
    {
        resetStart.interactable = state;
        aiStart.interactable = state;
        Debug.Log($"SetResetBtnState");
    }


    public void RefereshScore(int num)
    {
        scoreNum.text = (num*10).ToString();
        Debug.Log("RefreshScore");
    }

}
