using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[System.Serializable]
public struct KeyController
{
    public string Name;
    public DATA_Key KeyToObserve;
    public UnityEvent OnPressed;
    public UnityEvent OnReleased;
    public UnityEvent OnStay;
}

public class UTILITY_InputController : MonoBehaviour_Utility
{
    #region PROPERTIES
    [Header(".INPUT UTILITY")]
    public DATA_Axis AxisValue;
    [ReadOnly]
    public Vector2 DebugAxisValue;
    [Range(0f, .9f)]
    public float AxisMinimumTreshold = 0.01f;
    public string HorizontalInputName = "Horizontal";
    public string VerticalInputName = "Vertical";


    [Header(".INPUT UTILITY/Keys")]
    public List<KeyController> Keys;
    #endregion

    #region UNITY METHODS
    private void UpdateAxisTimeStamp(float x, float y, float delta)
    {
        if (x == 0)
        {
            if (AxisValue.TimestampX > 0)
            {
                AxisValue.TimestampX.Value = 0;
            }
            AxisValue.TimestampX.Value -= delta;
        }
        else
        {
            if (AxisValue.TimestampX < 0)
            {
                AxisValue.TimestampX.Value = 0;
            }
            AxisValue.TimestampX.Value += delta;
        }

        if (y == 0)
        {
            if (AxisValue.TimestampY > 0)
            {
                AxisValue.TimestampY.Value = 0;
            }
            AxisValue.TimestampY.Value -= delta;
        }
        else
        {
            if (AxisValue.TimestampY < 0)
            {
                AxisValue.TimestampY.Value = 0;
            }
            AxisValue.TimestampY.Value += delta;
        }
    }

    // Update is called once per frame
    void Update()
    {
        float delta = Time.deltaTime;

        if(AxisValue)
        {
            float x = Input.GetAxisRaw(HorizontalInputName);
            if (AxisValue.Value.x != x)
            {
                x = Mathf.Abs(x) > AxisMinimumTreshold ? x : 0;
            }
            float y = Input.GetAxisRaw(VerticalInputName);
            if (AxisValue.Value.y != y)
            {
                y = Mathf.Abs(y) > AxisMinimumTreshold ? y : 0;
            }
            AxisValue.Value = new Vector2(x, y);
            if(HasFlag(VerboseMask.Control))
            {
                DebugAxisValue = AxisValue.Value;
            }

            UpdateAxisTimeStamp(x, y, delta);
        }

        foreach (KeyController keyController in Keys)
        {
            DATA_Key dataKey = keyController.KeyToObserve;
            KeyCode[] keys = dataKey.KeyCodes;
            float count = keys.Length;

            for (int i = 0; i < count; ++i)
            {
                // If key pressed for the 1st time.
                if (Input.GetKeyDown(keys[i]))
                {
                    dataKey.Value = true;
                    dataKey.Timestamp = 0;
                    keyController.OnPressed?.Invoke();
                    break;
                }
                else if (dataKey.Value)
                {
                    bool allKeyReleased = true;
                    for (int j = 0; j < count; ++j)
                    {
                        if(Input.GetKey(keys[j]))
                        {
                            allKeyReleased = false;
                            break;
                        }
                    }

                    // If there no remaining key pressed... key up for the first time.
                    if (allKeyReleased)
                    {
                        dataKey.Value = false;
                        dataKey.Timestamp = 0;
                        keyController.OnReleased?.Invoke();
                    }
                    // if key is pressed.
                    else
                    {
                        dataKey.Timestamp += delta;
                        keyController.OnStay?.Invoke();
                        break;
                    }
                }
                // if key is released.
                else
                {
                    keyController.KeyToObserve.Timestamp -= delta;
                }
            }
        }
    }
    #endregion
}
