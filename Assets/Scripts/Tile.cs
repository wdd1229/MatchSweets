using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Tile : MonoBehaviour
{



    // 运动参数
    [SerializeField] private float initialUpSpeed = 1500f;   // 初始上升速度(像素/秒)
    [SerializeField] private float gravity = 2800f;         // 重力加速度(像素/秒²)
    [SerializeField] private float fadeDuration = 1.2f;     // 淡出时长
    [SerializeField] private float bottomThreshold = -1200f;// 屏幕底部阈值

    private RectTransform rect;

    private Vector2 startPos;






    public int xIndex;
    public int yIndex;
    public GridType gridType;
    public enum TileState { Idle, Moving, Clearing, Checking };
    public TileState currentState = TileState.Idle;

    private Animator animator;

    public int fallDistance = 0;

    public GridManager gridManager;

    private void Awake()
    {
        rect = GetComponent<RectTransform>();

        gridManager = transform.parent.GetComponent<GridManager>();
        animator = GetComponent<Animator>();
    }

    public void Init(int x,int y,GridType gridType)
    {
        this.gridType = gridType;
        xIndex = x;
        yIndex = y;
        //if (gridType == GridType.Empty || gridType == GridType.Top)
        //{
        //    name = gridType.ToString() + "(" + xIndex + ", " + yIndex + ")";

        //}
        //else
        //{
        //    name = "Tile (" + xIndex + ", " + yIndex + ")";
        //}

        // 设置方块的名字为其坐标
    }

    /// <summary>
    /// 下落逻辑
    /// </summary>
    /// <param name="distance"></param>
    public void StartFalling(int y,int distance)
    {
        if (fallDistance != 0)
        {
            fallDistance += 1;
            return;
        }
        fallDistance=distance;
        SetState(TileState.Moving);

        StartCoroutine(FallToPosition());
    }

    IEnumerator FallToPosition()
    {
        RectTransform grid = transform.GetComponent<RectTransform>();
        //grid.sizeDelta = new Vector2(gridManager.gridItemWidth, gridManager.gridItemHeight);
        //grid.anchoredPosition = new Vector2(
        //    -gridManager.totalGridWidth / 2 + gridManager.gridItemWidth / 2 + xIndex * (gridManager.gridItemWidth + gridManager.padding),
        //    -gridManager.totalGridHeight / 2 + gridManager.gridItemHeight / 2 + endIndexY * (gridManager.gridItemHeight + gridManager.padding)
        //);

        //Vector3 start=transform.localPosition;
        //Vector3 end = gridManager.BGtiles[xIndex,yIndex].transform.localPosition;
        //Vector3 end = CalculatePos(xIndex, yIndex);

        Vector2 start= grid.anchoredPosition;
        Vector2 end = CalculatePos(xIndex, yIndex);


        float duration = 0.05f * fallDistance;//下落速度
        float elapsed = 0;

        while (elapsed < duration)
        {
            //end = gridManager.BGtiles[xIndex, yIndex].transform.localPosition;//下落过程中可能会需要再次下落更多，所以这里再次设置一下
            end = CalculatePos(xIndex, yIndex);//下落过程中可能会需要再次下落更多，所以这里再次设置一下
            elapsed += Time.deltaTime;
            //transform.localPosition = Vector3.Lerp(start, end, elapsed / duration);
            grid.anchoredPosition = Vector2.Lerp(start, end, elapsed / duration);
            yield return null;

        }
        //transform.localPosition = end;
        grid.anchoredPosition = end;
        SetState(TileState.Idle);
        fallDistance = 0;
        // 下落完成后检查匹配
        //StartCoroutine(CheckForMatchAfterFalling());
    }

    public Vector2 CalculatePos(int Xindex,int Yindex)
    {
       return gridManager.CalculateGridPos(Xindex, Yindex);
    }

    IEnumerator CheckForMatchAfterFalling()
    {
        yield return new WaitForSeconds(1.0f);
        // 调用 GridManager 或其他相关组件进行匹配检查
        // 例如: gridManager.CheckForMatchesAt(xIndex, yIndex);
        //gridManager.CheckForMatchesAt(xIndex, yIndex);
    }

    public void ResetState()
    {
        //SetState(TileState.Idle);
        currentState = TileState.Idle;
        UpdateAnimationState();
    }

    /// <summary>
    /// 设置不同状态
    /// </summary>
    /// <param name="state"></param>
    public void SetState(TileState state)
    {
        if (currentState == TileState.Clearing)
            return;
        currentState = state;
        UpdateAnimationState();
    }

    

    public void Clear()
    {
        currentState = TileState.Clearing;
    }

    /// <summary>
    /// 根据不同状态 播放不同状态 不同逻辑
    /// </summary>
    public void UpdateAnimationState()
    {
        if (animator == null) return;
        switch (currentState)
        {
            case TileState.Idle:
                animator.Play("idle");
                break;
            case TileState.Moving:
                //animator.Play("move");
                break;
            case TileState.Clearing:
                if(gameObject.activeSelf)
                    animator.Play("clear");
                break;
            case TileState.Checking:
                animator.Play("move");
                break;
            default: 
                break;
        }
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
