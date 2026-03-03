# Test2023MCP - Unity MCP & Skill Testing Repository

本项目旨在记录和测试 **Model Context Protocol (MCP)** 以及 **Trae IDE Skills** 在 Unity 开发环境中的实际应用与效果。通过本仓库，我们探索 AI 辅助工具如何提升 Unity 项目的开发效率、代码质量以及资源管理能力。

## 🎯 项目目标

*   **MCP 集成测试**：验证通过 MCP 协议与 Unity 编辑器进行交互的可行性与稳定性。
*   **Skill 开发与验证**：开发、测试并优化各类 Trae IDE Skills，使其更好地服务于 Unity 工作流。
*   **自动化工作流探索**：利用 AI Agent 实现资源审计、代码生成、Bug 报告等任务的自动化。

## 🛠️ 集成的 Skills

本项目目前集成并测试了以下核心 Skills：

### 1. Unity Resource Auditor (资源审计)
*   **功能**：深度分析 Unity 项目中的资源（纹理、模型、音频等），检查是否符合最佳实践，并识别潜在的性能问题或配置错误。
*   **产出**：生成的审计报告和修复建议保存在 `AnalyzeResources/` 目录下。
*   **相关文件**：
    *   `.trae/skills/unity-resource-auditor/`：Skill 源码及配置。
    *   `Assets/Editor/ResourceOptimizer.cs`：辅助资源优化的编辑器脚本。

### 2. OpenMCP (Unity 自动化控制)
*   **功能**：通过 HTTP 接口实现对 Unity Editor 的自动化控制，支持场景管理、物体创建、组件修改等操作。
*   **特点**：采用源码驱动策略，能够动态发现 API，实现灵活的编辑器交互。
*   **相关文件**：
    *   `.trae/skills/openmcp/`：Skill 定义。
    *   `OpenMCPUnityPlugin` / `OpenClawUnityPlugin`：相关的 Unity 插件项目文件。

### 3. ProjectPlanner (项目规划)
*   **功能**：提供系统性的需求分析、任务规划及文档管理。
*   **应用**：用于在编码前生成 `plan.md`、`architecture.md` 等文档，确保开发方向的正确性。

### 4. BugReport (缺陷报告)
*   **功能**：快速生成标准化的 Markdown 格式 Bug 报告，便于问题追踪与修复。

## 📂 目录结构说明

```
Test2023MCP/
├── .trae/skills/           # 存放所有自定义 Skill 的配置与脚本
├── AnalyzeResources/       # 存放 Resource Auditor 生成的审计报告与修复建议
├── Assets/                 # Unity 项目资源目录
│   ├── Editor/             # 编辑器扩展脚本
│   └── SimpleNaturePack/   # 测试用的场景与资源包
├── Packages/               # Unity 包管理配置
└── ProjectSettings/        # 项目设置
```

## 🚀 快速开始

1.  **环境要求**：
    *   Unity 2020.3 或更高版本。
    *   Trae IDE (支持 MCP 与 Skill 功能)。
    *   Python 3.x (部分 Skill 脚本依赖)。

2.  **使用方法**：
    *   在 Trae IDE 中打开本项目。
    *   在对话框中通过指令调用相应的 Skill（例如：`@unity-resource-auditor` 或自然语言描述需求）。
    *   查看 IDE 返回的执行结果或生成的文档。

## 📝 更新日志

*   **2026-03-03**: 初始化 README 文档，记录当前集成的 Skills (Resource Auditor, OpenMCP 等) 及项目结构。
