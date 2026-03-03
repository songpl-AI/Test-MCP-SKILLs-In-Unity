---
name: "unity-resource-auditor"
description: "分析 Unity 项目资源，检查最佳实践、优化项和潜在错误。当用户要求分析资源或检查资产健康状况时调用。"
---

# Unity 资源审计 (Unity Resource Auditor)

此技能通过分析 Unity 项目的 `Assets` 文件夹，识别资源使用统计信息，并根据最佳实践发现潜在的优化问题。同时支持通过 MCP 工具进行更深度的资源查找和操作。

## 工作流程 (Workflow)

当调用此技能时，请执行以下步骤：

1.  **MCP 辅助检查 (可选)**
    *   检查是否存在 Unity 相关的 MCP 工具（如 `OpenMCP`）。
    *   如果存在，可尝试调用相关 MCP 工具获取场景层级、组件引用等更深度的运行时信息。

2.  **执行静态分析脚本**
    *   运行本技能目录下的 `scripts/analyze_unity_assets.py`。
    *   （请结合当前 Skill 的实际安装路径来执行该脚本）

3.  **生成并保存报告**
    *   脚本会自动在 `AnalyzeResources` 目录下创建带有时间戳的子文件夹（如 `Audit_yyyy-MM-dd-HH-mm-ss`）。
    *   生成的 Markdown 报告将保存至该文件夹，命名为 `ResourceAudit_yyyy-MM-dd-HH-mm-ss.md`。
    *   生成的 Excel (CSV) 报告将保存至该文件夹，命名为 `ResourceAudit_yyyy-MM-dd-HH-mm-ss.csv`。

4.  **展示结果**
    *   读取脚本输出的摘要信息向用户汇报。
    *   告知用户完整报告所在的文件夹路径。

5.  **提供修复建议 (Fix Proposal)**
    *   **查阅参考资料**: 读取本技能目录下的 `references/` 文件夹中的优化指南（如 `texture_guidelines.md` 等），以获取最新的最佳实践和详细解释。
    *   **分析与记录**: 结合审计报告和参考资料，在**同一文件夹**下生成修复建议文档，命名为 `FixProposal_yyyy-MM-dd-HH-mm-ss.md`。在文档中引用相关指南作为理论依据。
    *   **检索现有脚本**: 检查项目中 `Assets/Editor` 目录下是否已存在资源优化脚本。
    *   **给出行动建议**:
        *   若存在相关脚本，建议用户运行该脚本（通过菜单项或 API）。
        *   若不存在，**主动提议** 为用户创建一个 Editor 脚本（例如 `ResourceOptimizer.cs`），以一键修复报告中提到的通用问题。
        *   若问题复杂（如大纹理需要人工确认），则给出具体的操作建议。

## 脚本说明

该脚本 (`scripts/analyze_unity_assets.py`) 直接解析 `.meta` 文件 (YAML 格式) 来检查导入设置，无需打开 Unity 编辑器。

详细的检查规则和优化建议请参阅 `references/` 目录下的文档：
*   [Texture Guidelines](references/texture_guidelines.md)
*   [Model Guidelines](references/model_guidelines.md)
*   [Audio Guidelines](references/audio_guidelines.md)
