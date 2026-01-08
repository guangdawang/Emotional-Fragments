using UnityEngine;

/// <summary>
/// 共鸣点 - 标记碎片在重构阶段应该被吸引的位置
/// </summary>
public class ResonancePoint : MonoBehaviour
{
    [Header("视觉效果")]
    public GameObject visualEffect;
    public Color inactiveColor = Color.gray;
    public Color activeColor = Color.green;
    
    [Header("状态")]
    public bool isActive = false;
    public float activationRadius = 2f;
    
    [Header("引用")]
    public EmotionStateMachine emotionSystem;
    
    private Renderer pointRenderer;
    private Light pointLight;
    private ParticleSystem particles;

    private void Start()
    {
        // 获取组件
        pointRenderer = GetComponent<Renderer>();
        pointLight = GetComponent<Light>();
        particles = GetComponent<ParticleSystem>();
        
        // 获取情绪系统
        if (emotionSystem == null)
            emotionSystem = FindObjectOfType<EmotionStateMachine>();
        
        // 设置初始状态
        UpdateVisualState();
    }

    private void Update()
    {
        // 检查是否应该激活
        CheckActivationCondition();
        
        // 更新视觉效果
        UpdateVisualEffects();
    }

    private void CheckActivationCondition()
    {
        // 在重构阶段且希望值较高时激活
        if (emotionSystem != null && emotionSystem.currentPhase == Phase.Reconstruction)
        {
            if (emotionSystem.hope > 0.5f)
            {
                isActive = true;
            }
        }
        else
        {
            isActive = false;
        }
    }

    private void UpdateVisualState()
    {
        // 更新材质颜色
        if (pointRenderer != null && pointRenderer.material.HasProperty("_Color"))
        {
            pointRenderer.material.color = isActive ? activeColor : inactiveColor;
        }

        // 更新灯光
        if (pointLight != null)
        {
            pointLight.enabled = isActive;
            pointLight.color = activeColor;
        }

        // 更新粒子系统
        if (particles != null)
        {
            if (isActive && !particles.isPlaying)
            {
                particles.Play();
            }
            else if (!isActive && particles.isPlaying)
            {
                particles.Stop();
            }
        }
    }

    private void UpdateVisualEffects()
    {
        if (emotionSystem == null) return;

        // 根据希望值调整视觉效果强度
        if (isActive)
        {
            float hopeInfluence = emotionSystem.hope;
            
            if (pointLight != null)
            {
                pointLight.intensity = 1f + hopeInfluence * 2f;
                pointLight.range = 2f + hopeInfluence * 3f;
            }
            
            // 颜色在激活色和希望影响之间插值
            if (pointRenderer != null && pointRenderer.material.HasProperty("_Color"))
            {
                Color blendedColor = Color.Lerp(inactiveColor, activeColor, hopeInfluence);
                pointRenderer.material.color = blendedColor;
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        // 当碎片进入共鸣点时触发效果
        if (other.CompareTag("Fragment") && isActive)
        {
            // 通知情绪系统
            if (emotionSystem != null)
            {
                emotionSystem.ProcessPlayerAction(ActionType.PlaceFragment, 0.5f);
            }
            
            // 可以在这里添加碎片"融合"效果
            Debug.Log("碎片与共鸣点融合");
        }
    }

    private void OnDrawGizmosSelected()
    {
        // 绘制激活半径 Gizmo
        Gizmos.color = isActive ? Color.green : Color.gray;
        Gizmos.DrawWireSphere(transform.position, activationRadius);
    }
}