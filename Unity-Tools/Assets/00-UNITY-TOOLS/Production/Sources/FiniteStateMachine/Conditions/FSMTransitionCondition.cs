using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;


#if UNITY_EDITOR
using UnityEditor;
using UnityEditorInternal;


public abstract class FSMTransitionCondition : MonoBehaviour
{
    public enum EConditionComparer
    {
        Equal = 0,
        NotEqual,
        MoreThan,
        MoreOrEqualThan,
        LessThan,
        LessOrEqualThan
    }

    public EConditionComparer CompareAsEqual;

    public virtual bool IsConditionValid()
    {
        return false;
    }

    public virtual void Initialize(DATA_BlackBoard blackboard, GameObject target)
    {}

    public virtual void Subscribe(UnityAction unityAction)
    {}

    public virtual void UnSubscribe(UnityAction unityAction)
    {}
}

public abstract class BlackboardConditionTemplate<T, VariableT, RefT> : FSMTransitionCondition
    where VariableT : DataVariable<T> where RefT : DataReference<T, VariableT>
{
    public StringReference Alpha;
    public RefT Beta;

    protected VariableT Variable;

    public override string ToString()
    {
        return "Condition[" + this.name + "]: (BlackboardValue[" + Alpha.Value + "]){" + Variable.Value + "} is " + CompareAsEqual + " to {" + Beta.Value +"}?";
    }

    public override void Initialize(DATA_BlackBoard blackboard, GameObject target)
    {
        Variable = blackboard.GetVariable<T, VariableT>(target, Alpha);
    }

    public override void Subscribe(UnityAction unityAction)
    {
        if (Variable != null)
        {
            Variable.OnValueChanged += unityAction;
        }
        if (Beta != null && Beta.Variable != null)
        {
            Beta.Variable.OnValueChanged += unityAction;
        }
    }

    public override void UnSubscribe(UnityAction unityAction)
    {
        if (Variable != null)
        {
            Variable.OnValueChanged -= unityAction;
        }
        if (Beta != null && Beta.Variable != null)
        {
            Beta.Variable.OnValueChanged -= unityAction;
        }
    }
}

public abstract class VariableConditionTemplate<T, VariableT, ReferenceT> : FSMTransitionCondition
    where VariableT : DataVariable<T> where ReferenceT : DataReference<T, VariableT>
{
    public ReferenceT Alpha;
    public ReferenceT Beta;

    public override string ToString()
    {
        return "Condition[" + this.name + "]: \"" + Alpha.VariableName +"\"{" + Alpha.Value + "} is " + CompareAsEqual + " to \"" + Beta.VariableName + "\"{" + Beta.Value + "}?";
    }

    public override void Subscribe(UnityAction unityAction)
    {
        if (Alpha != null && Alpha.Variable != null)
        {
            Alpha.Variable.OnValueChanged += unityAction;
        }
        if (Beta != null && Beta.Variable != null)
        {
            Beta.Variable.OnValueChanged += unityAction;
        }
    }

    public override void UnSubscribe(UnityAction unityAction)
    {
        if (Alpha != null && Alpha.Variable != null)
        {
            Alpha.Variable.OnValueChanged -= unityAction;
        }
        if (Beta != null && Beta.Variable != null)
        {
            Beta.Variable.OnValueChanged -= unityAction;
        }
    }
}

#endif

/*
[System.Serializable]
public enum CONDITIONSIMPLEOPERATOR
{
    EQUAL,
    NOTEQUAL
}

[System.Serializable]
public enum CONDITIONNUMBEROPERATOR
{
    LESS = 0,
    LESSEQUAL,
    EQUAL,
    GREATEREQUAL,
    GREATER,
    NOTEQUAL
}

[System.Serializable]
public class TransitionBoolCondition
{
    #region PROPERTIES

    [SerializeField]
    protected string boolVariableName;

    [SerializeField]
    protected CONDITIONSIMPLEOPERATOR conditionOperator;

    [SerializeField]
    protected bool comparaisonValue;

    BoolVariable boolVariable = null;

    #endregion

    #region PUBLIC METHODS

    public void Init(FiniteStateMachine FSM)
    {
        //boolVariable = FSM.GetBool(boolVariableName);
    }

    public bool CheckCondition()
    {
        if (boolVariable != null)
        {
            if (conditionOperator == CONDITIONSIMPLEOPERATOR.EQUAL)
                return boolVariable.Value == comparaisonValue;
            else if (conditionOperator == CONDITIONSIMPLEOPERATOR.NOTEQUAL)
                return boolVariable.Value != comparaisonValue;
            else
                return false;
        }
        else
            return false;
    }

    public void Subscribe(UnityAction unityAction)
    {
        if (boolVariable != null)
            boolVariable.OnValueChanged += unityAction;

    }

    public void UnSubscribe(UnityAction unityAction)
    {
        if (boolVariable != null)
            boolVariable.OnValueChanged -= unityAction;
    }

    #endregion
}

[System.Serializable]
public class TransitionIntCondition
{
    #region PROPERTIES

    [SerializeField]
    string IntVariableName;

    [SerializeField]
    CONDITIONNUMBEROPERATOR conditionOperator;

    [SerializeField]
    int comparaisonValue;

    IntVariable IntVariable = null;

    #endregion

    #region PUBLIC METHODS

    public void Init(FiniteStateMachine FSM)
    {
        //IntVariable = FSM.GetInteger(IntVariableName);
    }

    public bool CheckCondition()
    {
        if (IntVariable != null)
        {
            if (conditionOperator == CONDITIONNUMBEROPERATOR.LESS)
                return IntVariable.Value < comparaisonValue;
            else if (conditionOperator == CONDITIONNUMBEROPERATOR.LESSEQUAL)
                return IntVariable.Value <= comparaisonValue;
            else if (conditionOperator == CONDITIONNUMBEROPERATOR.EQUAL)
                return IntVariable.Value == comparaisonValue;
            else if (conditionOperator == CONDITIONNUMBEROPERATOR.GREATER)
                return IntVariable.Value >= comparaisonValue;
            else if (conditionOperator == CONDITIONNUMBEROPERATOR.GREATEREQUAL)
                return IntVariable.Value > comparaisonValue;
            else if (conditionOperator == CONDITIONNUMBEROPERATOR.NOTEQUAL)
                return IntVariable.Value != comparaisonValue;
            else
                return false;
        }
        else
            return false;
    }

    public void Subscribe(UnityAction unityAction)
    {
        if (IntVariable != null)
            IntVariable.OnValueChanged += unityAction;
    }

    public void UnSubscribe(UnityAction unityAction)
    {
        if (IntVariable != null)
            IntVariable.OnValueChanged -= unityAction;
    }
    #endregion
}

[System.Serializable]
public class TransitionFloatCondition
{
    #region PROPERTIES

    [SerializeField]
    string FloatVariableName;

    [SerializeField]
    CONDITIONNUMBEROPERATOR conditionOperator;

    [SerializeField]
    int comparaisonValue;

    FloatVariable FloatVariable = null;

    #endregion

    #region PUBLIC METHODS

    public void Init(FiniteStateMachine FSM)
    {
        //FloatVariable = FSM.GetFloat(FloatVariableName);
    }

    public bool CheckCondition()
    {
        if (FloatVariable != null)
        {
            if (conditionOperator == CONDITIONNUMBEROPERATOR.LESS)
                return FloatVariable.Value < comparaisonValue;
            else if (conditionOperator == CONDITIONNUMBEROPERATOR.LESSEQUAL)
                return FloatVariable.Value <= comparaisonValue;
            else if (conditionOperator == CONDITIONNUMBEROPERATOR.EQUAL)
                return FloatVariable.Value == comparaisonValue;
            else if (conditionOperator == CONDITIONNUMBEROPERATOR.GREATER)
                return FloatVariable.Value >= comparaisonValue;
            else if (conditionOperator == CONDITIONNUMBEROPERATOR.GREATEREQUAL)
                return FloatVariable.Value > comparaisonValue;
            else if (conditionOperator == CONDITIONNUMBEROPERATOR.NOTEQUAL)
                return FloatVariable.Value != comparaisonValue;
            else
                return false;
        }
        else
            return false;
    }

    public void Subscribe(UnityAction unityAction)
    {
        if (FloatVariable != null)
            FloatVariable.OnValueChanged += unityAction;

    }

    public void UnSubscribe(UnityAction unityAction)
    {
        if (FloatVariable != null)
            FloatVariable.OnValueChanged -= unityAction;
    }

    #endregion
}

[System.Serializable]
public class TransitionStringCondition
{
    #region PROPERTIES

    [SerializeField]
    StringVariable stringVariableName;

    [SerializeField]
    CONDITIONSIMPLEOPERATOR conditionOperator;

    [SerializeField]
    string comparaisonValue;

    StringVariable stringVariable = null;

    #endregion

    #region PUBLIC METHODS

    public void Init(FiniteStateMachine FSM)
    {
        //stringVariable = FSM.GetString(stringVariableName);
    }

    public bool CheckCondition()
    {
        if (stringVariable != null)
        {
            if (conditionOperator == CONDITIONSIMPLEOPERATOR.EQUAL)
                return stringVariable.Value == comparaisonValue;
            else if (conditionOperator == CONDITIONSIMPLEOPERATOR.NOTEQUAL)
                return stringVariable.Value != comparaisonValue;
            else
                return false;
        }
        else
            return false;
    }

    public void Subscribe(UnityAction unityAction)
    {
        if (stringVariable != null)
            stringVariable.OnValueChanged += unityAction;
    }

    public void UnSubscribe(UnityAction unityAction)
    {
        if (stringVariable != null)
            stringVariable.OnValueChanged -= unityAction;
    }

    #endregion
}*/

