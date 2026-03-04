---
name: OpenMCP
description: 通过 HTTP 接口自动化控制 Unity Editor（创建物体、组件、场景管理等）。采用【源码驱动】策略，通过读取项目中的 OpenMCP 源码动态发现 API 和参数，而非依赖预定义列表。
---

# OpenMCP - Unity Editor Automation

此 Skill 用于通过 HTTP 接口直接控制 Unity Editor。
OpenMCP 提供了丰富的 API（目前 30+ 个），且处于快速迭代中。为了确保准确性并支持新功能，**必须**采用“源码驱动”的方式来发现和使用 API。

## 核心原则：源码即文档 (Source as Truth)

**不要**依赖任何硬编码的 API 列表。
**必须**在使用前通过读取项目中的 OpenMCP 源码来获取最新的路由、参数和逻辑。

## 执行步骤

当用户请求执行 Unity 操作（如“创建物体”、“修改组件”、“查询场景”）时，请按以下步骤操作：

### 1. 发现路由 (Discover Routes)
首先，定位并读取 `RequestRouter.cs` 文件，这是所有 API 的入口。
*   **搜索目标**：`RequestRouter.cs`
*   **路径特征**：`Packages/**/Editor/Core/RequestRouter.cs`
*   **行动**：读取文件，查找 `router.Register(...)` 调用。
*   **目的**：获取 HTTP Method (GET/POST) 和 URL Path (如 `/api/v1/gameobject/create`)。

### 2. 解析参数 (Analyze Parameters)
找到路由对应的 **Handler** 类，读取其源码以确定请求参数。
*   **搜索目标**：Handler 类文件（如 `GameObjectHandler.cs`, `SceneHandler.cs`）。
*   **路径特征**：`Packages/**/Editor/Handlers/*.cs`
*   **行动**：
    *   读取 Handler 方法（如 `HandleCreate`）。
    *   查找 `ctx.ParseBody<T>()` 以确定 JSON Body 结构。
    *   查找 `ctx.Query("key")` 以确定 URL Query 参数。
    *   查找 `ctx.PathParams` 以确定路径参数（如 `:id`）。

### 3. 构造请求 (Execute Request)
根据分析结果，构建并执行 `curl` 命令。
*   **Base URL**: `http://localhost:23456` (默认) 或检查 `HttpServer.cs` 确认端口。
*   **Headers**: `Content-Type: application/json` (对于 POST 请求)。
*   **Command**: `curl -s -X <METHOD> "<URL>" -d '<JSON>'`

## 常用文件索引

为了快速定位，以下是 OpenMCP 的核心文件结构（请使用 `Glob` 或 `SearchCodebase` 确认实际路径）：

*   **路由表**: `Editor/Core/RequestRouter.cs`
*   **业务逻辑**: `Editor/Handlers/`
    *   `GameObjectHandler.cs` (物体增删改查、组件操作)
    *   `SceneHandler.cs` (场景切换、保存、层级查询)
    *   `AssetHandler.cs` (资源导入、创建脚本/材质)
    *   `EditorHandler.cs` (播放、暂停、编译、撤销)
    *   `ConsoleHandler.cs` (日志获取、清理)
    *   (以及更多新增加的 Handler...)

## 示例流程

**任务**：用户想要 "给 Cube 添加一个 BoxCollider"。

1.  **AI 思考**："我需要找到添加组件的 API。"
2.  **查找路由**：读取 `RequestRouter.cs`，发现：
    ```csharp
    router.Register("POST", "/api/v1/gameobject/:path/component/add", goHandler.HandleAddComponent);
    ```
3.  **解析参数**：读取 `GameObjectHandler.cs` -> `HandleAddComponent`，发现：
    ```csharp
    var req = ctx.ParseBody<ComponentTypeRequest>();
    // ComponentTypeRequest 包含 { public string ComponentType; }
    ```
4.  **执行命令**：
    ```bash
    curl -X POST "http://localhost:23456/api/v1/gameobject/Cube/component/add" \
      -H "Content-Type: application/json" \
      -d '{"ComponentType": "BoxCollider"}'
    ```

## 常见问题排查 (Troubleshooting)

### Q: 为什么会出现 `curl: (7) Failed to connect to localhost port 23456`？
- **原因**:
    1.  **Unity Editor 未启动或已关闭**：插件仅在 Editor 运行时工作。
    2.  **正在编译脚本 (Domain Reload)**：每次修改 C# 脚本后，Unity 会重新编译。在此期间，OpenMCP 服务会主动停止，直到编译完成并重新加载域。
    3.  **后台挂起 (App Nap)**：MacOS 系统可能会暂停后台运行的 Unity Editor 进程以省电。
    4.  **端口冲突**：23456 端口被其他程序占用。
- **解决方案**:
    1.  **保持 Editor 运行**：确保 Unity Editor 处于打开状态。
    2.  **等待编译完成**：观察 Editor 右下角的小微调图标，等待其消失。
    3.  **强制后台运行**：已添加 `Assets/Editor/OpenMCPDiagnostics.cs` 脚本，强制设置 `Application.runInBackground = true`。
    4.  **手动重启服务**：在 Unity 菜单栏选择 `OpenMCP > Diagnostics > Restart Server`。
    5.  **使用检测脚本**：运行 `./check_mcp.sh` 等待服务就绪。

### Q: 为什么修改代码后 OpenMCP 连接断开且不自动恢复（需要点击 Unity）？
- **原因**: Unity 在后台运行时，默认的文件系统监控可能存在延迟，或者 "Auto Refresh" 机制未能及时检测到外部文件的变化。这导致 Unity 停留在旧状态，而 OpenMCP 可能因等待编译或状态不一致而无法响应。点击 Unity 会强制触发 `AssetDatabase.Refresh()`，从而开始编译（关闭 Server）-> 编译结束（重启 Server）。
- **解决方案**:
    1.  **主动触发刷新**: 在修改脚本后，通过 OpenMCP 的 `/api/v1/asset/refresh` 接口强制 Unity 刷新。这会立即触发编译（并暂时断开连接），但能确保流程自动化继续，无需人工干预。
    2.  **确保 Auto Refresh 开启**: 检查 Unity `Preferences > General > Auto Refresh` 是否勾选。
