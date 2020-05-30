using UnityEngine;


/*[System.Serializable, CreateAssetMenu(menuName = "Framework Data/Variable/Float2", order = 4)]
public class Float2Variable : Vector2Variable
{
    public FloatVariable XVariable;
    public FloatVariable YVariable;
    int m_CyclicCheck = 0;

    private void UpdateValue()
    {
        if (m_CyclicCheck < 2)
        {
            m_CyclicCheck++;
            Value = new Vector2(XVariable, YVariable);
        }
        else
        { 
            m_CyclicCheck = 0;
        }
    }

    private void UpdateVariables()
    {
        if(m_CyclicCheck < 2)
        {
            m_CyclicCheck++;
            XVariable.Value = Value.x;
            YVariable.Value = Value.y;
        }
        else
        {
            m_CyclicCheck = 0;
        }
    }

    public override void OnEnable()
    {
        base.OnEnable();


        if (XVariable)
        {
            XVariable.OnValueChanged = null;
            XVariable.OnValueChanged += UpdateValue;
        }
        if (YVariable)
        {
            YVariable.OnValueChanged = null;
            YVariable.OnValueChanged += UpdateValue;
        }

        OnValueChanged += UpdateVariables;

        //if (Application.isPlaying)
        {
        }
    }

    public override void OnDisable()
    {
        base.OnDisable();

        if(Application.isPlaying)
        {
            XVariable.OnValueChanged = null;
            YVariable.OnValueChanged = null;
        }
    }
}*/

/* OPT VER.*/
 [System.Serializable, CreateAssetMenu(menuName = "Framework Data/Input/Axis", order = 4)]
public class DATA_Axis : ScriptableObject, ISerializationCallbackReceiver
{
    public Vector2 InitialValue;
    public FloatReference XVariable;
    public FloatReference YVariable;

    public FloatReference TimestampX;
    public FloatReference TimestampY;

    /// <summary>
    /// Force value to 0 and reset timestamp. 
    /// </summary>
    public void ConsumeAxisX()
    {
        TimestampX.Value = 0;
    }

    /// <summary>
    /// Force value to 0 and reset timestamp. 
    /// </summary>
    public void ConsumeAxisY()
    {
        TimestampY.Value = 0;
    }

    public Vector2 Value
    {
        get { return new Vector2(XVariable, YVariable); }
        set
        {
            if (XVariable.Variable && XVariable.Variable.Value != value.x)
            {
                XVariable.Value = value.x;
            }
            if (YVariable.Variable && YVariable.Variable.Value != value.y)
            {
                YVariable.Value = value.y;
            }
        }
    }

    public static implicit operator Vector2(DATA_Axis _object)
    {
        return new Vector2(_object.XVariable, _object.YVariable);
    }

    public virtual void OnEnable()
    {
        if(XVariable.Variable)
        {
            XVariable.Variable.OnValueChanged = null;
        }
        if (YVariable.Variable)
        {
            YVariable.Variable.OnValueChanged = null;
        }
    }

    public virtual void OnDisable()
    {
        if (Application.isPlaying)
        {
            if (XVariable.Variable)
            {
                XVariable.Variable.OnValueChanged = null;
                XVariable.Value = InitialValue.x;
            }
            if (YVariable.Variable)
            {
                YVariable.Variable.OnValueChanged = null;
                YVariable.Value = InitialValue.y;
            }
        }
    }

    public void OnBeforeSerialize()
    {
        //Value = InitialValue;
    }

    public void OnAfterDeserialize()
    {
        if (!XVariable.UsingConstant && XVariable.Variable)
        {
            XVariable.Value = InitialValue.x;
        }
        if (!YVariable.UsingConstant && YVariable.Variable)
        {
            YVariable.Value = InitialValue.y;
        }
    }
}
     
