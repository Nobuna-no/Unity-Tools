using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InternalBlackboardParameter_String : InternalBlackboardParameter<StringVariable>
{
    public override IClonable Clone()
    {
        InternalBlackboardParameter_String obj = base.Clone() as InternalBlackboardParameter_String;
        obj.Value = Value;
        return obj;
    }
}