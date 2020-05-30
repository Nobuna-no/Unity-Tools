using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditorInternal;

public class ScriptableObject_AdvPostProcessing : ScriptableObject
{
    public delegate void PostProcessingAction();
    public PostProcessingAction OnAssetDeletion;
    public PostProcessingAction OnAssetImport;
}

public class CustomAssetModificationProcessor : UnityEditor.AssetModificationProcessor
{
    public static AssetDeleteResult OnWillDeleteAsset(string path, RemoveAssetOptions opt)
    {
        ScriptableObject_AdvPostProcessing asset = AssetDatabase.LoadAssetAtPath<ScriptableObject_AdvPostProcessing>(path);
        if(asset)
        {
            asset.OnAssetDeletion?.Invoke();
            Resources.UnloadAsset(asset);
            return AssetDeleteResult.DidNotDelete;
        }
        else
        {
            Object assetToUnload = AssetDatabase.LoadAssetAtPath<Object>(path);
            if(assetToUnload == null)
            {
                // Comment this function and compile if you want to be able to delete the asset.
                Debug.LogWarning("You can't delete this asset at the moment.");
                return AssetDeleteResult.FailedDelete;
            }
            Resources.UnloadAsset(assetToUnload);
            return AssetDeleteResult.DidNotDelete;
        }
    }
}

/*
class CUSTOM_AssetPostProcessor : AssetPostprocessor
{
    static void OnPostprocessAllAssets(string[] importedAssets, string[] deletedAssets, string[] movedAssets, string[] movedFromAssetPaths)
    {
        foreach (string str in importedAssets)
        {
            ScriptableObject asset = AssetDatabase.LoadAssetAtPath<ScriptableObject>(str);// as ScriptableObject_AdvPostProcessing;
            if (asset != null)
            {
                // It works!
            }
            
            //IAssetPostprocessingCallback asset = AssetDatabase.LoadAssetAtPath(str, typeof(IAssetPostprocessingCallback)) as IAssetPostprocessingCallback;
            //if (asset != null)
            //{
            //    asset.AssetImport();
            //}

            Debug.Log("Reimported Asset: " + str);
        }

        foreach (string str in deletedAssets)
        {
            // Cannot be load cause already lost all references and can't no longer be loaded.
            ScriptableObject asset = AssetDatabase.LoadAssetAtPath<ScriptableObject>(str);// as ScriptableObject_AdvPostProcessing;
            if (asset != null)
            {
               // ERROR: Always null
            }
                
            Debug.Log("Deleted Asset: " + str);
        }

        for (int i = 0; i < movedAssets.Length; i++)
        {
            IAssetPostprocessingCallback asset = AssetDatabase.LoadAssetAtPath(movedAssets[i], typeof(IAssetPostprocessingCallback)) as IAssetPostprocessingCallback;
            if (asset != null)
            {
                asset.AssetMove(movedAssets[i], movedFromAssetPaths[i]);
            }

            Debug.Log("Moved Asset: " + movedAssets[i] + " from: " + movedFromAssetPaths[i]);
        }
    }
}*/