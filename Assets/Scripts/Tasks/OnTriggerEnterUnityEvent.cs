using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class OnTriggerEnterUnityEvent : MonoBehaviour
{
    [SerializeField] UnityEvent onTriggerEnterChanges = null;
    [SerializeField] bool oneTimeUse = true;

    private void OnTriggerEnter(Collider other)
    {
        onTriggerEnterChanges?.Invoke();
        
        if(oneTimeUse == true)
        {
            gameObject.SetActive(false);
        }
    }
}
