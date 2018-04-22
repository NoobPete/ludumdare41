﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class CarController : MonoBehaviour
{
    public List<AxleInfo> axleInfos; // the information about each individual axle
    public float maxMotorTorque; // maximum torque the motor can apply to wheel
    public float maxSteeringAngle; // maximum steer angle the wheel can have

    public float maxBoostPower;
    public float jumpPower;

    public float airSpinPowerHorizontal;
    public float airSpinPowerVertical;
    public float speedometerMultiplier;

    private Rigidbody rb;
    private Vector3 lastFramePos;

    public Text speedField;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        lastFramePos = transform.position;
    }

    void Update()
    {
        if (Input.GetButtonDown("Fire1"))
        {
            rb.AddForce(Vector3.up * jumpPower);
        }
    }

    // finds the corresponding visual wheel
    // correctly applies the transform
    public void ApplyLocalPositionToVisuals(WheelCollider collider)
    {
        if (collider.transform.childCount == 0)
        {
            return;
        }

        Transform visualWheel = collider.transform.GetChild(0);

        Vector3 position;
        Quaternion rotation;
        collider.GetWorldPose(out position, out rotation);

        visualWheel.transform.position = position;
        visualWheel.transform.rotation = rotation * Quaternion.Euler(180,0,0);
    }

    public void FixedUpdate()
    {
        float motor = maxMotorTorque * Input.GetAxis("Speed");
        float steering = maxSteeringAngle * Input.GetAxis("Horizontal");

        bool onGround = false;

        foreach (AxleInfo axleInfo in axleInfos)
        {
            if (axleInfo.steering)
            {
                axleInfo.leftWheel.steerAngle = steering;
                axleInfo.rightWheel.steerAngle = steering;
            }
            if (axleInfo.motor)
            {
                axleInfo.leftWheel.motorTorque = motor;
                axleInfo.rightWheel.motorTorque = motor;
            }
            ApplyLocalPositionToVisuals(axleInfo.leftWheel);
            ApplyLocalPositionToVisuals(axleInfo.rightWheel);

            WheelHit wh;
            if (axleInfo.leftWheel.GetGroundHit(out wh)) {
                onGround = true;
            }
        }

        if (!onGround) {
            rb.AddRelativeTorque(new Vector3(0, airSpinPowerHorizontal * Time.deltaTime * Input.GetAxis("Horizontal"), 0));
            rb.AddRelativeTorque(new Vector3(airSpinPowerHorizontal * Time.deltaTime * Input.GetAxis("Vertical"), 0, 0));
        }


        if (Input.GetButton("Fire2"))
        {
            rb.AddForce(this.transform.forward * maxBoostPower * Time.deltaTime);
        }

        float distance = Vector3.Distance(lastFramePos, transform.position);

        speedField.text = Mathf.Floor(distance / Time.deltaTime * speedometerMultiplier).ToString();

        lastFramePos = transform.position;
    }
}

[System.Serializable]
public class AxleInfo
{
    public WheelCollider leftWheel;
    public WheelCollider rightWheel;
    public bool motor; // is this wheel attached to motor?
    public bool steering; // does this wheel apply steer angle?
}