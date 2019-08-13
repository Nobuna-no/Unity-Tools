using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class MonoBehaviour_Scheduler : MonoBehaviour_Utility
{
    [Header(".SCHEDULER/Settings")]
    [SerializeField, Range(0f, 1f)]
    protected float TickUpdateInSecond = 0.02f;
    //protected bool Pause;

    protected virtual void Start()
    {
        StartCoroutine(Update_Coroutine());
    }

    protected abstract void Tick();
    //protected abstract void Pause();
    //protected abstract void Resume();

    // Update is called once per frame
    private IEnumerator Update_Coroutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(TickUpdateInSecond);
            Tick();
        }
    }
}