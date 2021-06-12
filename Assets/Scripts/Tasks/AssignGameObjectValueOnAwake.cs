using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AssignGameObjectValueOnAwake : MonoBehaviour
{
    [SerializeField] GameObjectReference scriptableObjectToAssignTo;
    [SerializeField] bool shouldDisableAfter = true;

    private void Awake()
    {
        scriptableObjectToAssignTo.Value = gameObject;
        gameObject.SetActive(!shouldDisableAfter);
    }
}
