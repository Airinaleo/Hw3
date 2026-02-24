using UnityEngine;
using System.Collections.Generic;

public class WorldTransition : MonoBehaviour
{
    [System.Serializable]
    public class ObjectMaterialPair
    {
        public MeshRenderer renderer;
        public Material colorMaterial; // 目标彩色材质
    }

    [Header("需要变色的物体列表")]
    public List<ObjectMaterialPair> transitionObjects;
    
    [Header("原始的纯白哑光材质")]
    public Material whiteMatteMaterial;

    void Start()
    {
        // 游戏开始时，强制将所有物体设为纯白哑光
        foreach (var item in transitionObjects)
        {
            if (item.renderer != null)
                item.renderer.material = whiteMatteMaterial;
        }
    }

    // 将这个方法绑定到你的“选择象征物”按钮的 OnClick 事件上！
    public void TriggerColorTransition()
    {
        foreach (var item in transitionObjects)
        {
            if (item.renderer != null && item.colorMaterial != null)
                item.renderer.material = item.colorMaterial;
        }
    }
}