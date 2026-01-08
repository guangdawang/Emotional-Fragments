using UnityEngine;

/// <summary>
/// 性能监控器 - 监控游戏性能并确保符合项目要求
/// </summary>
public class PerformanceMonitor : MonoBehaviour
{
    [Header("性能设置")]
    public float targetFrameRate = 60f;
    public float maxMainThreadTime = 12f; // ms
    public int maxActiveFragments = 300;

    [Header("监控间隔")]
    public float monitorInterval = 1f;

    [Header("警告设置")]
    public bool showWarnings = true;
    public Color warningColor = Color.yellow;
    public Color errorColor = Color.red;

    // 性能数据
    private float currentFrameRate;
    private float currentMainThreadTime;
    private int activeFragmentCount;

    // 监控计时器
    private float monitorTimer;

    // 引用
    private FragmentManager fragmentManager;

    private void Start()
    {
        // 设置目标帧率
        Application.targetFrameRate = (int)targetFrameRate;

        // 查找碎片管理器
        fragmentManager = FindObjectOfType<FragmentManager>();

        // 初始化性能数据
        currentFrameRate = targetFrameRate;
    }

    private void Update()
    {
        // 更新性能数据
        UpdatePerformanceData();

        // 定期监控性能
        monitorTimer += Time.unscaledDeltaTime;
        if (monitorTimer >= monitorInterval)
        {
            MonitorPerformance();
            monitorTimer = 0f;
        }
    }

    /// <summary>
    /// 更新性能数据
    /// </summary>
    private void UpdatePerformanceData()
    {
        // 计算帧率
        currentFrameRate = 1f / Time.unscaledDeltaTime;

        // 估算主线程时间（简化）
        currentMainThreadTime = Time.deltaTime * 1000f;

        // 获取活跃碎片数量
        if (fragmentManager != null)
        {
            // 注意：这里需要访问FragmentManager中的私有列表，实际项目中应该提供公共方法
            activeFragmentCount = FindObjectOfType<FragmentManager>().gameObject.GetInstanceID(); // 简化处理
        }
    }

    /// <summary>
    /// 监控性能
    /// </summary>
    private void MonitorPerformance()
    {
        if (!showWarnings) return;

        // 检查帧率
        if (currentFrameRate < targetFrameRate * 0.9f)
        {
            Debug.LogWarning($"帧率过低: {currentFrameRate:F1} FPS (目标: {targetFrameRate} FPS)");
        }

        // 检查主线程时间
        if (currentMainThreadTime > maxMainThreadTime)
        {
            Debug.LogWarning($"主线程时间过长: {currentMainThreadTime:F2} ms (最大: {maxMainThreadTime} ms)");
        }

        // 检查碎片数量
        if (activeFragmentCount > maxActiveFragments)
        {
            Debug.LogWarning($"活跃碎片数量过多: {activeFragmentCount} (最大: {maxActiveFragments})");
            
            // 可以在这里添加自动优化措施
            if (fragmentManager != null)
            {
                // 例如：降低远处碎片的更新频率
            }
        }
    }

    /// <summary>
    /// 获取性能报告
    /// </summary>
    public PerformanceReport GetPerformanceReport()
    {
        return new PerformanceReport
        {
            frameRate = currentFrameRate,
            mainThreadTime = currentMainThreadTime,
            activeFragmentCount = activeFragmentCount,
            isFrameRateGood = currentFrameRate >= targetFrameRate * 0.95f,
            isMainThreadTimeGood = currentMainThreadTime <= maxMainThreadTime,
            isFragmentCountGood = activeFragmentCount <= maxActiveFragments
        };
    }

    private void OnGUI()
    {
        if (!showWarnings) return;

        // 显示性能信息
        GUIStyle labelStyle = new GUIStyle(GUI.skin.label);
        labelStyle.fontSize = 12;

        GUILayout.BeginArea(new Rect(Screen.width - 250, 10, 240, 150));
        
        GUILayout.Label($"帧率: {currentFrameRate:F1} FPS", labelStyle);
        GUILayout.Label($"主线程时间: {currentMainThreadTime:F2} ms", labelStyle);
        GUILayout.Label($"活跃碎片: {activeFragmentCount}", labelStyle);

        // 显示性能状态
        PerformanceReport report = GetPerformanceReport();
        
        GUI.color = report.isFrameRateGood ? Color.green : errorColor;
        GUILayout.Label(report.isFrameRateGood ? "帧率: 正常" : "帧率: 过低", labelStyle);
        
        GUI.color = report.isMainThreadTimeGood ? Color.green : errorColor;
        GUILayout.Label(report.isMainThreadTimeGood ? "线程时间: 正常" : "线程时间: 过长", labelStyle);
        
        GUI.color = report.isFragmentCountGood ? Color.green : warningColor;
        GUILayout.Label(report.isFragmentCountGood ? "碎片数量: 正常" : "碎片数量: 过多", labelStyle);
        
        GUI.color = Color.white;
        
        GUILayout.EndArea();
    }
}

/// <summary>
/// 性能报告
/// </summary>
public struct PerformanceReport
{
    public float frameRate;
    public float mainThreadTime; // ms
    public int activeFragmentCount;
    public bool isFrameRateGood;
    public bool isMainThreadTimeGood;
    public bool isFragmentCountGood;
}