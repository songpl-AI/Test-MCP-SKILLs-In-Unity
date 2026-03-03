---
name: "unity-csharp-best-practices"
description: "Provides guidelines and checks for writing efficient and clean C# code in Unity. Invoke when writing scripts or optimizing performance."
---

# Unity C# 最佳实践指南

本技能提供了一套在 Unity 项目中编写 C# 代码的最佳实践，重点关注性能、可维护性和项目特定的规范。

## 1. 性能优化 (高优先级)

### 1.1 Update 循环优化
- **避免高开销操作**: 禁止在 `Update`, `FixedUpdate`, 或 `LateUpdate` 中使用 `GetComponent`, `Find`, `FindObjectOfType`, 或 `Instantiate`/`Destroy`。
  - **解决方案**: 在 `Awake` 或 `Start` 中缓存引用。
- **字符串拼接**: 避免在循环中使用 `+` 进行字符串拼接。
  - **解决方案**: 使用 `System.Text.StringBuilder`。

### 1.2 内存管理与垃圾回收 (GC)
- **禁止 LINQ**: 根据项目规则，**严禁使用 LINQ** (如 `.Where`, `.Select`, `.ToList`)。它会分配内存并导致 GC 峰值。
  - **解决方案**: 使用标准的 `for` 或 `foreach` 循环。
- **对象池**: 避免频繁的 `Instantiate` 和 `Destroy`。
  - **解决方案**: 对重复使用的对象（如子弹、敌人）使用对象池。
- **结构体 vs 类**: 对小型数据容器使用 `struct` 以避免堆分配，但要注意值传递行为。

### 1.3 协程 (Coroutines)
- **缓存 YieldInstructions**: 避免在循环中使用 `yield return new WaitForSeconds(x)`。
  - **解决方案**: 将 `WaitForSeconds` 对象缓存为成员变量。

## 2. 代码结构与可维护性

### 2.1 序列化与封装
- **避免 Public 字段**: 不要仅仅为了在 Inspector 中显示而使用 `public` 字段。
  - **解决方案**: 使用 `[SerializeField] private` 将字段暴露给 Inspector，同时保持封装性。
- **Tooltip 与 Header**: 使用 `[Tooltip("说明")]` 和 `[Header("标题")]` 优化 Inspector UI。

### 2.2 组件访问
- **RequireComponent**: 使用 `[RequireComponent(typeof(Rigidbody))]` 确保依赖项存在。
- **TryGetComponent**: 检查组件是否存在时，使用 `TryGetComponent<T>(out T component)` 代替 `GetComponent<T>()`，效率更高 (Unity 2019.2+)。

### 2.3 编辑器脚本
- **位置**: 所有编辑器脚本（继承自 `Editor` 或使用 `UnityEditor` 命名空间）**必须**放置在名为 `Editor` 的文件夹（或其子文件夹）中。
- **条件编译**: 在运行时脚本中包含编辑器逻辑时，使用 `#if UNITY_EDITOR` ... `#endif`。

## 3. 项目特定规则 (Test2023MCP)

- **命名空间**: 如果项目使用命名空间，请确保脚本包含在正确的命名空间中。
- **Async/Await**: 如果未安装 `UniTask`，请使用协程处理帧相关逻辑。如果使用标准 `Task`，请注意线程问题（Unity API 非线程安全）。

## 4. 新脚本检查清单

1.  [ ] 是否在 `Awake`/`Start` 中缓存了引用？
2.  [ ] 是否避免了使用 LINQ？
3.  [ ] 是否将 `public` 字段替换为 `[SerializeField] private`？
4.  [ ] 是否在频繁字符串操作中使用了 `StringBuilder`？
5.  [ ] 编辑器脚本是否放在 `Editor` 文件夹中？

## 5. 参考资料 (References)

更多详细准则、案例研究和深度指南，请查阅 `references/` 目录下的文档：
*   **[性能优化指南](references/performance_optimization.md)**: 涵盖 CPU/GPU 优化、内存管理、UI 性能等。
*   **[编码风格与规范](references/coding_style.md)**: 详细的命名约定、代码组织结构。
