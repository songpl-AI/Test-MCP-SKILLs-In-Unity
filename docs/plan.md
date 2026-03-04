# 需求分析与开发计划：Capsule 移动与相机跟随

## 1. 目标
在当前 Unity 场景中创建一个 Capsule，实现 WASD 移动功能，并使相机跟随该 Capsule。
[新增] 增加跳跃功能。
[新增] 增加鼠标右键旋转相机功能（第三人称视角，支持水平和垂直旋转）。
[新增] 修复移动方向问题，支持相机缩放，区分 PC/Mobile 输入。

## 2. 需求分析
*   **主体**: Capsule (3D Primitive)。
*   **功能 1 (移动)**:
    *   输入方式: WASD 键盘输入 (PC) / 虚拟摇杆 (Mobile)。
    *   **[修正] 移动方向**: 必须相对于相机视角（按 W 始终朝相机前方移动）。
    *   组件: `CharacterController`。
*   **功能 2 (相机跟随 & 旋转)**:
    *   逻辑: 保持与 Capsule 的固定距离，平滑跟随。
    *   旋转: 右键拖拽 (PC) / 触屏拖拽 (Mobile)。
    *   **[新增] 缩放**: 鼠标滚轮 (PC) / 双指捏合 (Mobile) 控制相机距离。
    *   参数: `distance` (范围限制), `zoomSpeed`。
*   **功能 3 (跳跃)**:
    *   输入方式: 空格键 (PC) / UI 按钮 (Mobile)。
    *   逻辑: 仅在地面上可跳跃。
*   **功能 4 (多平台输入)**:
    *   抽象输入层: 区分 PC 和 Mobile。
    *   PC: 键盘 + 鼠标。
    *   Mobile: 屏幕触摸 (虚拟摇杆 + 划屏)。
    *   当前实现: 优先完善 PC 逻辑，Mobile 逻辑预留接口或简单实现（视当前资源情况，本计划优先保证 PC 逻辑正确并架构上支持扩展）。

## 3. 方案设计

### 3.1 脚本设计
*   **InputManager.cs (新增)**:
    *   单例或静态类，负责统一获取输入。
    *   `Vector2 GetMoveInput()`: 返回 WASD 或摇杆向量。
    *   `Vector2 GetLookInput()`: 返回鼠标移动或触屏滑动向量。
    *   `float GetZoomInput()`: 返回滚轮或捏合增量。
    *   `bool GetJumpInput()`: 返回跳跃指令。
*   **PlayerMovement.cs**:
    *   **[修改]**: 在 `Update` 中，通过 `InputManager` 获取输入。
    *   **[修改]**: 计算移动向量时，使用 `Camera.main.transform` 将输入向量转换为世界坐标系下的移动方向（`cameraForward * input.y + cameraRight * input.x`）。
*   **CameraFollow.cs**:
    *   **[修改]**: 在 `LateUpdate` 中，通过 `InputManager` 获取旋转和缩放输入。
    *   **[修改]**: 实现缩放逻辑 (`distance` 变化)。

### 3.2 场景操作流程
1.  创建 `InputManager` 脚本并挂载（或作为单例）。
2.  更新 `PlayerMovement` 和 `CameraFollow` 脚本。

## 4. 验收标准
- [x] 场景中出现一个 Capsule。
- [ ] **[修正]** 旋转相机后，按 W 键角色朝相机正前方移动，而不是世界坐标 Z 轴。
- [ ] **[新增]** 滚动鼠标滚轮，相机距离拉近/拉远。
- [ ] **[新增]** 代码结构上区分 PC 和 Mobile 输入逻辑。
- [x] 移动时，相机跟随 Capsule 移动。
- [x] 按空格键，Capsule 跳跃。
- [x] 按住鼠标右键并移动鼠标，相机围绕 Capsule 旋转。
- [ ] 代码中无 LINQ，无性能警告。
