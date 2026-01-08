using UnityEngine;

/// <summary>
/// 情绪材质更新器 - 根据情绪状态实时更新碎片材质
/// </summary>
public class EmotionMaterialUpdater : MonoBehaviour
{
    [Header("情绪系统引用")]
    public EmotionStateMachine emotionSystem;

    [Header("材质设置")]
    public Material fragmentMaterial;
    public string constraintInfluenceProperty = "_ConstraintInfluence";
    public string anxietyInfluenceProperty = "_AnxietyInfluence";
    public string hopeInfluenceProperty = "_HopeInfluence";
    public string emissiveIntensityProperty = "_EmissiveIntensity";

    [Header("视觉效果设置")]
    public float emissiveMultiplier = 2.0f;
    public float smoothnessMultiplier = 1.0f;

    private int constraintInfluenceID;
    private int anxietyInfluenceID;
    private int hopeInfluenceID;
    private int emissiveIntensityID;

    private void Start()
    {
        // 获取属性ID以提高性能
        constraintInfluenceID = Shader.PropertyToID(constraintInfluenceProperty);
        anxietyInfluenceID = Shader.PropertyToID(anxietyInfluenceProperty);
        hopeInfluenceID = Shader.PropertyToID(hopeInfluenceProperty);
        emissiveIntensityID = Shader.PropertyToID(emissiveIntensityProperty);

        // 获取场景中的情绪系统
        if (emotionSystem == null)
            emotionSystem = FindObjectOfType<EmotionStateMachine>();

        if (emotionSystem == null)
            Debug.LogError("未找到 EmotionStateMachine 实例");

        // 订阅情绪变化事件
        if (emotionSystem != null)
        {
            emotionSystem.OnEmotionChanged += OnEmotionChanged;
        }
    }

    private void OnDestroy()
    {
        // 取消订阅事件
        if (emotionSystem != null)
        {
            emotionSystem.OnEmotionChanged -= OnEmotionChanged;
        }
    }

    private void Update()
    {
        // 每帧更新材质属性
        UpdateMaterialProperties();
    }

    /// <summary>
    /// 更新材质属性
    /// </summary>
    private void UpdateMaterialProperties()
    {
        if (emotionSystem == null || fragmentMaterial == null)
            return;

        // 根据情绪值更新材质属性
        fragmentMaterial.SetFloat(constraintInfluenceID, emotionSystem.constraint);
        fragmentMaterial.SetFloat(anxietyInfluenceID, emotionSystem.anxiety);
        fragmentMaterial.SetFloat(hopeInfluenceID, emotionSystem.hope);

        // 根据希望值调整自发光强度
        float emissiveIntensity = emotionSystem.hope * emissiveMultiplier;
        fragmentMaterial.SetFloat(emissiveIntensityID, emissiveIntensity);

        // 可以根据需要添加更多材质属性的更新
    }

    /// <summary>
    /// 情绪变化事件处理
    /// </summary>
    private void OnEmotionChanged(EmotionType emotionType, float value)
    {
        // 可以在这里添加特定情绪变化时的特殊处理
        switch (emotionType)
        {
            case EmotionType.Constraint:
                // 压抑感变化时的处理
                break;
            case EmotionType.Anxiety:
                // 焦虑感变化时的处理
                break;
            case EmotionType.Agency:
                // 能动感变化时的处理
                break;
            case EmotionType.Hope:
                // 希望感变化时的处理
                break;
        }
    }

    /// <summary>
    /// 应用材质到所有碎片
    /// </summary>
    public void ApplyMaterialToFragments()
    {
        FragmentMotionController[] fragments = FindObjectsOfType<FragmentMotionController>();
        foreach (var fragment in fragments)
        {
            Renderer renderer = fragment.GetComponent<Renderer>();
            if (renderer != null && fragmentMaterial != null)
            {
                renderer.material = fragmentMaterial;
            }
        }
    }
}