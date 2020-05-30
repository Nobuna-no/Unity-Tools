using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class InternalBlackboardParameter_Vector3 : InternalBlackboardParameter<Vector3Variable>
{
    public override IClonable Clone()
    {
        InternalBlackboardParameter_Vector3 obj = base.Clone() as InternalBlackboardParameter_Vector3;
        obj.Value = Value;
        return obj;
    }
}