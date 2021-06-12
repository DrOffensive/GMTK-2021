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
        Debug.Log("Press button now");
        canInteract = true;
    }

    private void OnTriggerExit(Collider other)
    {
        //FinishShowingInteractPopup
        Debug.Log("Don't press button now");
        canInteract = false;
    }

    public void OnInteractPopupPressed()
    {
        onInteract?.Invoke();
    }
}
