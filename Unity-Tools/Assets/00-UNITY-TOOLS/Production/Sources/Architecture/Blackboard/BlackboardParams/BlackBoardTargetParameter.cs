using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Class that allow to get a variable reference to a blackboard of a defined target.
[System.Serializable]
public class BlackboardTargetParameter
{
    public GameObject Target;
    public StringReference EntryName;

    // Use to set the default value.
    public virtual void Initialize(DATA_BlackBoard board)
    { }
}

public class BlackboardTargetParameter_Template<T, TVariable> : BlackboardTargetParameter
    where TVariable : DataVariable<T>
{
    //public T DefaultValue;
    [ReadOnly]
    public TVariable Variable;

    public override void Initialize(DATA_BlackBoard board)
    {
        //board.SetValue(this, DefaultValue);
        Variable = board.GetVariable<T, TVariable>(this);
    }

    public static implicit operator T(BlackboardTargetParameter_Template<T, TVariable> _object)
    {
        return _object.Variable;
    }
}

[System.Serializable]
public class BBTP_Bool : BlackboardTargetParameter_Template<bool, BoolVariable>
{ }
[System.Serializable]
public class BBTP_Int : BlackboardTargetParameter_Template<int, IntVariable>
{ }
[System.Serializable]
public class BBTP_Float : BlackboardTargetParameter_Template<float, FloatVariable>
{ }
[System.Serializable]
public class BBTP_Vector2 : BlackboardTargetParameter_Template<Vector2, Vector2Variable>
{ }
[System.Serializable]
public class BBTP_Vector3 : BlackboardTargetParameter_Template<Vector3, Vector3Variable>
{ }
[System.Serializable]
public class BBTP_GameObject : BlackboardTargetParameter_Template<GameObject, GameObjectVariable>
{ }
