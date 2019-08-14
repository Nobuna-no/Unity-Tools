using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Animations;
using UnityEditor;

[ExecuteInEditMode]
public class FSMState : MonoBehaviour_Utility
{
    #region CLASS
    [System.Serializable]
    protected class StateEvent
    {
        public UnityEvent Enter;
        public UnityEvent Update;
        public UnityEvent Exit;
    }
    #endregion


    #region PROPERTIES
    [Header(".FSM STATE/Info")]
    [SerializeField, TextArea(1,5)]
    protected string _Description;

    [Header(".FSM STATE/Settings")]
    [SerializeField]
    private bool _AutoRefreshTransitionAtRuntime = true;

    [SerializeField]
    private List<FSMStateTransition> _Transitions = new List<FSMStateTransition>();
    public List<FSMStateTransition> Transitions { get => _Transitions; }

    [SerializeField]
    protected StateEvent _Events;

    protected FiniteStateMachine _SubFSMOwner = null;
    public FiniteStateMachine SubFSMOwner { get => _SubFSMOwner; }

    [SerializeField, HideInInspector]
    protected List<FSMStateModule> StateModules;

    private bool _NeedInitialization = true;
    public bool NeedInitialization { get => _NeedInitialization; }
    #endregion


    #region UNITY METHODS
    protected virtual void Awake()
    {
        _NeedInitialization = true;
    }

    protected virtual void Start()
    {
        if (Application.isPlaying)
        {
            _SubFSMOwner = GetComponentInParent<FiniteStateMachine>();
            if (!_SubFSMOwner)
            {
                Debug.Break();
                Verbose(VerboseMask.ErrorLog, "No FSM found in parent of " + gameObject + "!");
                return;
            }

            Initialize(_SubFSMOwner.Blackboard, _SubFSMOwner.BlackboardTarget);
        }
    }

    protected virtual void OnEnable()
    {
        if (_AutoRefreshTransitionAtRuntime)
        {
            gameObject.GetComponentsInChildren<FSMStateTransition>(_Transitions);
        }
    }

    protected virtual void OnDisable()
    {
        _NeedInitialization = true;
    }
    #endregion


    #region VIRTUAL METHODS    
    /// <summary>
    /// Methods to initialize BBP_ or BBTP_. (BlackboardParameter or BlackboardTargetParameter)
    /// </summary>
    /// <param name="blackBoard">The blackboard to link.</param>
    /// <param name="target">The target corresponding to the key of the wanted values.</param>
    public virtual void Initialize(DATA_BlackBoard blackBoard, GameObject target)
    {
        for (int i = 0, c = StateModules.Count; i < c; ++i)
        {
            StateModules[i].Initialize(blackBoard, target);
        }
        _NeedInitialization = false;
    }
  
    
    /// <summary>
    /// Calls each time this state is set as current in the FSM. First call at FSM Start().
    /// </summary>
    public virtual bool Enter()
    {
#if UNITY_EDITOR
        Verbose(VerboseMask.Log, "Enter() > Transition count["+ _Transitions.Count + "].");
#endif
        _Events.Enter.Invoke();

        for (int i = 0; i < _Transitions.Count; i++)
        {
            // if the subscription condition are validated.
            if(_Transitions[i].Subscribe())
            {
                return false;
            }
        }

        for (int i = 0, c = StateModules.Count; i < c; ++i)
        {
            StateModules[i].Enter();
        }

        return true;
    }
    
    /// <summary>
    /// Calls each update tick of the FSM. The update tick can be setup from the FSM parameters (default = 0.01f).
    /// </summary>
    public virtual void UpdateState()
    {
        _Events.Update.Invoke();

        for (int i = 0, c = StateModules.Count; i < c; ++i)
        {
            StateModules[i].UpdateState();
        }
    }

    /// <summary>
    /// Calls each time the FSM quit the state.
    /// </summary>
    public virtual void Exit()
    {
        _Events.Exit.Invoke();

        for (int i = 0; i < _Transitions.Count; i++)
        {
            _Transitions[i].UnSubscribe();
        }

        for(int i = 0, c = StateModules.Count; i < c; ++i)
        {
            StateModules[i].Exit();
        }
    }

    /// <summary>
    /// Usefull methods that allow to enable a blackboard value from the inspector (through events).
    /// </summary>
    /// <param name="entryName"></param>
    public virtual void EnableBooleanBlackboardValue(StringVariable entryName)
    {
        if (entryName != null)
        {
            if (_SubFSMOwner != null)
            {
                Verbose(VerboseMask.Log, "Enable \"" + entryName + "\" blackboard value!");
                _SubFSMOwner.Blackboard.SetValue<bool, BoolVariable>(_SubFSMOwner.BlackboardTarget, entryName, true);
            }
        }
    }

    /// <summary>
    ///  Usefull methods that allow to disable a blackboard value from the inspector (through events).
    /// </summary>
    /// <param name="entryName"></param>
    public virtual void DisableBooleanBlackboardValue(StringVariable entryName)
    {
        if (entryName != null)
        {
            if (_SubFSMOwner != null)
            {
                Verbose(VerboseMask.Log, "Disable \"" + entryName + "\" blackboard value!");
                _SubFSMOwner.Blackboard.SetValue<bool, BoolVariable>(_SubFSMOwner.BlackboardTarget, entryName, false);
            }
        }
    }

    /// <summary>
    /// Try to change state.
    /// </summary>
    /// <param name="state">The state to transit.</param>
    public virtual void SetState(FSMState state)
    {
        SubFSMOwner?.SetState(state);
    }
    #endregion

    
    #region PUBLIC METHODS
    public FSMStateModule_Preset GenerateStateModulePreset(int index)
    {
        if (index > StateModules.Count - 1)
        {
            return null;
        }

        return StateModules[index]?.GeneratePreset();
    }

    public void RemoveStateModule(int index)
    {
        if (index > StateModules.Count - 1)
        {
            return;
        }

        if (StateModules[index] != null)
        {
            FSMStateModule module = StateModules[index];
            StateModules.RemoveAt(index);
            DestroyImmediate(module);
        }
        else
        {
            StateModules.RemoveAt(index);
        }
    }

    public void UpdateStateModulePreset(int index)
    {
        if (index > StateModules.Count - 1)
        {
            return;
        }

        StateModules[index]?.CopyPreset();
    }

    public void AddStateModule(FSMStateModule module)
    {
        CheckModule();

        if (module == null)
        {
            Debug.Break();
            Verbose(VerboseMask.ErrorLog, "AddStateModule error: module is null!");
            return;
        }

        StateModules.Add(module);
        module.hideFlags = HideFlags.HideInInspector;
    }
    #endregion


    #region PRIVATE METHODS
    private void CheckModule()
    {
        if (StateModules == null)
        {
            StateModules = new List<FSMStateModule>();
            return;
        }

        for (int i = StateModules.Count - 1; i >= 0; --i)
        {
            if (StateModules[i] == null)
            {
                StateModules.RemoveAt(i);
            }
        }
    }
    #endregion
}
