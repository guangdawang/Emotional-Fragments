using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// 游戏管理器 - 协调整个游戏流程
/// </summary>
public class GameManager : MonoBehaviour
{
    [Header("系统引用")]
    public EmotionStateMachine emotionSystem;
    public FragmentManager fragmentManager;
    public LevelManager levelManager;
    public AudioManager audioManager;
    public UserInterfaceManager uiManager;

    [Header("玩家引用")]
    public PlayerController playerController;

    [Header("游戏设置")]
    public bool enableTesting = true;
    public KeyCode restartKey = KeyCode.R;

    private static GameManager instance;
    
    public static GameManager Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<GameManager>();
                if (instance == null)
                {
                    GameObject go = new GameObject("GameManager");
                    instance = go.AddComponent<GameManager>();
                }
            }
            return instance;
        }
    }

    private void Awake()
    {
        // 确保只有一个实例
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else if (instance != this)
        {
            Destroy(gameObject);
            return;
        }

        // 查找场景中的系统组件
        FindSystems();
    }

    private void Start()
    {
        // 初始化游戏
        InitializeGame();
    }

    private void Update()
    {
        // 处理输入
        HandleInput();
    }

    /// <summary>
    /// 查找场景中的系统组件
    /// </summary>
    private void FindSystems()
    {
        if (emotionSystem == null)
            emotionSystem = FindObjectOfType<EmotionStateMachine>();

        if (fragmentManager == null)
            fragmentManager = FindObjectOfType<FragmentManager>();

        if (levelManager == null)
            levelManager = FindObjectOfType<LevelManager>();

        if (audioManager == null)
            audioManager = FindObjectOfType<AudioManager>();

        if (uiManager == null)
            uiManager = FindObjectOfType<UserInterfaceManager>();

        if (playerController == null)
            playerController = FindObjectOfType<PlayerController>();
    }

    /// <summary>
    /// 初始化游戏
    /// </summary>
    private void InitializeGame()
    {
        Debug.Log("游戏初始化完成");
        
        // 可以在这里添加初始化逻辑
        if (emotionSystem != null)
        {
            // 设置初始情绪值
            emotionSystem.constraint = 0.2f;
            emotionSystem.anxiety = 0.1f;
            emotionSystem.agency = 0.3f;
            emotionSystem.hope = 0.4f;
        }
        
        // 初始化其他系统
        if (audioManager != null)
        {
            // 音频系统会在Start中自行初始化
        }
        
        if (uiManager != null)
        {
            // UI系统会在Start中自行初始化
        }
    }

    /// <summary>
    /// 处理输入
    /// </summary>
    private void HandleInput()
    {
        // 重新开始游戏
        if (Input.GetKeyDown(restartKey))
        {
            RestartGame();
        }
    }

    /// <summary>
    /// 重新开始游戏
    /// </summary>
    public void RestartGame()
    {
        Debug.Log("游戏重新开始");
        
        // 重新加载当前场景
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    /// <summary>
    /// 结束游戏
    /// </summary>
    public void EndGame()
    {
        Debug.Log("游戏结束");
        
        // 可以在这里添加游戏结束逻辑
        // 比如显示结局画面、统计数据等
        
        if (uiManager != null)
        {
            // 显示结局UI
        }
    }

    /// <summary>
    /// 获取当前游戏进度报告
    /// </summary>
    public GameProgressReport GetProgressReport()
    {
        GameProgressReport report = new GameProgressReport();
        
        if (emotionSystem != null)
        {
            report.currentPhase = emotionSystem.currentPhase;
            report.constraint = emotionSystem.constraint;
            report.anxiety = emotionSystem.anxiety;
            report.agency = emotionSystem.agency;
            report.hope = emotionSystem.hope;
            report.dominantEmotion = emotionSystem.GetDominantEmotion();
        }

        if (fragmentManager != null)
        {
            // 可以添加碎片系统相关的统计信息
        }

        return report;
    }

    private void OnGUI()
    {
        if (!enableTesting) return;

        // 显示测试信息
        GUIStyle labelStyle = new GUIStyle(GUI.skin.label);
        labelStyle.fontSize = 14;
        labelStyle.normal.textColor = Color.white;

        GUILayout.BeginArea(new Rect(10, Screen.height - 150, 300, 150));
        GUILayout.Label("=== 测试信息 ===", labelStyle);
        
        if (emotionSystem != null)
        {
            GUILayout.Label($"主导情绪: {emotionSystem.GetDominantEmotion()}", labelStyle);
        }
        
        GUILayout.Label("R - 重新开始游戏", labelStyle);
        GUILayout.EndArea();
    }
}

/// <summary>
/// 游戏进度报告
/// </summary>
public struct GameProgressReport
{
    public Phase currentPhase;
    public float constraint;
    public float anxiety;
    public float agency;
    public float hope;
    public EmotionType dominantEmotion;
}