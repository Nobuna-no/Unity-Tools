using System.Collections;
using System.Collections.Generic;
using UnityEngine;



[CreateAssetMenu(menuName = "Framework Data/Animation/Trigger", order = 0)]
public class DATA_AnimStateSynchronizer_Trigger : DATA_AnimStateSynchronizer
{
    public StateMachineSyncTrigger AnimatorParameterName;

    public override IClonable Clone()
    {
        DATA_AnimStateSynchronizer_Trigger newData = base.Clone() as DATA_AnimStateSynchronizer_Trigger;
        newData.AnimatorParameterName = AnimatorParameterName;
        newData.AnimationKeyEventData = AnimationKeyEventData;
        return newData;
    }

    public override void ForceAnimatorParameterValue(Animator animator)
    {
        AnimatorParameterName.Trigger(animator);
    }
}
