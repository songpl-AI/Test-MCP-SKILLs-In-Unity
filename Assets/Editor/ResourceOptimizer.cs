using UnityEngine;
using UnityEditor;
using System.IO;

public class ResourceOptimizer
{
    // ==========================================
    // 菜单项配置
    // ==========================================

    [MenuItem("Tools/Optimize Resources/1. 修复所有纹理设置 (Fix Textures)", false, 1)]
    public static void FixAllTextures()
    {
        string[] guids = AssetDatabase.FindAssets("t:Texture");
        int count = 0;
        
        try
        {
            foreach (string guid in guids)
            {
                string path = AssetDatabase.GUIDToAssetPath(guid);
                
                // 忽略 Editor 目录和 Packages
                if (path.Contains("/Editor/") || path.StartsWith("Packages/")) continue;
                
                TextureImporter importer = AssetImporter.GetAtPath(path) as TextureImporter;
                if (importer == null) continue;

                bool changed = false;

                // 1. 关闭 Read/Write (除非确实需要)
                if (importer.isReadable)
                {
                    // 这里不做自动关闭，因为有些脚本可能依赖它，只做日志提示或可选关闭
                    Debug.LogWarning($"[Texture] 建议手动检查 Read/Write: {path}");
                }

                // 2. Mipmaps 设置
                bool isUI = importer.textureType == TextureImporterType.Sprite || importer.textureType == TextureImporterType.GUI;
                
                if (isUI)
                {
                    if (importer.mipmapEnabled)
                    {
                        importer.mipmapEnabled = false;
                        changed = true;
                        Debug.Log($"[Texture] UI/Sprite 关闭 Mipmaps: {path}");
                    }
                }
                else
                {
                    // 3D 纹理默认开启 Mipmaps
                    if (!importer.mipmapEnabled && importer.textureType == TextureImporterType.Default)
                    {
                        importer.mipmapEnabled = true;
                        changed = true;
                        Debug.Log($"[Texture] 3D 纹理开启 Mipmaps: {path}");
                    }
                }

                // 3. 平台覆盖设置 (Android)
                TextureImporterPlatformSettings androidSettings = importer.GetPlatformTextureSettings("Android");
                if (!androidSettings.overridden)
                {
                    androidSettings.overridden = true;
                    androidSettings.format = TextureImporterFormat.ASTC_6x6; // 推荐格式
                    androidSettings.maxTextureSize = 2048;
                    importer.SetPlatformTextureSettings(androidSettings);
                    changed = true;
                    Debug.Log($"[Texture] 添加 Android 覆盖设置 (ASTC 6x6): {path}");
                }

                // 4. 平台覆盖设置 (iOS)
                TextureImporterPlatformSettings iosSettings = importer.GetPlatformTextureSettings("iPhone");
                if (!iosSettings.overridden)
                {
                    iosSettings.overridden = true;
                    iosSettings.format = TextureImporterFormat.ASTC_6x6; // 推荐格式
                    iosSettings.maxTextureSize = 2048;
                    importer.SetPlatformTextureSettings(iosSettings);
                    changed = true;
                    Debug.Log($"[Texture] 添加 iOS 覆盖设置 (ASTC 6x6): {path}");
                }

                if (changed)
                {
                    importer.SaveAndReimport();
                    count++;
                }
            }
        }
        finally
        {
            AssetDatabase.SaveAssets();
            EditorUtility.ClearProgressBar();
        }

        Debug.Log($"<color=green>纹理优化完成！共修复 {count} 个文件。</color>");
    }

    [MenuItem("Tools/Optimize Resources/2. 修复所有模型设置 (Fix Models)", false, 2)]
    public static void FixAllModels()
    {
        string[] guids = AssetDatabase.FindAssets("t:Model");
        int count = 0;

        try
        {
            foreach (string guid in guids)
            {
                string path = AssetDatabase.GUIDToAssetPath(guid);
                
                // 忽略 Editor 目录和 Packages
                if (path.Contains("/Editor/") || path.StartsWith("Packages/")) continue;

                ModelImporter importer = AssetImporter.GetAtPath(path) as ModelImporter;
                if (importer == null) continue;

                bool changed = false;

                // 1. 关闭 Read/Write
                if (importer.isReadable)
                {
                     // 同样，Read/Write 可能被脚本使用，这里保守一点，只警告或根据项目约定关闭
                     Debug.LogWarning($"[Model] 建议手动检查 Read/Write: {path}");
                }

                // 2. 开启网格压缩 (Mesh Compression)
                if (importer.meshCompression == ModelImporterMeshCompression.Off)
                {
                    importer.meshCompression = ModelImporterMeshCompression.Medium;
                    changed = true;
                    Debug.Log($"[Model] 开启网格压缩 (Medium): {path}");
                }

                // 3. 开启网格优化 (Optimize Mesh)
                if (!importer.optimizeMesh)
                {
                    importer.optimizeMesh = true;
                    changed = true;
                    Debug.Log($"[Model] 开启网格优化: {path}");
                }

                if (changed)
                {
                    importer.SaveAndReimport();
                    count++;
                }
            }
        }
        finally
        {
            AssetDatabase.SaveAssets();
            EditorUtility.ClearProgressBar();
        }

        Debug.Log($"<color=green>模型优化完成！共修复 {count} 个文件。</color>");
    }
}
