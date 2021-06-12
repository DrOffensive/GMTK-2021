using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Leash : MonoBehaviour
{
    Rigidbody Rigidbody => GetComponent<Rigidbody>();
    [SerializeField] Transform target;
    [SerializeField] float distance;
    [SerializeField] float smoothDampTime = 1f;
    Vector3 targetPosition = Vector3.zero;
    Vector3 currentVelocity = Vector3.zero;

    private void FixedUpdate()
    {
        Vector3 line = Rigidbody.position - target.position;

        if (line.magnitude > distance)
        {
            targetPosition = target.position + (line.normalized * distance);
            Rigidbody.MovePosition(Vector3.SmoothDamp(transform.position, targetPosition, ref currentVelocity, smoothDampTime));
        }
    }
}
