using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VariableCondition_Bool: VariableConditionTemplate<bool, BoolVariable, BoolReference>
{
    public override bool IsConditionValid()
    {
        switch (CompareAsEqual)
        {
            case EConditionComparer.Equal:
                return Alpha.Value == Beta.Value;
            case EConditionComparer.NotEqual:
                return Alpha.Value != Beta.Value;
            default:
                return false;
        }
    }
}
