using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu(fileName = "[ImpulseDesc]", menuName = "Framework Data/Physics/Impulse")]
public class ImpulseDescriptor : ScriptableObject
{
    public Vector3 Force = Vector3.zero;
    public float Duration = -1f;
    public UnityAction OnImpulseEnd;
}
