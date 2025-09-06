using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameUi : MonoBehaviour
{
    private Button resetStart;

    private Transform Canvas;

    private Text specialNum;
    private Text scoreNum;
    private void Awake()
    {
        Canvas = GameObject.Find("Canvas").transform;

        specialNum = transform.Find("SpecialText").GetComponent<Text>();
        scoreNum = transform.Find("ScoreText").GetComponent<Text>();

        resetStart = transform.Find("ResetStart").GetComponent<Button>();
        resetStart.onClick.AddListener(ResetStart);
        resetStart.interactable=false;
    }
    private void ResetStart()
    {
        SetResetBtnState(false);
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
    }

    public void SetResetBtnState(bool state)
    {
        resetStart.interactable = state;
    }

    public void RefereshScore(int num)
    {
        scoreNum.text = (num*10).ToString();
    }

}
