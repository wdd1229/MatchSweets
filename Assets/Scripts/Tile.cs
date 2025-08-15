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
        if (gridType == GridType.Null || gridType == GridType.Empty)
            return;
        xIndex = x;
        yIndex = y;
        this.gridType = gridType;
        // ���÷��������Ϊ������
        name = "Tile (" + xIndex + ", " + yIndex + ")";
    }

    /// <summary>
    /// �����߼�
    /// </summary>
    /// <param name="distance"></param>
    public void StartFalling(int distance)
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
        Vector3 start=transform.localPosition;
        Vector3 end = new Vector3(xIndex * GridManager.tileSize, -yIndex * GridManager.tileSize, transform.position.z);

        float duration = 0.1f * fallDistance;//�����ٶ�
        float elapsed = 0;
        while (elapsed < duration)
        {
            end = new Vector3(xIndex * GridManager.tileSize, -yIndex * GridManager.tileSize, transform.position.z);//��������п��ܻ���Ҫ�ٴ�������࣬���������ٴ�����һ��
            elapsed += Time.deltaTime;
            transform.localPosition = Vector3.Lerp(start, end, elapsed / duration);
            yield return null;

        }
        transform.localPosition = end;
        SetState(TileState.Idle);
        fallDistance = 0;
        // ������ɺ���ƥ��
        CheckForMatchAfterFalling();
    }

    private void CheckForMatchAfterFalling()
    {
        // ���� GridManager ����������������ƥ����
        // ����: gridManager.CheckForMatchesAt(xIndex, yIndex);
        gridManager.CheckForMatchesAt(xIndex, yIndex);
    }

    /// <summary>
    /// ���ò�ͬ״̬
    /// </summary>
    /// <param name="state"></param>
    public void SetState(TileState state)
    {
        currentState = state;
        UpdateAnimationState();
    }



    public void Clear()
    {
        currentState = TileState.Clearing;
    }

    /// <summary>
    /// ���ݲ�ͬ״̬ ���Ų�ͬ״̬ ��ͬ�߼�
    /// </summary>
    public void UpdateAnimationState()
    {
        if (animator == null) return;
        switch (currentState)
        {
            case TileState.Idle:
                break;
            case TileState.Moving:
                break;
            case TileState.Clearing:
                break;
            case TileState.Checking:
                break;
            default:
                break;
        }
    }
}
