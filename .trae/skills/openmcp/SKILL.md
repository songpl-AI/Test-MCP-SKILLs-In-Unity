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