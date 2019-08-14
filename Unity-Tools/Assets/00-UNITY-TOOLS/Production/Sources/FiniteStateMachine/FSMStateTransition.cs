using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[ExecuteInEditMode]
public class FSMStateTransition : MonoBehaviour_Utility
{
    #region PROPERTIES
    [Header(".FSM STATE/Info")]
    //[SerializeField, TextArea]
    //private string _Description = "...";

#if UNITY_EDITOR
    [SerializeField, TextArea(1, 10)]
    private string _Log = "";
#endif

    [SerializeField, ReadOnly]
    private int ValidationTimes = 0; 

    [HideInInspector, SerializeField]
    private FSMState _CurrentState = null;

    [HideInInspector, SerializeField]
    private FSMState _NextState = null;

    [HideInInspector]
    public List<FSMTransitionCondition> Conditions = new List<FSMTransitionCondition>();

#if UNITY_EDITOR
#pragma warning disable
    [HideInInspector, SerializeField]
    private bool _CurrentlySubscribed = false;
#endif

    private bool _NeedInitialization = true;
#endregion


#region UNITY METHODS
    private void Awake()
    {
        _NeedInitialization = true;
    }

    private void OnDisable()
    {
        _NeedInitialization = true;
    }
#endregion

    private void InternalLog(string message)
    {
        _Log += "\n" + Time.timeSinceLevelLoad.ToString("0:00") + "> " + message;
    }

#region PUBLIC METHODS
    public void Init()
    {
        _CurrentState = GetComponentInParent<FSMState>();
        if(_CurrentState == null)
        {
            Verbose(VerboseMask.WarningLog, "There is no FSMState found in " + this + "'s parent.");
            return;
        }

        FiniteStateMachine FSM = _CurrentState.GetComponentInParent<FiniteStateMachine>();
        if(FSM == null)
        {
            Verbose(VerboseMask.WarningLog, "There is no FiniteStateMachine found in " + this + "'s parent.");
            return;
        }

        if(FSM.Blackboard == null)
        {
            Verbose(VerboseMask.WarningLog, "Invalid Blackboard in FSM '" + FSM.gameObject + "'.");
            return;
        }

        for (int i = 0; i < Conditions.Count; i++)
        {
            Conditions[i].Initialize(FSM.Blackboard, FSM.BlackboardTarget);
        }
    }

    public bool Subscribe()
    {
        _Validated = false;
        if (_NeedInitialization)
        {
#if UNITY_EDITOR
            if(HasFlag(VerboseMask.Log))
            {
                InternalLog("Initialization.");
            }
#endif
            Init();
            _NeedInitialization = false;
        }

        for (int i = 0; i < Conditions.Count; i++)
        {
#if UNITY_EDITOR
            if(HasFlag(VerboseMask.Log))
            {
                InternalLog("Subscribing [" + Conditions[i].ToString() + "].");
            }
#endif
            Conditions[i].Subscribe(ComputeCondition);
        }

        _CurrentlySubscribed = true;
        ComputeCondition();
        return _Validated;
    }

    public void UnSubscribe()
    {
        //if (_NeedInitialization)
        //{
        //    return;
        //}

        for (int i = 0; i < Conditions.Count; i++)
        {
#if UNITY_EDITOR
            if (HasFlag(VerboseMask.Log))
            {
                InternalLog("UnSubscribing [" + Conditions[i].name + "].");
            }
#endif
            Conditions[i].UnSubscribe(ComputeCondition);
        }
        _CurrentlySubscribed = false;
    }

    private bool _Validated = false;
    public bool Validated { get => _Validated; }

    public void ComputeCondition()
    {
        _Validated = false;
        if(_NextState == null)
        {
            Debug.Break();
            Debug.LogError(this + ": Next State is null! It can't be for a StateTransition!");
            return;
        }
        bool condition = true;

        for (int i = 0; i < Conditions.Count; i++)
        {
            condition &= Conditions[i].IsConditionValid();

#if UNITY_EDITOR
            if (HasFlag(VerboseMask.Log))
            {
                InternalLog("(" + i + ") " + Conditions[i].ToString() + " => " + condition);
            }
#endif
            if (!condition)
            {
                return;
            }
        }


        if (condition)
        {
#if UNITY_EDITOR
            if (HasFlag(VerboseMask.Log))
            {
                InternalLog("Conditions are validated! Passing to new states: " + _NextState);
            }
#endif
            //FiniteStateMachine FSM = _CurrentState.GetComponentInParent<FiniteStateMachine>();
            _Validated = true;
            ValidationTimes++;
            _CurrentState.SetState(_NextState);
        }
    }
    #endregion
}

//[SerializeField]
//List<TransitionBoolCondition> BoolConditions = new List<TransitionBoolCondition>();
//
//[SerializeField]
//List<TransitionIntCondition> IntConditions = new List<TransitionIntCondition>();
//
//[SerializeField]
//List<TransitionFloatCondition> FloatConditions = new List<TransitionFloatCondition>();
//
//[SerializeField]
//List<TransitionStringCondition> StringConditions = new List<TransitionStringCondition>();

//for (int i = 0; i < BoolConditions.Count; i++)
//    BoolConditions[i].Init(FSM);
//for (int i = 0; i < IntConditions.Count; i++)
//    IntConditions[i].Init(FSM);
//for (int i = 0; i < FloatConditions.Count; i++)
//    FloatConditions[i].Init(FSM);
//for (int i = 0; i < StringConditions.Count; i++)
//    StringConditions[i].Init(FSM);

//for (int i = 0; i < BoolConditions.Count; i++)
//    condition &= BoolConditions[i].CheckCondition();
//for (int i = 0; i < IntConditions.Count; i++)
//    condition &= IntConditions[i].CheckCondition();
//for (int i = 0; i < FloatConditions.Count; i++)
//    condition &= FloatConditions[i].CheckCondition();
//for (int i = 0; i < StringConditions.Count; i++)
//    condition &= StringConditions[i].CheckCondition();

//for (int i = 0; i < BoolConditions.Count; i++)
//    BoolConditions[i].UnSubscribe(ComputeCondition);
//for (int i = 0; i < IntConditions.Count; i++)
//    IntConditions[i].UnSubscribe(ComputeCondition);
//for (int i = 0; i < FloatConditions.Count; i++)
//    FloatConditions[i].UnSubscribe(ComputeCondition);
//for (int i = 0; i < StringConditions.Count; i++)
//    StringConditions[i].UnSubscribe(ComputeCondition);

//for (int i = 0; i < BoolConditions.Count; i++)
//    BoolConditions[i].Subscribe(ComputeCondition);
//for (int i = 0; i < IntConditions.Count; i++)
//    IntConditions[i].Subscribe(ComputeCondition);
//for (int i = 0; i < FloatConditions.Count; i++)
//    FloatConditions[i].Subscribe(ComputeCondition);
//for (int i = 0; i < StringConditions.Count; i++)
//    StringConditions[i].Subscribe(ComputeCondition);

//private void Awake()
//{
//    state = GetComponentInParent<State>();
//    FSM = state.GetComponentInParent<FiniteStateMachine>();

//    for (int i = 0; i < BoolConditions.Count; i++)
//        BoolConditions[i].Init(FSM);
//    for (int i = 0; i < IntConditions.Count; i++)
//        IntConditions[i].Init(FSM);
//    for (int i = 0; i < FloatConditions.Count; i++)
//        FloatConditions[i].Init(FSM);
//    for (int i = 0; i < StringConditions.Count; i++)
//        StringConditions[i].Init(FSM);
//}