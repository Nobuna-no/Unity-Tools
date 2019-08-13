using UnityEngine;



[System.Serializable, CreateAssetMenu(menuName = "Framework Data/Variable/Integer", order = 2)]
public class IntVariable : DataVariable<int>
{
    MinMaxClamp<int> ClampValue = null;


    #region PRIVATE METHODS
    private void InternalClamping()
    {
        RuntimeValue = Mathf.Clamp(RuntimeValue, ClampValue.Min, ClampValue.Max);
    }

    private void GenerateMinMaxClampValue()
    {
        if (ClampValue == null)
        {
            ClampValue = new MinMaxClamp<int>();
            ClampValue.Min = CreateInstance<IntVariable>();
            ClampValue.Max = CreateInstance<IntVariable>();
        }
        ActiveValueClamping(true);
    }
    #endregion


    #region PUBLIC METHODS
    public void Increment()
    {
        Value++;
    }

    public void Decrement()
    {
        Value--;
    }

    public void ActiveValueClamping(bool active)
    {
        if (ClampValue == null)
        {
            return;
        }

        if (active)
        {
            OnValueChanged -= InternalClamping;
            OnValueChanged += InternalClamping;
        }
        else
        {
            OnValueChanged -= InternalClamping;
        }
    }

    public void ActiveValueClamping(int min, int max)
    {
        GenerateMinMaxClampValue();

        ClampValue.Min.Value = min;
        ClampValue.Max.Value = max;
    }

    public void ActiveValueClamping(IntVariable min, IntVariable max)
    {       
        if (!min || !max)
        {
            Debug.Log("[ERROR]<" + this + ">: Trying to ActiveValueClamping with invalid min or max");
            return;
        }

        GenerateMinMaxClampValue();

        ClampValue.Min = min;
        ClampValue.Max = max;
    }

    public void ActiveValueClamping(int min, IntVariable max)
    {
        if (!max)
        {
            Debug.Log("[ERROR]<" + this + ">: Trying to ActiveValueClamping with invalid max!");
            return;
        }

        GenerateMinMaxClampValue();

        ClampValue.Min.Value = min;
        ClampValue.Max = max;
    }

    public void ActiveValueClamping(IntVariable min, int max)
    {
        if (!min)
        {
            Debug.Log("[ERROR]<" + this + ">: Trying to ActiveValueClamping with invalid min!");
            return;
        }

        GenerateMinMaxClampValue();

        ClampValue.Min = min;
        ClampValue.Max.Value = max;
    }
    #endregion
}


