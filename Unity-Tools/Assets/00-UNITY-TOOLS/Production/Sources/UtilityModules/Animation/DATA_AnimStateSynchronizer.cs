using UnityEngine;
using UnityEngine.Events;

[System.Serializable]
public class DATA_AnimationKeyEventData
{
    public float AnimationClipSample = 60f;
    public float KeyEventFrame = 0f;
}

public class DATA_AnimStateSynchronizer : ScriptableObject, IClonable
{
    public enum EAnimatorCallbackListener
    {
        All,

        Assault,
        Momentum,
        Special,

        None,
        Other_ASKDEV
    }

    [Header(".DATA/Anim State Synchronizer")]
    public string Description;
    public EAnimatorCallbackListener AnimatorCallbackListener;
    [System.NonSerialized]
    public UnityAction Response;
    public DATA_AnimationKeyEventData AnimationKeyEventData;

    public virtual IClonable Clone()
    {
        DATA_AnimStateSynchronizer newData = (DATA_AnimStateSynchronizer)CreateInstance(GetType());
        newData.AnimatorCallbackListener = AnimatorCallbackListener;
        return newData;
    }

    public virtual void ForceAnimatorParameterValue(Animator animator)
    { }
}