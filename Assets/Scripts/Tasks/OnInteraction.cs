using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(Collider))]
public class OnInteraction : MonoBehaviour
{
    [SerializeField] UnityEvent onCorrectInteractionChanges = null;
    [SerializeField] UnityEvent onIncorrectInteractionChanges = null;
    bool isWithinCorrectZone = false;

    private void OnTriggerEnter(Collider other)
    {
        isWithinCorrectZone = true;
    }

    private void OnTriggerExit(Collider other)
    {
        isWithinCorrectZone = false;
    }

    public void OnInteractionProc()
    {
        if(isWithinCorrectZone == true)
        {
            OnCorrectInteraction();
        }
        else
        {
            OnIncorrectInteraction();
        }
    }

    public void OnCorrectInteraction()
    {
        onCorrectInteractionChanges?.Invoke();
    }

    public void OnIncorrectInteraction()
    {
        onIncorrectInteractionChanges.Invoke();
    }
}
