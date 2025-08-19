using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameUi : MonoBehaviour
{

    private Transform Canvas;

    private Text specialNum;
    private void Awake()
    {
        Canvas = GameObject.Find("Canvas").transform;

        specialNum = transform.Find("scoreText").GetComponent<Text>();

    }

    public void RefreshSpecial(int num)
    {
        specialNum.text = num.ToString();
    }

}
