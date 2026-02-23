using UnityEngine;
using UnityEngine.InputSystem;

public class HandAnimatorDriver : MonoBehaviour
{
    [Header("绑定手柄按键引用")]
    public InputActionProperty triggerAction; // 食指
    public InputActionProperty gripAction;    // 中指/侧边

    private Animator animator;

    void Start()
    {
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        if (animator == null) return;

        // 安全锁 3：增加空值判断，防止按键由于某种原因没绑定时报 NullReference 错误
        float triggerValue = triggerAction.action != null ? triggerAction.action.ReadValue<float>() : 0f;
        float gripValue = gripAction.action != null ? gripAction.action.ReadValue<float>() : 0f;

        // 传递给 Animator，注意 Animator 里的参数名必须为 Float 类型的 "Trigger" 和 "Grip"
        animator.SetFloat("Trigger", triggerValue);
        animator.SetFloat("Grip", gripValue);
    }
}