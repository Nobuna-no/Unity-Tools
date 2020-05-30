using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VariableCondition_Int : VariableConditionTemplate<int, IntVariable, IntReference>
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
                return Alpha > Beta;
            case EConditionComparer.MoreOrEqualThan:
                return Alpha >= Beta;
            case EConditionComparer.LessThan:
                return Alpha < Beta;
            case EConditionComparer.LessOrEqualThan:
                return Alpha <= Beta;
            default:
                return false;
        }
    }
}
