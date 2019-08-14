using UnityEngine;

[CreateAssetMenu(menuName = "Framework Data/Input/Key", order = 0)]
public class DATA_Key : BoolVariable
{
    [Header(".DATA/Key")]
    public string Name;
    public string Description;
    [Space]
    [Space]
    public KeyCode[] KeyCodes;

    /// <summary>
    /// (Value == true) mean that the key is down.
    /// Time use when Value is true means the time since it's pressed. 
    /// Time use when Value is false means the duration since the key was released.
    /// </summary>
    [System.NonSerialized]
    public float Timestamp;

    /// <summary>
    /// Force value to false and reset timestamp.
    /// </summary>
    public void ConsumeInput()
    {
        Value = false;
        Timestamp = 0;
    }
}

/*[System.Serializable]
   public struct RegionKeycodes
   {
       public SystemLanguage Region;
       public KeyCode[] KeyCodes;
   }*/
/* private bool ReleaseInput = true;
 //private bool InputActive = false;

 [Range(0.05f, 0.3f)]
 public float GhostingDuration = 0.15f;

 public bool ConsumeInput()
 {
     if(Value || Timestamp <= GhostingDuration)
     {
         //InputActive = true;

         ReleaseInput = false;
         Value = false;
         return true;
     }

     return false;
 }*/
