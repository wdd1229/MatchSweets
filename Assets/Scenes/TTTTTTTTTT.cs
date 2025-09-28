using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TTTTTTTTTT : MonoBehaviour
{
    // Start is called before the first frame update
    public Transform canvas;
    void Start()
    {
        // 在 Canvas 上设置一个 Sprite 位置点
        GameObject sprite = new GameObject("AnimatedSprite");
        sprite.transform.SetParent(canvas.transform, false);
        sprite.AddComponent<SpriteRenderer>();

        sprite.GetComponent<SpriteRenderer>().sortingOrder = 1; // 或更高
        //sprite.GetComponent<SpriteRenderer>().sortingLayerID = 10; // 或更高

        Debug.LogError("test success !");
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
