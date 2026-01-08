using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 用户界面管理器 - 管理游戏中的UI元素
/// </summary>
public class UserInterfaceManager : MonoBehaviour
{
    [Header("系统引用")]
    public EmotionStateMachine emotionSystem;
    public GameManager gameManager;
    public AudioManager audioManager;

    [Header("UI面板")]
    public GameObject emotionPanel;
    public GameObject tutorialPanel;
    public GameObject pausePanel;

    [Header("情绪显示组件")]
    public Text phaseText;
    public Slider constraintSlider;
    public Slider anxietySlider;
    public Slider agencySlider;
    public Slider hopeSlider;
    public Text constraintValueText;
    public Text anxietyValueText;
    public Text agencyValueText;
    public Text hopeValueText;

    [Header("教程文本")]
    public Text tutorialText;

    [Header("暂停菜单")]
    public Button resumeButton;
    public Button restartButton;
    public Button quitButton;

    [Header("颜色设置")]
    public Color orderPhaseColor = Color.blue;
    public Color chaosPhaseColor = Color.red;
    public Color reconstructionPhaseColor = Color.green;

    private bool isPaused = false;

    private void Start()
    {
        // 获取系统引用
        if (emotionSystem == null)
            emotionSystem = FindObjectOfType<EmotionStateMachine>();

        if (gameManager == null)
            gameManager = FindObjectOfType<GameManager>();

        if (audioManager == null)
            audioManager = FindObjectOfType<AudioManager>();

        // 初始化UI
        InitializeUI();

        // 订阅事件
        if (emotionSystem != null)
        {
            emotionSystem.OnPhaseChanged += OnPhaseChanged;
            emotionSystem.OnEmotionChanged += OnEmotionChanged;
        }

        // 设置按钮事件
        if (resumeButton != null)
            resumeButton.onClick.AddListener(TogglePause);
        
        if (restartButton != null)
            restartButton.onClick.AddListener(RestartGame);
        
        if (quitButton != null)
            quitButton.onClick.AddListener(QuitGame);
    }

    private void OnDestroy()
    {
        // 取消订阅事件
        if (emotionSystem != null)
        {
            emotionSystem.OnPhaseChanged -= OnPhaseChanged;
            emotionSystem.OnEmotionChanged -= OnEmotionChanged;
        }

        // 移除按钮事件
        if (resumeButton != null)
            resumeButton.onClick.RemoveListener(TogglePause);
        
        if (restartButton != null)
            restartButton.onClick.RemoveListener(RestartGame);
        
        if (quitButton != null)
            quitButton.onClick.RemoveListener(QuitGame);
    }

    private void Update()
    {
        // 处理暂停输入
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            TogglePause();
        }

        // 更新UI显示
        UpdateUIDisplay();
    }

    /// <summary>
    /// 初始化UI
    /// </summary>
    private void InitializeUI()
    {
        // 设置滑块范围
        if (constraintSlider != null) constraintSlider.maxValue = 1f;
        if (anxietySlider != null) anxietySlider.maxValue = 1f;
        if (agencySlider != null) agencySlider.maxValue = 1f;
        if (hopeSlider != null) hopeSlider.maxValue = 1f;

        // 隐藏暂停面板
        if (pausePanel != null)
            pausePanel.SetActive(false);

        // 显示初始教程
        ShowTutorialMessage("欢迎来到《回声之心》。使用WASD移动，鼠标查看四周。");
    }

    /// <summary>
    /// 更新UI显示
    /// </summary>
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

        // 更新情绪值文本
        if (constraintValueText != null) constraintValueText.text = emotionSystem.constraint.ToString("F2");
        if (anxietyValueText != null) anxietyValueText.text = emotionSystem.anxiety.ToString("F2");
        if (agencyValueText != null) agencyValueText.text = emotionSystem.agency.ToString("F2");
        if (hopeValueText != null) hopeValueText.text = emotionSystem.hope.ToString("F2");
    }

    /// <summary>
    /// 阶段变化处理
    /// </summary>
    private void OnPhaseChanged(Phase newPhase)
    {
        // 显示阶段变化的教程信息
        switch (newPhase)
        {
            case Phase.Order:
                ShowTutorialMessage("你处于秩序阶段。尝试与环境交互，感受压抑的氛围。");
                break;
            case Phase.Chaos:
                ShowTutorialMessage("世界开始崩塌！你获得了脉冲能力，点击鼠标左键使用它。");
                break;
            case Phase.Reconstruction:
                ShowTutorialMessage("现在你可以重建世界。靠近共鸣点并点击鼠标右键放置碎片。");
                break;
        }
    }

    /// <summary>
    /// 情绪变化处理
    /// </summary>
    private void OnEmotionChanged(EmotionType emotionType, float value)
    {
        // 可以在这里添加特定情绪变化时的UI反馈
    }

    /// <summary>
    /// 显示教程消息
    /// </summary>
    public void ShowTutorialMessage(string message)
    {
        if (tutorialText != null)
        {
            tutorialText.text = message;
            
            // 可以添加淡入淡出效果
            if (tutorialPanel != null)
            {
                tutorialPanel.SetActive(true);
                
                // 3秒后隐藏教程面板
                Invoke("HideTutorialMessage", 3f);
            }
        }
    }

    /// <summary>
    /// 隐藏教程消息
    /// </summary>
    private void HideTutorialMessage()
    {
        if (tutorialPanel != null)
        {
            tutorialPanel.SetActive(false);
        }
    }

    /// <summary>
    /// 切换暂停状态
    /// </summary>
    public void TogglePause()
    {
        isPaused = !isPaused;
        
        if (pausePanel != null)
        {
            pausePanel.SetActive(isPaused);
        }

        // 控制时间流逝
        Time.timeScale = isPaused ? 0f : 1f;

        // 控制光标显示
        Cursor.visible = isPaused;
        Cursor.lockState = isPaused ? CursorLockMode.None : CursorLockMode.Locked;
    }

    /// <summary>
    /// 重新开始游戏
    /// </summary>
    public void RestartGame()
    {
        if (gameManager != null)
        {
            gameManager.RestartGame();
        }
        else
        {
            // 如果没有游戏管理器，直接重新加载场景
            UnityEngine.SceneManagement.SceneManager.LoadScene(UnityEngine.SceneManagement.SceneManager.GetActiveScene().name);
        }
        
        // 恢复正常时间流逝
        Time.timeScale = 1f;
    }

    /// <summary>
    /// 退出游戏
    /// </summary>
    public void QuitGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}