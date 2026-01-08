using UnityEngine;
using System;

/// <summary>
/// 情绪状态机 - 核心情感架构控制器
/// 实现项目文档中定义的"压抑→混乱→重构"情感曲线
/// </summary>
public class EmotionStateMachine : MonoBehaviour
{
    // 情绪变化事件
    public event Action<EmotionType, float> OnEmotionChanged;
    public event Action<Phase> OnPhaseChanged;

    // 核心情绪维度 (0.0-1.0)
    [Header("当前情绪值")]
    [Range(0f, 1f)] public float constraint;      // 压抑/束缚感
    [Range(0f, 1f)] public float anxiety;        // 焦虑/混乱感
    [Range(0f, 1f)] public float agency;         // 控制/能动感
    [Range(0f, 1f)] public float hope;           // 希望/成长感

    [Header("阶段设置")]
    public Phase currentPhase;

    // 情绪变化速率
    [Header("情绪变化设置")]
    public float emotionDecayRate = 0.05f;
    public float emotionChangeSmoothing = 0.1f;

    // 内部变量用于平滑过渡
    private float targetConstraint;
    private float targetAnxiety;
    private float targetAgency;
    private float targetHope;

    private void Start()
    {
        currentPhase = Phase.Order;
        targetConstraint = constraint;
        targetAnxiety = anxiety;
        targetAgency = agency;
        targetHope = hope;
    }

    private void Update()
    {
        // 平滑过渡到目标情绪值
        constraint = Mathf.Lerp(constraint, targetConstraint, emotionChangeSmoothing);
        anxiety = Mathf.Lerp(anxiety, targetAnxiety, emotionChangeSmoothing);
        agency = Mathf.Lerp(agency, targetAgency, emotionChangeSmoothing);
        hope = Mathf.Lerp(hope, targetHope, emotionChangeSmoothing);
    }

    /// <summary>
    /// 处理玩家行为并更新情绪状态
    /// </summary>
    public void ProcessPlayerAction(ActionType action, float intensity = 1.0f)
    {
        // 保存变更前的阶段
        Phase oldPhase = currentPhase;

        switch (currentPhase)
        {
            case Phase.Order:
                HandleOrderPhase(action, intensity);
                break;
            case Phase.Chaos:
                HandleChaosPhase(action, intensity);
                break;
            case Phase.Reconstruction:
                HandleReconstructionPhase(action, intensity);
                break;
        }

        // 检查阶段转换条件
        CheckPhaseTransition();

        // 情绪自然衰减（避免永久累积）
        ApplyEmotionalDecay(intensity);

        // 触发情绪变化事件
        if (oldPhase != currentPhase)
        {
            OnPhaseChanged?.Invoke(currentPhase);
        }
    }

    private void HandleOrderPhase(ActionType action, float intensity)
    {
        switch (action)
        {
            case ActionType.MoveWithEffort:
                targetConstraint += 0.02f * intensity;
                break;
            case ActionType.AttemptCreation:
                targetHope += 0.01f * intensity;
                break;
            case ActionType.TouchCore:
                // 触发阶段转换到混乱阶段
                currentPhase = Phase.Chaos;
                targetAnxiety += 0.5f * intensity;
                break;
        }

        // 限制情绪值在0-1范围内
        targetConstraint = Mathf.Clamp01(targetConstraint);
        targetHope = Mathf.Clamp01(targetHope);
    }

    private void HandleChaosPhase(ActionType action, float intensity)
    {
        switch (action)
        {
            case ActionType.UsePulse:
                targetAgency += 0.03f * intensity;
                break;
            case ActionType.AvoidCollision:
                targetAnxiety += 0.02f * intensity;
                break;
        }

        // 限制情绪值在0-1范围内
        targetAgency = Mathf.Clamp01(targetAgency);
        targetAnxiety = Mathf.Clamp01(targetAnxiety);
    }

    private void HandleReconstructionPhase(ActionType action, float intensity)
    {
        switch (action)
        {
            case ActionType.PlaceFragment:
                targetHope += 0.05f * intensity;
                break;
            case ActionType.CreatePattern:
                targetAgency += 0.04f * intensity;
                break;
        }

        // 限制情绪值在0-1范围内
        targetHope = Mathf.Clamp01(targetHope);
        targetAgency = Mathf.Clamp01(targetAgency);
    }

    private void CheckPhaseTransition()
    {
        // 阶段转换逻辑
        switch (currentPhase)
        {
            case Phase.Order:
                // 当接触到秩序核心时转换到混乱阶段
                // 这个转换已经在HandleOrderPhase中通过TouchCore动作处理
                break;

            case Phase.Chaos:
                // 当焦虑值足够高时转换到重构阶段
                if (anxiety > 0.7f && agency > 0.5f)
                {
                    currentPhase = Phase.Reconstruction;
                }
                break;

            case Phase.Reconstruction:
                // 重构阶段是最终阶段，不会自动转换
                break;
        }
    }

    private void ApplyEmotionalDecay(float intensity)
    {
        float decay = emotionDecayRate * Time.deltaTime * intensity;
        targetConstraint = Mathf.Max(0f, targetConstraint - decay);
        targetAnxiety = Mathf.Max(0f, targetAnxiety - decay);
        targetAgency = Mathf.Max(0f, targetAgency - decay);
        targetHope = Mathf.Max(0f, targetHope - decay);
    }

    /// <summary>
    /// 获取当前主导情绪
    /// </summary>
    public EmotionType GetDominantEmotion()
    {
        float maxEmotion = Mathf.Max(constraint, anxiety, agency, hope);

        if (maxEmotion == constraint) return EmotionType.Constraint;
        if (maxEmotion == anxiety) return EmotionType.Anxiety;
        if (maxEmotion == agency) return EmotionType.Agency;
        return EmotionType.Hope;
    }

    /// <summary>
    /// 获取情绪值
    /// </summary>
    public float GetEmotionValue(EmotionType emotionType)
    {
        switch (emotionType)
        {
            case EmotionType.Constraint: return constraint;
            case EmotionType.Anxiety: return anxiety;
            case EmotionType.Agency: return agency;
            case EmotionType.Hope: return hope;
            default: return 0f;
        }
    }
}

/// <summary>
/// 游戏阶段枚举
/// </summary>
public enum Phase
{
    Order,           // 秩序阶段 - 压抑
    Chaos,           // 混乱阶段 - 焦虑
    Reconstruction   // 重构阶段 - 希望
}

/// <summary>
/// 玩家行为类型
/// </summary>
public enum ActionType
{
    MoveWithEffort,
    AttemptCreation,
    UsePulse,
    AvoidCollision,
    PlaceFragment,
    CreatePattern,
    TouchCore
}

/// <summary>
/// 情绪类型枚举
/// </summary>
public enum EmotionType
{
    Constraint,  // 压抑
    Anxiety,     // 焦虑
    Agency,      // 能动性
    Hope         // 希望
}