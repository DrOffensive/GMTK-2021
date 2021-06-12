using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody), typeof(HingeJoint))]
public class Bone : MonoBehaviour
{
    [SerializeField] Rigidbody connectedBone;
    Rigidbody Rigidbody => GetComponent<Rigidbody>();
    HingeJoint HingeJoint => GetComponent<HingeJoint>();

    private void Start()
    {
        Setup();
    }

    public void Setup ()
    {
        transform.SetParent(null);
        HingeJoint.connectedBody = connectedBone;
    }

}
