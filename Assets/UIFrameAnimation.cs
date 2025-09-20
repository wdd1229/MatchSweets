using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class UIFrameAnimation : MonoBehaviour
{
    public Sprite[] frames;
    public float frameDuration = 0.1f;
    private Image image;

    void Start()
    {
        image = GetComponent<Image>();
        AnimateLoop();
    }

    void AnimateLoop()
    {
        // 使用DOTween的序列功能
        Sequence sequence = DOTween.Sequence();

        foreach (var frame in frames)
        {
            sequence.AppendCallback(() => image.sprite = frame)
                   .AppendInterval(frameDuration);
        }

        sequence.SetLoops(-1, LoopType.Restart);
    }
}
