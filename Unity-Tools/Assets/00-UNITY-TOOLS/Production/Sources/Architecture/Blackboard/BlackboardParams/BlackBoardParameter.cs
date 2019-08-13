using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Class that allow to get a variable reference to a blackboard of an undefined target.
[System.Serializable]
public abstract class BlackboardParameter
{
    public StringReference EntryName;

    [SerializeField, HideInInspector]
    protected bool UseEntryName = true;
    // Use to set the default value.
    public virtual void Initialize(DATA_BlackBoard board, GameObject Target)
    { }
}

public class BlackboardParameter_Template<T, TVariable> : BlackboardParameter
    where TVariable : DataVariable<T>
{
    public TVariable Variable;

    public override void Initialize(DATA_BlackBoard board, GameObject Target)
    {
        if(!UseEntryName 
            || EntryName == null 
            || (EntryName.Variable == null && !EntryName.UsingConstant)
            || EntryName.Value.Length == 0)
        {
            return;
        }
        Variable = board.GetVariable<T, TVariable>(Target, EntryName);
    }

    public static implicit operator T(BlackboardParameter_Template<T, TVariable> _object)
    {
        return _object.Variable;
    }
}

[System.Serializable]
public class BBP_Bool : BlackboardParameter_Template<bool, BoolVariable>
{ }
[System.Serializable]
public class BBP_Int : BlackboardParameter_Template<int, IntVariable>
{ }
[System.Serializable]
public class BBP_Float : BlackboardParameter_Template<float, FloatVariable>
{ }
[System.Serializable]
public class BBP_String : BlackboardParameter_Template<string, StringVariable>
{ }
[System.Serializable]
public class BBP_Vector2 : BlackboardParameter_Template<Vector2, Vector2Variable>
{ }
[System.Serializable]
public class BBP_Vector3 : BlackboardParameter_Template<Vector3, Vector3Variable>
{ }
[System.Serializable]
public class BBP_GameObject : BlackboardParameter_Template<GameObject, GameObjectVariable>
{ }