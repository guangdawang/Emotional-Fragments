using UnityEngine;

/// <summary>
/// 碎片运动控制器 - 实现混合物理行为
/// 支持三种模式：有序、混沌、吸引
/// </summary>
public class FragmentMotionController : MonoBehaviour
{
    [Header("组件引用")]
    public Rigidbody rb;
    public EmotionStateMachine emotionSM;

    [Header("运动参数")]
    public MotionMode currentMode = MotionMode.Ordered;
    [Range(0.1f, 5f)] public float orderedSpeed = 0.5f;
    public Vector3 orderedDirection = Vector3.right;
    [Range(0.1f, 5f)] public float chaosNoiseScale = 1.5f;
    [Range(1f, 50f)] public float attractionForce = 10f;
    [Range(0f, 1f)] public float stabilityInfluence = 0.5f;

    private Vector3[] lorenzAttractorPoints;
    private int attractorIndex;
    private Vector3 velocity;
    private float noiseOffset;

    private void Start()
    {
        if (rb == null) rb = GetComponent<Rigidbody>();
        GenerateLorenzAttractor();
        noiseOffset = Random.value * 1000f; // 为每个碎片添加随机偏移
    }

    private void FixedUpdate()
    {
        Vector3 baseForce = Vector3.zero;

        switch (currentMode)
        {
            case MotionMode.Ordered:
                baseForce = CalculateOrderedMotion();
                break;
            
            case MotionMode.Chaotic:
                baseForce = CalculateChaoticMotion();
                // 脉冲响应由外部系统触发
                break;
            
            case MotionMode.Attractive:
                Transform resonancePoint = FindNearestResonancePoint();
                if (resonancePoint != null)
                {
                    baseForce = CalculateAttraction(resonancePoint.position);
                }
                break;
        }

        // 应用情绪稳定性修饰
        float stability = 1.0f - emotionSM.anxiety * stabilityInfluence;
        rb.AddForce(baseForce * stability, ForceMode.Force);
        
        // 添加少量阻尼防止速度无限增长
        rb.velocity *= 0.98f;
    }

    private Vector3 CalculateOrderedMotion()
    {
        // 有序运动：沿固定方向匀速移动，略微受到情绪影响
        float speedModifier = 1.0f + emotionSM.constraint * 0.5f;
        return orderedDirection.normalized * orderedSpeed * speedModifier;
    }

    private Vector3 CalculateChaoticMotion()
    {
        // 混沌运动：使用洛伦兹吸引子路径，受到焦虑情绪影响
        if (lorenzAttractorPoints.Length > 0)
        {
            Vector3 target = lorenzAttractorPoints[attractorIndex];
            attractorIndex = (attractorIndex + 1) % lorenzAttractorPoints.Length;
            
            // 添加噪声扰动，受焦虑情绪影响
            float noiseInfluence = emotionSM.anxiety * 0.5f;
            Vector3 noise = new Vector3(
                Mathf.PerlinNoise(noiseOffset + Time.time, 0f),
                Mathf.PerlinNoise(0f, noiseOffset + Time.time),
                Mathf.PerlinNoise(noiseOffset + Time.time, noiseOffset + Time.time)
            ) * noiseInfluence;
            
            return (target - transform.position).normalized * chaosNoiseScale + noise;
        }
        return Random.onUnitSphere * chaosNoiseScale;
    }

    private Vector3 CalculateAttraction(Vector3 targetPosition)
    {
        Vector3 direction = (targetPosition - transform.position).normalized;
        float distance = Vector3.Distance(transform.position, targetPosition);
        
        // 距离越近力越小，防止过度摆动
        float force = attractionForce / (distance + 1);
        
        // 受希望情绪影响，希望越高吸引力越强
        force *= (1.0f + emotionSM.hope);
        
        return direction * force;
    }

    private Transform FindNearestResonancePoint()
    {
        // 查找标记为“共鸣点”的对象
        GameObject[] points = GameObject.FindGameObjectsWithTag("ResonancePoint");
        Transform closest = null;
        float closestDist = Mathf.Infinity;

        foreach (GameObject point in points)
        {
            float dist = Vector3.Distance(transform.position, point.transform.position);
            if (dist < closestDist)
            {
                closestDist = dist;
                closest = point.transform;
            }
        }

        return closest;
    }

    /// <summary>
    /// 生成洛伦兹吸引子轨迹点
    /// </summary>
    private void GenerateLorenzAttractor()
    {
        const int count = 1000;
        lorenzAttractorPoints = new Vector3[count];
        Vector3 pos = new Vector3(0.1f, 0, 0);
        float dt = 0.01f;
        float x, y, z;

        for (int i = 0; i < count; i++)
        {
            x = pos.x; y = pos.y; z = pos.z;
            float dx = 10 * (y - x);
            float dy = x * (28 - z) - y;
            float dz = x * y - 8f/3f * z;

            pos += new Vector3(dx, dy, dz) * dt;
            lorenzAttractorPoints[i] = pos * 0.2f; // 缩放适应场景
        }
    }

    /// <summary>
    /// 接收脉冲力
    /// </summary>
    public void ApplyPulseForce(Vector3 force)
    {
        if (rb != null)
        {
            rb.AddForce(force, ForceMode.Impulse);
        }
    }

    /// <summary>
    /// 设置运动模式
    /// </summary>
    public void SetMotionMode(MotionMode mode)
    {
        currentMode = mode;
    }
}

/// <summary>
/// 运动模式枚举
/// </summary>
public enum MotionMode
{
    Ordered,
    Chaotic,
    Attractive
}