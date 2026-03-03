# Unity 性能优化深度指南

本文档作为 `unity-csharp-best-practices` 技能的详细参考，旨在沉淀项目中遇到的性能优化问题与解决方案。

## 1. 脚本优化 (CPU)

### 1.1 Update 循环
*   **原则**: 避免在 `Update` 中执行每帧不必要的计算。
*   **缓存**: 所有的 `GetComponent`, `Find`, `FindObjectOfType`, `GameObject.Find` 必须在 `Awake` 或 `Start` 中缓存。
*   **频率控制**: 如果某些逻辑不需要每帧运行（如 AI 决策、UI 刷新），使用计时器每隔几帧运行一次，或使用协程。
    ```csharp
    // 错误示例
    void Update() { CheckStatus(); } // 每帧检查

    // 优化示例
    private float _timer;
    void Update() {
        _timer += Time.deltaTime;
        if (_timer > 0.5f) { // 每 0.5 秒检查一次
            CheckStatus();
            _timer = 0f;
        }
    }
    ```

### 1.2 字符串与日志
*   **字符串拼接**: `string + string` 会产生大量临时内存分配。在循环中必须使用 `StringBuilder`。
*   **Debug.Log**: 发布版本必须剔除 `Debug.Log`，因为即便是空的 Log 也会消耗字符串构建和堆栈跟踪的开销。使用 `[Conditional("ENABLE_LOGS")]` 封装日志类。

### 1.3 物理系统
*   **射线检测 (Raycast)**:
    *   使用 `Physics.RaycastNonAlloc` 替代 `Physics.RaycastAll`，避免产生 GC。
    *   指定 LayerMask，避免检测不必要的层。
*   **碰撞体**: 避免使用 Mesh Collider，除非绝对必要。优先使用 Box/Sphere/Capsule Collider。
*   **刚体**: 静态物体不要挂 Rigidbody。移动的物体如果不需要物理模拟，勾选 `IsKinematic`。

## 2. 内存管理 (Memory & GC)

### 2.1 垃圾回收 (Garbage Collection)
*   **LINQ**: **严禁使用**。LINQ 会产生闭包和迭代器分配，在热路径中是性能杀手。
*   **Foreach**: Unity 5.5 以前版本的 `foreach` 会产生装箱操作（Boxing），虽然新版本已修复，但为了保险起见，对于 `List<T>` 建议使用 `for` 循环，对于 `Array` 使用 `for` 循环。
*   **装箱 (Boxing)**: 避免值类型（int, float, struct）转换为引用类型（object, interface）。例如 `string.Format("Score: {0}", score)` 会导致 `score` 装箱。

### 2.2 资源管理
*   **纹理**: 确保纹理压缩格式正确（Android: ASTC, iOS: ASTC/PVRTC）。非 UI 纹理必须开启 Mipmaps。
*   **音频**: 长音频使用 `Streaming`，短音效使用 `Decompress On Load`。
*   **AssetBundles**: 卸载不再使用的资源 `AssetBundle.Unload(true)`。

## 3. 渲染优化 (GPU)

### 3.1 Draw Calls (批处理)
*   **Static Batching**: 静态物体（如建筑、地形）必须标记为 `Static`。
*   **GPU Instancing**: 大量相同的物体（如草地、树木、子弹）使用相同的材质球，并勾选材质球上的 `Enable GPU Instancing`。
*   **Dynamic Batching**: 限制较多（顶点数限制），现代移动设备上通常不如 Instancing 有效。

### 3.2 Overdraw (过度绘制)
*   **UI**: 避免 UI 元素重叠。全屏 UI 背景如果是不透明的，应该遮挡住背后的 3D 场景渲染（如果是 3D UI）。
*   **粒子系统**: 减少粒子数量和屏幕覆盖面积。使用 `Texture Sheet Animation` 减少粒子数。

## 4. UI 性能 (UGUI)

*   **Canvas 分离**: 将动态更新的 UI 元素（如血条、倒计时）与静态 UI 元素（如背景、图标）分在不同的 Canvas 中。因为 Canvas 下任何一个元素变化都会导致整个 Canvas Rebuild。
*   **Raycast Target**: 默认创建的 Image/Text 都会勾选 `Raycast Target`。对于不需要交互的元素（如背景图、纯展示文字），**必须取消勾选**，以减少事件系统的遍历开销。
*   **Layout Groups**: `Horizontal/Vertical Layout Group` 性能开销较大，尽量少用，或在布局完成后禁用组件。

## 5. 项目特有检查项 (Test2023MCP)
*   **OpenMCP**: 确保通过 OpenMCP 生成的物体不包含多余的组件。
*   **资源审计**: 提交代码前，运行 `unity-resource-auditor` 检查资源设置。
