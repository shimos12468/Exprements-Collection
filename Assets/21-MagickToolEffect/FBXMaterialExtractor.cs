using UnityEditor;
using UnityEngine;

public class FBXMaterialExtractor
{
    [MenuItem("Tools/Extract Materials from FBX")]
    public static void ExtractMaterials()
    {
        // Prompt the user to select an FBX file
        string fbxPath = EditorUtility.OpenFilePanel("Select FBX File", "Assets", "fbx");

        if (string.IsNullOrEmpty(fbxPath))
        {
            Debug.LogError("No FBX file selected.");
            return;
        }

        // Convert the file path to a Unity-relative asset path
        fbxPath = fbxPath.Replace(Application.dataPath, "Assets");

        // Load the FBX model
        GameObject fbxModel = AssetDatabase.LoadAssetAtPath<GameObject>(fbxPath);

        if (fbxModel == null)
        {
            Debug.LogError("Failed to load FBX file: " + fbxPath);
            return;
        }

        // Create a folder for the extracted materials
        string outputFolder = System.IO.Path.Combine(System.IO.Path.GetDirectoryName(fbxPath), "ExtractedMaterials");
        if (!AssetDatabase.IsValidFolder(outputFolder))
        {
            AssetDatabase.CreateFolder(System.IO.Path.GetDirectoryName(fbxPath), "ExtractedMaterials");
        }

        // Extract materials from the FBX
        Renderer[] renderers = fbxModel.GetComponentsInChildren<Renderer>();
        foreach (Renderer renderer in renderers)
        {
            foreach (Material material in renderer.sharedMaterials)
            {
                if (material == null) continue;

                // Generate the new material path
                string newMaterialPath = System.IO.Path.Combine(outputFolder, material.name + ".mat");
                newMaterialPath = AssetDatabase.GenerateUniqueAssetPath(newMaterialPath);

                // Check if the material is already an asset
                string materialPath = AssetDatabase.GetAssetPath(material);
                if (!string.IsNullOrEmpty(materialPath))
                {
                    // Copy the material asset to the new folder
                    AssetDatabase.CopyAsset(materialPath, newMaterialPath);
                    Debug.Log($"Copied material: {newMaterialPath}");
                }
                else
                {
                    // Create a new material asset if the material isn't saved as an asset
                    Material newMaterial = new Material(material);
                    AssetDatabase.CreateAsset(newMaterial, newMaterialPath);
                    Debug.Log($"Created new material: {newMaterialPath}");
                }
            }
        }

        AssetDatabase.Refresh();
        Debug.Log("Material extraction complete!");
    }
}
