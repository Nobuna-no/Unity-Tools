using UnityEngine;


[System.Serializable, CreateAssetMenu(menuName = "Framework Data/Variable/Float2", order = 4)]
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
}