using UnityEngine;

/// <summary>
/// 玩家控制器 - 处理玩家输入并与情绪系统交互
/// </summary>
public class PlayerController : MonoBehaviour
{
    [Header("移动设置")]
    public float moveSpeed = 5f;
    public float mouseSensitivity = 100f;

    [Header("情绪系统引用")]
    public EmotionStateMachine emotionSystem;
    public FragmentManager fragmentManager;
    public AudioManager audioManager;

    [Header("能力设置")]
    public float pulseForce = 10f;
    public float pulseRadius = 5f;
    public float pulseCooldown = 1f;

    private Camera playerCamera;
    private float nextPulseTime;

    private void Start()
    {
        // 获取组件
        playerCamera = GetComponentInChildren<Camera>();
        
        // 锁定并隐藏光标
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        // 获取场景中的系统实例
        if (emotionSystem == null)
            emotionSystem = FindObjectOfType<EmotionStateMachine>();

        if (fragmentManager == null)
            fragmentManager = FindObjectOfType<FragmentManager>();
            
        if (audioManager == null)
            audioManager = FindObjectOfType<AudioManager>();

        if (emotionSystem == null)
            Debug.LogError("未找到 EmotionStateMachine 实例");
    }

    private void Update()
    {
        // 处理移动输入
        HandleMovement();

        // 处理视角输入
        HandleLook();

        // 处理能力输入
        HandleAbilities();
    }

    private void HandleMovement()
    {
        float moveHorizontal = Input.GetAxis("Horizontal");
        float moveVertical = Input.GetAxis("Vertical");

        Vector3 movement = transform.right * moveHorizontal + transform.forward * moveVertical;
        movement.y = 0f; // 保持在水平面上移动

        transform.Translate(movement.normalized * moveSpeed * Time.deltaTime, Space.World);
    }

    private void HandleLook()
    {
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;

        // 水平旋转玩家身体
        transform.Rotate(Vector3.up * mouseX);

        // 垂直旋转摄像机
        if (playerCamera != null)
        {
            Vector3 currentRotation = playerCamera.transform.localRotation.eulerAngles;
            float newRotationX = currentRotation.x - mouseY;
            
            // 限制垂直视角在-90到90度之间
            if (newRotationX > 180) newRotationX -= 360;
            newRotationX = Mathf.Clamp(newRotationX, -90f, 90f);
            
            playerCamera.transform.localRotation = Quaternion.Euler(newRotationX, 0f, 0f);
        }
    }

    private void HandleAbilities()
    {
        // 脉冲能力
        if (Input.GetButtonDown("Fire1") && Time.time >= nextPulseTime)
        {
            PerformPulseAbility();
            nextPulseTime = Time.time + pulseCooldown;
        }

        // 与秩序核心交互
        if (Input.GetButtonDown("Fire2"))
        {
            PerformInteraction();
        }
    }

    private void PerformPulseAbility()
    {
        if (emotionSystem == null) return;

        // 发送情绪事件
        emotionSystem.ProcessPlayerAction(ActionType.UsePulse);

        // 物理脉冲效果
        Collider[] colliders = Physics.OverlapSphere(transform.position, pulseRadius);
        foreach (Collider collider in colliders)
        {
            FragmentMotionController fragment = collider.GetComponent<FragmentMotionController>();
            if (fragment != null)
            {
                Vector3 forceDirection = (collider.transform.position - transform.position).normalized;
                // 力的大小受距离影响
                float distance = Vector3.Distance(transform.position, collider.transform.position);
                float forceMagnitude = pulseForce / (distance + 1f);
                
                fragment.ApplyPulseForce(forceDirection * forceMagnitude);
            }
        }

        // 播放脉冲音效
        if (audioManager != null)
        {
            audioManager.PlayPlayerPulseSound();
        }

        Debug.Log("脉冲能力已释放");
    }

    private void PerformInteraction()
    {
        if (emotionSystem == null) return;

        // 射线检测交互对象
        Ray ray = new Ray(playerCamera.transform.position, playerCamera.transform.forward);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, 10f))
        {
            // 检查是否击中秩序核心
            if (hit.collider.CompareTag("OrderCore"))
            {
                emotionSystem.ProcessPlayerAction(ActionType.TouchCore);
                Debug.Log("触碰秩序核心");
            }
            // 检查是否击中碎片
            else if (hit.collider.GetComponent<FragmentMotionController>() != null)
            {
                // 根据当前阶段执行不同操作
                switch (emotionSystem.currentPhase)
                {
                    case Phase.Order:
                        emotionSystem.ProcessPlayerAction(ActionType.AttemptCreation);
                        Debug.Log("尝试与碎片交互（秩序阶段）");
                        break;
                    case Phase.Chaos:
                        emotionSystem.ProcessPlayerAction(ActionType.AvoidCollision);
                        Debug.Log("避开碎片（混乱阶段）");
                        break;
                    case Phase.Reconstruction:
                        // 在重构阶段，可以放置碎片
                        emotionSystem.ProcessPlayerAction(ActionType.PlaceFragment);
                        
                        // 在击中点生成新碎片
                        if (fragmentManager != null)
                        {
                            fragmentManager.SpawnFragment(hit.point + hit.normal * 0.5f);
                        }
                        Debug.Log("放置碎片（重构阶段）");
                        break;
                }
                
                // 播放碎片交互音效
                if (audioManager != null)
                {
                    audioManager.PlayFragmentInteractionSound(hit.point);
                }
            }
        }
    }

    private void OnDrawGizmosSelected()
    {
        // 绘制脉冲范围 Gizmo
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, pulseRadius);
    }
}