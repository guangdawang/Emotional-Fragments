using UnityEngine;

/// <summary>
/// 情绪系统测试器 - 演示如何在运行时操作情绪状态机
/// </summary>
public class EmotionSystemTester : MonoBehaviour
{
    [Header("情绪系统引用")]
    public EmotionStateMachine emotionSystem;

    [Header("测试参数")]
    public KeyCode increaseConstraintKey = KeyCode.Alpha1;
    public KeyCode increaseAnxietyKey = KeyCode.Alpha2;
    public KeyCode increaseAgencyKey = KeyCode.Alpha3;
    public KeyCode increaseHopeKey = KeyCode.Alpha4;
    public KeyCode switchToChaosPhaseKey = KeyCode.F1;
    public KeyCode switchToReconstructionPhaseKey = KeyCode.F2;

    private void Start()
    {
        // 获取场景中的情绪状态机实例
        if (emotionSystem == null)
            emotionSystem = FindObjectOfType<EmotionStateMachine>();

        if (emotionSystem == null)
            Debug.LogError("未找到 EmotionStateMachine 实例，请确保场景中有挂载该脚本的对象");
    }

    private void Update()
    {
        // 测试按键输入以改变情绪状态
        if (Input.GetKeyDown(increaseConstraintKey))
        {
            emotionSystem.ProcessPlayerAction(ActionType.MoveWithEffort);
            Debug.Log($"[测试] 压抑感增加，当前值: {emotionSystem.constraint:F2}");
        }

        if (Input.GetKeyDown(increaseAnxietyKey))
        {
            emotionSystem.ProcessPlayerAction(ActionType.AvoidCollision);
            Debug.Log($"[测试] 焦虑感增加，当前值: {emotionSystem.anxiety:F2}");
        }

        if (Input.GetKeyDown(increaseAgencyKey))
        {
            emotionSystem.ProcessPlayerAction(ActionType.UsePulse);
            Debug.Log($"[测试] 能动感增加，当前值: {emotionSystem.agency:F2}");
        }

        if (Input.GetKeyDown(increaseHopeKey))
        {
            emotionSystem.ProcessPlayerAction(ActionType.PlaceFragment);
            Debug.Log($"[测试] 希望感增加，当前值: {emotionSystem.hope:F2}");
        }

        // 切换阶段
        if (Input.GetKeyDown(switchToChaosPhaseKey))
        {
            emotionSystem.currentPhase = Phase.Chaos;
            Debug.Log("[测试] 切换到混乱阶段");
        }

        if (Input.GetKeyDown(switchToReconstructionPhaseKey))
        {
            emotionSystem.currentPhase = Phase.Reconstruction;
            Debug.Log("[测试] 切换到重构阶段");
        }
    }

    private void OnGUI()
    {
        // 在屏幕上显示当前情绪状态
        GUIStyle labelStyle = new GUIStyle(GUI.skin.label);
        labelStyle.fontSize = 16;
        labelStyle.normal.textColor = Color.white;

        GUILayout.BeginArea(new Rect(10, 10, 300, 300));
        GUILayout.Label($"当前阶段: {emotionSystem.currentPhase}", labelStyle);
        GUILayout.Space(10);
        GUILayout.Label($"压抑感 (1): {emotionSystem.constraint:F2}", labelStyle);
        GUILayout.Label($"焦虑感 (2): {emotionSystem.anxiety:F2}", labelStyle);
        GUILayout.Label($"能动感 (3): {emotionSystem.agency:F2}", labelStyle);
        GUILayout.Label($"希望感 (4): {emotionSystem.hope:F2}", labelStyle);
        GUILayout.Space(10);
        GUILayout.Label("按键说明:", labelStyle);
        GUILayout.Label("F1 - 切换到混乱阶段", labelStyle);
        GUILayout.Label("F2 - 切换到重构阶段", labelStyle);
        GUILayout.EndArea();
    }
}