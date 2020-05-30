using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VariableCondition_String : VariableConditionTemplate<string, StringVariable, StringReference>
{
    public override bool IsConditionValid()
    {
        switch (CompareAsEqual)
        {
            case EConditionComparer.Equal:
                return Alpha.Value == Beta;
            case EConditionComparer.NotEqual:
                return Alpha.Value != Beta;
            default:
                return false;
        }
    }
}
