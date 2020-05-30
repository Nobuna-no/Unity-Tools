using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[ExecuteInEditMode]
public class FiniteStateMachine : FSMState
{
    #region PROPERTIES
    [Header(".FSM/Settings")]

    //private DATA_BlackBoard BlackBoard;
    [SerializeField]
    private DATA_BlackBoard _Blackboard;
    public DATA_BlackBoard Blackboard { get => _Blackboard; }
    [SerializeField]
    private GameObject _BlackboardTarget;
    public GameObject BlackboardTarget { get => _BlackboardTarget; }

    // Entry or current state.
    [SerializeField]
    private FSMState _InitialState = null;

    [SerializeField]
    private FloatReference _UpdateTickInSecond = new FloatReference(0.01f);

    [Header(".FSM/Infos")]
    [SerializeField, ReadOnly]
    private FSMState _CurrentState = null;
    [SerializeField, ReadOnly]
    private bool IsPrimaryFSM = false;

    [ReadOnly, TextArea, SerializeField]
    private string FSMStatus;

#if UNITY_EDITOR
    [SerializeField, ReadOnly]
    private bool _UpdateSignal = false;
#endif

    [SerializeField, ReadOnly]
    private float _RuntimeDeltaTime = 0f;
    private Coroutine _UpdateCrt;
    private float _LastTime = 0f;
    #endregion


    #region UNITY METHODS
    protected override void Start()
    {
        // We need to cancel basic FSMState Start().
    }
   
    protected override void OnEnable()
    {
        FiniteStateMachine[] parents = GetComponentsInParent<FiniteStateMachine>(false);
        // if there is no FSM parent, it means that this FSM is the primary one.
        if (parents.Length == 1) // If only itself...
        {
            if (_Description != null && !_Description.Contains("<Primary State Machine>"))
            {
                _Description = "<Primary State Machine>\n" + _Description;
            }
            FSMStatus = this.name + " >>";

            IsPrimaryFSM = true;

            if (Application.isPlaying)
            {
                if (!_Blackboard)
                {
                    Debug.Break();
                    Verbose(VerboseMask.ErrorLog, this + ": No blackboard set in the primary FSM!");
                }
                if(!_BlackboardTarget)
                {
                    Verbose(VerboseMask.WarningLog, this + ": No blackboard target set in the primary FSM. Taking own gameobject as target!");
                    _BlackboardTarget = this.gameObject;
                }
                // And so, auto launch it.
                base.Initialize(_Blackboard, _BlackboardTarget);
                Enter();
            }
        }
        else
        {
            string str = "";
            for (int i = parents.Length - 1; i > 0; --i)
            {
                str += parents[i].name + " > ";
            }
            FSMStatus = str + this.name;

            if (_Description != null && !_Description.Contains("<SubState>"))
            {
                _Description = "<SubState>\n" + _Description;
            }
            IsPrimaryFSM = false;

            FiniteStateMachine FSM = parents[parents.Length - 1];
            _SubFSMOwner = FSM;
            if (!Application.isPlaying)
            {
                Initialize(FSM.Blackboard, FSM.BlackboardTarget);
            }
        }
    }

    protected override void OnDisable()
    {
        base.OnDisable();
        Exit();
    }
    #endregion


    #region FSMSTATE METHODS
    public override void Initialize(DATA_BlackBoard blackBoard, GameObject target)
    {
        if (!blackBoard || !target)
        {
            Debug.Break();
            Verbose(VerboseMask.ErrorLog, this + ": No blackboard or No blackboard target set in the primary FSM!");
        }

        _Blackboard = blackBoard;
        _BlackboardTarget = target;

        base.Initialize(blackBoard, target);
    }


    public override bool Enter()
    {
        if(!base.Enter())
        {
            return false;
        }

        _CurrentState = _InitialState;

        if(_CurrentState.NeedInitialization)
        {
            _CurrentState?.Initialize(_Blackboard, _BlackboardTarget);
        }
        _CurrentState?.Enter();


        _UpdateCrt = StartCoroutine(Update_Coroutine());

        return true;
    }

    public override void Exit()
    {
        _CurrentState?.Exit();
        _CurrentState = null;

        if(_UpdateCrt != null)
        {
            StopCoroutine(_UpdateCrt);
            _UpdateCrt = null; 
        }

        base.Exit();
    }

    public override void EnableBooleanBlackboardValue(StringVariable entryName)
    {
        if (entryName != null)
        {
            Verbose(VerboseMask.Log, "Enable \"" + entryName + "\" blackboard value!");
            _Blackboard.SetValue<bool, BoolVariable>(_BlackboardTarget, entryName, true);
        }
    }

    public override void DisableBooleanBlackboardValue(StringVariable entryName)
    {
        if (entryName != null)
        {
            Verbose(VerboseMask.Log, "Disable \"" + entryName + "\" blackboard value!");
            _Blackboard.SetValue<bool, BoolVariable>(_BlackboardTarget, entryName, false);
        }
    }
    #endregion


    #region PUBLIC METHODS
    public override void SetState(FSMState state)
    {
        if (!IsPrimaryFSM)
        {
            // if current subFSM is state's parent.
            if (state.SubFSMOwner == this)
            {
                _CurrentState?.Exit();
                _CurrentState = state;
                _CurrentState.Enter();
                return;
            }

            FiniteStateMachine SubFSM = state as FiniteStateMachine;
            if (SubFSM != null)
            {
                if (!SubFSM.IsPrimaryFSM)
                {
                    // If same FSM parent.
                    if (SubFSM.SubFSMOwner == _SubFSMOwner)
                    {
                        _SubFSMOwner.SetState(state);
                    }
                    // if not.
                    else
                    {
                        Verbose(VerboseMask.ErrorLog, "Trying to set a substate machine(" + state.SubFSMOwner.name + ") from an invalid one. You must transit substates layer by layer (" + state.SubFSMOwner.FSMStatus + ").");
                    }
                }
                // if current state is a superior FSM
                else if (SubFSM == _SubFSMOwner)
                {
                    Exit();
                    state.Enter();
                }
                return;
            }
        }

        if (state.SubFSMOwner == this)
        {
            _CurrentState?.Exit();
            _CurrentState = state;
            _CurrentState.Enter();
        }
        else if (state.SubFSMOwner != null)
        {
            state.SubFSMOwner.SetState(state);
        }
        else
        {
            Verbose(VerboseMask.ErrorLog, "State hasn't a valid SubFSM and " + this + " isn't either a valid one. What did you do?");
        }
    }

    public void ExitSubStateMachine()
    {
        if (!IsPrimaryFSM)
        {
            Exit();
            FiniteStateMachine[] parents = GetComponentsInParent<FiniteStateMachine>(false);
            parents[1].Enter();
        }
        else
        {
            Verbose(VerboseMask.WarningLog, "Trying to exit primary state machine. It can't be.");
        }
    }
    #endregion


    #region COROUTINE
    private IEnumerator Update_Coroutine()
    {
        while (true)
        {
            _LastTime = Time.realtimeSinceStartup;
            yield return new WaitForSeconds(_UpdateTickInSecond);

            _RuntimeDeltaTime = Time.realtimeSinceStartup - _LastTime;

#if UNITY_EDITOR
            _UpdateSignal = !_UpdateSignal;
#endif
            
            base.UpdateState(_RuntimeDeltaTime);
            _CurrentState?.UpdateState(_RuntimeDeltaTime);
        }
    }
    #endregion
}


///________________________________________________________________________________________RIP
//[SerializeField, Tooltip("All transitions must be always checked.")]
//private FSMStateTransition[] _AnyStateTransition = null;

//    // We want to call this only in certain case. See OnEnable().
//    if(!IsPrimaryFSM)
//    {
//        base.Start();
//    }
//}


// void Update()
// {
//     if (CurrentState)
//         CurrentState.UpdateState();
// }


//ResetFiniteStateMachine();

//public void ResetFiniteStateMachine()
//{
//    if (!_Blackboard)
//    {
//        Debug.Break();
//        Verbose(VerboseMask.ErrorLog, this + ": No blackboard set!");
//    }

//    _CurrentState = _InitialState;

//    //ResetBinding();

//    //CurrentState?.Enter();
//    //if (CurrentState)
//    //    CurrentState.Enter();
//}

// SETTER


// GETTER
//public DATA_BlackBoard Blackboard
//{
//    return _Blackboard;
//}

/*private void ResetBinding()
{
    for (int i = 0; i < _AnyStateTransition.Length; ++i)
    {
        _AnyStateTransition[i].UnSubscribe();
    }

    for (int i = 0; i < _AnyStateTransition.Length; ++i)
    {
        //_AnyStateTransition[i].Init();
        _AnyStateTransition[i].Subscribe();
    }
}*/

//_CurrentState?.Exit();
//_CurrentState = null;
//
//base.Exit();
//for (int i = 0; i < _AnyStateTransition.Length; ++i)
//{
//    _AnyStateTransition[i].UnSubscribe();
//}

/*public BoolVariable GetBool(string key)
{
    BoolVariable b = InternalBlackBoard.GetBoolVariable(key);
    if(b != null)
    {
        return InternalBlackBoard.GetBoolVariable(key);
    }
    else
    {
        Debug.Break();
        Verbose(VerboseMask.ErrorLog, "Failed to get boolean from key [" + key+"]!");
        return null;
    }

    //if (BoolVariables.ContainsKey(key))
    //{
    //    if(BoolVariables[key] != null)
    //    return BoolVariables[key];
    //}
    //
    //return null;
}

public IntVariable GetInteger(string key)
{
    return InternalBlackBoard.GetIntVariable(key);
}

public FloatVariable GetFloat(string key)
{
    return InternalBlackBoard.GetFloatVariable(key);
}

public StringVariable GetString(string key)
{
    return InternalBlackBoard.GetStringVariable(key);
}

public Vector2Variable GetVector2(string key)
{
    return InternalBlackBoard.GetVector2Variable(key);
}

public Vector3Variable GetVector3(string key)
{
    return InternalBlackBoard.GetVector3Variable(key);
}

public GameObjectVariable GetGameObject(string key)
{
    return InternalBlackBoard.GetGameObjectVariable(key);
}*/

//if(!BlackBoard)
//{
//    Debug.Break();
//    Debug.LogError(this + ": No blackboard set!");
//}
//
//InternalBlackBoard = BlackBoard.Clone() as DATA_BlackBoard;
//CurrentState = InitialState;
/*if (BoolVariablesList.Count != 0)
    BoolVariables = new Dictionary<string, BoolVariable>();

if (IntVariablesList.Count != 0)
    IntVariables = new Dictionary<string, IntVariable>();

if (FloatVariablesList.Count != 0)
    FloatVariables = new Dictionary<string, FloatVariable>();

if (StringVariablesList.Count != 0)
    StringVariables = new Dictionary<string, StringVariable>();

for (int i = 0; i < BoolVariablesList.Count; i++)
{
    BlackboardParameter_Boolean dicoVariable = BoolVariablesList[i];
    BoolVariable variable = dicoVariable.MustBeShared ? dicoVariable.Value : ScriptableObject.CreateInstance<BoolVariable>();
    if (!dicoVariable.MustBeShared && dicoVariable.Value != null)
    {
        variable.Value = dicoVariable.Value;
    }
    BoolVariables.Add(dicoVariable.Name, variable);
}

for (int i = 0; i < IntVariablesList.Count; i++)
{
    BlackboardParameter_Int dicoVariable = IntVariablesList[i];
    IntVariable variable = dicoVariable.MustBeShared ? dicoVariable.Value : ScriptableObject.CreateInstance<IntVariable>();
    if (!dicoVariable.MustBeShared && dicoVariable.Value != null)
        variable.Value = dicoVariable.Value;

    IntVariables.Add(dicoVariable.Name, variable);
}

for (int i = 0; i < FloatVariablesList.Count; i++)
{
    BlackboardParameter_Float dicoVariable = FloatVariablesList[i];
    FloatVariable variable = dicoVariable.MustBeShared ? dicoVariable.Value : ScriptableObject.CreateInstance<FloatVariable>();
    if (!dicoVariable.MustBeShared && dicoVariable.Value != null)
        variable.Value = dicoVariable.Value;

    FloatVariables.Add(dicoVariable.Name, variable);
}

for (int i = 0; i < StringVariablesList.Count; i++)
{
    BlackboardParameter_String dicoVariable = StringVariablesList[i];
    StringVariable variable = dicoVariable.MustBeShared ? dicoVariable.Value : ScriptableObject.CreateInstance<StringVariable>();
    if (!dicoVariable.MustBeShared && dicoVariable.Value != null)
        variable.Value = dicoVariable.Value;

    StringVariables.Add(dicoVariable.Name, variable);
}*/

//[SerializeField]
//private List<BlackboardParameter_Boolean> BoolVariablesList = new List<BlackboardParameter_Boolean>();
//
//[SerializeField]
//private List<BlackboardParameter_Int> IntVariablesList = new List<BlackboardParameter_Int>();
//
//[SerializeField]
//private List<BlackboardParameter_Float> FloatVariablesList = new List<BlackboardParameter_Float>();
//
//[SerializeField]
//private List<BlackboardParameter_String> StringVariablesList = new List<BlackboardParameter_String>();
//
//private Dictionary<string, BoolVariable> BoolVariables;
//private Dictionary<string, IntVariable> IntVariables;
//private Dictionary<string, FloatVariable> FloatVariables;
//private Dictionary<string, StringVariable> StringVariables;

//try
//{
//    // Special case ennemy...
//    SetBoolVariableRef("Landing", GetComponentInParent<UNIT_MomentumSystem2D>().IsLanding);
//    SetBoolVariableRef("TargetSpotted", AgroZone.TargetDetected);
//}
//catch
//{
//    Verbose(VerboseMask.WarningLog, "Failed to get falling bool variable ref from Momentum2D Sytem.");
//}