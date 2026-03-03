# 纹理优化指南 (Texture Optimization Guidelines)

## 核心原则
纹理通常占据 Unity 项目最大的内存和包体空间。优化纹理设置是提升性能的最直接手段。

## 检查清单 (Checklist)

### 1. Read/Write Enabled (读写开启)
*   **规则**: 默认关闭。
*   **原因**: 开启此选项会使纹理在内存中保留两份副本（显存一份，系统内存一份）。
*   **例外**: 仅当脚本需要通过 `Texture2D.GetPixel` 或 `SetPixel` 访问纹理数据时才开启。

### 2. Mipmaps (多级渐远纹理)
*   **规则**:
    *   **3D 物体纹理**: **必须开启**。不仅减少远处物体的噪点（摩尔纹），还能提高 GPU 缓存命中率，节省带宽。
    *   **UI / 2D Sprite**: **必须关闭**。UI 元素通常以 1:1 像素显示，开启 Mipmaps 会浪费约 33% 的内存。

### 3. Max Size (最大尺寸)
*   **规则**: 限制在 **2048** 以内。
*   **原因**: 移动设备对 4096 及以上纹理的支持和性能开销较大。
*   **建议**: 检查纹理实际用途，背景图可大，小物件纹理应限制在 512 或 256。

### 4. Compression Format (压缩格式)
*   **规则**: 必须针对目标平台（Android/iOS）覆盖默认设置。
*   **推荐设置**:
    *   **Android**: `ASTC 6x6` (平衡) 或 `ASTC 8x8` (高压缩)。若不支持 ASTC，回退到 `ETC2`。
    *   **iOS**: `ASTC 6x6` 或 `PVRTC` (旧设备)。
    *   **PC**: `DXT5` / `BC7`。
*   **注意**: 避免使用 `RGBA 32 bit` 或 `RGB 24 bit`，除非对画质有极端要求且纹理很小。

### 5. Filtering & Aniso (采样与各向异性)
*   **Filtering**:
    *   `Bilinear` (双线性): 默认推荐。
    *   `Trilinear` (三线性): 在 Mipmap 级别间平滑过渡，开销稍大。
*   **Anisotropic Filtering**:
    *   **规则**: 默认关闭或设为 1。
    *   **例外**: 地面、路面等以掠射角观察的纹理，可适当开启 (2x-4x)，否则远处会模糊。

## 常见问题
*   **法线贴图 (Normal Map)**: 确保 Texture Type 设为 `Normal map`，否则光照计算会出错。
*   **不透明纹理**: 如果纹理没有透明通道，确保关闭 `Alpha Source` 或生成不带 Alpha 的压缩格式（如 RGB Compressed ETC2），可节省空间。
