using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class FloatingScore : MonoBehaviour
{
    private Text scoreText;
    private RectTransform rectTransform; // 手动分配UI元素的RectTransform
    private float floatSpeed = 300; // 漂浮速度（单位：像素/秒）
    private float floatDuration = 1f; // 动画持续时间
    private float startTime;

    private void Awake()
    {
        scoreText=transform.Find("score").GetComponent<Text>();
        rectTransform = transform.GetComponent<RectTransform>();
    }

    public void Init(int score)
    {
        scoreText.text=score.ToString();
    }
    private float speed = 300; // 漂浮速度（单位：像素/秒）
    private float time = 1.5f; // 动画持续时间
    void Start()
    {
        startTime =0;
    }
    void Update()
    {
        // 向上漂浮
        transform.Translate(Vector3.up * floatSpeed * Time.deltaTime);

        startTime += Time.deltaTime;
        if (startTime >= floatDuration)
        {
            Destroy(gameObject);
        }
    }



}
