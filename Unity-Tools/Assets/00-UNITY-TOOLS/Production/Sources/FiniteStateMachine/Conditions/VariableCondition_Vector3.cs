using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VariableCondition_Vector3 : VariableConditionTemplate<Vector3, Vector3Variable, Vector3Reference>
{
    public override bool IsConditionValid()
    {
        switch (CompareAsEqual)
        {
            case EConditionComparer.Equal:
                return Alpha.Value == Beta;
            case EConditionComparer.NotEqual:
                return Alpha.Value != Beta;
            case EConditionComparer.MoreThan:
                return Alpha.Value.sqrMagnitude > Beta.Value.sqrMagnitude;
            case EConditionComparer.MoreOrEqualThan:
                return Alpha.Value.sqrMagnitude >= Beta.Value.sqrMagnitude;
            case EConditionComparer.LessThan:
                return Alpha.Value.sqrMagnitude < Beta.Value.sqrMagnitude;
            case EConditionComparer.LessOrEqualThan:
                return Alpha.Value.sqrMagnitude <= Beta.Value.sqrMagnitude;
            default:
                return false;
        }
    }
}
