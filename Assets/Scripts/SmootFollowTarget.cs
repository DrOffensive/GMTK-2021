using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SmootFollowTarget : MonoBehaviour
{
    [SerializeField] Transform lookAtTarget = null;
    [SerializeField] Transform targetTransform = null;
    [SerializeField] Vector3 positionOffset = Vector3.zero;
    [SerializeField] float timeOfSmoothDamp = 1f;
    Vector3 targetPosition = Vector3.zero;
    Vector3 currentVelocity = Vector3.zero;


    // Update is called once per frame
    void Update()
    {
        targetPosition = targetTransform.TransformPoint(positionOffset);
        targetPosition = Vector3.SmoothDamp(transform.position, targetPosition, ref currentVelocity, timeOfSmoothDamp);
        targetPosition.y = transform.position.y;

        transform.position = targetPosition;
        transform.LookAt(lookAtTarget.position);
    }
}
