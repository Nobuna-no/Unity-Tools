using UnityEngine;


[System.Serializable]
public class Vector2Reference : DataReference<Vector2, Vector2Variable>
{
    public Vector2Reference(Vector2 constantValue)
    {
        ConstantValue = constantValue;
    }
}
