using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static TriggerBase;

public class ObstacleTrigger : TriggerBase
{
    private void Awake()
    {
        triggerType = TriggerType.Obstacle;
    }

    public override void OnTriggerEnter2D(Collider2D collider2D)
    {
        base.OnTriggerEnter2D(collider2D);
    }


    public override void OnTriggerStay2D(Collider2D collider2D)
    {
        base.OnTriggerStay2D(collider2D);

    }

    public override void OnTriggerExit2D(Collider2D collider2D)
    {
        base.OnTriggerExit2D(collider2D);
    }
}
