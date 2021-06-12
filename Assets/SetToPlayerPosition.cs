using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetToPlayerPosition : MonoBehaviour
{
    [SerializeField] GameObjectReference playerReference = null;

    public void SetPositionToPlayerPosition()
    {
        transform.position = playerReference.Value.transform.position;
    }
}
