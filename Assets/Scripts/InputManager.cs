using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputManager : MonoBehaviour
{
    //Maybe camera stuff later on not needed rn

    public static float GetUpDownAxis()
    {
        return Input.GetAxis("Horizontal");
    }

    public static float GetLeftRightAxis()
    {
        return Input.GetAxis("Vertical");
    }

    public static bool GetInteract()
    {
        return Input.GetButtonDown("Interact");
    }
}
