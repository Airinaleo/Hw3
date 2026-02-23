using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

[RequireComponent(typeof(XRGrabInteractable))]
public class DoughInteractable : MonoBehaviour
{
    private XRGrabInteractable grabInteractable;
    private Vector3 initialScale;
    private float initialHandDistance;

    // 记录手部上一帧的位置，用来计算“拍”的速度
    private Vector3 lastHandPosition;

    void Awake()
    {
        grabInteractable = GetComponent<XRGrabInteractable>();
        initialScale = transform.localScale;
        
        // 允许双手同时抓取
        grabInteractable.selectMode = InteractableSelectMode.Multiple;

        // 监听手的“触碰（Hover）”事件，代替物理碰撞
        grabInteractable.hoverEntered.AddListener(OnHandTouch);
    }

    void OnDestroy()
    {
        grabInteractable.hoverEntered.RemoveListener(OnHandTouch);
    }

    void Update()
    {
        // === 逻辑 1：双手拉伸 ===
        if (grabInteractable.interactorsSelecting.Count == 2)
        {
            Transform hand1 = grabInteractable.interactorsSelecting[0].transform;
            Transform hand2 = grabInteractable.interactorsSelecting[1].transform;
            
            float currentDistance = Vector3.Distance(hand1.position, hand2.position);
            
            if (initialHandDistance == 0) initialHandDistance = currentDistance;

            // 根据双手距离拉长 X 轴，同时让 Y 和 Z 变细（保持体积守恒）
            float stretchFactor = currentDistance / initialHandDistance;
            transform.localScale = new Vector3(
                initialScale.x * stretchFactor, 
                initialScale.y / Mathf.Sqrt(stretchFactor), 
                initialScale.z / Mathf.Sqrt(stretchFactor)
            );
        }
        else
        {
            initialHandDistance = 0; // 松开后重置
            
            // 记录单手抓取时的位置，供下一帧计算速度（可选扩展）
            if (grabInteractable.interactorsSelecting.Count == 1)
            {
                lastHandPosition = grabInteractable.interactorsSelecting[0].transform.position;
            }
        }
    }

    // === 逻辑 2：单手拍扁（绝对安全，不需要手模型有 Collider） ===
    private void OnHandTouch(HoverEnterEventArgs args)
    {
        // 检查摸到球的是不是直接交互器（手）
        if (args.interactorObject is UnityEngine.XR.Interaction.Toolkit.Interactors.XRDirectInteractor)
        {
            // 只要手摸上来，就让它瞬间变扁一点点（Y轴缩小，XZ轴变宽）
            // 这里你可以根据需要调整数值，目前是拍一下扁 10%
            Vector3 currentScale = transform.localScale;
            transform.localScale = new Vector3(
                currentScale.x * 1.1f, 
                currentScale.y * 0.9f, 
                currentScale.z * 1.1f
            );
        }
    }
}