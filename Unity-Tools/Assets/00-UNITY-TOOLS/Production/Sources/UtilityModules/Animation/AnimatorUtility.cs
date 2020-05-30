using UnityEngine;


[System.Serializable]
public class StateMachineSynchData<T>
{
    protected T m_Data;
    public string ParameterName;

    /** METHODS
     */
    public static implicit operator T(StateMachineSynchData<T> _object)
    {
        return _object.m_Data;
    }

    // Set value and synchronize with the state machine
    virtual public void SetData(T _value, Animator _animator = null)
    { }
}

[System.Serializable]
public class StateMachineSyncDataOfBool : StateMachineSynchData<bool>
{
    public override void SetData(bool _value, Animator _animator = null)
    {
        m_Data = _value;
        if (_animator != null && ParameterName.Length > 0)
        {
            _animator.SetBool(ParameterName, System.Convert.ToBoolean(m_Data));
        }
        else
            Debug.LogWarning("Invalid StateMachine Parameters name '" + ParameterName + "' passed !");
    }
}

[System.Serializable]
public class StateMachineSyncTrigger
{
    public string ParameterName;

    public void Trigger(Animator _animator)
    {
        if (_animator != null && ParameterName.Length > 0)
        {
            _animator.SetTrigger(ParameterName);
        }
        else
            Debug.LogWarning("Invalid StateMachine Parameters name '" + ParameterName + "' passed !");
    }

    public void Reset(Animator _animator)
    {
        if (_animator != null && ParameterName.Length > 0)
        {
            _animator.ResetTrigger(ParameterName);
        }
        else
            Debug.LogWarning("Invalid StateMachine Parameters name '" + ParameterName + "' passed !");
    }
}

[System.Serializable]
public class StateMachineSyncDataOfInt : StateMachineSynchData<int>
{
    public override void SetData(int _value, Animator _animator = null)
    {
        m_Data = _value;
        if (_animator != null && ParameterName.Length > 0)
        {
            _animator.SetInteger(ParameterName, System.Convert.ToInt32(m_Data));
        }
        else
            Debug.LogWarning("Invalid StateMachine Parameters name '" + ParameterName + "' passed !");
    }
}

[System.Serializable]
public class StateMachineSyncDataOfFloat : StateMachineSynchData<float>
{
    public override void SetData(float _value, Animator _animator = null)
    {
        m_Data = _value;
        if (_animator != null && ParameterName.Length > 0)
        {
            _animator.SetFloat(ParameterName, Mathf.Abs(System.Convert.ToSingle(m_Data)));
        }
        else
            Debug.LogWarning("Invalid StateMachine Parameters name '" + ParameterName + "' passed !");
    }
}

public class AnimatorHelper
{
    #region PROPERTIES

    private Transform RefTransform = null;
    private bool IsFacingRight;

    #endregion


    #region CONSTRUCTOR
    public AnimatorHelper(Transform _transform, bool _facingRight = true)
    {
        if (_transform == null)
            Debug.LogWarning("Null transform's ref passed as argument.");
        RefTransform = _transform;
        IsFacingRight = _facingRight;
    }
    #endregion


    #region PUBLIC METHODS
    public void Update(float _axisMove)
    {
        if (RefTransform)
        {
            if (_axisMove > 0 && !IsFacingRight)
                Flip();
            else if (_axisMove < 0 && IsFacingRight)
                Flip();
        }
    }

    public void Flip()
    {
        IsFacingRight = !IsFacingRight;
        Vector3 scale = RefTransform.localScale;
        scale.x *= -1;
        RefTransform.localScale = scale;
    }

    public float Direction
    {
        get { return IsFacingRight ? 1 : -1; }
    }
    #endregion
}