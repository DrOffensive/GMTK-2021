using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class KickManager : MonoBehaviour
{

    Rigidbody Rigidbody => GetComponent<Rigidbody>();
    [SerializeField] Transform doggo;
    [SerializeField] Leg leftLeg, rightLeg;
    [SerializeField] float kickForce;
    [SerializeField] float artificialForce;

    [System.Serializable]
    public struct Leg
    {
        public HingeJoint thigh, calf;
    }

    private void Start()
    {
        leftLeg.thigh.useMotor = false;
        rightLeg.thigh.useMotor = false;
        leftLeg.calf.useMotor = false;
        rightLeg.calf.useMotor = false;


    }

    private void Update()
    {
        if (InputManager.GetLeftRightAxis() < 0)
            Kick(true);
        else if(InputManager.GetLeftRightAxis() > 0)
            Kick(false);
        else
        {
            leftLeg.thigh.useMotor = false;
            rightLeg.thigh.useMotor = false;
            leftLeg.calf.useMotor = false;
            rightLeg.calf.useMotor = false;
        }
    }

    void Kick (bool left)
    {
        if (left)
        {
            Rigidbody.AddForce(doggo.TransformDirection(Vector3.left) * artificialForce, ForceMode.Impulse);
            leftLeg.thigh.useMotor = true;
            rightLeg.thigh.useMotor = false;
            var motor = leftLeg.thigh.motor;
            //motor.targetVelocity = kickForce;
            motor.force = kickForce;
            leftLeg.thigh.motor = motor;
            leftLeg.calf.useMotor = true;
            rightLeg.calf.useMotor = false;
            motor = leftLeg.calf.motor;
            //motor.targetVelocity = -kickForce;
            motor.force = -kickForce;
            leftLeg.calf.motor = motor;
        }
        else
        {
            Rigidbody.AddForce(doggo.TransformDirection(Vector3.right) * artificialForce, ForceMode.Impulse);
            leftLeg.thigh.useMotor = false;
            rightLeg.thigh.useMotor = true;
            var motor = rightLeg.thigh.motor;
            //motor.targetVelocity = kickForce;
            motor.force = kickForce;
            rightLeg.thigh.motor = motor;
            leftLeg.calf.useMotor = false;
            rightLeg.calf.useMotor = true;
            motor = rightLeg.calf.motor;
            //motor.targetVelocity = -kickForce;
            motor.force = -kickForce;
            rightLeg.calf.motor = motor;
        }
    }
}
