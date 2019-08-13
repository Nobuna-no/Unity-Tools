using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditorInternal;

[System.Serializable]
public class AssetInfo
{
    public string AssetPath;

    public System.Type GetAssetType()
    {
        return System.Type.GetType(AssetPath + ", Assembly-CSharp");
    }
}

public class AssetInfo<T> : AssetInfo
{
    public T ComplementaryData;
}