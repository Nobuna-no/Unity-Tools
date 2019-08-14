using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;


public class FSMStateModule_WaitFor : FSMStateModule
{
    #region TYPES
   
    [System.Serializable]
    public class Attributes
    {
        public MinMaxRange DurationInSecond = new MinMaxRange(0, 10);
        public bool UseSecondsRealtime = false;
    }
    #endregion


    #region PROPERTIES
    [Header(".FSM STATE MODULE/Wait For")]
    [SerializeField]
    private FSMStateModule_WaitFor_Preset _Preset = null;
    [SerializeField]
    private Attributes _Data = null;
    public FSMState _AutoTransitionNextState;

    [Header(".FSM STATE MODULE/Wait For - Event")]
    [SerializeField]
    public UnityEvent OnWaitCompleted;

#if UNITY_EDITOR
    [Header(".FSM STATE MODULE/Wait For - Infos")]
    [SerializeField, ReadOnly]
    private float LastWaitingTime;
#endif
    [SerializeField, ReadOnly]
    private bool _CancelCoroutine = false;
    #endregion


    #region PUBLIC METHODS
    public override void Initialize(DATA_BlackBoard blackBoard, GameObject target)
    {
        if (_Preset != null)
        {
            if (_Data == null)
            {
                _Data = new Attributes();
            }
            if(_Preset.Data != null)
            {
                _Data.DurationInSecond = _Preset.Data.DurationInSecond;
                _Data.UseSecondsRealtime = _Preset.Data.UseSecondsRealtime;
            }
        }

        if(_Data == null)
        {
            Debug.Break();
            Verbose(VerboseMask.ErrorLog, "Data is null!");
        }
    }

    public override void Enter()
    {
        _CancelCoroutine = false;
        Verbose(VerboseMask.Log, "Enter WaitFor Module() + _CancelCoroutine = " + _CancelCoroutine);
        StartCoroutine(WaitFor_Coroutine());
    }

    public override void Exit()
    {
        _CancelCoroutine = true;
        Verbose(VerboseMask.Log, "Exit WaitFor Module()  + _CancelCoroutine = " + _CancelCoroutine);
    }

    public override FSMStateModule_Preset GeneratePreset()
    {
        FSMStateModule_WaitFor_Preset instance = ScriptableObject.CreateInstance<FSMStateModule_WaitFor_Preset>();
        instance.Data = new Attributes();
        instance.Data.DurationInSecond = _Data.DurationInSecond;
        instance.Data.UseSecondsRealtime = _Data.UseSecondsRealtime;
        return instance;
    }

    [ContextMenu("Copy please!")]
    public override void CopyPreset()
    {
        _Data.DurationInSecond = _Preset.Data.DurationInSecond;
        _Data.UseSecondsRealtime = _Preset.Data.UseSecondsRealtime;
    }
    #endregion

    #region Coroutine
    private IEnumerator WaitFor_Coroutine()
    {
        if (_Data.UseSecondsRealtime)
        {
#if UNITY_EDITOR
            yield return new WaitForSecondsRealtime(LastWaitingTime = _Data.DurationInSecond.Draw());
#else
            yield return new WaitForSecondsRealtime(_Data._DurationInSecond.Draw());
#endif
        }
        else
        {
#if UNITY_EDITOR
            yield return new WaitForSeconds(LastWaitingTime = _Data.DurationInSecond.Draw());
#else
            yield return new WaitForSeconds(_Data._DurationInSecond.Draw());
#endif
        }

        if (!_CancelCoroutine && _Data != null)
        {
            Verbose(VerboseMask.Log, "Complete coroutine! + _CancelCoroutine = " + _CancelCoroutine);
            OnWaitCompleted?.Invoke();

            if (_AutoTransitionNextState != null)
            {
                Verbose(VerboseMask.Log, "Auto transition to \"" + _AutoTransitionNextState.name + "\"");
                GetComponent<FSMState>()?.SetState(_AutoTransitionNextState);
            }
        }
        else
        {
            Verbose(VerboseMask.Log, "Cancel coroutine!");
        }
    }
    #endregion
}