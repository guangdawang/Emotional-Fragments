using UnityEngine;

/// <summary>
/// 音频管理器 - 处理游戏中的动态音频系统
/// </summary>
public class AudioManager : MonoBehaviour
{
    [Header("情绪系统引用")]
    public EmotionStateMachine emotionSystem;

    [Header("FMOD事件")]
    [FMODUnity.EventRef]
    public string backgroundMusicEvent = "";
    
    [FMODUnity.EventRef]
    public string ambientSoundEvent = "";
    
    [FMODUnity.EventRef]
    public string playerPulseEvent = "";
    
    [FMODUnity.EventRef]
    public string fragmentInteractionEvent = "";

    [Header("音频参数")]
    public string constraintParameter = "Constraint";
    public string anxietyParameter = "Anxiety";
    public string hopeParameter = "Hope";
    public string phaseParameter = "Phase";

    // FMOD事件实例
    private FMOD.Studio.EventInstance musicInstance;
    private FMOD.Studio.EventInstance ambientInstance;
    
    private bool isInitialized = false;

    private void Start()
    {
        // 获取场景中的情绪系统
        if (emotionSystem == null)
            emotionSystem = FindObjectOfType<EmotionStateMachine>();

        if (emotionSystem == null)
        {
            Debug.LogError("未找到 EmotionStateMachine 实例");
            return;
        }

        // 订阅情绪变化事件
        emotionSystem.OnEmotionChanged += OnEmotionChanged;
        emotionSystem.OnPhaseChanged += OnPhaseChanged;

        // 初始化音频系统
        InitializeAudio();
    }

    private void OnDestroy()
    {
        // 取消订阅事件
        if (emotionSystem != null)
        {
            emotionSystem.OnEmotionChanged -= OnEmotionChanged;
            emotionSystem.OnPhaseChanged -= OnPhaseChanged;
        }

        // 清理FMOD事件实例
        CleanupAudio();
    }

    /// <summary>
    /// 初始化音频系统
    /// </summary>
    private void InitializeAudio()
    {
        try
        {
            // 创建FMOD事件实例
            if (!string.IsNullOrEmpty(backgroundMusicEvent))
            {
                musicInstance = FMODUnity.RuntimeManager.CreateInstance(backgroundMusicEvent);
                musicInstance.start();
            }

            if (!string.IsNullOrEmpty(ambientSoundEvent))
            {
                ambientInstance = FMODUnity.RuntimeManager.CreateInstance(ambientSoundEvent);
                ambientInstance.start();
            }

            isInitialized = true;
            Debug.Log("音频系统初始化成功");
        }
        catch (System.Exception e)
        {
            Debug.LogError($"音频系统初始化失败: {e.Message}");
            isInitialized = false;
        }
    }

    private void Update()
    {
        // 每帧更新音频参数
        if (isInitialized)
        {
            UpdateAudioParameters();
        }
    }

    /// <summary>
    /// 更新音频参数
    /// </summary>
    private void UpdateAudioParameters()
    {
        if (emotionSystem == null) return;

        // 更新背景音乐参数
        if (musicInstance.isValid())
        {
            musicInstance.setParameterByName(constraintParameter, emotionSystem.constraint);
            musicInstance.setParameterByName(anxietyParameter, emotionSystem.anxiety);
            musicInstance.setParameterByName(hopeParameter, emotionSystem.hope);
            musicInstance.setParameterByName(phaseParameter, (float)emotionSystem.currentPhase);
        }

        // 更新环境音效参数
        if (ambientInstance.isValid())
        {
            ambientInstance.setParameterByName(constraintParameter, emotionSystem.constraint);
            ambientInstance.setParameterByName(anxietyParameter, emotionSystem.anxiety);
            ambientInstance.setParameterByName(hopeParameter, emotionSystem.hope);
        }
    }

    /// <summary>
    /// 情绪变化事件处理
    /// </summary>
    private void OnEmotionChanged(EmotionType emotionType, float value)
    {
        // 可以在这里添加特定情绪变化时的音频效果
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
    /// 阶段变化事件处理
    /// </summary>
    private void OnPhaseChanged(Phase newPhase)
    {
        Debug.Log($"音频管理器: 阶段已切换到 {newPhase}");
        
        // 阶段变化时可以触发特殊的音频效果
        PlayPhaseTransitionSound(newPhase);
    }

    /// <summary>
    /// 播放阶段转换音效
    /// </summary>
    private void PlayPhaseTransitionSound(Phase newPhase)
    {
        // 这里可以播放阶段转换的特殊音效
        // 例如，从秩序到混乱时播放破裂声，从混乱到重构时播放和谐音
        switch (newPhase)
        {
            case Phase.Order:
                // 进入秩序阶段
                break;
            case Phase.Chaos:
                // 进入混乱阶段
                break;
            case Phase.Reconstruction:
                // 进入重构阶段
                break;
        }
    }

    /// <summary>
    /// 播放玩家脉冲音效
    /// </summary>
    public void PlayPlayerPulseSound()
    {
        if (!string.IsNullOrEmpty(playerPulseEvent) && isInitialized)
        {
            FMODUnity.RuntimeManager.PlayOneShot(playerPulseEvent);
        }
    }

    /// <summary>
    /// 播放碎片交互音效
    /// </summary>
    public void PlayFragmentInteractionSound(Vector3 position)
    {
        if (!string.IsNullOrEmpty(fragmentInteractionEvent) && isInitialized)
        {
            FMODUnity.RuntimeManager.PlayOneShot(fragmentInteractionEvent, position);
        }
    }

    /// <summary>
    /// 清理音频系统
    /// </summary>
    private void CleanupAudio()
    {
        if (musicInstance.isValid())
        {
            musicInstance.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
            musicInstance.release();
        }

        if (ambientInstance.isValid())
        {
            ambientInstance.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
            ambientInstance.release();
        }
    }

    /// <summary>
    /// 设置音乐层音量
    /// </summary>
    public void SetMusicLayerVolume(string layerName, float volume)
    {
        if (musicInstance.isValid())
        {
            musicInstance.setParameterByName(layerName, volume);
        }
    }
}