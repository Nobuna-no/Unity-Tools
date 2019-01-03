using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum EDataType
{
    Boolean,
    Integer,
    Floating,
    String,
    Gameobject,
    Texture2D
};

[System.Serializable]
public class ProceduralData
{
    public EDataType Type;
    public string Name;

    private int m_Index;
    public int Index { get { return m_Index; } }

    public ProceduralData(ProceduralData _DD, int _Index)
    {
        Type = _DD.Type;
        Name = _DD.Name;
        m_Index = _Index;
    }
}

//[System.Serializable]
//public class DynamicData<T> : ProceduralData
//{
//    public T Value;

//    public DynamicData(ProceduralData _DD, int _Index)
//       :base(_DD, _Index)
//    { }
//}

[System.Serializable]
public class DynamicData_Boolean : ProceduralData
{
    public bool Value;

    public DynamicData_Boolean(ProceduralData _DD, int _Index)
        : base(_DD, _Index)
    { }
}
[System.Serializable]
public class DynamicData_Integer : ProceduralData
{
    public int Value;

    public DynamicData_Integer(ProceduralData _DD, int _Index)
        : base(_DD, _Index)
    { }
}
[System.Serializable]
public class DynamicData_Float : ProceduralData
{
    public float Value;

    public DynamicData_Float(ProceduralData _DD, int _Index)
         :base(_DD, _Index)
    { }
}
[System.Serializable]
public class DynamicData_String : ProceduralData
{
    public string Value;

    public DynamicData_String(ProceduralData _DD, int _Index)
         :base(_DD, _Index)
    { }
}
[System.Serializable]
public class DynamicData_GameObject : ProceduralData
{
    public GameObject Value;

    public DynamicData_GameObject(ProceduralData _DD, int _Index)
         :base(_DD, _Index)
    { }
}
[System.Serializable]
public class DynamicData_Texture2D : ProceduralData
{
    public Texture2D Value;

    public DynamicData_Texture2D(ProceduralData _DD, int _Index)
         :base(_DD, _Index)
    { }
}

// List of data in the row. 
[CreateAssetMenu(fileName = "[DynamicRow]", menuName ="Editor Tools/Dynamic Database/Dynamic Row")]
public class SO_DynamicRow : ScriptableObject
{
    /** VARIABLES
     */
    public List<ProceduralData> Data;

    public List<DynamicData_Boolean>    BooleanData;
    public List<DynamicData_Integer>    IntegerData;
    public List<DynamicData_Float>      FloatData;
    public List<DynamicData_String>     StringData;
    public List<DynamicData_GameObject> GameObjectnData;
    public List<DynamicData_Texture2D>  Texture2DData;

    //private Dictionary<ProceduralData, int> BooleanDataMap = null;
    //private Dictionary<ProceduralData, int> IntegerDataMap = null;
    //private Dictionary<ProceduralData, int> FloatDataMap = null;
    //private Dictionary<ProceduralData, int> StringDataMap = null;
    //private Dictionary<ProceduralData, int> GameObjectnDataMap = null;
    //private Dictionary<ProceduralData, int> Texture2DDataMap = null;
    //private bool m_bLoad = false;

    /** CUSTOM METHODS
     */
    public bool Initialize(SO_DynamicRow _DynamicRow)
    {
        if (!_DynamicRow)
        {
            Debug.LogError("Invalid dynamic row pass as template!");
            return false;
        }
        
        int Count = _DynamicRow.Data.Count;
        Data = new List<ProceduralData>();
        int index = 0;
        for (int i = 0; i < Count; ++i)
        {
            ProceduralData data = _DynamicRow.Data[i];

            switch (data.Type)
            {
                case EDataType.Boolean:
                    if(BooleanData == null)
                    {
                        BooleanData = new List<DynamicData_Boolean>();
                    }
                    index = BooleanData.Count;
                    BooleanData.Add(new DynamicData_Boolean(data, index));
                    break;
                case EDataType.Integer:
                    if (IntegerData == null)
                    {
                        IntegerData = new List<DynamicData_Integer>();
                    }
                    index = IntegerData.Count;
                    IntegerData.Add(new DynamicData_Integer(data, index));
                    break;
                case EDataType.Floating:
                    if (FloatData == null)
                    {
                        FloatData = new List<DynamicData_Float>();
                    }
                    index = FloatData.Count;
                    FloatData.Add(new DynamicData_Float(data, index));
                    break;
                case EDataType.String:
                    if (StringData == null)
                    {
                        StringData = new List<DynamicData_String>();
                    }
                    index = StringData.Count;
                    StringData.Add(new DynamicData_String(data, index));
                    break;
                case EDataType.Gameobject:
                    if (GameObjectnData == null)
                    {
                        GameObjectnData = new List<DynamicData_GameObject>();
                    }
                    index = GameObjectnData.Count;
                    GameObjectnData.Add(new DynamicData_GameObject(data, index));
                    break;
                case EDataType.Texture2D:
                    if (Texture2DData == null)
                    {
                        Texture2DData = new List<DynamicData_Texture2D>();
                    }
                    index = Texture2DData.Count;
                    Texture2DData.Add(new DynamicData_Texture2D(data, index));
                    break;
            }
            Data.Add(new ProceduralData(data, index));
        }
        return true;
    }

    //public void Load()
    //{
    //    if(m_bLoad)
    //    {
    //        return;
    //    }

    //    //foreach (ProceduralData data in Data)
    //    //{
    //    //    switch (data.Type)
    //    //    {
    //    //        case EDataType.Boolean:
    //    //            if (BooleanDataMap == null)
    //    //            {
    //    //                BooleanDataMap = new Dictionary<ProceduralData, DynamicData_Boolean>();
    //    //            }
    //    //            BooleanDataMap.Add(data, new DynamicData_Boolean(data));
    //    //            break;
    //    //        case EDataType.Integer:
    //    //            if (IntegerDataMap == null)
    //    //            {
    //    //                IntegerDataMap = new Dictionary<ProceduralData, DynamicData_Integer>();
    //    //            }
    //    //            IntegerDataMap.Add(data, new DynamicData_Integer(data));
    //    //            break;
    //    //        case EDataType.Floating:
    //    //            if (FloatDataMap == null)
    //    //            {
    //    //                FloatDataMap = new Dictionary<ProceduralData, DynamicData_Float>();
    //    //            }
    //    //            FloatDataMap.Add(data, new DynamicData_Float(data));
    //    //            break;
    //    //        case EDataType.String:
    //    //            if (StringDataMap == null)
    //    //            {
    //    //                StringDataMap = new Dictionary<ProceduralData, DynamicData_String>();
    //    //            }
    //    //            StringDataMap.Add(data, new DynamicData_String(data));
    //    //            break;
    //    //        case EDataType.Gameobject:
    //    //            if (GameObjectnDataMap == null)
    //    //            {
    //    //                GameObjectnDataMap = new Dictionary<ProceduralData, DynamicData_GameObject>();
    //    //            }
    //    //            GameObjectnDataMap.Add(data, new DynamicData_GameObject(data));
    //    //            break;
    //    //        case EDataType.Texture2D:
    //    //            if (Texture2DDataMap == null)
    //    //            {
    //    //                Texture2DDataMap = new Dictionary<ProceduralData, DynamicData_Texture2D>();
    //    //            }
    //    //            Texture2DDataMap.Add(data, new DynamicData_Texture2D(data));
    //    //            break;
    //    //    }
    //    //}
    //    m_bLoad = true;
    //}

    //public void Release()
    //{
    //    if(!m_bLoad)
    //    {
    //        return;
    //    }

    //    //if(BooleanDataMap != null)
    //    //    BooleanDataMap.Clear();
    //    //if(IntegerDataMap != null)
    //    //    IntegerDataMap.Clear();
    //    //if(FloatDataMap != null)
    //    //    FloatDataMap.Clear();
    //    //if(StringDataMap != null)
    //    //    StringDataMap.Clear();
    //    //if(GameObjectnDataMap != null)
    //    //    GameObjectnDataMap.Clear();
    //    //if(Texture2DDataMap != null)
    //    //    Texture2DDataMap.Clear();

    //    m_bLoad = false;
    //}

    private void OnDestroy()
    {
        if(BooleanData!=null)
            BooleanData.Clear();
        if(IntegerData != null)
            IntegerData.Clear();
        if(FloatData != null)
            FloatData.Clear();
        if (StringData != null)
            StringData.Clear();
        if (GameObjectnData != null)
            GameObjectnData.Clear();
        if (Texture2DData != null)
            Texture2DData.Clear();
    }

    /** Return the complete form of a procedural data. Need to be cast in the good type to be complete. */
    public ProceduralData GetDynamicData(ProceduralData _ProceduralData)
    {
        switch (_ProceduralData.Type)
        {
            case EDataType.Boolean:
                return BooleanData[_ProceduralData.Index];
            case EDataType.Integer:
                return IntegerData[_ProceduralData.Index];
            case EDataType.Floating:
                return FloatData[_ProceduralData.Index];
            case EDataType.String:
                return StringData[_ProceduralData.Index];
            case EDataType.Gameobject:
                return GameObjectnData[_ProceduralData.Index];
            case EDataType.Texture2D:
                return Texture2DData[_ProceduralData.Index];
        }
        return null;
    }

    public bool GetData<T>(string Name, ref T Out_Value)
    {
        //foreach(ProceduralData data in Data)
        //{
        //    if(data.Name != Name)
        //    {
        //        continue;
        //    }

        //    DynamicData<T> Buff = data as DynamicData<T>;
        //    if (Buff != null)
        //    {
        //        Out_Value = Buff.Value;
        //        return true;
        //    } 
        //}
        return false;
    }
}

///** When creating a new database, we can generate a procedural row according to one dynamic row. 
// * The procedural row is generate and use by the editor tools and is a comestible version of the dynamic row. */
//[System.Serializable]
//public class SO_ProceduralRow : ScriptableObject
//{
//    /** VARIABLES
//     */
//    // Complete version of the row's data.
//    public List<DynamicData> Data;


//    /** CONSTRUCTOR
//     */
//    public void Initialize(SO_DynamicRow _DynamicRow)
//    {
//        Data = new List<DynamicData>();
//        foreach (DynamicData data in _DynamicRow.Data)
//        {
//            switch(data.Type)
//            {
//                case EDataType.Boolean:
//                    Data.Add(new DynamicData_Boolean(data));
//                    break;
//                case EDataType.Integer:
//                    Data.Add(new DynamicData_Integer(data));
//                    break;
//                case EDataType.Floating:
//                    Data.Add(new DynamicData_Float(data));
//                    break;
//                case EDataType.String:
//                    Data.Add(new DynamicData_String(data));
//                    break;
//                case EDataType.Gameobject:
//                    Data.Add(new DynamicData_GameObject(data));
//                    break;
//                case EDataType.Texture2D:
//                    Data.Add(new DynamicData_Texture2D(data));
//                    break;
//            }
//        }
//    }
//}



