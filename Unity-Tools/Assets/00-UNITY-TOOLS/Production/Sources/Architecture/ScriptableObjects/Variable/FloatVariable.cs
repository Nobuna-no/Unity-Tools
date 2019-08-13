using UnityEngine;


[System.Serializable, CreateAssetMenu(menuName = "Framework Data/Variable/Floating", order = 3)]
public class FloatVariable : DataVariable<float>
{
    MinMaxClamp<float> ClampValue = null;


    #region PRIVATE METHODS
    private void InternalClamping()
    {
        RuntimeValue = Mathf.Clamp(RuntimeValue, ClampValue.Min, ClampValue.Max);
    }

    private void GenerateMinMaxClampValue()
    {
        if (ClampValue == null)
        {
            ClampValue = new MinMaxClamp<float>();
            ClampValue.Min = CreateInstance<FloatVariable>();
            ClampValue.Max = CreateInstance<FloatVariable>();
        }
    }
    #endregion


    #region PUBLIC METHODS
    public void ActiveValueClamping(bool active)
    {
        if (ClampValue == null)
        {
            return;
        }

        if(active)
        {
            OnValueChanged -= InternalClamping;
            OnValueChanged += InternalClamping;
        }
        else
        {
            OnValueChanged -= InternalClamping;
        }
    }

    public void ActiveValueClamping(float min, float max)
    {
        GenerateMinMaxClampValue();

        if (!ClampValue.Min)
        {
            ClampValue.Min = new DataVariable<float>();
        }
        ClampValue.Min.Value = min;

        if (!ClampValue.Max)
        {
            ClampValue.Max = new DataVariable<float>();
        }
        ClampValue.Max.Value = max;

        ActiveValueClamping(true);
    }

    public void ActiveValueClamping(FloatVariable min, FloatVariable max)
    {
        if (!min || !max)
        {
            Debug.Log("[ERROR]<" + this + ">: Trying to ActiveValueClamping with invalid min or max");
            return;
        }

        GenerateMinMaxClampValue();

        ClampValue.Min = min;
        ClampValue.Max = max;

        ActiveValueClamping(true);
    }

    public void ActiveValueClamping(float min, FloatVariable max)
    {
        if (!max)
        {
            Debug.Log("[ERROR]<" + this + ">: Trying to ActiveValueClamping with invalid max!");
            return;
        }

        GenerateMinMaxClampValue();

        ClampValue.Min.Value = min;
        ClampValue.Max = max;

        ActiveValueClamping(true);
    }

    public void ActiveValueClamping(FloatVariable min, float max)
    {
        if (!min)
        {
            Debug.Log("[ERROR]<" + this + ">: Trying to ActiveValueClamping with invalid min!");
            return;
        }

        GenerateMinMaxClampValue();

        ClampValue.Min = min;
        ClampValue.Max.Value = max;

        ActiveValueClamping(true);
    }

    public void SetValue(float value)
    {
        Value = value;
    }

    public void Invert()
    {
        Value = -Value;
    }
    #endregion
}


