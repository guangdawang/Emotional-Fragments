using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

/// <summary>
/// 后期处理控制器 - 根据情绪状态调整场景的整体视觉效果
/// </summary>
public class PostProcessController : MonoBehaviour
{
    [Header("情绪系统引用")]
    public EmotionStateMachine emotionSystem;

    [Header("后期处理体积")]
    public Volume postProcessVolume;

    // 后期处理效果
    private Bloom bloom;
    private Vignette vignette;
    private ColorAdjustments colorAdjustments;
    private ChromaticAberration chromaticAberration;

    [Header("情绪影响设置")]
    public EmotionEffectSettings orderSettings;
    public EmotionEffectSettings chaosSettings;
    public EmotionEffectSettings reconstructionSettings;

    private void Start()
    {
        // 获取场景中的情绪系统
        if (emotionSystem == null)
            emotionSystem = FindObjectOfType<EmotionStateMachine>();

        if (emotionSystem == null)
            Debug.LogError("未找到 EmotionStateMachine 实例");

        // 初始化后期处理效果
        InitializePostProcessing();

        // 订阅阶段变化事件
        if (emotionSystem != null)
        {
            emotionSystem.OnPhaseChanged += OnPhaseChanged;
        }
    }

    private void OnDestroy()
    {
        // 取消订阅事件
        if (emotionSystem != null)
        {
            emotionSystem.OnPhaseChanged -= OnPhaseChanged;
        }
    }

    private void Update()
    {
        // 每帧更新后期处理效果
        UpdatePostProcessing();
    }

    /// <summary>
    /// 初始化后期处理效果
    /// </summary>
    private void InitializePostProcessing()
    {
        if (postProcessVolume == null)
        {
            postProcessVolume = FindObjectOfType<Volume>();
        }

        if (postProcessVolume != null)
        {
            // 获取或创建后期处理效果
            if (!postProcessVolume.profile.TryGet(out bloom))
            {
                bloom = postProcessVolume.profile.Add<Bloom>(true);
            }

            if (!postProcessVolume.profile.TryGet(out vignette))
            {
                vignette = postProcessVolume.profile.Add<Vignette>(true);
            }

            if (!postProcessVolume.profile.TryGet(out colorAdjustments))
            {
                colorAdjustments = postProcessVolume.profile.Add<ColorAdjustments>(true);
            }

            if (!postProcessVolume.profile.TryGet(out chromaticAberration))
            {
                chromaticAberration = postProcessVolume.profile.Add<ChromaticAberration>(true);
            }
        }
        else
        {
            Debug.LogWarning("未找到后期处理Volume，部分视觉效果将不可用");
        }
    }

    /// <summary>
    /// 更新后期处理效果
    /// </summary>
    private void UpdatePostProcessing()
    {
        if (emotionSystem == null || postProcessVolume == null)
            return;

        // 根据当前阶段和情绪值插值计算效果参数
        EmotionEffectSettings currentSettings = GetCurrentSettings();
        
        // 插值计算当前效果参数
        float constraint = emotionSystem.constraint;
        float anxiety = emotionSystem.anxiety;
        float hope = emotionSystem.hope;

        // 应用效果
        if (bloom != null)
        {
            bloom.intensity.value = Mathf.Lerp(currentSettings.bloomIntensityMin, currentSettings.bloomIntensityMax, hope);
            bloom.threshold.value = Mathf.Lerp(currentSettings.bloomThresholdMin, currentSettings.bloomThresholdMax, anxiety);
        }

        if (vignette != null)
        {
            vignette.intensity.value = Mathf.Lerp(currentSettings.vignetteIntensityMin, currentSettings.vignetteIntensityMax, constraint);
            vignette.smoothness.value = Mathf.Lerp(currentSettings.vignetteSmoothnessMin, currentSettings.vignetteSmoothnessMax, hope);
        }

        if (colorAdjustments != null)
        {
            // 根据情绪状态混合颜色
            Color targetColor = Color.white;
            if (emotionSystem.currentPhase == Phase.Order)
                targetColor = Color.Lerp(Color.white, orderSettings.tintColor, constraint);
            else if (emotionSystem.currentPhase == Phase.Chaos)
                targetColor = Color.Lerp(orderSettings.tintColor, chaosSettings.tintColor, anxiety);
            else if (emotionSystem.currentPhase == Phase.Reconstruction)
                targetColor = Color.Lerp(chaosSettings.tintColor, reconstructionSettings.tintColor, hope);

            colorAdjustments.colorFilter.value = targetColor;
            colorAdjustments.saturation.value = Mathf.Lerp(currentSettings.saturationMin, currentSettings.saturationMax, hope);
            colorAdjustments.contrast.value = Mathf.Lerp(currentSettings.contrastMin, currentSettings.contrastMax, anxiety);
        }

        if (chromaticAberration != null)
        {
            // 焦虑值影响色差效果
            chromaticAberration.intensity.value = Mathf.Lerp(
                currentSettings.chromaticAberrationMin, 
                currentSettings.chromaticAberrationMax, 
                anxiety
            );
        }
    }

    /// <summary>
    /// 获取当前阶段的设置
    /// </summary>
    private EmotionEffectSettings GetCurrentSettings()
    {
        switch (emotionSystem.currentPhase)
        {
            case Phase.Order:
                return orderSettings;
            case Phase.Chaos:
                return chaosSettings;
            case Phase.Reconstruction:
                return reconstructionSettings;
            default:
                return orderSettings;
        }
    }

    /// <summary>
    /// 阶段变化处理
    /// </summary>
    private void OnPhaseChanged(Phase newPhase)
    {
        Debug.Log($"后期处理控制器: 阶段已切换到 {newPhase}");
        // 阶段变化时可以添加特殊效果
    }
}

[System.Serializable]
public class EmotionEffectSettings
{
    [Header("辉光设置")]
    public float bloomIntensityMin = 0.5f;
    public float bloomIntensityMax = 2.0f;
    public float bloomThresholdMin = 0.7f;
    public float bloomThresholdMax = 1.0f;

    [Header("暗角设置")]
    public float vignetteIntensityMin = 0.2f;
    public float vignetteIntensityMax = 0.5f;
    public float vignetteSmoothnessMin = 0.3f;
    public float vignetteSmoothnessMax = 0.6f;

    [Header("颜色调整")]
    public Color tintColor = Color.white;
    public float saturationMin = -20f;
    public float saturationMax = 20f;
    public float contrastMin = 10f;
    public float contrastMax = 20f;

    [Header("色差设置")]
    public float chromaticAberrationMin = 0.0f;
    public float chromaticAberrationMax = 0.5f;
}