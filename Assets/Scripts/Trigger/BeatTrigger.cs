using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class BeatTrigger : TriggerBase
{
    Rigidbody2D rigidbody;
    private void Awake()
    {
        triggerType=TriggerType.Beat;

        moveType=MoveType.Vertical;
        
        rigidbody=GetComponent<Rigidbody2D>();
    }

    public float dirOdds = 0.5f;

    private float speed = 300;

    private void Start()
    {
        rigidbody.velocity = Vector2.down* speed;
    }


    public override void OnTriggerEnter2D(Collider2D collider2D)
    {
        TriggerBase triggerBase  = collider2D.GetComponent<TriggerBase>();
        if (triggerBase != null && triggerBase.triggerType==TriggerBase.TriggerType.Obstacle)
        {
            //triggerBase.gameObject.GetComponent<BoxCollider2D>().isTrigger = false;
            //rigidbody.gravityScale = 0;
            rigidbody.constraints = RigidbodyConstraints2D.FreezePositionY;

            //向左或向右随机移动
            moveType=MoveType.Horizontal;
            if (UnityEngine.Random.value < dirOdds)
            {
                rigidbody.velocity = Vector2.left * speed;
                //向左
                //rigidbody.MovePosition(new Vector2(transform.position.x - 120, transform.position.y));
            }
            else
            {
                rigidbody.velocity = Vector2.right * speed;

                //向右
                //rigidbody.MovePosition(new Vector2(transform.position.x + 120, transform.position.y));
            }
        }else if(triggerBase != null && triggerBase.triggerType == TriggerBase.TriggerType.Reward)
        {
            rigidbody.constraints = RigidbodyConstraints2D.FreezePositionY;
        }
        base.OnTriggerEnter2D(collider2D);
    }
    

    public override void OnTriggerStay2D(Collider2D collider2D)
    {
        //Debug.LogError("OnTriggerStay2D");
        base.OnTriggerStay2D(collider2D);

    }

    public override void OnTriggerExit2D(Collider2D collider2D)
    {
        if (moveType == MoveType.Horizontal) 
        {
            rigidbody.constraints = RigidbodyConstraints2D.FreezePositionX;
            rigidbody.constraints &= ~RigidbodyConstraints2D.FreezePositionY;
            rigidbody.velocity = Vector2.down * speed;
        }
        base.OnTriggerExit2D(collider2D);
    }
}
