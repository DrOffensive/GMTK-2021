using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AssignButtonOnAwake : MonoBehaviour
{
    [SerializeField] InteractionButton scriptableObjectToAssignTo;
    [SerializeField] bool shouldDisableAfter = true;

    private void Awake()
    {
        scriptableObjectToAssignTo.InterationButtonObject = gameObject;
        gameObject.SetActive(!shouldDisableAfter);
    }
}
