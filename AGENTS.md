# AGENTS.md - AI 智能体协作指南

本文件旨在为参与本项目开发的 AI 智能体（Agents）提供上下文信息、开发规范和最佳实践指南。

## 1. 项目概览 (Project Overview)

*   **项目名称**: Test2023MCP
*   **引擎版本**: Unity 2020.3+
*   **核心功能**:
    *   集成 **OpenMCP** (`com.openmcp.unity-editor-mcp`)，支持通过 MCP 协议控制 Unity 编辑器。
    *   包含 **SimpleNaturePack** 资源，用于构建演示场景。
    *   集成了资源审计与优化工具 (`AnalyzeResources`, `Assets/Editor/ResourceOptimizer.cs`)。

## 2. 目录结构说明 (Directory Structure)

*   `Assets/`
    *   `Editor/`: 存放编辑器扩展脚本，如资源优化工具 `ResourceOptimizer.cs`。
    *   `SimpleNaturePack/`: 核心美术资源（模型、材质、纹理、预制体）。
    *   `Scenes/`: 存放示例场景。
*   `AnalyzeResources/`: 存放自动生成的资源审计报告 (`ResourceAudit_*.md`) 和修复建议 (`FixProposal_*.md`)。
*   `Packages/`: 项目依赖管理，核心依赖包括 `com.openmcp.unity-editor-mcp`。
*   `.trae/skills/`: 包含特定于此项目的 AI 技能配置（如 `unity-resource-auditor`, `openmcp`, `team-experience-base`）。

##### 3. Unity 开发最佳实践 (Best Practices)

在本项目中进行开发或资源管理时，**必须首先查阅 `team-experience-base` Skill 中的避坑指南**，并严格遵守以下规范： 3.1 资源优化 (Resource Optimization)

参考 `AnalyzeResources` 目录下的审计报告，重点关注以下优化项：

*   **纹理 (Textures)**:
    *   **Mipmaps**: 对于非 UI 的 3D 物体纹理，**必须**开启 `Generate Mip Maps`。
    *   **压缩格式**:
        *   Android/iOS 平台应覆盖设置 (Override)，使用 **ASTC** 格式 (如 ASTC 6x6)。
        *   避免使用默认的未压缩格式，以减少包体大小和显存占用。
*   **模型 (Models)**:
    *   **网格压缩 (Mesh Compression)**: 应设置为 `Low` 或 `Medium`，以减小 Asset Bundle 大小。
    *   **网格优化 (Optimize Mesh)**: **必须**勾选，以提高 GPU 渲染性能。
*   **自动化工具**:
    *   优先使用 `Assets/Editor/ResourceOptimizer.cs` 中的工具进行批量修复，而不是手动逐个修改。
    *   运行 `Tools > Resource Optimizer > Fix All Issues` (如果实现了该菜单项) 或调用相应 API。

### 3.2 编码规范 (Coding Standards)

*   **脚本位置**: 编辑器脚本（继承自 `Editor` 或使用 `UnityEditor` 命名空间）必须放在 `Editor` 文件夹内。
*   **命名**: 使用 PascalCase 命名类和方法，camelCase 命名局部变量。
*   **性能**:
    *   避免在 `Update` 循环中使用 `GetComponent`、`Find` 等高开销操作。
    *   使用 `StringBuilder` 进行字符串拼接。
    *   **禁止使用 LINQ**: 避免使用 LINQ 扩展方法（如 `.Where()`, `.Select()`, `.ToList()`），因为它们会产生额外的垃圾回收 (GC) 开销，尤其是在热代码路径中。请使用标准的 `for` 或 `foreach` 循环代替。

## 4. AI 智能体工作流 (Agent Workflow)

AI 智能体在处理本项目任务时，应遵循以下工作流：

1.  **环境感知与经验检索**:
    *   使用 `LS` 和 `Read` 工具检查项目结构和关键文件（如 `manifest.json`, `AGENTS.md`）。
    *   检查 `.trae/skills` 了解可用技能。
    *   **关键步骤**: 调用 `team-experience-base` Skill 或查阅其 `references` 目录，确认当前任务是否有已知的“坑”或最佳实践。
2.  **资源审计与修复**:
    *   在进行资源修改前，先调用 `unity-resource-auditor` 技能（如有）或检查 `AnalyzeResources` 目录下的最新报告。
    *   根据 `FixProposal` 制定修改计划。
3.  **场景操作**:
    *   涉及场景物体创建、修改时，优先使用 `OpenMCP` 技能/工具，以确保操作的准确性和兼容性。
4.  **验证**:
    *   修改代码或资源后，通过编译检查（无报错）和重新生成审计报告来验证修复效果。

## 5. 常用命令与工具 (Tools & Commands)

*   **资源审计**: 使用 `unity-resource-auditor` 技能生成报告。
*   **避坑指南**: 使用 `team-experience-base` 获取项目特定的经验总结。
*   **代码搜索**: 使用 `SearchCodebase` 查找现有实现，避免重复造轮子。
*   **文件操作**: 使用 `Read` 读取文件内容，`Write` / `SearchReplace` 修改文件。

---
*最后更新时间: 2026-03-03*
