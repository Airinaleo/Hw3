using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

[RequireComponent(typeof(XRGrabInteractable))]
public class DoughInteractable : MonoBehaviour
{
    private XRGrabInteractable grabInteractable;
    private Vector3 initialScale;
    private float initialHandDistance;

    [Header("绑定握拳信号(控制是否能砸)")]
    public InputActionProperty leftGripAction;  
    public InputActionProperty rightGripAction; 

    // ✨新增音效 1：在面板里暴露一个槽位，用来拖入 AudioSource 组件
    [Header("砸扁音效")]
    public AudioSource smashSound; 

    private Transform hoveringHand;
    private Vector3 lastHandPosition;
    private float smashCooldown = 0f;

    void Awake()
    {
        grabInteractable = GetComponent<XRGrabInteractable>();
        initialScale = transform.localScale;
        grabInteractable.selectMode = InteractableSelectMode.Multiple;
        
        grabInteractable.hoverEntered.AddListener(OnHoverEnter);
        grabInteractable.hoverExited.AddListener(OnHoverExit);
    }

    void OnEnable()
    {
        if (leftGripAction.action != null) leftGripAction.action.Enable();
        if (rightGripAction.action != null) rightGripAction.action.Enable();
    }

    void OnDisable()
    {
        if (leftGripAction.action != null) leftGripAction.action.Disable();
        if (rightGripAction.action != null) rightGripAction.action.Disable();
    }

    void OnDestroy()
    {
        if (grabInteractable != null)
        {
            grabInteractable.hoverEntered.RemoveListener(OnHoverEnter);
            grabInteractable.hoverExited.RemoveListener(OnHoverExit);
        }
    }

    private void OnHoverEnter(HoverEnterEventArgs args)
    {
        hoveringHand = args.interactorObject.transform;
        lastHandPosition = hoveringHand.position;
    }

    private void OnHoverExit(HoverExitEventArgs args)
    {
        if (hoveringHand == args.interactorObject.transform) hoveringHand = null;
    }

    void Update()
    {
        smashCooldown -= Time.deltaTime;

        if (hoveringHand != null && grabInteractable.interactorsSelecting.Count == 0)
        {
            float velocityY = (hoveringHand.position.y - lastHandPosition.y) / Time.deltaTime;
            float leftGrip = leftGripAction.action != null ? leftGripAction.action.ReadValue<float>() : 0f;
            float rightGrip = rightGripAction.action != null ? rightGripAction.action.ReadValue<float>() : 0f;
            
            bool isFist = leftGrip > 0.5f || rightGrip > 0.5f;

            if (isFist && velocityY < -0.5f && smashCooldown <= 0f)
            {
                SmashDough();
                smashCooldown = 0.5f; 
            }
            lastHandPosition = hoveringHand.position;
        }

        if (grabInteractable.interactorsSelecting.Count == 2)
        {
            Transform h1 = grabInteractable.interactorsSelecting[0].transform;
            Transform h2 = grabInteractable.interactorsSelecting[1].transform;
            float dist = Vector3.Distance(h1.position, h2.position);
            
            if (initialHandDistance == 0) initialHandDistance = dist;

            float factor = dist / initialHandDistance;
            transform.localScale = new Vector3(initialScale.x * factor, initialScale.y / Mathf.Sqrt(factor), initialScale.z / Mathf.Sqrt(factor));
        }
        else { initialHandDistance = 0; }
    }

    private void SmashDough()
    {
        // 先获取当前底部高度
        float bottomY = GetComponent<Collider>().bounds.min.y;
        transform.rotation = Quaternion.identity;

        Vector3 lastScale = transform.localScale;
        if (lastScale.y < initialScale.y * 0.15f) return; 

        // ✨新增音效 2：只要触发了砸扁逻辑，就播放声音！
        if (smashSound != null) smashSound.Play();

        float flattenAmount = 0.05f; 
        float growAmount = 0.025f;   
        
        transform.localScale = new Vector3(lastScale.x + growAmount, lastScale.y - flattenAmount, lastScale.z + growAmount);

        // 绝对吸附贴合桌面
        transform.position = new Vector3(transform.position.x, bottomY + (transform.localScale.y * 0.5f), transform.position.z);
    }
}