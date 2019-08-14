using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;


public class FSMStateModule_Move2D : FSMStateModule
{
    #region TYPES
    public enum EMoveValueInterpretation
    {
        Acceleration,
        Duration,
    }

    [System.Flags]
    public enum EMoveAxis
    {
        X = 1 << 0,
        Y = 1 << 1,
    }

    [System.Serializable]
    public class Attributes
    {
        [Tooltip("Must the move be repeated?")]
        public bool Loop = false;
        [Layer]
        public EMoveAxis MoveAxis = EMoveAxis.X;
        public CurveReference XVelocity;
        public CurveReference YVelocity;

        public Vector2Reference MaxSpeed = new Vector2Reference(new Vector2(1, 1));
        public EMoveValueInterpretation UseMoveValueAs;
        public Vector2Reference MoveValues = new Vector2Reference(new Vector2(1, 1));
        [SerializeField, Range(0, 1)]
        public float LerpSpeed = 0.5f;
    }
    #endregion


    #region PROPERTIES
    [Header(".FSM STATE MODULE/Move 2D")]
    [SerializeField]
    private FSMStateModule_Move2D_Preset _Preset = null;

    [SerializeField]
    private Attributes _Data = null;

    [Header(".FSM STATE MODULE/Move - Blackboard Value")]
    [SerializeField]
    private BBP_Float _Forward = null;

    [Header(".FSM STATE MODULE/Move - Event")]
    [SerializeField]
    private UnityEvent _OnCompleteTimestamp = null;


    [Header(".FSM STATE MODULE/Move - Infos")]
    [SerializeField, ReadOnly]
    private Rigidbody2D _OwnRigidbody;
    [SerializeField, ReadOnly]
    private Vector2 _Stamps;
#if UNITY_EDITOR
    [SerializeField, ReadOnly]
    private float _DeltaTime = 0;
#endif
    #endregion


    #region PUBLIC METHODS
    public override FSMStateModule_Preset GeneratePreset()
    {
        FSMStateModule_Move2D_Preset instance = ScriptableObject.CreateInstance<FSMStateModule_Move2D_Preset>();
        Fill(instance);
        return instance;
    }

    public void Fill(FSMStateModule_Move2D_Preset target)
    {
        target.Data = new Attributes();
        target.Data.Loop = _Data.Loop;
        target.Data.MoveAxis = _Data.MoveAxis;
        target.Data.XVelocity = _Data.XVelocity;
        target.Data.YVelocity = _Data.YVelocity;
        target.Data.MaxSpeed = _Data.MaxSpeed;
        target.Data.UseMoveValueAs = _Data.UseMoveValueAs;
        target.Data.MoveValues = _Data.MoveValues;
        target.Data.LerpSpeed = _Data.LerpSpeed;
    }

    [ContextMenu("Copy please!")]
    public override void CopyPreset()
    {
        _Data.Loop = _Preset.Data.Loop;
        _Data.MoveAxis = _Preset.Data.MoveAxis;
        _Data.XVelocity = _Preset.Data.XVelocity;
        _Data.YVelocity = _Preset.Data.YVelocity;

        _Data.MaxSpeed = _Preset.Data.MaxSpeed;
        _Data.UseMoveValueAs = _Preset.Data.UseMoveValueAs;
        _Data.MoveValues = _Preset.Data.MoveValues;
        _Data.LerpSpeed = _Preset.Data.LerpSpeed;
    }

    public override void Initialize(DATA_BlackBoard blackBoard, GameObject target)
    {
        if (_Preset.Data != null)
        {
            CopyPreset();
        }

        _Forward.Initialize(blackBoard, target);
        if (!_Forward.IsValid)
        {
            Verbose(VerboseMask.WarningLog, "_Forward is invalid => Default value is 1");
            _Forward.Variable.Value = 1;
        }

        if (_Forward.Variable == null)
        {
            Verbose(VerboseMask.ErrorLog, "_Forward.EntryName => no blackboard parameter found with the entry name \"" + _Forward.EntryName + "\".");
        }


        if (target != null)// && _Player.Variable != null)
        {
            _OwnRigidbody = target.GetComponent<Rigidbody2D>();
            if (!_OwnRigidbody)
            {
                Verbose(VerboseMask.ErrorLog, "_OwnRigidbody is null => Player.Variable.Value.GetComponent<Rigidbody2D>()");
            }
        }
        else
        {
            Verbose(VerboseMask.ErrorLog, "_Player.Initialize(blackBoard, target) => Failure");
        }
    }

    public override void Enter()
    {
        _Stamps = Vector2.zero;

        //return base.Enter();
    }

    public override void UpdateState()
    {
        base.UpdateState();

#if UNITY_EDITOR
        _DeltaTime = Time.deltaTime;
#endif
        Vector2 _CurrentVelocity = Vector2.zero;

        switch (_Data.UseMoveValueAs)
        {
            case EMoveValueInterpretation.Acceleration:
                _CurrentVelocity = ComputeVelocityWithAcceleration();
                break;
            case EMoveValueInterpretation.Duration:
                _CurrentVelocity = ComputeVelocityWithDuration();
                break;
            default:
                break;
        }


        if ((_Data.MoveAxis & EMoveAxis.X) != 0 && (_Data.MoveAxis & EMoveAxis.Y) != 0)
        {
            _OwnRigidbody.velocity = Vector2.Lerp(_OwnRigidbody.velocity, _CurrentVelocity, _Data.LerpSpeed);
        }
        else if ((_Data.MoveAxis & EMoveAxis.X) != 0)
        {
            _OwnRigidbody.velocity = Vector2.Lerp(_OwnRigidbody.velocity, new Vector2(_CurrentVelocity.x, _OwnRigidbody.velocity.y), _Data.LerpSpeed);
        }
        else
        {
            _OwnRigidbody.velocity = Vector2.Lerp(_OwnRigidbody.velocity, new Vector2(_OwnRigidbody.velocity.x, _CurrentVelocity.y), _Data.LerpSpeed);
        }
    }
    #endregion


    #region PRIVATE METHODS
    private Vector2 ComputeVelocityWithAcceleration()
    {
        Vector2 ret = Vector2.zero;

        bool endX = false, endY = false;
        if (_Data.MoveValues.Value.x == 0 || _Stamps.x >= 1f)
        {
            endX = true;
            ret.x = _Forward * _Data.MaxSpeed.Value.x * _Data.XVelocity.Value.Evaluate(_Stamps.x);
        }
        else
        {
            _Stamps.x += _Data.MoveValues.Value.x * Time.deltaTime;
            ret.x = _Forward * _Data.MaxSpeed.Value.x * _Data.XVelocity.Value.Evaluate(_Stamps.x);
        }

        if (_Data.MoveValues.Value.y == 0 || _Stamps.y >= 1f)
        {
            endY = true;
            ret.y = _Data.MaxSpeed.Value.y * _Data.YVelocity.Value.Evaluate(_Stamps.y);
        }
        else
        {
            _Stamps.y += _Data.MoveValues.Value.y * Time.deltaTime;
            ret.y = _Data.MaxSpeed.Value.y * _Data.YVelocity.Value.Evaluate(_Stamps.y);
        }

        if (endX && endY)
        {
            if (!_Data.Loop)
            {
                _OnCompleteTimestamp?.Invoke();
                //return ret;
            }
            else
            {
                _Stamps = Vector2.zero;
            }
        }

        return ret;
    }

    private Vector2 ComputeVelocityWithDuration()
    {
        Vector2 ret = Vector2.zero;

        float ratio = 0;

        bool endX = false, endY = false;
        if (_Data.MoveValues.Value.x == 0 || (_Stamps.x / _Data.MoveValues.Value.x) >= 1f)
        {
            endX = true;
            if (_Data.MoveValues.Value.x != 0)
            {
                ratio = _Stamps.x / _Data.MoveValues.Value.x;
                ret.x = _Forward * _Data.MaxSpeed.Value.x * _Data.XVelocity.Value.Evaluate(ratio);
            }
        }
        else
        {
            _Stamps.x += Time.deltaTime;
            ratio = _Stamps.x / _Data.MoveValues.Value.x;
            ret.x = _Forward * _Data.MaxSpeed.Value.x * _Data.XVelocity.Value.Evaluate(ratio);
        }

        if (_Data.MoveValues.Value.y == 0 || (_Stamps.y / _Data.MoveValues.Value.y) >= 1f)
        {
            endY = true;
            if (_Data.MoveValues.Value.y != 0)
            {
                ratio = _Stamps.y / _Data.MoveValues.Value.y;
                ret.y = _Data.MaxSpeed.Value.y * _Data.YVelocity.Value.Evaluate(ratio);
            }
        }
        else
        {
            _Stamps.y += Time.deltaTime;
            ratio = _Stamps.y / _Data.MoveValues.Value.y;
            ret.y = _Data.MaxSpeed.Value.y * _Data.YVelocity.Value.Evaluate(ratio);
        }

        if (endX && endY)
        {
            if (!_Data.Loop)
            {
                _OnCompleteTimestamp?.Invoke();
            }
            else
            {
                _Stamps = Vector2.zero;
            }
        }

        return ret;
    }
    #endregion
}