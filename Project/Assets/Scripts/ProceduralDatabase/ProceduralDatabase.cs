using UnityEngine;
using UnityEditor;

using System.Collections;
using System.Collections.Generic;

public class ProceduralDatabaseUtility 
{
    [MenuItem("Assets/Create/Procedural Database")]
    public static SO_ProceduralRowList Create(string _SavePath)
    {
        SO_ProceduralRowList asset = ScriptableObject.CreateInstance<SO_ProceduralRowList>();

        AssetDatabase.CreateAsset(asset, _SavePath);
        AssetDatabase.SaveAssets();
        return asset;
    }

    public static bool Delete(SO_ProceduralRowList _RowList)
    {
        foreach(SO_DynamicRow data in _RowList.Rows)
        {
            Delete(data);
        }

        return AssetDatabase.DeleteAsset(AssetDatabase.GetAssetPath(_RowList));
    }

    public static SO_DynamicRow CreateRow(string _SaveFilenamePath, SO_DynamicRow _StructureRow, bool _bCreateAsset)
    {
        SO_DynamicRow asset = ScriptableObject.CreateInstance<SO_DynamicRow>();
        asset.Initialize(_StructureRow);

        if (_bCreateAsset)
        {
            AssetDatabase.CreateAsset(asset, _SaveFilenamePath + ".asset");
            AssetDatabase.SaveAssets();
        }
        return asset;
    }

    public static bool Delete(SO_DynamicRow _Item)
    {
        return AssetDatabase.DeleteAsset(AssetDatabase.GetAssetPath(_Item));
    }
}
