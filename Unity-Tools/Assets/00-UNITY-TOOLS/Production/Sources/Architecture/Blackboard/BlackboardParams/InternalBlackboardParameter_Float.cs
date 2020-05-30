using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class InternalBlackboardParameter_Float : InternalBlackboardParameter<FloatVariable>
{
    public override IClonable Clone()
    {
        InternalBlackboardParameter_Float obj = base.Clone() as InternalBlackboardParameter_Float;
        obj.Value = Value;
        return obj;
    }
}