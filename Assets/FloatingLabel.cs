using UnityEngine;

public class FloatingLabel : MonoBehaviour
{
    [Header("【1】跟随目标 (拖入桌上的球或圆柱体)")]
    public Transform targetObject;

    [Header("【2】位置偏移量 (调整Y轴悬浮高度)")]
    public Vector3 offset = new Vector3(0, 0.25f, 0); 

    [Header("【3】是否永远面向玩家头显?")]
    public bool faceCamera = true;

    private Transform mainCamera;

    void Start()
    {
        if (Camera.main != null) mainCamera = Camera.main.transform;
    }

    void LateUpdate()
    {
        // 目标死了，标签跟着死
        if (targetObject == null)
        {
            Destroy(gameObject);
            return; 
        }

        // 纯净跟随位置，不跟随缩放
        transform.position = targetObject.position + offset;

        // 广告牌面向摄像机
        if (faceCamera && mainCamera != null)
        {
            transform.LookAt(transform.position + mainCamera.rotation * Vector3.forward, 
                             mainCamera.rotation * Vector3.up);
        }
    }
}