using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
/// <summary>
/// 得分显示控制
/// </summary>
public class floatingScore : MonoBehaviour
{
    public float floatSpeed = 400.0f;   // 上飘速度
    public float duration = 1f;     // 显示持续时间
    public float scaleDuration = 0.2f; // 放大动画时长

    private Vector3 initialScale;
    private float timer = 0f;
    private bool isScaling = true;
    private RectTransform rectTransform; // 用于UI移动


    private Text scoreText;
    private void Awake()
    {
        // 如果是UI对象，获取RectTransform
        rectTransform = GetComponent<RectTransform>();
        scoreText = transform.Find("scoreText").GetComponent<Text>();
    }

    private void Start()
    {
        initialScale = transform.localScale;
        transform.localScale = Vector3.zero; // 初始缩放为0
        Destroy(gameObject, duration);       // 3秒后销毁
    }

    void Update()
    {
        // 上飘移动：如果是UI元素，使用anchoredPosition
        if (rectTransform != null)
        {
            // anchoredPosition是Vector2，但我们需要修改Y
            Vector2 pos = rectTransform.anchoredPosition;
            pos.y += floatSpeed * Time.deltaTime;
            rectTransform.anchoredPosition = pos;
        }
        else // 非UI对象（世界空间）
        {
            transform.position += Vector3.up * floatSpeed * Time.deltaTime;
        }

        // 放大动画处理
        if (isScaling)
        {
            timer += Time.deltaTime;
            float progress = Mathf.Clamp01(timer / scaleDuration);
            transform.localScale = initialScale * progress;

            if (progress >= 1f) isScaling = false;
        }
    }

    // 外部调用设置分数文本
    public void SetScore(int score)
    {
        scoreText.text = $"+{score}"; // 例如显示"+100"
    }
}
