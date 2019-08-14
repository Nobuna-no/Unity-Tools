using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlackboardCondition_Vector3 : BlackboardConditionTemplate<Vector3, Vector3Variable, Vector3Reference>
{
    public override bool IsConditionValid()
    {
        switch (CompareAsEqual)
        {
            case EConditionComparer.Equal:
                return Variable.Value == Beta.Value;
            case EConditionComparer.NotEqual:
                return Variable.Value != Beta.Value;
            case EConditionComparer.MoreThan:
                return Variable.Value.sqrMagnitude > Beta.Value.sqrMagnitude;
            case EConditionComparer.MoreOrEqualThan:
                return Variable.Value.sqrMagnitude >= Beta.Value.sqrMagnitude;
            case EConditionComparer.LessThan:
                return Variable.Value.sqrMagnitude < Beta.Value.sqrMagnitude;
            case EConditionComparer.LessOrEqualThan:
                return Variable.Value.sqrMagnitude <= Beta.Value.sqrMagnitude;
            default:
                return false;
        }
    }
}

