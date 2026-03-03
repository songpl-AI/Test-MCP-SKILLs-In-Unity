# 模型优化指南 (Model Optimization Guidelines)

## 核心原则
模型主要影响包体大小和渲染管线压力。合理的导入设置可以显著降低这些开销。

## 检查清单 (Checklist)

### 1. Read/Write Enabled (读写开启)
*   **规则**: 默认关闭。
*   **原因**: 开启此选项会使网格数据在内存中保留两份副本（显存一份，系统内存一份）。
*   **例外**: 仅当脚本需要通过 `Mesh.GetVertices` 或 `Mesh.SetVertices` 访问顶点数据时才开启。

### 2. Mesh Compression (网格压缩)
*   **规则**: 开启并设置等级（Low/Medium/High）。
*   **原因**: 压缩可以减小 Asset Bundle 包体大小，对运行时性能无负面影响。
*   **建议**:
    *   **Low**: 大多数情况下的默认选择。
    *   **Medium/High**: 针对远处或对精度要求不高的模型。

### 3. Optimize Mesh (网格优化)
*   **规则**: **必须开启**。
*   **原因**: Unity 会重新排列顶点和索引以优化 GPU 缓存命中率，从而提高渲染性能。
*   **例外**: 极少数情况下，如果顶点顺序是硬编码的（如程序化生成），可能需要关闭。

### 4. Normals & Tangents (法线与切线)
*   **规则**: 按需导入。
*   **建议**:
    *   **Import**: 默认选择。
    *   **Calculate**: 如果源模型法线有问题，可让 Unity 重新计算。
    *   **None**: 对于不需要光照计算的模型（如 UI 元素、简单的粒子特效网格），关闭法线和切线导入可以节省约 50% 的顶点数据量。

### 5. Rig (骨骼与动画)
*   **规则**:
    *   **Animation Type**:
        *   `None`: 静态物体（环境、道具）。
        *   `Generic`: 非人形动画。
        *   `Humanoid`: 人形动画（支持重定向）。
    *   **Optimize Game Objects**: 对于 Humanoid 模型，开启此选项可以减少 Transform 层级深度，提高 CPU 性能。

## 常见问题
*   **多材质 (Multiple Materials)**:
    *   **问题**: 一个模型若使用了多个材质，会导致多次 Draw Call。
    *   **建议**: 尽量合并材质，或使用纹理图集 (Texture Atlas)。
*   **高顶点数 (High Vertex Count)**:
    *   **规则**: 移动端单个网格建议控制在 500-2000 顶点以内，角色模型控制在 5000-10000 顶点以内。
    *   **LOD**: 对于复杂模型，务必使用 LOD (Level of Detail) 技术。
