using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class InternalBlackboardParameter_Transform : InternalBlackboardParameter<TransformVariable>
{
    public override IClonable Clone()
    {
        InternalBlackboardParameter_Transform obj = base.Clone() as InternalBlackboardParameter_Transform;
        obj.Value = Value;
        return obj;
    }
}