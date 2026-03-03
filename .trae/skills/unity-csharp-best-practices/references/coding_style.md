# Unity C# 编码风格与规范

本文档定义了项目中 C# 代码的格式、命名和结构标准。遵循一致的风格有助于提高代码可读性和团队协作效率。

## 1. 命名约定 (Naming Conventions)

### 1.1 大小写规则
*   **PascalCase** (大驼峰): 用于 `class`, `struct`, `enum`, `delegate`, `event`, `method`, `property`, `namespace`。
    ```csharp
    public class PlayerController : MonoBehaviour {
        public void TakeDamage(int amount) { ... }
    }
    ```
*   **camelCase** (小驼峰): 用于局部变量、方法参数。
    ```csharp
    void CalculateScore(int currentScore) {
        int bonusPoints = 10;
    }
    ```

### 1.2 字段命名 (Fields)
*   **私有字段 (Private Fields)**: 使用 `_` 前缀 + camelCase。这有助于快速区分成员变量和局部变量。
    ```csharp
    [SerializeField] private int _health;
    private float _moveSpeed;
    ```
*   **公有字段 (Public Fields)**: 尽量避免使用公有字段（改用属性）。如果必须使用，使用 PascalCase。
*   **常量 (Constants)**: 使用 PascalCase 或 全大写（SNAKE_CASE，仅限特定情况）。
    ```csharp
    public const int MaxPlayers = 10;
    ```

### 1.3 接口 (Interfaces)
*   使用 `I` 前缀。
    ```csharp
    public interface IDamageable { ... }
    ```

## 2. 代码格式化 (Formatting)

### 2.1 括号 (Braces)
*   **Allman 风格** (推荐) 或 **K&R 风格**。项目中应保持统一。Unity 官方通常倾向于 **Allman** (括号独占一行)。
    ```csharp
    // Allman Style
    if (condition)
    {
        DoSomething();
    }
    ```

### 2.2 缩进 (Indentation)
*   使用 **4个空格** 或 **1个 Tab**。确保编辑器设置一致（推荐使用 `.editorconfig`）。

### 2.3 命名空间 (Namespaces)
*   所有脚本应包含在项目根命名空间下，例如 `Test2023MCP.Gameplay`。
*   避免在命名空间内缩进（取决于团队偏好，但减少缩进通常更好）。

## 3. 注释规范 (Comments)

### 3.1 方法与类
*   使用 XML 文档注释 `///` 为公开的 API、复杂的算法和类提供说明。
    ```csharp
    /// <summary>
    /// Calculates the final damage based on armor and penetration.
    /// </summary>
    /// <param name="rawDamage">Base incoming damage.</param>
    /// <returns>Final damage applied to health.</returns>
    public int CalculateDamage(int rawDamage) { ... }
    ```

### 3.2 代码内部
*   使用 `//` 进行行内注释。
*   注释应解释 **"为什么"** (Why) 而不是 "是什么" (What)。代码本身应该清晰到不需要解释 "是什么"。

## 4. 组织结构 (Organization)

### 4.1 文件结构
*   每个文件只包含一个主要类（除非是紧密相关的轻量级类/结构体）。
*   文件名必须与类名完全一致。

### 4.2 成员顺序
推荐顺序：
1.  Nested Enums / Structs / Classes
2.  Constants / Static Fields
3.  Fields (Inspector / Private)
4.  Events / Delegates
5.  Properties
6.  Unity Messages (Awake, Start, Update...)
7.  Public Methods
8.  Private Methods
