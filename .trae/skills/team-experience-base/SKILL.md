---
name: "team-experience-base"
description: "项目避坑指南与常见问题索引。当用户编写代码、处理资源或询问最佳实践时调用，以避免重复错误。"
---

# Team Experience Base (项目避坑指南)

这是一个项目级的经验库，旨在记录开发过程中常见的错误、陷阱和最佳实践，防止重复踩坑。

## 什么时候使用此 Skill

当用户进行以下操作或涉及以下领域时，**必须**先查阅本 Skill 中的相关 Reference 文档：

1.  **编写或修改核心代码逻辑时**：检查通用编码避坑指南。
2.  **导入、设置或优化美术资源时**：检查资源管理避坑指南。
3.  **遇到难以解决的报错或异常时**：检查是否有类似的历史案例。
4.  **进行架构设计或重构时**：参考过往的架构决策和教训。

## 索引 (References)

请根据当前任务的上下文，选择性阅读以下文档：

*   **[通用编码避坑指南](references/general_coding.md)**
    *   涉及：C# 语言特性陷阱、Unity API 使用误区、性能热点、内存泄漏等。
    *   *关键词*：`Update`, `Coroutine`, `LINQ`, `GC`, `NullReference`

*   **[资源管理避坑指南](references/resource_management.md)**
    *   涉及：纹理设置、模型导入、Prefab 嵌套、AssetBundle、内存占用等。
    *   *关键词*：`Texture`, `Mesh`, `Compression`, `Mipmap`, `Resources`

## 如何贡献

如果你在开发过程中解决了新的“坑”或总结了新的经验，请：
1.  在 `references/` 目录下找到对应的分类文件（或新建）。
2.  按照“问题描述 -> 原因分析 -> 解决方案 -> 预防措施”的格式添加记录。
3.  更新本文件 (`SKILL.md`) 的索引。
