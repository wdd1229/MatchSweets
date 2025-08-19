using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tile : MonoBehaviour
{
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
        gridManager = transform.parent.GetComponent<GridManager>();
        animator = GetComponent<Animator>();
    }

    public void Init(int x,int y,GridType gridType)
    {
        this.gridType = gridType;
        xIndex = x;
        yIndex = y;
        if (gridType == GridType.Empty || gridType == GridType.Top)
        {
            name = gridType.ToString() + "(" + xIndex + ", " + yIndex + ")";

        }
        else
        {
            name = "Tile (" + xIndex + ", " + yIndex + ")";
        }

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
        //RectTransform grid = transform.GetComponent<RectTransform>();
        //grid.sizeDelta = new Vector2(gridManager.gridItemWidth, gridManager.gridItemHeight);
        //grid.anchoredPosition = new Vector2(
        //    -gridManager.totalGridWidth / 2 + gridManager.gridItemWidth / 2 + xIndex * (gridManager.gridItemWidth + gridManager.padding),
        //    -gridManager.totalGridHeight / 2 + gridManager.gridItemHeight / 2 + endIndexY * (gridManager.gridItemHeight + gridManager.padding)
        //);

        Vector3 start=transform.localPosition;
        Vector3 end = gridManager.BGtiles[xIndex,yIndex].transform.localPosition;

        float duration = 0.08f * fallDistance;//下落速度
        float elapsed = 0;
        while (elapsed < duration)
        {
            end = gridManager.BGtiles[xIndex, yIndex].transform.localPosition;//下落过程中可能会需要再次下落更多，所以这里再次设置一下
            elapsed += Time.deltaTime;
            transform.localPosition = Vector3.Lerp(start, end, elapsed / duration);
            yield return null;

        }
        transform.localPosition = end;
        SetState(TileState.Idle);
        fallDistance = 0;
        // 下落完成后检查匹配
        //StartCoroutine(CheckForMatchAfterFalling());
    }

    IEnumerator CheckForMatchAfterFalling()
    {
        yield return new WaitForSeconds(1.0f);
        // 调用 GridManager 或其他相关组件进行匹配检查
        // 例如: gridManager.CheckForMatchesAt(xIndex, yIndex);
        //gridManager.CheckForMatchesAt(xIndex, yIndex);
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
                break;
            case TileState.Clearing:
                animator.Play("clear");
                break;
            case TileState.Checking:
                animator.Play("move");
                break;
            default:
                break;
        }
    }
}
