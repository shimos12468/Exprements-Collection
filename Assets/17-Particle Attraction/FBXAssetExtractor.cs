using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

public class FBXAssetExtractor : EditorWindow
{
    private List<Object> selectedFBXs = new List<Object>();
    private Vector2 scrollPosition;
    private BuildTargetGroup selectedPlatform = BuildTargetGroup.Standalone;

    // Mapping of platforms to their optimal texture formats
    private static readonly Dictionary<BuildTargetGroup, TextureImporterFormat[]> PlatformTextureFormats =
        new Dictionary<BuildTargetGroup, TextureImporterFormat[]>
    {
        { BuildTargetGroup.Standalone, new[] {
            TextureImporterFormat.DXT5,
            TextureImporterFormat.BC7
        }},
        { BuildTargetGroup.Android, new[] {
            TextureImporterFormat.ETC2_RGBA8,
            TextureImporterFormat.ASTC_4x4
        }},
        { BuildTargetGroup.iOS, new[] {
            TextureImporterFormat.PVRTC_RGBA4,
            TextureImporterFormat.ASTC_4x4
        }},
        { BuildTargetGroup.WebGL, new[] {
            TextureImporterFormat.DXT5,
            TextureImporterFormat.BC7
        }},
        { BuildTargetGroup.PS4, new[] {
            TextureImporterFormat.BC7,
            TextureImporterFormat.BC4
        }},
        { BuildTargetGroup.XboxOne, new[] {
            TextureImporterFormat.BC7,
            TextureImporterFormat.BC4
        }}
    };

    [MenuItem("Tools/FBX Asset Extractor")]
    public static void ShowWindow()
    {
        GetWindow<FBXAssetExtractor>("FBX Asset Extractor");
    }

    private void OnGUI()
    {
        EditorGUILayout.LabelField("FBX Asset Extraction Tool", EditorStyles.boldLabel);

        // Platform selection dropdown
        selectedPlatform = (BuildTargetGroup)EditorGUILayout.EnumPopup("Target Platform", selectedPlatform);

        // Add FBX button
        if (GUILayout.Button("Add FBX"))
        {
            string fbxPath = EditorUtility.OpenFilePanel("Select FBX File", "", "fbx");

            if (!string.IsNullOrEmpty(fbxPath))
            {
                // Convert full system path to Unity asset path
                string assetPath = FileUtil.GetProjectRelativePath(fbxPath);
                Object loadedFBX = AssetDatabase.LoadMainAssetAtPath(assetPath);

                if (loadedFBX != null && !selectedFBXs.Contains(loadedFBX))
                {
                    selectedFBXs.Add(loadedFBX);
                }
            }
        }

        // Scrollable list of selected FBXs
        EditorGUILayout.LabelField("Selected FBX Files:", EditorStyles.boldLabel);
        scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);

        for (int i = selectedFBXs.Count - 1; i >= 0; i--)
        {
            EditorGUILayout.BeginHorizontal();

            // Display FBX object
            selectedFBXs[i] = EditorGUILayout.ObjectField(selectedFBXs[i], typeof(Object), false);

            // Remove button
            if (GUILayout.Button("X", GUILayout.Width(30)))
            {
                selectedFBXs.RemoveAt(i);
            }

            EditorGUILayout.EndHorizontal();
        }

        EditorGUILayout.EndScrollView();

        // Extract button
        GUI.enabled = selectedFBXs.Count > 0;
        if (GUILayout.Button("Extract and Optimize Assets"))
        {
            ExtractAssetsFromFBXs(selectedFBXs.ToArray(), selectedPlatform);
        }
        GUI.enabled = true;

        // Additional info
        EditorGUILayout.HelpBox($"Selected {selectedFBXs.Count} FBX files for extraction", MessageType.Info);
    }

    private void ExtractAssetsFromFBXs(Object[] fbxObjects, BuildTargetGroup targetPlatform)
    {
        int processedCount = 0;

        foreach (Object fbxObject in fbxObjects)
        {
            try
            {
                // Progress bar for multiple FBX extractions
                EditorUtility.DisplayProgressBar(
                    "Extracting Assets",
                    $"Processing {fbxObject.name}...",
                    (float)processedCount / fbxObjects.Length
                );

                // Get the path of the selected FBX
                string fbxPath = AssetDatabase.GetAssetPath(fbxObject);

                // Verify it's an FBX file
                if (!fbxPath.ToLower().EndsWith(".fbx"))
                {
                    Debug.LogWarning($"Skipping {fbxObject.name}: Not an FBX file");
                    continue;
                }

                // Get the directory and filename of the FBX
                string directoryPath = System.IO.Path.GetDirectoryName(fbxPath);
                string fbxFileName = System.IO.Path.GetFileNameWithoutExtension(fbxPath);

                // Create main folder for the FBX
                string mainFolderPath = System.IO.Path.Combine(directoryPath, $"{fbxFileName}_{targetPlatform}");
                Directory.CreateDirectory(mainFolderPath);

                // Create subfolders
                string textureFolder = System.IO.Path.Combine(mainFolderPath, "Textures");
                string materialFolder = System.IO.Path.Combine(mainFolderPath, "Materials");
                string animationFolder = System.IO.Path.Combine(mainFolderPath, "Animations");

                Directory.CreateDirectory(textureFolder);
                Directory.CreateDirectory(materialFolder);
                Directory.CreateDirectory(animationFolder);

                // Extract and optimize textures for the selected platform
                ExtractAndOptimizeTextures(fbxPath, textureFolder, targetPlatform);

                // Extract materials
                ExtractMaterials(fbxPath, materialFolder);

                // Extract animations
                ExtractAnimations(fbxPath, animationFolder);

                processedCount++;
                Debug.Log($"Assets extracted and optimized for {fbxFileName} (Platform: {targetPlatform})");
            }
            catch (System.Exception ex)
            {
                Debug.LogError($"Error processing {fbxObject.name}: {ex.Message}");
            }
        }

        // Clear progress bar
        EditorUtility.ClearProgressBar();

        // Refresh the Asset Database
        AssetDatabase.Refresh();

        // Final summary
        Debug.Log($"Completed extraction for {processedCount} FBX files");
    }

    private void ExtractAndOptimizeTextures(string fbxPath, string outputFolder, BuildTargetGroup targetPlatform)
    {
        // Load all texture dependencies of the FBX
        Object[] textures = AssetDatabase.LoadAllAssetsAtPath(fbxPath)
            .Where(asset => asset is Texture2D)
            .ToArray();

        foreach (Texture2D texture in textures)
        {
            if (texture != null)
            {
                string texturePath = System.IO.Path.Combine(outputFolder, texture.name + ".png");

                // Save original texture
                SaveTextureToPNG(texture, texturePath);

                // Optimize texture for the target platform
                OptimizeTextureForPlatform(texturePath, targetPlatform);
            }
        }
    }

    private void OptimizeTextureForPlatform(string texturePath, BuildTargetGroup targetPlatform)
    {
        // Get the asset importer for the texture
        TextureImporter importer = AssetImporter.GetAtPath(texturePath) as TextureImporter;

        if (importer == null)
        {
            Debug.LogWarning($"Could not find importer for {texturePath}");
            return;
        }

        // Reset importer settings
        importer.textureType = TextureImporterType.Default;
        importer.mipmapEnabled = true;
        importer.maxTextureSize = 2048; // Default max size, can be adjusted

        // Apply platform-specific compression
        if (PlatformTextureFormats.TryGetValue(targetPlatform, out TextureImporterFormat[] formats))
        {
            // Try to use the first supported format
            importer.SetPlatformTextureSettings(new TextureImporterPlatformSettings
            {
                name = targetPlatform.ToString(),
                overridden = true,
                maxTextureSize = 2048,
                format = formats[0],
                compressionQuality = (int)TextureCompressionQuality.Normal
            });
        }
        else
        {
            Debug.LogWarning($"No specific texture format found for {targetPlatform}");
        }

        // Apply the changes
        AssetDatabase.ImportAsset(texturePath, ImportAssetOptions.ForceUpdate);
    }

    private void ExtractMaterials(string fbxPath, string outputFolder)
    {
        // Load all material dependencies of the FBX
        Object[] materials = AssetDatabase.LoadAllAssetsAtPath(fbxPath)
            .Where(asset => asset is Material)
            .ToArray();

        foreach (Material material in materials)
        {
            if (material != null)
            {
                string materialPath = System.IO.Path.Combine(outputFolder, material.name + ".mat");
                AssetDatabase.CopyAsset(AssetDatabase.GetAssetPath(material), materialPath);
            }
        }
    }

    private void ExtractAnimations(string fbxPath, string outputFolder)
    {
        // Load all animation clips associated with the FBX
        Object[] animations = AssetDatabase.LoadAllAssetsAtPath(fbxPath)
            .Where(asset => asset is AnimationClip)
            .ToArray();

        foreach (AnimationClip clip in animations)
        {
            if (clip != null)
            {
                string animationPath = System.IO.Path.Combine(outputFolder, clip.name + ".anim");
                AssetDatabase.CopyAsset(AssetDatabase.GetAssetPath(clip), animationPath);
            }
        }
    }

    private void SaveTextureToPNG(Texture2D texture, string outputPath)
    {
        // Convert texture to readable texture
        RenderTexture tmp = RenderTexture.GetTemporary(
            texture.width,
            texture.height,
            0,
            RenderTextureFormat.Default,
            RenderTextureReadWrite.Linear);

        Graphics.Blit(texture, tmp);
        RenderTexture previous = RenderTexture.active;
        RenderTexture.active = tmp;

        Texture2D myTexture2D = new Texture2D(texture.width, texture.height);
        myTexture2D.ReadPixels(new Rect(0, 0, tmp.width, tmp.height), 0, 0);
        myTexture2D.Apply();

        RenderTexture.active = previous;
        RenderTexture.ReleaseTemporary(tmp);

        // Save texture as PNG
        byte[] bytes = myTexture2D.EncodeToPNG();
        File.WriteAllBytes(outputPath, bytes);
    }
}