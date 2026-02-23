using UnityEngine;
using UnityEngine.InputSystem;

public class HandAnimatorDriver : MonoBehaviour
{
    [Header("绑定手柄按键")]
    public InputActionProperty triggerAction; // 食指扳机
    public InputActionProperty gripAction;    // 中指/侧边握持

    private Animator animator;

    void Start()
    {
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        // 读取按键按下的深度值 (0 到 1)
        float triggerValue = triggerAction.action.ReadValue<float>();
        float gripValue = gripAction.action.ReadValue<float>();

        // 传递给 Animator 的参数（名字必须和你在 Animator 里建的一模一样）
        animator.SetFloat("Trigger", triggerValue);
        animator.SetFloat("Grip", gripValue);
    }
}