using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 情绪UI面板 - 显示当前情绪状态的用户界面
/// </summary>
public class EmotionUIPanel : MonoBehaviour
{
    [Header("情绪系统引用")]
    public EmotionStateMachine emotionSystem;

    [Header("UI元素")]
    public Text phaseText;
    public Slider constraintSlider;
    public Slider anxietySlider;
    public Slider agencySlider;
    public Slider hopeSlider;
    
    public Image constraintBar;
    public Image anxietyBar;
    public Image agencyBar;
    public Image hopeBar;

    [Header("颜色设置")]
    public Color orderPhaseColor = Color.blue;
    public Color chaosPhaseColor = Color.red;
    public Color reconstructionPhaseColor = Color.green;

    private void Start()
    {
        // 获取场景中的情绪状态机实例
        if (emotionSystem == null)
            emotionSystem = FindObjectOfType<EmotionStateMachine>();

        if (emotionSystem == null)
            Debug.LogError("未找到 EmotionStateMachine 实例");

        // 初始化UI元素
        InitializeUI();
    }

    private void Update()
    {
        // 更新UI显示
        UpdateUIDisplay();
    }

    private void InitializeUI()
    {
        // 设置初始值
        if (constraintSlider != null) constraintSlider.maxValue = 1f;
        if (anxietySlider != null) anxietySlider.maxValue = 1f;
        if (agencySlider != null) agencySlider.maxValue = 1f;
        if (hopeSlider != null) hopeSlider.maxValue = 1f;
    }

    private void UpdateUIDisplay()
    {
        if (emotionSystem == null) return;

        // 更新阶段显示
        if (phaseText != null)
        {
            phaseText.text = $"阶段: {emotionSystem.currentPhase}";
            // 根据阶段更改文本颜色
            switch (emotionSystem.currentPhase)
            {
                case Phase.Order:
                    phaseText.color = orderPhaseColor;
                    break;
                case Phase.Chaos:
                    phaseText.color = chaosPhaseColor;
                    break;
                case Phase.Reconstruction:
                    phaseText.color = reconstructionPhaseColor;
                    break;
            }
        }

        // 更新情绪值显示
        if (constraintSlider != null) constraintSlider.value = emotionSystem.constraint;
        if (anxietySlider != null) anxietySlider.value = emotionSystem.anxiety;
        if (agencySlider != null) agencySlider.value = emotionSystem.agency;
        if (hopeSlider != null) hopeSlider.value = emotionSystem.hope;

        // 更新情绪条颜色和填充
        UpdateEmotionBars();
    }

    private void UpdateEmotionBars()
    {
        if (constraintBar != null)
        {
            constraintBar.fillAmount = emotionSystem.constraint;
            // 根据值调整颜色（从蓝到紫）
            constraintBar.color = Color.Lerp(Color.blue, Color.magenta, emotionSystem.constraint);
        }

        if (anxietyBar != null)
        {
            anxietyBar.fillAmount = emotionSystem.anxiety;
            // 根据值调整颜色（从黄到红）
            anxietyBar.color = Color.Lerp(Color.yellow, Color.red, emotionSystem.anxiety);
        }

        if (agencyBar != null)
        {
            agencyBar.fillAmount = emotionSystem.agency;
            // 根据值调整颜色（从灰到白）
            agencyBar.color = Color.Lerp(Color.gray, Color.white, emotionSystem.agency);
        }

        if (hopeBar != null)
        {
            hopeBar.fillAmount = emotionSystem.hope;
            // 根据值调整颜色（从黑到绿）
            hopeBar.color = Color.Lerp(Color.black, Color.green, emotionSystem.hope);
        }
    }

    // 用于测试的方法
    public void OnConstraintIncreaseButton()
    {
        if (emotionSystem != null)
            emotionSystem.ProcessPlayerAction(ActionType.MoveWithEffort);
    }

    public void OnAnxietyIncreaseButton()
    {
        if (emotionSystem != null)
            emotionSystem.ProcessPlayerAction(ActionType.AvoidCollision);
    }

    public void OnAgencyIncreaseButton()
    {
        if (emotionSystem != null)
            emotionSystem.ProcessPlayerAction(ActionType.UsePulse);
    }

    public void OnHopeIncreaseButton()
    {
        if (emotionSystem != null)
            emotionSystem.ProcessPlayerAction(ActionType.PlaceFragment);
    }

    public void OnSwitchToChaosPhase()
    {
        if (emotionSystem != null)
            emotionSystem.currentPhase = Phase.Chaos;
    }

    public void OnSwitchToReconstructionPhase()
    {
        if (emotionSystem != null)
            emotionSystem.currentPhase = Phase.Reconstruction;
    }
}