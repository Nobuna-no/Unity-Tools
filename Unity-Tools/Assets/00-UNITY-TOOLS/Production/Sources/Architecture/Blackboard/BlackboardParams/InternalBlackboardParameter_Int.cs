using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InternalBlackboardParameter_Int : InternalBlackboardParameter<IntVariable>
{
    public override IClonable Clone()
    {
        InternalBlackboardParameter_Int obj = base.Clone() as InternalBlackboardParameter_Int;
        obj.Value = Value;
        return obj;
    }
}