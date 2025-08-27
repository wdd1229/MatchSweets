using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RewardUI : MonoBehaviour
{
    private GameObject rewardTip;
    private Text rewardMsg;
    private void Awake()
    {
        rewardTip = transform.Find("RewardTip").gameObject;
        rewardMsg=rewardTip.transform.Find("Text").GetComponent<Text>();
        rewardTip.SetActive(false);
    }

    public void ShowRewardTip(string msg)
    {
        rewardMsg.text ="恭喜获得"+ msg;
        rewardTip.SetActive(true);
    }
}
