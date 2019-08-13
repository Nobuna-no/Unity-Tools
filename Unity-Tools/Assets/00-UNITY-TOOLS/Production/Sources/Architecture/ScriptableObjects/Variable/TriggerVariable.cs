using UnityEngine;
using UnityEngine.Events;


[CreateAssetMenu(menuName = "Framework Data/Variable/Trigger", order = 0)]
public class TriggerVariable : ScriptableObject
{
    public UnityAction OnTrigger;

    public virtual void Trigger()
    {
        if (OnTrigger != null)
        {
            OnTrigger.Invoke();
        }
    }
}

