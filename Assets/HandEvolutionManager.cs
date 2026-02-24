using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class HandEvolutionManager : MonoBehaviour
{
    [Header("【1】双手渲染器 (拖入 LeftHand1 和 RightHand1 的模型)")]
    public Renderer leftHandRenderer;
    public Renderer rightHandRenderer;

    [Header("【2】三个阶段的材质")]
    public Material cloudMat;  // 阶段1：白色软糯
    public Material nebulaMat; // 阶段2：蓝紫星云 (抓起物品时)
    public Material powerMat;  // 阶段3：橙红能量 (推向箱子时)

    [Header("【3】手部粒子特效 (拖入挂在手上的 Particle System)")]
    public ParticleSystem leftParticles;
    public ParticleSystem rightParticles;

    [Header("【4】目标与交互")]
    public Transform boxSocketTransform; // 拖入刚才建的 DropZone 物体
    public GameObject prayerUI;          // 拖入双手合十的 UI

    private int currentStage = 1;
    private Transform currentHeldObject;

    void Start()
    {
        SetHandMaterial(cloudMat);
        if(leftParticles) leftParticles.Stop();
        if(rightParticles) rightParticles.Stop();
        if(prayerUI) prayerUI.SetActive(false);
    }

    // 🌟 事件1：当玩家抓起任何象征物时触发 (进入星云状态)
    public void OnObjectGrabbed(SelectEnterEventArgs args)
    {
        if (currentStage == 1 || currentStage == 4) 
        {
            currentStage = 2;
            SetHandMaterial(nebulaMat);
        }
        currentHeldObject = args.interactableObject.transform;
    }

    // 🌟 事件2：当玩家松手时触发
    public void OnObjectReleased(SelectExitEventArgs args)
    {
        currentHeldObject = null;
    }

    // 🌟 事件3：当物品被放入快递箱时触发 (进入等待合十状态)
    public void OnObjectPlacedInBox(SelectEnterEventArgs args)
    {
        currentStage = 4;
        SetHandMaterial(cloudMat); // 放下重担，恢复纯净云朵
        if(leftParticles) leftParticles.Stop();
        if(rightParticles) rightParticles.Stop();
    }

    void Update()
    {
        // 阶段 3 逻辑：抓着物体，推向箱子的过程 (蓄力)
        if (currentStage >= 2 && currentStage < 4 && currentHeldObject != null && boxSocketTransform != null)
        {
            // 计算手中物体离快递箱的距离
            float distanceToBox = Vector3.Distance(currentHeldObject.position, boxSocketTransform.position);
            
            // 假设 0.8 米开始产生能量吸取感，距离越近 progress 越趋近 1
            float progress = Mathf.Clamp01((0.8f - distanceToBox) / 0.8f);

            if (progress > 0.3f && currentStage == 2)
            {
                currentStage = 3;
                SetHandMaterial(powerMat);
                if(leftParticles && !leftParticles.isPlaying) leftParticles.Play();
                if(rightParticles && !rightParticles.isPlaying) rightParticles.Play();
            }

            // 粒子颜色随着距离变近，从蓝色平滑变为橙红色！
            if (currentStage == 3)
            {
                Color lerpedColor = Color.Lerp(Color.blue, new Color(1f, 0.4f, 0f), progress);
                if (leftParticles) { var main = leftParticles.main; main.startColor = lerpedColor; }
                if (rightParticles) { var main = rightParticles.main; main.startColor = lerpedColor; }
            }
        }

        // 阶段 4 逻辑：物品已放入箱内，检测双手合十
        if (currentStage == 4)
        {
            if (leftHandRenderer != null && rightHandRenderer != null)
            {
                // 计算两只物理手的距离
                float handDist = Vector3.Distance(leftHandRenderer.transform.position, rightHandRenderer.transform.position);
                
                // 如果双手距离小于 15 厘米，视为双手合十！
                if (handDist < 0.15f && !prayerUI.activeSelf)
                {
                    prayerUI.SetActive(true);
                }
            }
        }
    }

    private void SetHandMaterial(Material mat)
    {
        if (leftHandRenderer) leftHandRenderer.material = mat;
        if (rightHandRenderer) rightHandRenderer.material = mat;
    }
}