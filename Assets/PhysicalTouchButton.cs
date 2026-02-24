using UnityEngine;

public class PhysicalTouchButton : MonoBehaviour
{
    public BoxController boxController; 

    private void OnTriggerEnter(Collider other)
    {
        // 🌟 打印日志测谎！如果你摸了但控制台没这句话，说明碰撞体没碰到！
        Debug.Log("【封箱雷达】检测到碰撞！碰它的物体是：" + other.gameObject.name);

        // 只要碰它的东西名字不带 Box 或 DropZone，就去试图关箱子
        if (!other.name.Contains("Box") && !other.name.Contains("DropZone"))
        {
            if (boxController != null) boxController.TryCloseBox();
        }
    }
}