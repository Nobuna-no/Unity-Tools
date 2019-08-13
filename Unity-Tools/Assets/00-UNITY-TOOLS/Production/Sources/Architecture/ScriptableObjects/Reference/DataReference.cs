using UnityEngine;
using UnityEngine.Events;

public class DataReference<T, DataValue> where DataValue : DataVariable<T>
{
    [SerializeField, HideInInspector]
    private bool UseConstant = true;
    public bool UsingConstant
    { get => UseConstant; }

    public string VariableName
    {
        get
        { return UseConstant ? "" : Variable?.name; }
    }
    public T ConstantValue;
    public DataValue Variable;
    
    public bool IsValid()
    {
        if (!UseConstant)
            return Variable != null;
        else return true;
    }

    public void TrySetReference(DataValue refVar)
    {
        if(refVar)
        {
            Variable = refVar;
            UseConstant = false;
        }
    }

    // Only for non constant.
    public void AddActionOnValueChanged(UnityAction action)
    {
        if(!UseConstant)
        {
            Variable.OnValueChanged += action;
        }
    }

    public T Value
    {
        get
        {
            if(UseConstant)
            {
                return ConstantValue;
            }
            else
            {
                if(!Variable)
                {
                    Debug.LogWarning("[WARNING]<" + (VariableName.Length == 0 ? "NONE" : VariableName) + ">: Is set to use variable ref but as a null ref! Returning constant value!");
                    return ConstantValue;
                }
                return Variable.Value;
            }
        }
        set
        {
            if(UseConstant)
            {
                //Debug.LogWarning("[WARNING]<" + (VariableName.Length == 0 ? "NONE" : VariableName) + ">: Modifying constant reference. Think about using DataVariable instead.");
                ConstantValue = value;
            }
            else
            {
                if(Variable)
                {
                    Variable.Value = value;
                }
            }
        }
    }

    public static implicit operator T(DataReference<T, DataValue> _object)
    {
        return _object.Value;
    }
}