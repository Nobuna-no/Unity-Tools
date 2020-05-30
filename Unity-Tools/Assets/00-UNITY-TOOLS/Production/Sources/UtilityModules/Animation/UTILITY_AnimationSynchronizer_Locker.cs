using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UTILITY_AnimationSynchronizer_Locker : UTILITY_AnimationSynchronizer
{
    [Header(".UTILITY/Animation Synchronizer Locker Required")]
    public StateMachineSyncDataOfBool LockParameterName;

    private float LockTimestamp = 0;
    private bool OnTriggerLockRoutine = false;

    private Dictionary<string, float> AnimationClipDurationMap;

    protected override void Start()
    {
        base.Start();

        GenerateClipsDurationMap();
    }

    private void GenerateClipsDurationMap()
    {
        AnimationClipDurationMap = new Dictionary<string, float>();
        RuntimeAnimatorController RAC = OwnAnimator.runtimeAnimatorController;
        int count = RAC.animationClips.Length;
        for (int i = 0; i < count; ++i)
        {
            AnimationClip c = RAC.animationClips[i];
            string name = c.name;
            float length = c.length;
            Verbose(VerboseMask.Log, "Add clip[" + i + "]: \"" + name + "\" of length: " + length);
            AnimationClipDurationMap.Add(name, length);
        }
    }

    private void AutoLockForClipDuration()
    {
        try
        {
            LockTimestamp = AnimationClipDurationMap[OwnAnimator.GetCurrentAnimatorClipInfo(0)[0].clip.name] / OwnAnimator.GetCurrentAnimatorStateInfo(0).speed;
            // It will reset the timestamp duration and extend the coroutine if it already run.
            if (!OnTriggerLockRoutine)
            {
                StartCoroutine(Lock_Coroutine());
            }
        }
        catch
        {
            Verbose(VerboseMask.WarningLog, "Fail to AutoLockForClipDuration!");
        }
    }

    private IEnumerator Lock_Coroutine()
    {
        OnTriggerLockRoutine = true;
        SetLock(true);

        while (LockTimestamp > 0)
        {
            LockTimestamp -= Time.deltaTime * TimeScale;
            yield return new WaitForFixedUpdate();
        }

        SetLock(false);
        OnTriggerLockRoutine = false;
    }


    private void Unlock()
    {
        LockTimestamp = 0f;
    }


    public void SetLock(bool value)
    {
        LockParameterName.SetData(value, OwnAnimator);
    }
}