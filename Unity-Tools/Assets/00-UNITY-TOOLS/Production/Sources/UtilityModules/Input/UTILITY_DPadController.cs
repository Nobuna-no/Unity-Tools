using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

// Usefull class for dpad controller (gamepad controller).
public class UTILITY_DPadController : MonoBehaviour_Utility
{
    [Header(".UTILITY/DPad Controller")]
    [SerializeField] private string verticalAxisName = "DPadVertical";
    [SerializeField] private string horizontalAxisName = "DPadHorizontal";

    [SerializeField] private UnityEvent OnDPadLeft = null;
    [SerializeField] private UnityEvent OnDPadRight = null;
    [SerializeField] private UnityEvent OnDPadUp = null;
    [SerializeField] private UnityEvent OnDPadDown = null;

    private float prevVertical = 0; 
    private float prevHorizontal = 0;

    // Update is called once per frame
    void Update()
    {
        //Debug.Log(Input.GetAxisRaw(verticalAxisName) + " : " + Input.GetAxisRaw(horizontalAxisName));
        if (prevVertical != Input.GetAxisRaw(verticalAxisName))
        {
            prevVertical = Input.GetAxisRaw(verticalAxisName);
            switch (Input.GetAxisRaw(verticalAxisName))
            {
                case -1:
                    OnDPadDown?.Invoke();
                    return;
                case 1:
                    OnDPadUp?.Invoke();
                    return;
            }
        }
        if (prevHorizontal != Input.GetAxisRaw(horizontalAxisName))
        {
            prevHorizontal = Input.GetAxisRaw(horizontalAxisName);
            switch (Input.GetAxisRaw(horizontalAxisName))
            {
                case 1:
                    OnDPadLeft?.Invoke();
                    return;
                case -1:
                    OnDPadRight?.Invoke();
                    return;
            }
        }
    }
}
