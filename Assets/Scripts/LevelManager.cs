using System.Collections;
using UnityEngine;

/// <summary>
/// 关卡管理器 - 管理不同阶段的关卡布局和对象
/// </summary>
public class LevelManager : MonoBehaviour
{
    [Header("情绪系统引用")]
    public EmotionStateMachine emotionSystem;

    [Header("关卡设置")]
    public LevelSettings orderLevel;
    public LevelSettings chaosLevel;
    public LevelSettings reconstructionLevel;

    [Header("过渡设置")]
    public float phaseTransitionDuration = 3f;

    private GameObject currentLevelObject;
    private Coroutine transitionCoroutine;

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

        // 订阅阶段变化事件
        emotionSystem.OnPhaseChanged += OnPhaseChanged;

        // 初始化第一个关卡
        InitializeLevel(Phase.Order);
    }

    private void OnDestroy()
    {
        // 取消订阅事件
        if (emotionSystem != null)
        {
            emotionSystem.OnPhaseChanged -= OnPhaseChanged;
        }
    }

    /// <summary>
    /// 阶段变化处理
    /// </summary>
    private void OnPhaseChanged(Phase newPhase)
    {
        // 开始关卡过渡
        if (transitionCoroutine != null)
            StopCoroutine(transitionCoroutine);

        transitionCoroutine = StartCoroutine(TransitionToLevel(newPhase));
    }

    /// <summary>
    /// 过渡到新关卡
    /// </summary>
    private IEnumerator TransitionToLevel(Phase newPhase)
    {
        // 淡出当前关卡
        if (currentLevelObject != null)
        {
            yield return StartCoroutine(FadeOutLevel(currentLevelObject, phaseTransitionDuration / 2));
        }

        // 初始化新关卡
        InitializeLevel(newPhase);

        // 淡入新关卡
        if (currentLevelObject != null)
        {
            yield return StartCoroutine(FadeInLevel(currentLevelObject, phaseTransitionDuration / 2));
        }

        transitionCoroutine = null;
    }

    /// <summary>
    /// 初始化关卡
    /// </summary>
    private void InitializeLevel(Phase phase)
    {
        // 清理当前关卡
        if (currentLevelObject != null)
        {
            Destroy(currentLevelObject);
        }

        // 根据阶段创建关卡
        LevelSettings settings = GetLevelSettings(phase);
        currentLevelObject = new GameObject($"{phase} Level");
        currentLevelObject.transform.SetParent(transform);

        // 创建关卡环境
        CreateLevelEnvironment(settings);

        // 创建关卡对象
        CreateLevelObjects(settings);

        Debug.Log($"关卡已初始化: {phase}");
    }

    /// <summary>
    /// 创建关卡环境
    /// </summary>
    private void CreateLevelEnvironment(LevelSettings settings)
    {
        if (currentLevelObject == null) return;

        // 创建天空盒或环境对象
        GameObject environment = new GameObject("Environment");
        environment.transform.SetParent(currentLevelObject.transform);

        // 根据设置应用环境效果
        switch (settings.environmentType)
        {
            case EnvironmentType.Geometric:
                CreateGeometricEnvironment(environment, settings);
                break;
            case EnvironmentType.Chaotic:
                CreateChaoticEnvironment(environment, settings);
                break;
            case EnvironmentType.Organic:
                CreateOrganicEnvironment(environment, settings);
                break;
        }
    }

    /// <summary>
    /// 创建几何环境（秩序阶段）
    /// </summary>
    private void CreateGeometricEnvironment(GameObject parent, LevelSettings settings)
    {
        // 创建基本几何形状作为环境
        for (int i = 0; i < settings.environmentObjectCount; i++)
        {
            GameObject shape = GameObject.CreatePrimitive(PrimitiveType.Cube);
            shape.transform.SetParent(parent.transform);
            
            // 随机位置和大小
            shape.transform.position = new Vector3(
                Random.Range(-settings.environmentSpread, settings.environmentSpread),
                Random.Range(-settings.environmentSpread/2, settings.environmentSpread/2),
                Random.Range(-settings.environmentSpread, settings.environmentSpread)
            );
            
            float scale = Random.Range(settings.minObjectScale, settings.maxObjectScale);
            shape.transform.localScale = Vector3.one * scale;
            
            // 添加视觉组件
            Renderer renderer = shape.GetComponent<Renderer>();
            if (renderer != null && settings.environmentMaterial != null)
            {
                renderer.material = settings.environmentMaterial;
            }
        }
    }

    /// <summary>
    /// 创建混沌环境（混乱阶段）
    /// </summary>
    private void CreateChaoticEnvironment(GameObject parent, LevelSettings settings)
    {
        // 创建破碎的几何形状
        for (int i = 0; i < settings.environmentObjectCount; i++)
        {
            GameObject shape = GameObject.CreatePrimitive(PrimitiveType.Cube);
            shape.transform.SetParent(parent.transform);
            
            // 随机位置和旋转
            shape.transform.position = new Vector3(
                Random.Range(-settings.environmentSpread, settings.environmentSpread),
                Random.Range(-settings.environmentSpread/2, settings.environmentSpread/2),
                Random.Range(-settings.environmentSpread, settings.environmentSpread)
            );
            
            shape.transform.rotation = Random.rotation;
            
            float scale = Random.Range(settings.minObjectScale, settings.maxObjectScale);
            shape.transform.localScale = Vector3.one * scale;
            
            // 添加视觉组件
            Renderer renderer = shape.GetComponent<Renderer>();
            if (renderer != null && settings.environmentMaterial != null)
            {
                renderer.material = settings.environmentMaterial;
            }
        }
    }

    /// <summary>
    /// 创建有机环境（重构阶段）
    /// </summary>
    private void CreateOrganicEnvironment(GameObject parent, LevelSettings settings)
    {
        // 创建球体和胶囊体模拟有机形态
        for (int i = 0; i < settings.environmentObjectCount; i++)
        {
            GameObject shape;
            if (Random.value > 0.5f)
            {
                shape = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            }
            else
            {
                shape = GameObject.CreatePrimitive(PrimitiveType.Capsule);
            }
            
            shape.transform.SetParent(parent.transform);
            
            // 随机位置
            shape.transform.position = new Vector3(
                Random.Range(-settings.environmentSpread, settings.environmentSpread),
                Random.Range(-settings.environmentSpread/2, settings.environmentSpread/2),
                Random.Range(-settings.environmentSpread, settings.environmentSpread)
            );
            
            // 随机缩放
            float scale = Random.Range(settings.minObjectScale, settings.maxObjectScale);
            shape.transform.localScale = new Vector3(scale, scale, scale);
            
            // 添加视觉组件
            Renderer renderer = shape.GetComponent<Renderer>();
            if (renderer != null && settings.environmentMaterial != null)
            {
                renderer.material = settings.environmentMaterial;
            }
        }
    }

    /// <summary>
    /// 创建关卡对象
    /// </summary>
    private void CreateLevelObjects(LevelSettings settings)
    {
        if (currentLevelObject == null) return;

        // 创建秩序核心（仅在秩序阶段）
        if (settings.hasOrderCore)
        {
            GameObject core = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            core.name = "Order Core";
            core.tag = "OrderCore";
            core.transform.SetParent(currentLevelObject.transform);
            core.transform.position = settings.orderCorePosition;
            core.transform.localScale = Vector3.one * settings.orderCoreScale;

            // 添加发光材质
            Renderer renderer = core.GetComponent<Renderer>();
            if (renderer != null && settings.coreMaterial != null)
            {
                renderer.material = settings.coreMaterial;
            }

            // 添加光源
            Light coreLight = core.AddComponent<Light>();
            coreLight.type = LightType.Point;
            coreLight.color = Color.blue;
            coreLight.intensity = 2f;
            coreLight.range = 5f;
        }

        // 创建共鸣点（在重构阶段）
        if (settings.resonancePointCount > 0)
        {
            for (int i = 0; i < settings.resonancePointCount; i++)
            {
                GameObject point = new GameObject($"Resonance Point {i+1}");
                point.tag = "ResonancePoint";
                point.transform.SetParent(currentLevelObject.transform);
                
                // 随机位置
                point.transform.position = new Vector3(
                    Random.Range(-settings.resonancePointSpread, settings.resonancePointSpread),
                    Random.Range(-settings.resonancePointSpread/2, settings.resonancePointSpread/2),
                    Random.Range(-settings.resonancePointSpread, settings.resonancePointSpread)
                );

                // 添加共鸣点组件
                ResonancePoint rp = point.AddComponent<ResonancePoint>();
                rp.emotionSystem = emotionSystem;
                
                // 添加可视化对象
                GameObject visual = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                visual.name = "Visual";
                visual.transform.SetParent(point.transform);
                visual.transform.localPosition = Vector3.zero;
                visual.transform.localScale = Vector3.one * 0.5f;
                
                // 添加光源
                Light pointLight = point.AddComponent<Light>();
                pointLight.type = LightType.Point;
                pointLight.range = 3f;
                pointLight.intensity = 1f;
            }
        }
    }

    /// <summary>
    /// 获取关卡设置
    /// </summary>
    private LevelSettings GetLevelSettings(Phase phase)
    {
        switch (phase)
        {
            case Phase.Order:
                return orderLevel;
            case Phase.Chaos:
                return chaosLevel;
            case Phase.Reconstruction:
                return reconstructionLevel;
            default:
                return orderLevel;
        }
    }

    /// <summary>
    /// 淡出关卡
    /// </summary>
    private IEnumerator FadeOutLevel(GameObject level, float duration)
    {
        // 这里可以实现淡出效果
        float elapsedTime = 0f;
        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            float alpha = 1f - (elapsedTime / duration);
            // 应用透明度或其他淡出效果
            yield return null;
        }
    }

    /// <summary>
    /// 淡入关卡
    /// </summary>
    private IEnumerator FadeInLevel(GameObject level, float duration)
    {
        // 这里可以实现淡入效果
        float elapsedTime = 0f;
        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            float alpha = elapsedTime / duration;
            // 应用透明度或其他淡入效果
            yield return null;
        }
    }
}

[System.Serializable]
public class LevelSettings
{
    [Header("环境设置")]
    public EnvironmentType environmentType;
    public Material environmentMaterial;
    public Material coreMaterial;
    public int environmentObjectCount = 10;
    public float environmentSpread = 20f;
    public float minObjectScale = 1f;
    public float maxObjectScale = 5f;

    [Header("特殊对象")]
    public bool hasOrderCore = false;
    public Vector3 orderCorePosition = Vector3.zero;
    public float orderCoreScale = 2f;

    public int resonancePointCount = 0;
    public float resonancePointSpread = 15f;
}

public enum EnvironmentType
{
    Geometric,  // 几何环境（秩序阶段）
    Chaotic,    // 混沌环境（混乱阶段）
    Organic     // 有机环境（重构阶段）
}