using UnityEngine;

public class SetSpriteSize : MonoBehaviour
{
    public SpriteRenderer sprite;
    public Vector2 desiredSize = new Vector2(400, 400); // 你想要的精灵尺寸（像素数）


    

    public Transform canvasTransform;

    void Start()
    {
        // 确定 RectTransform
        RectTransform rectTransform = sprite.gameObject.GetComponent<RectTransform>();

        if (rectTransform == null)
        {
            // 如果没有 RectTransform，需要添加
            rectTransform = sprite.gameObject.AddComponent<RectTransform>();
            rectTransform.sizeDelta = desiredSize;
            rectTransform.anchorMin = new Vector2(0, 0);
            rectTransform.anchorMax = new Vector2(1, 1);
            rectTransform.pivot = new Vector2(0.5f, 0.5f); // 居中
        }
        else
        {
            rectTransform.sizeDelta = desiredSize;
        }

        // 设置锚点、缩放中心
        //sprite.gameObject.transform.SetParent(canvasTransform, false);
    }
}
