using UnityEngine;
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

    public float driftStifness = 0.05f;
    public float normalStifness = 1;

    public float downforce = 0f;

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

        if (Input.GetButton("Fire3"))
        {
            foreach (AxleInfo axleInfo in axleInfos)
            {
                WheelFrictionCurve wfc = axleInfo.leftWheel.sidewaysFriction;
                wfc.stiffness = driftStifness;
                axleInfo.leftWheel.sidewaysFriction = wfc;

                WheelFrictionCurve wfc2 = axleInfo.rightWheel.sidewaysFriction;
                wfc2.stiffness = driftStifness;
                axleInfo.leftWheel.sidewaysFriction = wfc2;
            }
        } else
        {
            foreach (AxleInfo axleInfo in axleInfos)
            {
                WheelFrictionCurve wfc = axleInfo.leftWheel.sidewaysFriction;
                wfc.stiffness = normalStifness;
                axleInfo.leftWheel.sidewaysFriction = wfc;

                WheelFrictionCurve wfc2 = axleInfo.rightWheel.sidewaysFriction;
                wfc2.stiffness = normalStifness;
                axleInfo.leftWheel.sidewaysFriction = wfc2;
            }
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
            rb.AddRelativeTorque(new Vector3(0, airSpinPowerHorizontal * Input.GetAxis("Horizontal"), 0));
            rb.AddRelativeTorque(new Vector3(airSpinPowerHorizontal * Input.GetAxis("Vertical"), 0, 0));
        } else
        {
            rb.AddRelativeForce(-Vector3.up * downforce);
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