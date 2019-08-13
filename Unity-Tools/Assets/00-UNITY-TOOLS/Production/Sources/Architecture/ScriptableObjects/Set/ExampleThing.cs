using UnityEngine;

public class ExampleThing : MonoBehaviour
{
    public ExampleRuntimeSet RuntimeSet;

    private void OnEnable()
    {
        RuntimeSet.Add(this);
    }

    private void OnDisable()
    {
        RuntimeSet.Remove(this);
    }
}
