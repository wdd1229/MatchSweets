using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameUi : MonoBehaviour
{

    private Transform Canvas;

    private Text specialNum;
    private Text scoreNum;
    private void Awake()
    {
        Canvas = GameObject.Find("Canvas").transform;

        specialNum = transform.Find("SpecialText").GetComponent<Text>();
        scoreNum = transform.Find("ScoreText").GetComponent<Text>();

    }

    public void RefreshSpecial(int num)
    {
        specialNum.text = num.ToString();
    }

    public void RefereshScore(int num)
    {
        scoreNum.text = (num*10).ToString();
    }

}
