using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class InternalBlackboardParameter_Vector2 : InternalBlackboardParameter<Vector2Variable>
{
    public override IClonable Clone()
    {
        InternalBlackboardParameter_Vector2 obj = base.Clone() as InternalBlackboardParameter_Vector2;
        obj.Value = Value;
        return obj;
    }
}