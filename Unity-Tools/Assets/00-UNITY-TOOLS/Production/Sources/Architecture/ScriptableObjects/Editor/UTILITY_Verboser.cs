using UnityEngine;

[CreateAssetMenu(menuName = "Framework Data/Debug/Verboser", order = 100)]
public class UTILITY_Verboser : ScriptableObject
{
    public StringReference Message;

    public void Verbose()
    {
        Debug.Log("<" + this + ">: " + Message);
    }

    public void Verbose(string text)
    {
        Debug.Log("<" + this + ">: " + text);
    }

    public void Verbose(ScriptableObject text)
    {
        Debug.Log("<" + this + ">: " + text.ToString());
    }

    public void VerboseWarning(string text)
    {
        Debug.LogWarning("<" + this + ">: " + text);
    }

    public void VerboseBreakError(string text)
    {
        Debug.Break();
        Debug.LogError("<" + this + ">: " + text);
    }
}
