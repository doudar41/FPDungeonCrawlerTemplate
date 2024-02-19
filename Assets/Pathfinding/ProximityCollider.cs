using System.Collections;
using System.Collections.Generic;
using UnityEngine;using UnityEngine.Events;

public class ProximityCollider : MonoBehaviour
{

    public UnityEvent<Collider> OnEnter, OnExit;

    private void OnTriggerEnter(Collider other)
    {
        print("trigger");
        OnEnter.Invoke(other);
    }

    private void OnTriggerExit(Collider other)
    {
        OnExit.Invoke(other);
    }

    private void OnDestroy()
    {
        OnEnter.RemoveAllListeners();
        OnExit.RemoveAllListeners();

    }
}
