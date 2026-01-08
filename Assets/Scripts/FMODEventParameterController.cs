#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;

/// <summary>
/// FMOD事件参数控制器 - 用于在编辑器中测试音频参数
/// </summary>
[CustomEditor(typeof(AudioManager))]
public class FMODEventParameterController : Editor
{
    private AudioManager audioManager;
    private float constraintValue = 0f;
    private float anxietyValue = 0f;
    private float hopeValue = 0f;
    private int phaseValue = 0;

    private void OnEnable()
    {
        audioManager = (AudioManager)target;
    }

    public override void OnInspectorGUI()
    {
        // 绘制默认检查器
        DrawDefaultInspector();

        EditorGUILayout.Space();
        EditorGUILayout.LabelField("=== FMOD 参数测试 ===", EditorStyles.boldLabel);

        // 参数滑块
        constraintValue = EditorGUILayout.Slider("压抑感 (Constraint)", constraintValue, 0f, 1f);
        anxietyValue = EditorGUILayout.Slider("焦虑感 (Anxiety)", anxietyValue, 0f, 1f);
        hopeValue = EditorGUILayout.Slider("希望感 (Hope)", hopeValue, 0f, 1f);
        phaseValue = EditorGUILayout.IntSlider("阶段 (Phase)", phaseValue, 0, 2);

        EditorGUILayout.Space();

        // 测试按钮
        if (GUILayout.Button("更新音频参数"))
        {
            UpdateFMODParameters();
        }

        if (GUILayout.Button("播放测试音效"))
        {
            PlayTestSound();
        }
    }

    private void UpdateFMODParameters()
    {
        // 这里可以添加直接更新FMOD参数的代码
        // 由于我们在运行时环境中，这部分主要用于编辑器测试
        Debug.Log($"更新FMOD参数: Constraint={constraintValue}, Anxiety={anxietyValue}, Hope={hopeValue}, Phase={phaseValue}");
    }

    private void PlayTestSound()
    {
        // 播放测试音效
        if (!string.IsNullOrEmpty(audioManager.fragmentInteractionEvent))
        {
            FMODUnity.RuntimeManager.PlayOneShot(audioManager.fragmentInteractionEvent);
        }
    }
}
#endif