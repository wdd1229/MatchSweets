using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RewardTrigger : TriggerBase
{
    private Text rewardText;
    private void Awake()
    {
        triggerType = TriggerType.Reward;
        rewardText = transform.Find("Image/Text").GetComponent<Text>();
    }

    public override void OnTriggerEnter2D(Collider2D collider2D)
    {
        GameManager.Instance.ShowRewardTip(rewardText.text);
        Debug.LogError($"{rewardText.text}");
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
