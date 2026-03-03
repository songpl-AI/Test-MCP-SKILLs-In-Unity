# 资源管理避坑指南 (Resource Management Pitfalls)

本文件记录了在本项目中关于资源导入、设置和管理的常见问题与规范。

## 1. 纹理 (Textures)

### 1.1 Mipmaps 缺失
*   **问题描述**: 3D 场景中的物体纹理看起来有噪点或闪烁，且远处模糊。
*   **原因分析**: 未开启 `Generate Mip Maps`。
*   **规范**: 对于非 UI 的 3D 物体纹理，**必须**开启 `Generate Mip Maps`。UI 元素通常**不需要** Mipmaps。

### 1.2 压缩格式选择
*   **问题描述**: 包体过大或显存占用过高。
*   **规范**:
    *   **PC**: DXT1 (不带 Alpha) / DXT5 (带 Alpha) 或 BC7。
    *   **iOS/Android**: 必须覆盖默认设置，使用 **ASTC** 格式 (推荐 ASTC 6x6 或 4x4)。
    *   **禁止**: 使用 `RGB 24 bit` 或 `RGBA 32 bit`（除非用于极其特殊的用途且尺寸很小）。

### 1.3 读写权限 (Read/Write Enabled)
*   **问题描述**: 开启 `Read/Write Enabled`。
*   **后果**: 纹理内存占用翻倍（一份在显存，一份在内存供 CPU 访问）。
*   **规范**: 默认**关闭**。仅当脚本需要通过 `GetPixel`/`SetPixel` 访问纹理数据时才开启。

## 2. 模型 (Models)

### 2.1 网格压缩 (Mesh Compression)
*   **规范**: 建议设置为 `Low` 或 `Medium`，以减小 Asset Bundle 大小，除非出现明显的视觉瑕疵。

### 2.2 优化网格 (Optimize Mesh)
*   **规范**: **必须**勾选，这会重排顶点以提高 GPU 缓存命中率，提升渲染性能。

### 2.3 导入材质 (Import Materials)
*   **建议**: 如果可能，尽量不通过模型导入器导入材质，而是手动创建材质并引用，以避免材质丢失或重复创建的问题。

## 3. 音频 (Audio)

### 3.1 加载类型 (Load Type)
*   **短音效 (SFX)**: `Decompress On Load` (解压后驻留内存，响应快)。
*   **长音乐 (BGM)**: `Streaming` (流式播放，节省内存)。
*   **中等长度**: `Compressed In Memory`。

### 3.2 格式 (Format)
*   **移动端**: 推荐使用 `Vorbis`。
*   **iOS**: 可以考虑 `MP3` 或 `ADPCM` (CPU开销较低)。

## 4. 预制体 (Prefabs)

### 4.1 嵌套预制体 (Nested Prefabs)
*   **最佳实践**: 充分利用嵌套预制体功能，将复杂的 UI 或场景物体拆分为小的模块。修改子模块会自动同步到所有引用的地方。

### 4.2 变体 (Variants)
*   **最佳实践**: 对于同一种类但属性略有不同的物体（如不同颜色的怪物），使用 Prefab Variant 而不是复制一份新的 Prefab。
