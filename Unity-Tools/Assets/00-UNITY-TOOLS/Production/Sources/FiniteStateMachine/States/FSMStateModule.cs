using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(FSMState))]
public abstract class FSMStateModule : MonoBehaviour_Utility
{
    public virtual void Initialize(DATA_BlackBoard blackBoard, GameObject target)
    { }
    public virtual void Enter()
    { }
    public virtual void UpdateState()
    { }
    public virtual void Exit()
    { }

    public abstract FSMStateModule_Preset GeneratePreset();

    public abstract void CopyPreset();

    [ContextMenu("Remove Component", false, 1)]
    public void RemoveComponentOverride()
    {
        Verbose(VerboseMask.WarningLog, "Please use the button to remove the state module.");
    }
}
