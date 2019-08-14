using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlackboardCondition_Vector2 : BlackboardConditionTemplate<Vector2, Vector2Variable, Vector2Reference>
{
    public override bool IsConditionValid()
    {
        switch (CompareAsEqual)
        {
            case EConditionComparer.Equal:
                return Variable.Value == Beta;
            case EConditionComparer.NotEqual:
                return Variable.Value != Beta;
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
