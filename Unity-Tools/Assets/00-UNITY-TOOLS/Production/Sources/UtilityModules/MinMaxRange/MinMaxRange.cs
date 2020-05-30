using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct MinMaxRange
{
    public float MinValue /*= 0*/;
    public float MaxValue /*= 0*/;
    [SerializeField, HideInInspector]
    private float MinLimit /*= -1*/;
    [SerializeField, HideInInspector]
    private float MaxLimit /*= 1*/;

    public MinMaxRange(float minLimit, float maxLimit, float minValue = 0, float maxValue = 0)
    {
        MinLimit = minLimit;
        MaxLimit = maxLimit;
        MinValue = minValue;
        MaxValue = maxValue;
    }

    /// <summary>
    /// Draw a random value between Min and Max values.
    /// </summary>
    /// <returns></returns>
    public float Draw()
    {
        if (MinValue == MaxValue)
            return MinValue;

        return Random.Range(MinValue, MaxValue);
    }
}