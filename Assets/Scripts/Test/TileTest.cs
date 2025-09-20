using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TileTest : MonoBehaviour
{
    // 运动参数
    [SerializeField] private float initialUpSpeed = 800f;   // 初始上升速度(像素/秒)
    [SerializeField] private float gravity = 1500f;         // 重力加速度(像素/秒²)
    [SerializeField] private float fadeDuration = 1.2f;     // 淡出时长
    [SerializeField] private float bottomThreshold = -1200f;// 屏幕底部阈值

    private RectTransform rect;
    private Image image;
    private Vector2 startPos;
    private Color originalColor;

    void Awake()
    {
        rect = GetComponent<RectTransform>();
        image = transform.Find("body").GetComponent<Image>();
        //originalColor = image.color;
    }

    // 启动爆炸动画
    public void StartExplosion(Vector2 direction)
    {
        startPos = rect.anchoredPosition;
        StartCoroutine(ExplodeCoroutine(direction.normalized));
    }

    private IEnumerator ExplodeCoroutine(Vector2 dir)
    {
        float elapsed = 0f;
        float fadeElapsed = 0f;
        Vector2 velocity = dir * initialUpSpeed;
        Vector2 currentPos = Vector2.zero;

        // 三阶段循环：上升-&gt;下落-&gt;离屏
        while (true)
        {
            elapsed += Time.deltaTime;

            // 1. 位置更新 (使用运动方程)
            velocity.y -= gravity * Time.deltaTime;  // 应用重力
            currentPos += velocity * Time.deltaTime;
            rect.anchoredPosition = startPos + currentPos;

            // 2. 淡出效果 (使用Mathf.Sign替代比较)
            //if (Mathf.Sign(fadeDuration - fadeElapsed) == 1)
            //{
            //    fadeElapsed += Time.deltaTime;
            //    float alpha = 1 - fadeElapsed / fadeDuration;
            //    image.color = new Color(
            //        originalColor.r,
            //        originalColor.g,
            //        originalColor.b,
            //        alpha
            //    );
            //}

            // 3. 离屏检测 (使用Mathf.Sign)
            if (Mathf.Sign(rect.anchoredPosition.y - bottomThreshold) == -1)
                break;

            yield return null;
        }

        gameObject.SetActive(false);

        //Destroy(gameObject);
    }
}
