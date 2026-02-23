using UnityEngine;
using TMPro;

public class UITextChanger : MonoBehaviour
{
    [Header("【1】上方的引导文字")]
    public TextMeshProUGUI introduction; 
    
    [Header("【2】在这里依次填入你的4页引导语")]
    [TextArea]
    public string[] dialogueLines; 

    [Header("【3】主控制按钮的文字 (Next)")]
    public TextMeshProUGUI buttonText; 
    public string newButtonText = "继续"; 

    [Header("【4】第三页专属的交互UI打包组")]
    public GameObject page3UI; // 把你刚才建的 Page3_UI 拖到这里！

    [Header("【5】滑块上方的分数文字")]
    public TextMeshProUGUI scoreText; 

    // 这个方法专门给滑块用，拖动时自动调用
    public void UpdateScoreText(float value)
    {
        if (scoreText != null)
        {
            scoreText.text = value.ToString(); // 把滑块的数字变成文字
        }
    }

    // 记录当前播到第几句（从0开始计数）
    private int currentIndex = 0; 

    void Start()
    {
        // 游戏刚开始时，确保只显示第一句话，并隐藏第3页的特殊UI
        if (dialogueLines.Length > 0 && introduction != null)
        {
            introduction.text = dialogueLines[0];
        }
        if (page3UI != null) page3UI.SetActive(false);
    }

    // 主按钮(Next)按下时触发
    public void ChangeTheText()
    {
        if (buttonText != null) buttonText.text = newButtonText;

        currentIndex++; // 页码+1

        if (currentIndex < dialogueLines.Length)
        {
            // 播放下一句
            if (introduction != null) introduction.text = dialogueLines[currentIndex];
            
            // 🌟 核心魔法：如果是第3页（数组索引是2），就显示专属UI，否则隐藏！
            if (currentIndex == 2) 
            {
                if (page3UI != null) page3UI.SetActive(true);
            }
            else 
            {
                if (page3UI != null) page3UI.SetActive(false);
            }
        }
        else
        {
            // 所有引导词播完后的状态
            introduction.text = "现在，请在桌上选择一个实体象征物。";
            if (page3UI != null) page3UI.SetActive(false); // 确保滑块消失
            gameObject.SetActive(false); // 可选：把“继续”按钮自己也隐藏掉
        }
    }
}