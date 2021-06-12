using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(Collider))]
public class InteractWithObject : MonoBehaviour
{
    //Interact Popup
    [SerializeField] UnityEvent onInteract = null;
    bool canInteract = false;

    private void Update()
    {
        if(canInteract == true && InputManager.GetInteract())
        {
            onInteract?.Invoke();
            canInteract = false;
        }
    }


    private void OnTriggerEnter(Collider other)
    {
        //Show interact popup
        canInteract = true;
    }

    private void OnTriggerExit(Collider other)
    {
        //FinishShowingInteractPopup
        canInteract = false;
    }

    public void OnInteractPopupPressed()
    {
        onInteract?.Invoke();
    }
}
