using UnityEditor;
using UnityEngine;

public class FBXTextureExtractor
{
    [MenuItem("Tools/Extract Textures from FBX")]
    public static void ExtractTextures()
    {
        // Prompt the user to select an FBX file
        string fbxPath = EditorUtility.OpenFilePanel("Select FBX File", "Assets", "fbx");

        if (string.IsNullOrEmpty(fbxPath))
        {
            Debug.LogError("No FBX file selected.");
            return;
        }

        // Convert the file path to a relative Unity asset path
        fbxPath = fbxPath.Replace(Application.dataPath, "Assets");

        // Load the FBX model as a GameObject
        GameObject fbxModel = AssetDatabase.LoadAssetAtPath<GameObject>(fbxPath);

        if (fbxModel == null)
        {
            Debug.LogError("Failed to load FBX file: " + fbxPath);
            return;
        }

        // Create a folder for the extracted textures
        string outputFolder = System.IO.Path.Combine(System.IO.Path.GetDirectoryName(fbxPath), "ExtractedTextures");
        if (!AssetDatabase.IsValidFolder(outputFolder))
        {
            AssetDatabase.CreateFolder(System.IO.Path.GetDirectoryName(fbxPath), "ExtractedTextures");
        }

        // Extract textures from the materials
        Renderer[] renderers = fbxModel.GetComponentsInChildren<Renderer>();
        foreach (Renderer renderer in renderers)
        {
            foreach (Material material in renderer.sharedMaterials)
            {
                if (material == null) continue;

                foreach (string textureProperty in material.GetTexturePropertyNames())
                {
                    Texture texture = material.GetTexture(textureProperty);
                    if (texture == null) continue;

                    string texturePath = AssetDatabase.GetAssetPath(texture);
                    if (string.IsNullOrEmpty(texturePath))
                    {
                        Debug.LogWarning("Texture is embedded or not accessible as an asset: " + texture.name);
                        continue;
                    }

                    // Copy the texture to the new folder
                    string outputFilePath = System.IO.Path.Combine(outputFolder, System.IO.Path.GetFileName(texturePath));
                    AssetDatabase.CopyAsset(texturePath, outputFilePath);
                    Debug.Log($"Extracted texture: {outputFilePath}");
                }
            }
        }

        AssetDatabase.Refresh();
        Debug.Log("Texture extraction complete!");
    }
}
