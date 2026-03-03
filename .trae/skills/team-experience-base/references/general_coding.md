# 通用编码避坑指南 (General Coding Pitfalls)

本文件记录了在本项目（及 Unity 开发中）常见的编码陷阱和最佳实践。

## 1. 性能相关 (Performance)

### 1.1 禁止在热代码路径使用 LINQ
*   **问题描述**: 在 `Update`、`FixedUpdate` 或高频调用的方法中使用 `.Where()`, `.Select()`, `.ToList()` 等 LINQ 方法。
*   **原因分析**: LINQ 会产生额外的垃圾回收 (GC) 开销，导致帧率波动。
*   **解决方案**: 使用标准的 `for` 或 `foreach` 循环代替。
*   **示例**:
    ```csharp
    // ❌ 错误示范
    var enemies = allObjects.Where(x => x.tag == "Enemy").ToList();

    // ✅ 正确示范
    List<GameObject> enemies = new List<GameObject>();
    foreach (var obj in allObjects) {
        if (obj.CompareTag("Enemy")) {
            enemies.Add(obj);
        }
    }
    ```

### 1.2 避免频繁的 GetComponent 和 Find
*   **问题描述**: 在 `Update` 中调用 `GetComponent<T>()` 或 `GameObject.Find()`.
*   **解决方案**: 在 `Awake` 或 `Start` 中缓存引用。

### 1.3 字符串拼接
*   **问题描述**: 在循环或高频调用中使用 `+` 进行字符串拼接。
*   **解决方案**: 使用 `StringBuilder`。

## 2. Unity API 陷阱 (Unity API Traps)

### 2.1 比较 Tag
*   **问题描述**: 使用 `obj.tag == "Player"`。
*   **原因分析**: 会产生字符串分配。
*   **解决方案**: 使用 `obj.CompareTag("Player")`。

### 2.2 Transform 修改
*   **问题描述**: 频繁修改 `transform.position` 和 `transform.rotation`。
*   **原因分析**: 每次修改都会触发生理/渲染系统的同步。
*   **解决方案**: 如果需要同时修改位置和旋转，使用 `transform.SetPositionAndRotation()`。

## 3. 异步与协程 (Async & Coroutines)

### 3.1 协程中的死循环
*   **问题描述**: `while(true)` 循环中没有 `yield return`。
*   **后果**: 导致 Unity 编辑器卡死。
*   **解决方案**: 确保循环内部至少有一个 `yield return null` 或其他等待指令。

### 3.2 异步 void
*   **问题描述**: 使用 `async void` 而不是 `async Task`（除了事件处理器）。
*   **原因分析**: `async void` 发生的异常无法被调用者捕获，可能导致程序崩溃。
*   **解决方案**: 尽可能使用 `async Task` 或 `UniTask`。

## 4. 常见异常 (Common Exceptions)

### 4.1 NullReferenceException
*   **常见原因**:
    *   脚本执行顺序问题（在 A 的 `Awake` 中访问 B，但 B 还没初始化）。
    *   对象被销毁后仍然访问。
*   **预防**:
    *   使用 `TryGetComponent`。
    *   检查 `if (obj != null)`（注意 Unity Object 的 null check 有性能开销，但在逻辑关键处是必要的）。
