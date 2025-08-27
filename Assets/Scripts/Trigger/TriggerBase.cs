using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class TriggerBase:MonoBehaviour
{
    public enum TriggerType { Beat,Obstacle,Reward};
    
    public enum MoveType { Horizontal , Vertical }
    
    public TriggerType triggerType;
    public MoveType moveType;

    //public TriggerBase(TriggerType triggerType)
    //{
    //    this.triggerType = triggerType;
    //}

    public virtual void OnTriggerEnter2D(Collider2D collider2D)
    {

    }

    public virtual void OnTriggerStay2D(Collider2D collider2D)
    {

    }
    
    public virtual void  OnTriggerExit2D(Collider2D collider2D)
    {

    }

}
