using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

[RequireComponent(typeof(XRSimpleInteractable))]
public class ChopGlass : MonoBehaviour
{
    [Header("劈开后生成的两半预制体")]
    public GameObject choppedHalvesPrefab; 
    
    [Header("挥手劈砍的速度阈值 (正数，越小越容易触发)")]
    public float chopSpeedThreshold = 1.5f; 

    [Header("玻璃碎裂音效")]
    public AudioClip shatterSound; // 注意：这里是 AudioClip，不是 AudioSource

    private XRSimpleInteractable simpleInteractable;
    private Transform hoveringHand;
    private Vector3 lastHandPosition;
    private bool isChopped = false;

    void Awake()
    {
        simpleInteractable = GetComponent<XRSimpleInteractable>();
        simpleInteractable.hoverEntered.AddListener(OnHoverEnter);
        simpleInteractable.hoverExited.AddListener(OnHoverExit);
    }

    void OnDestroy()
    {
        if (simpleInteractable != null)
        {
            simpleInteractable.hoverEntered.RemoveListener(OnHoverEnter);
            simpleInteractable.hoverExited.RemoveListener(OnHoverExit);
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
        if (hoveringHand != null && !isChopped)
        {
            // 关键修改：计算两帧之间的空间距离，得到绝对速度（不再区分上下左右）
            float speed = Vector3.Distance(hoveringHand.position, lastHandPosition) / Time.deltaTime;

            // 只要手挥动的速度超过阈值
            if (speed > chopSpeedThreshold)
            {
                ExecuteChop();
            }
            lastHandPosition = hoveringHand.position;
        }
    }

    private void ExecuteChop()
    {
        isChopped = true;

        // 关键修改：生成一个独立的空间音效，即使本物体销毁，声音也能完整播完！
        if (shatterSound != null)
        {
            AudioSource.PlayClipAtPoint(shatterSound, transform.position);
        }

        if (choppedHalvesPrefab != null)
        {
            GameObject halves = Instantiate(choppedHalvesPrefab, transform.position, transform.rotation);
            
            foreach (Rigidbody rb in halves.GetComponentsInChildren<Rigidbody>())
            {
                // 把原本的推力调小到 0.5f，防止它们飞下桌子
                Vector3 randomForce = new Vector3(Random.Range(-0.5f, 0.5f), 0.2f, Random.Range(-0.5f, 0.5f)).normalized;
                rb.AddForce(randomForce * 0.5f, ForceMode.Impulse);
            }
        }

        // 销毁完整状态的自己
        Destroy(gameObject);
    }
}