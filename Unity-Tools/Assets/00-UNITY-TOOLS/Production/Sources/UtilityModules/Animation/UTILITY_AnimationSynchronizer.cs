using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;



public class UTILITY_AnimationSynchronizer : MonoBehaviour_Utility
{
    #region ENUM
    public enum ERendererMode
    {
        SpriteRenderer,
        MeshRenderer,
    }
    #endregion


    #region PROPERTIES
    [Header(".UTILITY/Animation Synchronizer")]
    public ERendererMode Mode;
    public bool EnableLightingModule;
    public bool InvertXFacing = false;
    public bool AutoUpdateFacing = true;

    public FloatVariable DirectionRef;
    public DATA_BlackBoard Data;

    [Header(".UTILITY/Optional")]
    public SpriteRenderer OwnRenderer;
    public Animator OwnAnimator;
    public FloatReference TimeScale;


    private DATA_AnimStateSynchronizer[] TriggerEventSynchronizer;

    public UnityAction AssaultKeyResponse;
    public UnityAction SpecialKeyResponse;
    public UnityAction MomentumKeyResponse;

    private Dictionary<DATA_AnimStateSynchronizer, UnityAction> RuntimeAnimatorSynchronizerEvents;
    
    protected Transform OwnTransform;
    protected FloatVariable AnimationSpeedRef;
    #endregion


    #region UNITY METHODS
    private void Awake()
    {
        //meshInstances = GetComponentsInChildren<Anima2D.SpriteMeshInstance>();
    }

    protected virtual void Start()
    {
        InitOwners();

        InitReferences();

        //ResetLanding();
    }

    public void OnDisable()
    {
        RuntimeAnimatorSynchronizerEvents?.Clear();

        if (DirectionRef && DirectionRef.OnValueChanged != null)
        {
            DirectionRef.OnValueChanged -= UpdateFacing;
        }
        if (TimeScale.Variable && TimeScale.Variable.OnValueChanged != null)
        {
            TimeScale.Variable.OnValueChanged -= UpdateAnimationSpeed;
        }
        if (AnimationSpeedRef && AnimationSpeedRef.OnValueChanged != null)
        {
            AnimationSpeedRef.OnValueChanged -= UpdateAnimationSpeed;
        }
    }
    #endregion


    #region PRIVATE METHODS

    private void InitOwners()
    {
        if (Mode == ERendererMode.SpriteRenderer)
        {
            if (!OwnRenderer)
            {
                OwnRenderer = GetComponent<SpriteRenderer>();
                if (OwnRenderer == null)
                {
                    Debug.LogError("Renderer is empty");
                    return;
                }
            }
            if (OwnRenderer && EnableLightingModule)
            {
                OwnRenderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.On;
                OwnRenderer.receiveShadows = true;
            }
        }
        else
        {
            OwnTransform = GetComponent<Transform>();
        }

        if (!OwnAnimator)
        {
            OwnAnimator = GetComponent<Animator>();
            if (!OwnAnimator)
            {
                Debug.LogError(this + " no animator found!!!");
                return;
            }
        }

        RuntimeAnimatorSynchronizerEvents = new Dictionary<DATA_AnimStateSynchronizer, UnityAction>();
    }

    protected virtual void InitReferences()
    {
        // Step 1: Try to get a Core2D and setup time scale callback
        // UNIT_Core2D temp = GetComponent<UNIT_Core2D>();
        // if (!temp)
        // {
        //     temp = GetComponentInParent<UNIT_Core2D>();
        //     if (!temp)
        //     {
        //         TimeScale = ScriptableObject.CreateInstance<FloatVariable>();
        //         TimeScale.Value = 1f;
        //         return;
        //     }
        // }

        //DirectionRef = temp.DirectionRef;
        // TimeScale = temp.CoreStats.TimeScale;
        // AnimationSpeedRef = temp.CoreStats.AnimationSpeed;

        // TimeScale.OnValueChanged += UpdateAnimationSpeed;
        // AnimationSpeedRef.OnValueChanged += UpdateAnimationSpeed;

        if(AutoUpdateFacing)
        {
            DirectionRef.OnValueChanged += UpdateFacing;
        }
    }

    public void UpdateFacing()
    {
        bool b = DirectionRef < 0f;
        if(InvertXFacing)
        {
            b = !b;
        }

        FlipX(b);
    }

    private void UpdateAnimationSpeed()
    {
        OwnAnimator.speed = AnimationSpeedRef * TimeScale;
    }

    private void BindRuntimeAnimSyncResponse(DATA_AnimStateSynchronizer tmp)
    {
        switch (tmp.AnimatorCallbackListener)
        {
            case DATA_AnimStateSynchronizer.EAnimatorCallbackListener.Assault:
                AssaultKeyResponse += tmp.Response;
                break;
            case DATA_AnimStateSynchronizer.EAnimatorCallbackListener.Momentum:
                MomentumKeyResponse += tmp.Response;
                break;
            case DATA_AnimStateSynchronizer.EAnimatorCallbackListener.Special:
                SpecialKeyResponse += tmp.Response;
                break;
            case DATA_AnimStateSynchronizer.EAnimatorCallbackListener.All:
                AssaultKeyResponse += tmp.Response;
                AssaultKeyResponse += () => 
                {
                    SpecialKeyResponse -= tmp.Response;
                    MomentumKeyResponse -= tmp.Response;
                };
                SpecialKeyResponse += tmp.Response;
                SpecialKeyResponse += () =>
                {
                    AssaultKeyResponse -= tmp.Response;
                    MomentumKeyResponse -= tmp.Response;
                };
                MomentumKeyResponse += tmp.Response;
                MomentumKeyResponse += () =>
                {
                    AssaultKeyResponse -= tmp.Response;
                    SpecialKeyResponse -= tmp.Response;
                };
                break;

            default:
                break;
        }
    }
    #endregion
        
        
    #region PUBLIC METHODS
    public void ForceFlip()
    {
        DirectionRef.Invert();
        UpdateFacing();
    }

    public void FlipX(bool flip)
    {
        if (Mode == ERendererMode.SpriteRenderer)
        {
            OwnRenderer.flipX = flip;
        }
        else
        {
            Vector3 temp = OwnTransform.localEulerAngles;
            temp.y = (flip ? 180f : 0f);
            OwnTransform.localEulerAngles = temp;

            // FlipNormals(flip);
        }
    }

    public void Flip()
    {
        if (Mode == ERendererMode.SpriteRenderer)
        {
            OwnRenderer.flipX = !OwnRenderer.flipX;
        }
        else
        {
            Vector3 temp = OwnTransform.localEulerAngles;
            temp.y = (temp.y == 0 ? 180f : 0f);
            OwnTransform.localEulerAngles = temp;

            //FlipNormals(temp.y != 0);
        }
    }

    // Usefull to sync ablity animation!
    public bool TrySyncAnimState(DATA_AnimStateSynchronizer assync, bool value = false)
    {
        // Way A: Try get the anim synchronizer as trigger. 
        DATA_AnimStateSynchronizer_Trigger tmp = assync as DATA_AnimStateSynchronizer_Trigger;
        if (tmp)
        {
            // Step A.1: Check if there is the method to procedurally invoke the parameter triggering for this anim sync. 
            if (!RuntimeAnimatorSynchronizerEvents.ContainsKey(assync))
            {
                // If not, create the lambda and store it in a map using the anim sync as key.
                RuntimeAnimatorSynchronizerEvents.Add(tmp, () => { tmp.AnimatorParameterName.Trigger(OwnAnimator); });
            }

            // Step A.2: Bind delegate "Response" of the anim synchronizer to the wanted event (which may be raised from the animation clip).
            BindRuntimeAnimSyncResponse(assync);

            AnimatorClipInfo[] a = OwnAnimator.GetNextAnimatorClipInfo(0);
            for(int i = 0; i < a.Length; ++i)
            {
                Verbose(VerboseMask.Log, "Current clip BEFORE invoke:" + a[i].clip.name);
            }
            // Step A.3: Trigger the parameter.
            RuntimeAnimatorSynchronizerEvents[tmp].Invoke();

            a = OwnAnimator.GetNextAnimatorClipInfo(0);
            for (int i = 0; i < a.Length; ++i)
            {
                Verbose(VerboseMask.Log, "Current clip AFTER invoke:" + a[i].clip.name);
            }
            return true;
        }
    
        // Way B: Same as way A for boolean parameters.
        DATA_AnimStateSynchronizer_Bool tmp2 = assync as DATA_AnimStateSynchronizer_Bool;
        if (tmp2)
        {
            if (!RuntimeAnimatorSynchronizerEvents.ContainsKey(assync))
            {
                RuntimeAnimatorSynchronizerEvents.Add(tmp2, () => { tmp2.AnimatorParameterName.SetData(value, OwnAnimator); });
            }
            Verbose(VerboseMask.Control, "Current clip BEFORE invoke:" + OwnAnimator.GetCurrentAnimatorClipInfo(0)[0].clip.name);
            BindRuntimeAnimSyncResponse(assync);
            RuntimeAnimatorSynchronizerEvents[tmp2].Invoke();
            Verbose(VerboseMask.Control, "Current clip AFTER invoke:" + OwnAnimator.GetCurrentAnimatorClipInfo(0)[0].clip.name);
            return true;
        }

        return false;
    }
    #endregion


    #region RESPONSIVE ANIMATOR EVENT
    public void OnAssaultEvent()
    {
        AssaultKeyResponse?.Invoke();
        AssaultKeyResponse = null;
    }


    public void OnMomentumEvent()
    {
        MomentumKeyResponse?.Invoke();
        MomentumKeyResponse = null;
    }


    public void OnSpecialEvent()
    {
        SpecialKeyResponse?.Invoke();
        SpecialKeyResponse = null;
    }
    #endregion
}


//public StateMachineSyncDataOfBool CrouchingParameterName;
//public StateMachineSyncDataOfBool WalkingParameterName;
//public StateMachineSyncDataOfBool RunningParameterName;
//public StateMachineSyncDataOfBool FallingParameterName;
//public StateMachineSyncDataOfBool IFrameParameterName;
//public StateMachineSyncDataOfBool LockParameterName;
//public StateMachineSyncDataOfBool WallRunParameterName;
//public StateMachineSyncDataOfBool WallGripParameterName;

//public StateMachineSyncDataOfBool LandingBoolParameterName;
//public StateMachineSyncTrigger LandingParameterName;
//public StateMachineSyncTrigger ClimbingParameterName;
//public StateMachineSyncTrigger DeathParameterName;

/*public void TriggerDeath()
{
    DeathParameterName.Trigger(OwnAnimator);
}

public void TriggerLanding()
{
    LandingParameterName.Trigger(OwnAnimator);
}

public void SetLanding(bool value)
{
    LandingBoolParameterName.SetData(value, OwnAnimator);
}

public void ResetLanding()
{
    OwnAnimator.ResetTrigger(LandingParameterName.ParameterName);
    //LandingParameterName.Trigger(OwnAnimator);
}

public void TriggerClimbing()
{
    ClimbingParameterName.Trigger(OwnAnimator);
}

public void SetCrouching(bool value)
{
    CrouchingParameterName.SetData(value, OwnAnimator);
}

public void SetWalking(bool value)
{
    WalkingParameterName.SetData(value, OwnAnimator);
}

public void SetWallRun(bool value)
{
    WallRunParameterName.SetData(value, OwnAnimator);
}

public void SetWallGrip(bool value)
{
    WallGripParameterName.SetData(value, OwnAnimator);
}

public void SetRunning(bool value)
{
    RunningParameterName.SetData(value, OwnAnimator);
}

public void SetFalling(bool value)
{
    FallingParameterName.SetData(value, OwnAnimator);
}

public void SetIFrame(bool value)
{
    IFrameParameterName.SetData(value, OwnAnimator);
}


*/
/// <summary>
/// Trigger whatever you want with Trigger....()!
/// </summary>
