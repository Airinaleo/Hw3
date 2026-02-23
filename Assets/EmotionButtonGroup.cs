using UnityEngine;
using UnityEngine.UI; // 控制颜色需要用到UI

public class EmotionButtonGroup : MonoBehaviour
{
    [Header("把你的5个情绪按钮拖到这里")]
    public Image[] buttons; // 存放5个按钮的背景图

    public Color normalColor = Color.white; // 默认颜色（白色或灰色）
    public Color selectedColor = Color.green; // 选中后的颜色（亮绿色）

    // 当任意按钮被点击时，呼叫这个方法
    public void SelectEmotion(int selectedIndex)
    {
        // 遍历所有按钮，把选中的变绿，没选中的变白
        for (int i = 0; i < buttons.Length; i++)
        {
            if (i == selectedIndex)
                buttons[i].color = selectedColor;
            else
                buttons[i].color = normalColor;
        }
    }
}