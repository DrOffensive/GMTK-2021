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
        if(other.CompareTag("Player") == false)
        {
            return;
        }

        isWithinCorrectZone = true;
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player") == false)
        {
            return;
        }

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
