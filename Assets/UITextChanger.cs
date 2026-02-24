using UnityEngine;
using TMPro;

public class UITextChanger : MonoBehaviour
{
    [Header("【1】上方的引导文字")]
    public TextMeshProUGUI introduction; 
    
    [Header("【2】在这里依次填入你的引导语")]
    [TextArea]
    public string[] dialogueLines; 

    [Header("【3】主控制按钮的文字 (Next)")]
    public TextMeshProUGUI buttonText; 
    public string newButtonText = "Next"; 

    [Header("【4】第三页专属的交互UI打包组")]
    public GameObject page3UI; 

    [Header("【5】滑块上方的分数文字")]
    public TextMeshProUGUI scoreText; 

    // 🌟 新增：在这里连接你的世界变色管理器
    [Header("【6】世界变色管理器 (拖入挂了WorldTransition的物体)")]
    public WorldTransition worldTransitionManager;

    public void UpdateScoreText(float value)
    {
        if (scoreText != null)
        {
            scoreText.text = value.ToString(); 
        }
    }

    private int currentIndex = 0; 

    void Start()
    {
        if (dialogueLines.Length > 0 && introduction != null)
        {
            introduction.text = dialogueLines[0];
        }
        if (page3UI != null) page3UI.SetActive(false);
    }

    public void ChangeTheText()
    {
        currentIndex++; // 按下按钮，步数 +1

        // 状态 1：还在播放正常的 dialogueLines
        if (currentIndex < dialogueLines.Length)
        {
            if (buttonText != null) buttonText.text = newButtonText;
            if (introduction != null) introduction.text = dialogueLines[currentIndex];
            
            // 如果是第3页（数组索引是2），显示专属UI
            if (currentIndex == 2) 
            {
                if (page3UI != null) page3UI.SetActive(true);
            }
            else 
            {
                if (page3UI != null) page3UI.SetActive(false);
            }
        }
        // 状态 2：引导语播完了，显示最终指示，并改变按钮文字
        else if (currentIndex == dialogueLines.Length)
        {
            if (introduction != null) introduction.text = "Now, choose a physical symbol on the table to represent your feelings.";
            if (buttonText != null) buttonText.text = "Choice"; // 按钮变身为 Choice
            if (page3UI != null) page3UI.SetActive(false); // 确保滑块消失
        }
        // 状态 3：玩家按下了 "Choice" 按钮！触发变色并隐藏按钮
        else if (currentIndex > dialogueLines.Length)
        {
            // 💡 触发变色魔法！
            if (worldTransitionManager != null)
            {
                worldTransitionManager.TriggerColorTransition();
            }
            else
            {
                Debug.LogWarning("注意：你忘记把 WorldTransitionManager 拖进脚本里了！");
            }

            // 变色后，隐藏这个按钮自己，防止玩家继续乱点
            gameObject.SetActive(false); 
        }
    }
}