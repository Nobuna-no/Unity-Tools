using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlackboardCondition_String : BlackboardConditionTemplate<string, StringVariable, StringReference>
{
    public override bool IsConditionValid()
    {
        switch (CompareAsEqual)
        {
            case EConditionComparer.Equal:
                return Variable.Value == Beta.Value;
            case EConditionComparer.NotEqual:
                return Variable.Value != Beta.Value;
            default:
                return false;
        }
    }
}
