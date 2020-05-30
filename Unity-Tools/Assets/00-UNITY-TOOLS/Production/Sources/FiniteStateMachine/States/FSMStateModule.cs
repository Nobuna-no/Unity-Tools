using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(FSMState))]
public abstract class FSMStateModule : MonoBehaviour_Utility
{
    #region PROPERTIES
    private GameObject _Owner;
    public GameObject Owner { get => _Owner; }

    private DATA_BlackBoard _Blackboard;
    public DATA_BlackBoard Blackboard { get => _Blackboard; }
    #endregion

    #region PUBLIC METHODS
    /// <summary>
    /// Methods to initialize BBP_ or BBTP_. (BlackboardParameter or BlackboardTargetParameter)
    /// </summary>
    /// <param name="blackBoard"></param>
    /// <param name="target"></param>
    public virtual void Initialize(DATA_BlackBoard blackBoard, GameObject target)
    {
        _Owner = target;
        _Blackboard = blackBoard;
        //CopyPreset();
    }

    public virtual void OnStateBegin()
    { }
    public virtual void OnStateUpdate(float deltaTime)
    { }
    public virtual void OnStateExit()
    { }


    //public abstract FSMStateModule_Preset GeneratePreset();

    //public abstract void CopyPreset();

    [ContextMenu("Remove Component", false, 1)]
    public void RemoveComponentOverride()
    {
        Verbose(VerboseMask.WarningLog, "Please use the button to remove the state module.");
    }
    #endregion

}
