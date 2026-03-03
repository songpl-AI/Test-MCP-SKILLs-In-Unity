using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float gravity = -9.81f;

    private CharacterController _characterController;
    private Vector3 _velocity;

    private void Awake()
    {
        _characterController = GetComponent<CharacterController>();
    }

    private void Update()
    {
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");

        // 使用 Transform 的方向来移动，这样移动是相对于玩家朝向的
        // 如果想要绝对方向移动，可以使用 new Vector3(horizontal, 0, vertical)
        // 这里为了简单起见，使用绝对方向，因为 Capsule 默认没有旋转控制
        Vector3 move = new Vector3(horizontal, 0, vertical);
        
        _characterController.Move(move * moveSpeed * Time.deltaTime);

        // 重力处理
        if (_characterController.isGrounded && _velocity.y < 0)
        {
            _velocity.y = -2f;
        }

        _velocity.y += gravity * Time.deltaTime;
        _characterController.Move(_velocity * Time.deltaTime);
    }
}
