using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(Collider))]
public class InteractWithObject : MonoBehaviour
{
    //Interact Popup
    [SerializeField] UnityEvent onInteract = null;
    [SerializeField] GameObjectReference interactionButtonScriptable = null;
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
        if(interactionButtonScriptable != null)
        {
            interactionButtonScriptable.Value.SetActive(true);
        }

        canInteract = true;
    }

    private void OnTriggerExit(Collider other)
    {
        if (interactionButtonScriptable != null)
        {
            interactionButtonScriptable.Value.SetActive(false);
        }

        canInteract = false;
    }

    public void OnInteractPopupPressed()
    {
        onInteract?.Invoke();        
    }
}
