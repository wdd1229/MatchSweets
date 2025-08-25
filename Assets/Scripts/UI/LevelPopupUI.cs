using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
/// <summary>
/// 过关后的弹窗提示
/// </summary>
public class LevelPopupUI : MonoBehaviour
{
    public Button NextLevelBtn;
    public Text SpecialText;
    public Text ScoreText;
    public Text TitleText;
    public void Init()
    {
        NextLevelBtn = transform.Find("BG/NextLevelBtn").GetComponent<Button>();
        SpecialText= transform.Find("BG/SpecialText").GetComponent<Text>();
        ScoreText = transform.Find("BG/ScoreText").GetComponent<Text>();
        TitleText = transform.Find("BG/titleIma/TitleText").GetComponent<Text>();

        NextLevelBtn.onClick.AddListener(() => {
            
            
            gameObject.SetActive(false);

            GameManager.Instance.GameReset();


        });

    }

    public void RefreshUI(int specialNum,int scoreNum)
    {
        SpecialText.text = specialNum.ToString();
        ScoreText.text = (scoreNum*10).ToString();

        Show();
    }

    public void Show()
    {
        gameObject.SetActive(true);
    }
}
