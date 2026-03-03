from .utils import get_file_size_mb

def analyze_texture_importer(meta_data, path, asset_issues):
    importer = meta_data.get('TextureImporter', {})
    if not importer: return

    # Check Read/Write
    if importer.get('isReadable', 0) == 1:
        asset_issues.append(f"开启了读/写 (高内存占用)")

    # Check Mipmaps
    mipmap = importer.get('mipmapEnabled', 0)
    if 'UI' in path or 'Sprite' in path:
        if mipmap == 1:
            asset_issues.append(f"UI/Sprite 纹理开启了 Mipmaps (浪费内存)")
    else:
        # For non-UI textures (assumed 3D), mipmaps should generally be enabled for bandwidth optimization
        if mipmap == 0:
             asset_issues.append(f"非 UI 纹理关闭了 Mipmaps (带宽风险)")

    # Check Max Size
    max_texture_size = importer.get('maxTextureSize', 0)
    if max_texture_size > 2048:
        asset_issues.append(f"最大尺寸 > 2048 ({max_texture_size}) (性能风险)")

    # Check Format (Platform specific is complex, checking for generic inefficient formats)
    # This is a basic check, specific platform overrides are often in 'platformSettings'
    texture_settings = importer.get('textureSettings', {})
    
    # Check Filtering
    filter_mode = texture_settings.get('filterMode', 1)
    if filter_mode == 2: # Trilinear
         asset_issues.append(f"开启了三线性过滤 (更高带宽)")
    
    # Check Aniso Level
    aniso = texture_settings.get('aniso', 1)
    if aniso > 1:
        asset_issues.append(f"各向异性等级 > 1 ({aniso}) (更高带宽)")

    if filter_mode == 0: # Point
        pass # Point filter is fine for pixel art, but worth noting if not intentional
    
    # Check Android/iOS overrides exist
    platform_settings = importer.get('platformSettings', [])
    has_android = False
    has_ios = False
    if isinstance(platform_settings, list):
        for ps in platform_settings:
            name = ps.get('name', '')
            if name == 'Android': has_android = True
            if name == 'iPhone': has_ios = True
            
    if not has_android or not has_ios:
         asset_issues.append(f"缺少特定平台覆盖设置 (Android/iOS)")


def analyze_model_importer(meta_data, path, asset_issues):
    importer = meta_data.get('ModelImporter', {})
    if not importer: return

    # Check Read/Write
    meshes = importer.get('meshes', {})
    is_readable = 0
    if isinstance(meshes, dict):
        is_readable = meshes.get('isReadable', 0)
    if importer.get('isReadable', 0) == 1:
        is_readable = 1
        
    if is_readable == 1:
        asset_issues.append(f"开启了读/写 (高内存占用)")

    # Mesh Compression
    mesh_compression = 0
    if isinstance(meshes, dict):
        mesh_compression = meshes.get('meshCompression', 0)
    if mesh_compression == 0:
         asset_issues.append(f"网格压缩未开启")
         
    # Check for Optimize Mesh
    optimize_mesh = 0
    if isinstance(meshes, dict):
        optimize_mesh = meshes.get('optimizeMesh', 0)
    if optimize_mesh == 0:
        asset_issues.append(f"网格优化未开启 (性能风险)")

    # Check Material Import
    materials = importer.get('materials', {})
    import_materials = 1
    if isinstance(materials, dict):
        import_materials = materials.get('importMaterials', 1)
    
    # Note: Importing materials is default but often recommended to be off for strict pipelines
    # asset_issues.append(f"Material Import Enabled")


def analyze_audio_importer(meta_data, path, asset_issues):
    importer = meta_data.get('AudioImporter', {})
    if not importer: return

    default_settings = importer.get('defaultSettings', {})
    load_type = default_settings.get('loadType', -1)
    # 0: DecompressOnLoad, 1: CompressedInMemory, 2: Streaming
    
    file_size = get_file_size_mb(path)
    
    if load_type == 0 and file_size > 0.5:
        asset_issues.append(f"大文件 > 0.5MB 设置为 DecompressOnLoad (内存峰值风险)")
    
    if load_type == 2 and file_size < 0.2:
        asset_issues.append(f"小文件 < 0.2MB 设置为 Streaming (CPU 开销)")

    force_to_mono = importer.get('forceToMono', 0)
    if force_to_mono == 0 and file_size > 0.5:
         asset_issues.append(f"大文件 > 0.5MB 未强制单声道 (检查是否为 3D)")
