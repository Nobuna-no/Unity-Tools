using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[System.Serializable]
public class SO_ProceduralRowList : ScriptableObject
{
    /** VARIABLES
     */
    public List<SO_DynamicRow> Rows;

    //[System.NonSerialized]
    public SO_DynamicRow RowStructure = null;
    //[System.NonSerialized]
    public bool bGenerateDataObject = true;

    public string DataSavePath = "Assets/";
}

/**
 public class SO_UIDProceduralRowList : SO_ProceduralRowList
    {
        public Dictionnary<int, SO_DynamicRow> MapRow;
        
    }
     
     
     */
