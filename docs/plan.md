# 需求分析与开发计划：Capsule 移动与相机跟随

## 1. 目标
在当前 Unity 场景中创建一个 Capsule，实现 WASD 移动功能，并使相机跟随该 Capsule。

## 2. 需求分析
*   **主体**: Capsule (3D Primitive)。
*   **功能 1 (移动)**:
    *   输入方式: WASD 键盘输入。
    *   移动方式: 前后左右平移。
    *   组件: 建议使用 `CharacterController` 以处理基本的碰撞和移动，或者简单的 `Transform` 修改（如果不需要物理）。为了通用性，将使用 `CharacterController`。
*   **功能 2 (相机跟随)**:
    *   相机: 场景中的 Main Camera。
    *   逻辑: 保持与 Capsule 的固定距离，平滑跟随。
*   **约束**:
    *   禁止使用 LINQ。
    *   遵循 Unity 最佳实践（缓存组件引用，不频繁 Find）。
    *   使用 `OpenMCP` 进行场景操作。

## 3. 方案设计

### 3.1 脚本设计
*   **PlayerMovement.cs**:
    *   属性: `speed` (移动速度)。
    *   组件依赖: `CharacterController`。
    *   逻辑: 在 `Update` 中读取 Input，调用 `SimpleMove` 或 `Move`。
*   **CameraFollow.cs**:
    *   属性: `target` (Transform), `smoothSpeed` (float), `offset` (Vector3)。
    *   逻辑: 在 `LateUpdate` 中更新相机位置，使用 `Vector3.Lerp` 实现平滑跟随。

### 3.2 场景操作流程 (OpenMCP)
1.  创建 Capsule (`GameObject.CreatePrimitive`).
2.  重命名为 "PlayerCapsule".
3.  添加 `PlayerMovement` 组件（会自动添加 `CharacterController` 依赖，如果脚本中有 `RequireComponent`）。
4.  获取 Main Camera。
5.  添加 `CameraFollow` 组件。
6.  设置 `CameraFollow` 的 `target` 为 "PlayerCapsule"。

## 4. 验收标准
- [ ] 场景中出现一个 Capsule。
- [ ] 按 W/S 键，Capsule 前后移动。
- [ ] 按 A/D 键，Capsule 左右移动。
- [ ] 移动时，相机跟随 Capsule 移动，保持相对静止或平滑过渡。
- [ ] 代码中无 LINQ，无性能警告。
