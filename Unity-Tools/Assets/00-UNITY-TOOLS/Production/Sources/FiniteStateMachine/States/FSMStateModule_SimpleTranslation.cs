using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;


public class FSMStateModule_SimpleTranslation : FSMStateModule
{
    #region CLASS
    [System.Serializable]
    public class Attributes
    {
        public Attributes()
        {
            Distance = new Vector3Reference();
            XVelocity = new CurveReference();
            YVelocity = new CurveReference();
            ZVelocity = new CurveReference();
            DurationInSeconds = new FloatReference(1f);
        }

        public Vector3Reference Distance;
        public CurveReference XVelocity;
        public CurveReference YVelocity;
        public CurveReference ZVelocity;

        public FloatReference DurationInSeconds;
    }
    #endregion


    #region PROPERTIES
    [Header(".FSM STATE MODULE/Simple Translation")]
    public Vector3Reference Distance;
    public CurveReference XVelocity;
    public CurveReference YVelocity;
    public CurveReference ZVelocity;

    public FloatReference DurationInSeconds;

    [Header(".FSM STATE MODULE/Simple Translation - Event")]
    [SerializeField]
    private UnityEvent _OnTranslationComplete = null;


    [Header(".FSM STATE MODULE/Simple Translation - Infos")]
    private float _TimeStamp = 0;
    private Vector3 _Origin;
    #endregion

    #region PUBLIC METHODS
    public override void OnStateBegin()
    {
        _TimeStamp = 0;
        _Origin = Owner.transform.position;
    }

    public override void OnStateUpdate(float deltaTime)
    {
        if (_TimeStamp >= DurationInSeconds)
        {
            return;
        }

        _TimeStamp += deltaTime;

        Vector3 currentPos;
        currentPos.x = _Origin.x + (Distance.Value.x * XVelocity.Value.Evaluate(_TimeStamp / DurationInSeconds));
        currentPos.y = _Origin.y + (Distance.Value.y * YVelocity.Value.Evaluate(_TimeStamp / DurationInSeconds));
        currentPos.z = _Origin.z + (Distance.Value.z * ZVelocity.Value.Evaluate(_TimeStamp / DurationInSeconds));

        Owner.transform.position = currentPos;

        if (_TimeStamp >= DurationInSeconds)
        {
            _OnTranslationComplete?.Invoke();
        }
    }
    public override void OnStateExit()
    { }

    //public override FSMStateModule_Preset GeneratePreset()
    //{
    //    //FSMStateModule_SimpleTranslation_Preset instance = ScriptableObject.CreateInstance<FSMStateModule_SimpleTranslation_Preset>();
    //    //instance.Data = new Attributes();
    //    //instance.Data.XVelocity = _Data.XVelocity;
    //    //instance.Data.YVelocity = _Data.YVelocity;
    //    //instance.Data.ZVelocity = _Data.ZVelocity;
    //    //instance.Data.Distance = _Data.Distance;
    //    //instance.Data.DurationInSeconds = _Data.DurationInSeconds;
    //    return null;
    //}

    //[ContextMenu("Copy please!")]
    //public override void CopyPreset()
    //{
    //    //if (_Preset != null && _Preset.Data != null)
    //    //{
    //    //    _Data.XVelocity = _Preset.Data.XVelocity;
    //    //    _Data.YVelocity = _Preset.Data.YVelocity;
    //    //    _Data.ZVelocity = _Preset.Data.ZVelocity;
    //    //    _Data.Distance = _Preset.Data.Distance;
    //    //    _Data.DurationInSeconds = _Preset.Data.DurationInSeconds;
    //    //}
    //}
    #endregion

}
