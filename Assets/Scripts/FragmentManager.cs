using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 碎片管理器 - 负责生成、管理和优化场景中的碎片
/// </summary>
public class FragmentManager : MonoBehaviour
{
    [Header("碎片预制体")]
    public GameObject fragmentPrefab;

    [Header("生成设置")]
    [Tooltip("初始生成的碎片数量")]
    public int initialFragmentCount = 50;
    
    [Tooltip("最大同时存在的碎片数量")]
    public int maxFragmentCount = 300;
    
    [Tooltip("生成区域大小")]
    public Vector3 spawnArea = new Vector3(20, 20, 20);

    [Header("阶段特定设置")]
    public PhaseSettings orderPhaseSettings;
    public PhaseSettings chaosPhaseSettings;
    public PhaseSettings reconstructionPhaseSettings;

    [Header("引用")]
    public EmotionStateMachine emotionSystem;

    // 碎片列表
    private List<GameObject> fragments = new List<GameObject>();
    private ObjectPool fragmentPool;

    private void Start()
    {
        // 获取场景中的情绪状态机实例
        if (emotionSystem == null)
            emotionSystem = FindObjectOfType<EmotionStateMachine>();

        if (emotionSystem == null)
        {
            Debug.LogError("未找到 EmotionStateMachine 实例");
            return;
        }

        // 订阅阶段变化事件
        emotionSystem.OnPhaseChanged += OnPhaseChanged;

        // 初始化对象池
        if (fragmentPrefab != null)
        {
            fragmentPool = new ObjectPool(fragmentPrefab, initialFragmentCount);
            
            // 生成初始碎片
            GenerateInitialFragments();
        }
        else
        {
            Debug.LogError("未分配碎片预制体");
        }
    }

    private void OnDestroy()
    {
        // 取消订阅事件
        if (emotionSystem != null)
            emotionSystem.OnPhaseChanged -= OnPhaseChanged;
    }

    private void Update()
    {
        // 根据情绪状态更新碎片行为
        UpdateFragmentsBehavior();
    }

    /// <summary>
    /// 生成初始碎片
    /// </summary>
    private void GenerateInitialFragments()
    {
        for (int i = 0; i < initialFragmentCount && fragments.Count < maxFragmentCount; i++)
        {
            SpawnFragment();
        }
    }

    /// <summary>
    /// 生成单个碎片
    /// </summary>
    public void SpawnFragment(Vector3 position)
    {
        if (fragments.Count >= maxFragmentCount)
            return;

        GameObject fragment = fragmentPool.GetObject();
        if (fragment != null)
        {
            fragment.SetActive(true);
            fragment.transform.position = position;
            fragment.transform.rotation = Random.rotation;

            // 获取碎片运动控制器并设置情绪系统引用
            FragmentMotionController motionController = fragment.GetComponent<FragmentMotionController>();
            if (motionController != null)
            {
                motionController.emotionSM = emotionSystem;
                motionController.SetMotionMode(GetMotionModeFromPhase(emotionSystem.currentPhase));
            }

            fragments.Add(fragment);
        }
    }

    /// <summary>
    /// 生成单个碎片（随机位置）
    /// </summary>
    public void SpawnFragment()
    {
        Vector3 spawnPosition = new Vector3(
            Random.Range(-spawnArea.x / 2, spawnArea.x / 2),
            Random.Range(-spawnArea.y / 2, spawnArea.y / 2),
            Random.Range(-spawnArea.z / 2, spawnArea.z / 2)
        );
        SpawnFragment(spawnPosition);
    }

    /// <summary>
    /// 根据当前阶段确定碎片运动模式
    /// </summary>
    private MotionMode GetMotionModeFromPhase(Phase phase)
    {
        switch (phase)
        {
            case Phase.Order:
                return MotionMode.Ordered;
            case Phase.Chaos:
                return MotionMode.Chaotic;
            case Phase.Reconstruction:
                return MotionMode.Attractive;
            default:
                return MotionMode.Ordered;
        }
    }

    /// <summary>
    /// 更新碎片行为
    /// </summary>
    private void UpdateFragmentsBehavior()
    {
        // 更新所有碎片的运动模式以匹配当前阶段
        MotionMode targetMode = GetMotionModeFromPhase(emotionSystem.currentPhase);
        
        foreach (GameObject fragment in fragments)
        {
            if (fragment != null && fragment.activeInHierarchy)
            {
                FragmentMotionController motionController = fragment.GetComponent<FragmentMotionController>();
                if (motionController != null && motionController.currentMode != targetMode)
                {
                    motionController.SetMotionMode(targetMode);
                }
            }
        }
    }

    /// <summary>
    /// 移除碎片
    /// </summary>
    public void RemoveFragment(GameObject fragment)
    {
        if (fragments.Contains(fragment))
        {
            fragments.Remove(fragment);
            fragment.SetActive(false);
            fragmentPool.ReturnObject(fragment);
        }
    }

    /// <summary>
    /// 阶段变化处理
    /// </summary>
    private void OnPhaseChanged(Phase newPhase)
    {
        Debug.Log($"阶段已切换到: {newPhase}");
        
        // 根据新阶段调整碎片行为
        switch (newPhase)
        {
            case Phase.Order:
                ApplyPhaseSettings(orderPhaseSettings);
                break;
            case Phase.Chaos:
                ApplyPhaseSettings(chaosPhaseSettings);
                break;
            case Phase.Reconstruction:
                ApplyPhaseSettings(reconstructionPhaseSettings);
                break;
        }
    }

    /// <summary>
    /// 应用阶段设置
    /// </summary>
    private void ApplyPhaseSettings(PhaseSettings settings)
    {
        if (settings.spawnNewFragments)
        {
            // 生成额外碎片
            int fragmentsToSpawn = settings.additionalFragmentsToSpawn;
            for (int i = 0; i < fragmentsToSpawn; i++)
            {
                SpawnFragment();
            }
        }
    }

    private void OnDrawGizmosSelected()
    {
        // 在场景视图中可视化生成区域
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireCube(transform.position, spawnArea);
    }
}

[System.Serializable]
public class PhaseSettings
{
    [Tooltip("是否在进入此阶段时生成新碎片")]
    public bool spawnNewFragments = false;
    
    [Tooltip("进入此阶段时额外生成的碎片数量")]
    public int additionalFragmentsToSpawn = 0;
}

/// <summary>
/// 简单对象池系统
/// </summary>
public class ObjectPool
{
    private GameObject prefab;
    private Queue<GameObject> pooledObjects;
    private int initialSize;

    public ObjectPool(GameObject prefab, int initialSize)
    {
        this.prefab = prefab;
        this.initialSize = initialSize;
        this.pooledObjects = new Queue<GameObject>();
        
        // 预先生成对象
        for (int i = 0; i < initialSize; i++)
        {
            GameObject obj = GameObject.Instantiate(prefab);
            obj.SetActive(false);
            pooledObjects.Enqueue(obj);
        }
    }

    public GameObject GetObject()
    {
        if (pooledObjects.Count > 0)
        {
            return pooledObjects.Dequeue();
        }
        else
        {
            // 如果池为空，则创建新对象
            return GameObject.Instantiate(prefab);
        }
    }

    public void ReturnObject(GameObject obj)
    {
        obj.SetActive(false);
        pooledObjects.Enqueue(obj);
    }
}