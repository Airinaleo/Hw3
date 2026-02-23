using UnityEngine;
using UnityEngine.InputSystem;

public class AnimateHandOnInput : MonoBehaviour
{
    [Header("输入配置")]
    public InputActionProperty gripAnimationAction; 
    public Animator handAnimator;

    void Update()
    {
        // 只要能读到按键值，手就能动！不掺杂任何其他复杂的 XR 逻辑。
        if (gripAnimationAction.action != null)
        {
            float gripValue = gripAnimationAction.action.ReadValue<float>();
            handAnimator.SetFloat("Grip", gripValue);
        }
    }
}