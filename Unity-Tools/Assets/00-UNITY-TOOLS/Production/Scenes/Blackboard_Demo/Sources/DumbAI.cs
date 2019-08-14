using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class DumbAI : MonoBehaviour
{
    [SerializeField]
    private DATA_BlackBoard _Blackboard = null;

    [SerializeField]
    private BBP_GameObject _BBPTarget = null;
    

    // Start is called before the first frame update
    void Start()
    {
        _BBPTarget.Initialize(_Blackboard, this.gameObject);
        _BBPTarget.Variable.OnValueChanged += UpdateState;

        UpdateState();
        GetComponent<Collider>().isTrigger = true;
    }

    private void UpdateState()
    {
        if (_BBPTarget.Variable.Value != null)
        {
            GetComponent<MeshRenderer>().material.color = Color.red;
        }
        else
        {
            GetComponent<MeshRenderer>().material.color = Color.black;
        }
    }


    private void OnTriggerStay(Collider other)
    {
        if(other.gameObject.layer != gameObject.layer)
        {
            _BBPTarget.Variable.Value = other.gameObject;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if(other.gameObject.layer != gameObject.layer)
        {
            _BBPTarget.Variable.Value = null;
        }
    }
}
