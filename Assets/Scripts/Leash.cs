using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class Leash : MonoBehaviour
{

    LineRenderer LineRenderer => GetComponent<LineRenderer>();
    Rigidbody Rigidbody => GetComponent<Rigidbody>();
    [SerializeField] Transform target;
    [SerializeField] float distance;
    [SerializeField] float smoothDampTime = 1f;
    Vector3 targetPosition = Vector3.zero;
    Vector3 currentVelocity = Vector3.zero;

    private void Start()
    {
        LineRenderer.positionCount = 2;
    }

    private void FixedUpdate()
    {
        Vector3 line = Rigidbody.position - target.position;

        if (line.magnitude > distance)
        {
            targetPosition = target.position + (line.normalized * distance);
            Rigidbody.MovePosition(Vector3.SmoothDamp(transform.position, targetPosition, ref currentVelocity, smoothDampTime));
        }

        LineRenderer.SetPosition(0, target.position);
        LineRenderer.SetPosition(1, transform.position);
    }
}
