using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

[RequireComponent(typeof(Rigidbody))]
public class PhysicalHandFollow : MonoBehaviour
{
    [Header("追踪目标 (必须填 LeftHandAnchor)")]
    public Transform targetController; 
    
    [Header("抓取组件 (消除抓取抖动)")]
    public UnityEngine.XR.Interaction.Toolkit.Interactors.XRDirectInteractor directInteractor; 
    
    private Rigidbody rb;
    private Collider handCollider;
    
    public float followSpeed = 30f; 
    public float teleportDistance = 0.3f; // 🌟 如果手离手柄超过30厘米(比如摇杆走路)，瞬间瞬移跟上，绝不颤动！

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        handCollider = GetComponent<Collider>();
        
        // 关键物理设置
        rb.useGravity = false;
        rb.isKinematic = false;
        rb.interpolation = RigidbodyInterpolation.Interpolate;

        if (directInteractor != null)
        {
            directInteractor.selectEntered.AddListener(OnGrab);
            directInteractor.selectExited.AddListener(OnRelease);
        }
    }

    private void OnGrab(SelectEnterEventArgs args)
    {
        Collider[] objColliders = args.interactableObject.transform.GetComponentsInChildren<Collider>();
        foreach (var objCol in objColliders)
        {
            if (handCollider != null && objCol != null)
                Physics.IgnoreCollision(handCollider, objCol, true);
        }
    }

    private void OnRelease(SelectExitEventArgs args)
    {
        Collider[] objColliders = args.interactableObject.transform.GetComponentsInChildren<Collider>();
        foreach (var objCol in objColliders)
        {
            if (handCollider != null && objCol != null)
                Physics.IgnoreCollision(handCollider, objCol, false);
        }
    }

    void FixedUpdate() 
    {
        if (targetController == null) return;

        // 1. 位置跟随：保留物理阻挡
        Vector3 positionDifference = targetController.position - transform.position;
        if (positionDifference.magnitude > teleportDistance)
        {
            // 玩家用摇杆走路时，瞬间瞬移手部，消除追赶产生的剧烈颤动
            transform.position = targetController.position;
            rb.linearVelocity = Vector3.zero;
        }
        else
        {
            // 正常挥手时，用速度追赶（遇到桌子会被挡住）
            rb.linearVelocity = positionDifference * followSpeed;
        }

        // 2. 旋转跟随：终极杀招！无视一切阻力，强制锁死手腕方向和 Anchor 完全一致！
        rb.MoveRotation(targetController.rotation);
    }
}