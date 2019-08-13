using UnityEngine;


[System.Serializable]
public class FloatReference : DataReference<float, FloatVariable>
{
    public FloatReference(float value)
    {
        ConstantValue = value;
    }
}


