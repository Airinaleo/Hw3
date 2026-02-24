using UnityEngine;

public class BoxController : MonoBehaviour
{
    [Header("箱子模型状态")]
    public GameObject openedBox;
    public GameObject closedBox;

    [Header("插槽系统 (拖入DropZone)")]
    public UnityEngine.XR.Interaction.Toolkit.Interactors.XRSocketInteractor socketInteractor;

    [Header("封箱感应区 (拖入FlapTouchZone)")]
    public GameObject flapTouchZone;

    [Header("封箱音效")]
    public AudioSource closeSound;

    void Start()
    {
        if (openedBox) openedBox.SetActive(true);
        if (closedBox) closedBox.SetActive(false);
    }

    public void TryCloseBox()
    {
        // 🌟 打印插槽状态！看看到底有没有识别到球！
        Debug.Log("【封箱大脑】收到关箱请求！当前插槽是否有物品？ " + (socketInteractor != null ? socketInteractor.hasSelection.ToString() : "Null"));

        if (socketInteractor != null && socketInteractor.hasSelection)
        {
            openedBox.SetActive(false);
            closedBox.SetActive(true);

            if (closeSound) closeSound.Play();
            if (flapTouchZone) flapTouchZone.SetActive(false);
            
            Debug.Log("【封箱大脑】关箱成功！");
        }
    }
}