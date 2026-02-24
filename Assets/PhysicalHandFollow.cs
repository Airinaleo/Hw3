using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

[RequireComponent(typeof(Rigidbody))]
public class PhysicalHandFollow : MonoBehaviour
{
    [Header("追踪目标 (必须填 LeftHandAnchor)")]
    public Transform targetController; 
    
    [Header("抓取组件 (消除抖动关键)")]
    public UnityEngine.XR.Interaction.Toolkit.Interactors.XRDirectInteractor directInteractor; 
    
    private Rigidbody rb;
    private Collider handCollider;
    
    public float followSpeed = 20f; 
    public float maxDistance = 0.5f; 

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        handCollider = GetComponent<Collider>();
        
        // 🌟 错在这里！必须是 150f 以上，否则转身手卡住！
        rb.maxAngularVelocity = 150f; 

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

        Vector3 positionDifference = targetController.position - transform.position;
        if (positionDifference.magnitude > maxDistance)
        {
            transform.position = targetController.position;
            rb.linearVelocity = Vector3.zero;
        }
        else
        {
            rb.linearVelocity = positionDifference * followSpeed;
        }

        Quaternion rotationDifference = targetController.rotation * Quaternion.Inverse(transform.rotation);
        rotationDifference.ToAngleAxis(out float angle, out Vector3 axis);
        if (angle > 180f) angle -= 360f; 
        
        if (axis != Vector3.zero && !float.IsNaN(axis.x))
        {
            // 🌟 乘数改为 50f，让旋转立刻响应
            rb.angularVelocity = axis * (angle * Mathf.Deg2Rad * 50f);
        }
    }
}