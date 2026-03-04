using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [SerializeField] private Transform target;
    [SerializeField] private float distance = 10.0f; // 相机距离
    [SerializeField] private float rotationSpeed = 5.0f;
    [SerializeField] private float zoomSpeed = 5.0f; // 缩放速度
    [SerializeField] private float minDistance = 2.0f; // 最小距离
    [SerializeField] private float maxDistance = 20.0f; // 最大距离
    [SerializeField] private float smoothSpeed = 0.125f; // 平滑因子
    [SerializeField] private float minPitch = -20f; // 俯仰角限制（向下看）
    [SerializeField] private float maxPitch = 80f;  // 俯仰角限制（向上看）

    private float _currentYaw;
    private float _currentPitch;

    private void Start()
    {
        if (target == null)
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player != null)
            {
                target = player.transform;
            }
        }

        // 初始化角度和距离
        if (target != null)
        {
            Vector3 direction = transform.position - target.position;
            distance = direction.magnitude;
            
            // 初始化当前角度
            Vector3 angles = transform.eulerAngles;
            _currentYaw = angles.y;
            _currentPitch = angles.x;
        }
    }

    private void LateUpdate()
    {
        if (target == null) return;

        // 旋转
        bool isRotating = InputManager.Instance != null ? InputManager.Instance.IsRightMouseButtonHeld() : Input.GetMouseButton(1);
        if (isRotating)
        {
            Vector2 lookInput = InputManager.Instance != null ? InputManager.Instance.GetLookInput() : new Vector2(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y"));
            
            _currentYaw += lookInput.x * rotationSpeed;
            _currentPitch -= lookInput.y * rotationSpeed;
            _currentPitch = Mathf.Clamp(_currentPitch, minPitch, maxPitch);
        }

        // 缩放
        float zoomInput = InputManager.Instance != null ? InputManager.Instance.GetZoomInput() : Input.GetAxis("Mouse ScrollWheel");
        if (Mathf.Abs(zoomInput) > 0.001f)
        {
            distance -= zoomInput * zoomSpeed;
            distance = Mathf.Clamp(distance, minDistance, maxDistance);
        }

        // 计算目标旋转
        Quaternion rotation = Quaternion.Euler(_currentPitch, _currentYaw, 0);
        
        // 计算目标位置：目标位置 - (旋转方向 * 距离)
        // 给目标加一个高度偏移，避免看脚底
        Vector3 targetCenter = target.position + Vector3.up * 1.5f; 
        Vector3 desiredPosition = targetCenter - (rotation * Vector3.forward * distance);

        // 平滑移动位置
        transform.position = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed);
        
        // 始终看向目标中心
        transform.LookAt(targetCenter);
    }
}
