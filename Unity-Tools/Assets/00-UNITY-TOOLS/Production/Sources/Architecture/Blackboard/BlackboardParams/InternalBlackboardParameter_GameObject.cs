using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InternalBlackboardParameter_GameObject : InternalBlackboardParameter<GameObjectVariable>
{
    public override IClonable Clone()
    {
        InternalBlackboardParameter_GameObject obj = base.Clone() as InternalBlackboardParameter_GameObject;
        obj.Value = Value;
        return obj;
    }
}