using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class CustomRigidbody : MonoBehaviour
{
    #region PROPERTIES
    [Header(".CUSTOM RIGIDBODY")]
    public bool Sleep = false;
    public float Mass = 1;
    public float UpdateTime = 0.02f;
    public bool UseGravity = false;
    public Vector3 Gravity = new Vector3(0f, 9.81f, 0f);


    [Header(".CUSTOM RIGIDBODY/Info")]
    [SerializeField, ReadOnly] private Vector3 _momentum; // (kg.m/s)
    [SerializeField, ReadOnly] private Vector3 _currentForce; // (N)
    [SerializeField, ReadOnly] private Vector3 _velocity; // (m.s)
    [SerializeField, ReadOnly] private Vector3 _acceleration; // (m.s²)
    #endregion
    

    #region UNITY METHODS
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(Tick_Coroutine());
    }

    private void OnCollisionEnter(Collision collision)
    {
        Sleep = true;
        return;
        CustomRigidbody rigid = collision.gameObject.GetComponent<CustomRigidbody>();
        if (rigid)
        {
            rigid.AddForce(Mass * _acceleration);
            AddForce(rigid.Mass * rigid._acceleration-_currentForce);
        }
        else
        {

        }
        //_acceleration = Vector3.zero;
        AddForce(-_currentForce);// = -Gravity;
    }

    //private void OnCollisionStay(Collision collision)
    //{
    //    _currentForce = -Gravity;
    //}
    #endregion


    #region PUBLIC METHODS
    [ContextMenu("Reset gravity vector")]
    public void ResetGravityVector()
    {
        Vector3 GravityVector = new Vector3(0f, 9.81f, 0f);
    }

    public void AddForce(Vector3 force)
    {
        Sleep = false;
        //_velocity1
        _currentForce += force;
    }

    public void AddForce(Vector3Variable force)
    {
        Sleep = false;
        //_velocity1
        _currentForce += force;
    }

    public void AddForce(ImpulseDescriptor impulse)
    {
        Sleep = false;
        //_pendingImpulsions.Add(impulse);
        StartCoroutine(LaunchAccelerationImpulseDuration(impulse));
        //impulse.OnImpulseEnd += () => { RemovePendingImpulsion(impulse); };
    }


    /// <summary>
    /// Add force at local location of the object. Can be interpreted as torque.
    /// </summary>
    /// <param name="force"></param>
    /// <param name="location"></param>
    public void AddForceAt(Vector3 force, Vector3 location)
    {

    }
    #endregion

    #region PRIVATE METHODS
    private void Tick()
    {
        if (UseGravity)
        {
            _acceleration = (Gravity * Mass + _currentForce) / Mass;
        }
        else
        {
            _acceleration = _currentForce / Mass;
        }

        _velocity += _acceleration * UpdateTime;// new Vector3(_acceleration.x * UpdateTime, _acceleration.y * UpdateTime, _acceleration.z * UpdateTime);

        _momentum = _velocity * Mass;


        transform.position += _velocity * UpdateTime;   
    }

    //private void RemovePendingImpulsion(ImpulseDescriptor impulse)
    //{
    //    _pendingImpulsions.Remove(impulse);
    //}
    #endregion


    #region COROUTINE
    // Update is called once per frame
    private IEnumerator Tick_Coroutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(UpdateTime);
            if (!Sleep)
            {
                Tick();
            }
        }
    }

    private IEnumerator LaunchAccelerationImpulseDuration(ImpulseDescriptor impulsion)
    {
        _currentForce += impulsion.Force;
        if(impulsion.Duration <= 0)
        {
            yield return new WaitForEndOfFrame();
        }
        else
        {
            yield return new WaitForSeconds(impulsion.Duration);
        }
        _currentForce -= impulsion.Force;
        //impulsion.OnImpulseEnd.Invoke();
        //impulsion.OnImpulseEnd = null;
    }
    #endregion



}
