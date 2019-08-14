using UnityEngine;

// Usefull for skill that you want to last for long (as DEMACIA!!! or kick combo animation)
[CreateAssetMenu(menuName = "Framework Data/Animation/Boolean", order = 1)]
public class DATA_AnimStateSynchronizer_Bool : DATA_AnimStateSynchronizer
{
    public StateMachineSyncDataOfBool AnimatorParameterName;
    public bool Value;

    public override IClonable Clone()
    {
        DATA_AnimStateSynchronizer_Bool newData = base.Clone() as DATA_AnimStateSynchronizer_Bool;
        newData.AnimatorParameterName = AnimatorParameterName;
        newData.Value = Value;
        return newData;
    }

    public override void ForceAnimatorParameterValue(Animator animator)
    {
        AnimatorParameterName.SetData(Value, animator);
    }
}
