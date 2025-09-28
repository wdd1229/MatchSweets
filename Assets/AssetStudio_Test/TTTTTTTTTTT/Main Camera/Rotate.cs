using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rotate : MonoBehaviour
{
    // 旋转速度，可以在Inspector面板中调整
    public float rotationSpeed = 100f;
    // 旋转轴，可以在Inspector面板中调整
    public Vector3 rotationAxis = Vector3.up;

    // Update is called once per frame
    void Update()
    {
        // 每帧旋转物体
        transform.Rotate(rotationAxis * rotationSpeed * Time.deltaTime);
    }
}
