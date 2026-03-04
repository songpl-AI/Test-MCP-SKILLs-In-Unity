# TPS Character Controller 最佳实践

本文件记录了在 Unity 中实现高质量第三人称射击/视角（TPS）角色控制系统的核心模式和代码片段。

## 1. 架构设计 (Architecture)

推荐采用 **InputManager - PlayerMovement - CameraFollow** 三层解耦架构：

*   **InputManager**: 统一处理所有输入（PC/Mobile/Gamepad），对上层屏蔽具体设备差异。
*   **PlayerMovement**: 负责角色的物理移动、跳跃、重力处理。
*   **CameraFollow**: 负责相机的跟随、旋转、缩放。

## 2. 核心代码片段 (Code Snippets)

### 2.1 基于相机视角的移动 (Relative Movement)
**问题**: 按 W 键角色应该朝相机前方移动，而不是世界坐标的 Z 轴。
**解决方案**: 将输入向量投影到相机视角的水平面上。

```csharp
// 获取相机方向（忽略 Y 轴，只取水平方向）
Vector3 cameraForward = Camera.main.transform.forward;
Vector3 cameraRight = Camera.main.transform.right;
cameraForward.y = 0;
cameraRight.y = 0;
cameraForward.Normalize();
cameraRight.Normalize();

// 将输入转换为世界坐标移动方向
Vector3 move = cameraForward * input.y + cameraRight * input.x;
_characterController.Move(move * moveSpeed * Time.deltaTime);
```

### 2.2 自由视角相机 (Free Look Camera)
**特性**: 支持围绕目标球形旋转、限制俯仰角、平滑跟随、滚轮缩放。

```csharp
// 旋转计算 (LateUpdate)
_currentYaw += Input.GetAxis("Mouse X") * rotationSpeed;
_currentPitch -= Input.GetAxis("Mouse Y") * rotationSpeed; // 注意 Y 轴通常是反向的
_currentPitch = Mathf.Clamp(_currentPitch, minPitch, maxPitch); // 限制俯仰角

// 缩放计算
distance -= Input.GetAxis("Mouse ScrollWheel") * zoomSpeed;
distance = Mathf.Clamp(distance, minDistance, maxDistance);

// 位置计算
Quaternion rotation = Quaternion.Euler(_currentPitch, _currentYaw, 0);
// 目标中心点偏移（看向头部而不是脚底）
Vector3 targetCenter = target.position + Vector3.up * 1.5f;
Vector3 desiredPosition = targetCenter - (rotation * Vector3.forward * distance);

// 平滑移动
transform.position = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed);
transform.LookAt(targetCenter);
```

### 2.3 跳跃与重力 (Jump & Gravity)
**物理公式**: `v = sqrt(h * -2 * g)`，其中 `h` 是期望跳跃高度。

```csharp
// 跳跃
if (jumpInput && _characterController.isGrounded)
{
    _velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
}

// 重力应用
_velocity.y += gravity * Time.deltaTime;
_characterController.Move(_velocity * Time.deltaTime);

// 地面重置（防止重力无限累积）
if (_characterController.isGrounded && _velocity.y < 0)
{
    _velocity.y = -2f; // 给一个小向下的力确保贴地
}
```

## 3. 常见坑与注意事项 (Pitfalls)

*   **InputManager 单例**: 必须确保场景切换时不丢失，或在每个场景重新初始化。推荐使用 `DontDestroyOnLoad` 或依赖注入。
*   **LateUpdate**: 相机跟随必须放在 `LateUpdate`，以确保在目标移动（`Update`）完成后才更新相机位置，避免抖动。
*   **CharacterController vs Rigidbody**: 对于非物理强交互的角色（如简单的跑跳），推荐 `CharacterController`，控制感更好且无须处理摩擦力等物理细节。
*   **移动端适配**: `InputManager` 应预留虚拟摇杆和触屏滑动的接口，避免在业务逻辑层写 `if (Application.isMobilePlatform)`。
