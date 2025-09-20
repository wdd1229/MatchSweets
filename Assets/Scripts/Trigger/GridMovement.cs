using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static TriggerBase;

public class GridMovement : MonoBehaviour
{
    public float stepInterval = 0.2f; // 每步间隔(秒)
    public float stepSize = 120;       // 每格距离
    private Rigidbody2D rb;
    private float timer;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.isKinematic = true; // 引用[4]方案：避免物理干扰
    }
    private bool canMove=true;
    void FixedUpdate()
    {
        if (canMove)
        {
            timer += Time.deltaTime;
            if (timer >= stepInterval)
            {
                timer = 0;
                // 离散移动一格
                
                Vector2 newPos =  new Vector2(rb.position.x, GetComponent<RectTransform>().localPosition.y - stepSize);
                Debug.LogError($"rb.position:{rb.position}  targetPos:{newPos} stepSize:{ stepSize} ");
                rb.MovePosition(newPos); // 保持物理碰撞检测
            }
        }
    }
    public  void OnTriggerEnter2D(Collider2D collider2D)
    {
        canMove = false;
    }
}
