using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonoBehaviour_Utility : MonoBehaviour
{
    #region ENUM
    [System.Flags]
    public enum VerboseMask
    {
        Log = 1 << 0,
        WarningLog = 1 << 1,
        ErrorLog = 1 << 2,

        Gizmos = 1 << 3,
        Control = 1 << 4,
    }
    #endregion

    
    #region PARAMETERS
    [Header(".UTILITY/Debug")]
    [Layer]
    public VerboseMask DebugMode = VerboseMask.ErrorLog | VerboseMask.WarningLog;
    #endregion


    #region PROTECTED METHODS
    protected bool CompareMask(VerboseMask a, VerboseMask b)
    {
#if UNITY_EDITOR
        return (a & b) != 0;
#else
        return false;
#endif
    }

    protected bool HasFlag(VerboseMask flag)
    {
#if UNITY_EDITOR
        return (DebugMode & flag) != 0;
#else
        return false;
#endif
    }

    protected void Verbose(VerboseMask mask, string content)
    {
#if UNITY_EDITOR
        if (!CompareMask(DebugMode, mask))
        {
            return;
        }

        content = "[DEBUG]<" + this + ">: " + content;
        switch (mask)
        {
            case VerboseMask.Log:
                Debug.Log(content);
                break;
            case VerboseMask.WarningLog:
                Debug.LogWarning(content);
                break;
            case VerboseMask.ErrorLog:
                Debug.LogError(content);
                break;
            default:
                Debug.Log("[GIZMO]-" + content);
                break;
        }
#endif
    }
#endregion
}
