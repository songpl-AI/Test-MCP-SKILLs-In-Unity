using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float jumpHeight = 2f;
    [SerializeField] private float gravity = -9.81f;

    private CharacterController _characterController;
    private Vector3 _velocity;

    private void Awake()
    {
        _characterController = GetComponent<CharacterController>();
    }

    private void Update()
    {
        // 重力处理（在地面时重置垂直速度，防止无限累积）
        if (_characterController.isGrounded && _velocity.y < 0)
        {
            _velocity.y = -2f;
        }

        Vector2 input = InputManager.Instance != null ? InputManager.Instance.GetMoveInput() : new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));

        // 获取相机方向（投影到水平面）
        Vector3 cameraForward = Camera.main.transform.forward;
        Vector3 cameraRight = Camera.main.transform.right;
        cameraForward.y = 0;
        cameraRight.y = 0;
        cameraForward.Normalize();
        cameraRight.Normalize();

        // 计算移动方向
        Vector3 move = cameraForward * input.y + cameraRight * input.x;
        
        _characterController.Move(move * moveSpeed * Time.deltaTime);

        // 跳跃逻辑
        bool jump = InputManager.Instance != null ? InputManager.Instance.GetJumpInput() : Input.GetButtonDown("Jump");
        if (jump && _characterController.isGrounded)
        {
            // v = sqrt(h * -2 * g)
            _velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
        }

        _velocity.y += gravity * Time.deltaTime;
        _characterController.Move(_velocity * Time.deltaTime);
    }
}
