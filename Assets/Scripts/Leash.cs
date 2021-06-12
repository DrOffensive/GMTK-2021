using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Leash : MonoBehaviour
{
    Rigidbody Rigidbody => GetComponent<Rigidbody>();
    [SerializeField] Transform target;
    [SerializeField] float distance;
    private void FixedUpdate()
    {

        Vector3 line = Rigidbody.position - target.position;
        if (line.magnitude > distance)
            Rigidbody.MovePosition(target.position + (line.normalized * distance));
        


    }
}
