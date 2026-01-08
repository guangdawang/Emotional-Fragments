using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

/// <summary>
/// 视觉反馈系统 - 根据情绪状态调整场景视觉效果
/// </summary>
public class VisualFeedbackSystem : MonoBehaviour
{
    [Header("情绪系统引用")]
    public EmotionStateMachine emotionSystem;

    [Header("后期处理")]
    public Volume postProcessVolume;
    private Bloom bloom;
    private Vignette vignette;
    private ColorAdjustments colorAdjustments;

    [Header("灯光控制")]
    public Light directionalLight;
    private Material skyMaterial;

    [Header("颜色渐变")]
    [Tooltip("压抑阶段的颜色")]
    public Color orderColor = new Color(0.17f, 0.24f, 0.31f); // #2C3E50 深蓝
    
    [Tooltip("混乱阶段的颜色")]
    public Color chaosColor = new Color(0.91f, 0.23f, 0.17f); // #E74C3C 红色
    
    [Tooltip("重构阶段的颜色")]
    public Color reconstructionColor = new Color(0.18f, 0.8f, 0.44f); // #2ECC71 绿色

    private void Start()
    {
        // 获取场景中的情绪状态机实例
        if (emotionSystem == null)
            emotionSystem = FindObjectOfType<EmotionStateMachine>();

        if (emotionSystem == null)
            Debug.LogError("未找到 EmotionStateMachine 实例");

        // 初始化后期处理效果
        InitializePostProcessing();

        // 获取主方向光
        if (directionalLight == null)
            directionalLight = FindObjectOfType<Light>();
    }

    private void InitializePostProcessing()
    {
        if (postProcessVolume == null)
        {
            // 尝试获取场景中的Volume组件
            postProcessVolume = FindObjectOfType<Volume>();
        }

        if (postProcessVolume != null)
        {
            // 获取或创建后期处理效果
            postProcessVolume.profile.TryGet(out bloom);
            if (bloom == null)
            {
                bloom = postProcessVolume.profile.Add<Bloom>(true);
            }

            postProcessVolume.profile.TryGet(out vignette);
            if (vignette == null)
            {
                vignette = postProcessVolume.profile.Add<Vignette>(true);
            }

            postProcessVolume.profile.TryGet(out colorAdjustments);
            if (colorAdjustments == null)
            {
                colorAdjustments = postProcessVolume.profile.Add<ColorAdjustments>(true);
            }
        }
        else
        {
            Debug.LogWarning("未找到后期处理Volume，部分视觉效果将不可用");
        }
    }

    private void Update()
    {
        // 根据情绪状态更新视觉效果
        UpdateVisualEffects();
    }

    private void UpdateVisualEffects()
    {
        if (emotionSystem == null) return;

        // 插值计算当前阶段的混合权重
        float transitionProgress = 0f;
        Color targetColor = orderColor;

        switch (emotionSystem.currentPhase)
        {
            case Phase.Order:
                targetColor = orderColor;
                transitionProgress = emotionSystem.constraint;
                break;
            case Phase.Chaos:
                targetColor = Color.Lerp(orderColor, chaosColor, emotionSystem.anxiety);
                transitionProgress = emotionSystem.anxiety;
                break;
            case Phase.Reconstruction:
                targetColor = Color.Lerp(chaosColor, reconstructionColor, emotionSystem.hope);
                transitionProgress = emotionSystem.hope;
                break;
        }

        // 更新后期处理效果
        UpdatePostProcessing(targetColor, transitionProgress);

        // 更新灯光
        UpdateLighting(targetColor, transitionProgress);
    }

    private void UpdatePostProcessing(Color targetColor, float transitionProgress)
    {
        if (bloom != null)
        {
            // 根据希望感调整辉光效果
            bloom.intensity.value = Mathf.Lerp(0.5f, 2.0f, emotionSystem.hope);
            bloom.threshold.value = Mathf.Lerp(1.0f, 0.7f, emotionSystem.anxiety);
        }

        if (vignette != null)
        {
            // 根据压抑感调整暗角效果
            vignette.intensity.value = Mathf.Lerp(0.2f, 0.5f, emotionSystem.constraint);
            vignette.smoothness.value = Mathf.Lerp(0.3f, 0.6f, emotionSystem.hope);
        }

        if (colorAdjustments != null)
        {
            // 根据情绪状态调整颜色
            colorAdjustments.colorFilter.value = targetColor;
            colorAdjustments.saturation.value = Mathf.Lerp(-20f, 20f, emotionSystem.hope);
            colorAdjustments.contrast.value = Mathf.Lerp(10f, 20f, emotionSystem.anxiety);
        }
    }

    private void UpdateLighting(Color targetColor, float transitionProgress)
    {
        if (directionalLight != null)
        {
            // 根据情绪状态调整灯光颜色和强度
            directionalLight.color = targetColor;
            directionalLight.intensity = Mathf.Lerp(0.8f, 1.5f, emotionSystem.hope);
        }
    }

    // 在检视面板中可视化颜色
    private void OnValidate()
    {
        // 仅在编辑器中执行
#if UNITY_EDITOR
        UpdateEditorPreview();
#endif
    }

#if UNITY_EDITOR
    private void UpdateEditorPreview()
    {
        // 这里可以添加编辑器预览逻辑
    }
#endif
}