using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlackboardCondition_Float : BlackboardConditionTemplate<float, FloatVariable, FloatReference>
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
                return Variable > Beta;
            case EConditionComparer.MoreOrEqualThan:
                return Variable >= Beta;
            case EConditionComparer.LessThan:
                return Variable < Beta;
            case EConditionComparer.LessOrEqualThan:
                return Variable <= Beta;
            default:
                return false;
        }
    }
}
