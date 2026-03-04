using UnityEngine;

public class InputManager : MonoBehaviour
{
    public static InputManager Instance { get; private set; }

    [SerializeField] private bool forceMobileMode = false;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    public Vector2 GetMoveInput()
    {
        if (IsMobile())
        {
            // TODO: 实现虚拟摇杆逻辑，目前暂留接口
            return Vector2.zero; 
        }
        else
        {
            return new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
        }
    }

    public Vector2 GetLookInput()
    {
        if (IsMobile())
        {
            // TODO: 实现触屏滑动逻辑
            return Vector2.zero;
        }
        else
        {
            return new Vector2(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y"));
        }
    }

    public float GetZoomInput()
    {
        if (IsMobile())
        {
            // TODO: 实现双指捏合逻辑
            return 0f;
        }
        else
        {
            return Input.GetAxis("Mouse ScrollWheel");
        }
    }

    public bool GetJumpInput()
    {
        if (IsMobile())
        {
            // TODO: 实现 UI 按钮逻辑
            return false;
        }
        else
        {
            return Input.GetButtonDown("Jump");
        }
    }

    public bool IsRightMouseButtonHeld()
    {
        if (IsMobile())
        {
            // 移动端始终允许旋转（或者通过特定区域触摸）
            // 这里简单返回 true 模拟触摸拖拽
            return Input.touchCount > 0;
        }
        else
        {
            return Input.GetMouseButton(1);
        }
    }

    private bool IsMobile()
    {
        return forceMobileMode || Application.isMobilePlatform;
    }
}
