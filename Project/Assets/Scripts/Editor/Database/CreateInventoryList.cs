using System.Collections;
using UnityEngine;
using UnityEditor;

public class CreateInventoryList
{
    [MenuItem("Assets/Create/Inventory Item List")]
    public static InventoryItemList Create()
    {
        InventoryItemList asset = ScriptableObject.CreateInstance<InventoryItemList>();

        AssetDatabase.CreateAsset(asset, "Assets/Data/InventoryItemList.asset");
        AssetDatabase.SaveAssets();
        return asset;
    }
}

public class InventoryItem_AssetSupervisor
{
    //[MenuItem("Assets/Create/InventoryItem")]
    public static InventoryItem Create(string name)
    {
        InventoryItem asset = ScriptableObject.CreateInstance<InventoryItem>();

        AssetDatabase.CreateAsset(asset, "Assets/Data/Inventory/"+name+".asset");
        AssetDatabase.SaveAssets();
        return asset;
    }

    public static bool Delete(InventoryItem _item)
    {
        return AssetDatabase.DeleteAsset(AssetDatabase.GetAssetPath(_item));
    }
}