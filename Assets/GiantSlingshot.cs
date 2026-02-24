using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using System.Collections;

[RequireComponent(typeof(UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable))]
public class GiantSlingshot : MonoBehaviour
{
    [Header("【1】弹弓结构")]
    public Transform leftPole;
    public Transform rightPole;
    public Transform restPosition; // 🌟 新建一个空物体，放在皮兜默认的静止位置，拖入这里！
    
    [Header("【2】视觉与能量变色")]
    public LineRenderer rubberBand;
    public Color normalColor = Color.white;
    public Color energyColor = new Color(1f, 0.4f, 0f); // 橙红色能量
    public float maxPullDistance = 1.0f; // 最多能拉多远

    [Header("【3】音效")]
    public AudioSource stretchSound; // 拉扯时的绷紧声
    public AudioSource releaseSound; // 发射的爆破声

    [Header("【4】发射目标 (拖入 BoxManager)")]
    public Rigidbody targetBox;
    public float launchForce = 1200f; // 发射力度

    private UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable grabInteractable;
    private Transform currentHand;
    private bool isPulled = false;

    void Start()
    {
        grabInteractable = GetComponent<UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable>();
        
        // 🌟 防抖神技：告诉 XR 系统不要乱动它，全由我们代码接管！
        grabInteractable.trackPosition = false;
        grabInteractable.trackRotation = false;

        grabInteractable.selectEntered.AddListener(OnGrab);
        grabInteractable.selectExited.AddListener(OnRelease);

        rubberBand.positionCount = 3; // 左柱 -> 皮兜 -> 右柱
    }

    private void OnGrab(SelectEnterEventArgs args)
    {
        currentHand = args.interactorObject.transform;
        isPulled = true;
        if (stretchSound) stretchSound.Play();
    }

    private void OnRelease(SelectExitEventArgs args)
    {
        isPulled = false;
        currentHand = null;
        if (stretchSound) stretchSound.Stop();

        // 计算拉了多远
        float pullDist = Vector3.Distance(transform.position, restPosition.position);
        
        // 如果拉扯距离超过 20 厘米，视为有效发射！
        if (pullDist > 0.2f)
        {
            if (releaseSound) releaseSound.Play();
            LaunchBox();
        }

        // 皮兜瞬间弹回原位
        transform.position = restPosition.position;
        UpdateRubberBand(0f);
    }

    void Update()
    {
        if (isPulled && currentHand != null && restPosition != null)
        {
            // 计算手相对于静止点的位置
            Vector3 pullVector = currentHand.position - restPosition.position;
            
            // 限制拉力距离，防止皮筋拉得无限长
            float dist = Mathf.Clamp(pullVector.magnitude, 0, maxPullDistance);
            transform.position = restPosition.position + pullVector.normalized * dist;

            // 计算蓄力比例 (0 到 1)
            float tension = dist / maxPullDistance;
            UpdateRubberBand(tension);

            // 细节：拉得越紧，声音音调越高
            if (stretchSound) stretchSound.pitch = 1f + tension;
        }
        else if (restPosition != null)
        {
            // 没被抓的时候，平滑地回到原位
            transform.position = Vector3.Lerp(transform.position, restPosition.position, Time.deltaTime * 15f);
            UpdateRubberBand(0f);
        }
    }

    private void UpdateRubberBand(float tension)
    {
        if (leftPole == null || rightPole == null) return;

        rubberBand.SetPosition(0, leftPole.position);
        rubberBand.SetPosition(1, transform.position); // 皮兜位置
        rubberBand.SetPosition(2, rightPole.position);

        // 颜色随着拉力，从白光渐变成橙红能量光！
        Color currentColor = Color.Lerp(normalColor, energyColor, tension);
        rubberBand.startColor = currentColor;
        rubberBand.endColor = currentColor;
    }

    private void LaunchBox()
    {
        if (targetBox != null)
        {
            // 解除物理锁定
            targetBox.isKinematic = false;

            // 计算发射方向 (从皮兜指向静止点)
            Vector3 launchDirection = (restPosition.position - transform.position).normalized;
            // 加一点向上的仰角，让它能抛物线飞向远方
            launchDirection = (launchDirection + Vector3.up * 0.3f).normalized;

            // 施加爆炸性的推力
            targetBox.AddForce(launchDirection * launchForce, ForceMode.Impulse);

            // 启动“飞向远方直到消失”的魔法
            StartCoroutine(ShrinkAndDisappear());
        }
    }

    private IEnumerator ShrinkAndDisappear()
    {
        // 先让它飞 2 秒
        yield return new WaitForSeconds(2f);
        
        // 然后花 1.5 秒慢慢缩小到 0
        float t = 0;
        Vector3 startScale = targetBox.transform.localScale;
        while (t < 1.5f)
        {
            t += Time.deltaTime;
            targetBox.transform.localScale = Vector3.Lerp(startScale, Vector3.zero, t / 1.5f);
            yield return null;
        }
        
        // 最后彻底隐藏
        targetBox.gameObject.SetActive(false);
    }
}