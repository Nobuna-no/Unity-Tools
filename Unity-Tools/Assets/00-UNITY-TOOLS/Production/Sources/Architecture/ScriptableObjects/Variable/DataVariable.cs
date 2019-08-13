using UnityEngine;
using UnityEngine.Events;


public class DataVariable<T> : ScriptableObject, ISerializationCallbackReceiver
{
    public UnityAction OnValueChanged;

    [SerializeField]
    private T InitialValue = default;
    [SerializeField, ReadOnly]
    protected T RuntimeValue = default;

    public T Value
    {
        get { return RuntimeValue; }
        set
        {
            RuntimeValue = value;
            if (OnValueChanged != null)
            {
                OnValueChanged.Invoke();
            }
        }
    }

    public override string ToString()
    {
        return base.ToString() + "{" + Value + "}";
    }

    public static implicit operator T(DataVariable<T> _object)
    {
        return _object.Value;
    }

    public virtual void OnEnable()
    {
        OnValueChanged = null;
    }

    public virtual void OnDisable()
    {
        if(Application.isPlaying)
        {
            OnValueChanged = null;
            Value = InitialValue;
        }
    }

    public void OnBeforeSerialize()
    {
        //Value = InitialValue;
    }

    public void OnAfterDeserialize()
    {
        Value = InitialValue;
    }
}

public class MinMaxClamp<T>
{
    public DataVariable<T> Min;
    public DataVariable<T> Max;
}