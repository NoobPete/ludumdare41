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
    public float airSpinPowerPitch;
    public float speedometerMultiplier;
    public float groundSpinForce;
    // Takes from the rb on start
    private float defaultDrag;
    // NOTE: also effect reverse
    public float breakDrag;

    public float speedHelper;
    public float groundSpinForceDrift;

    private Rigidbody rb;
    private Vector3 lastFramePos;

    public Text speedField;

    public float driftStifness = 0.05f;
    public float normalStifness = 1;

    public float downforce = 0f;
    private float speed = 0;

    public float maxBoost = 20f;
    public float boostRegen = 0.1f;
    public float currentBoost;

    public float boostUsage = 1f;

    public float maxJumps = 1f;
    public float jumpRegen = 0.01f;
    public float currentJump;

    public GameObject restartPoint;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        lastFramePos = transform.position;

        defaultDrag = rb.drag;
        currentBoost = maxBoost;
        currentJump = maxJumps;
    }

    void Update()
    {
        // Jump
        if (Input.GetButtonDown("Fire1"))
        {
            if (currentJump >= 1)
            {
                rb.AddRelativeForce(Vector3.up * jumpPower);
                currentJump--;
            }
        }

        currentJump = Mathf.Min (maxJumps, currentJump + jumpRegen * Time.deltaTime);

        // Drift
        if (Input.GetButton("Fire3"))
        {
            foreach (AxleInfo axleInfo in axleInfos)
            {
                {
                    WheelFrictionCurve wfc = axleInfo.leftWheel.sidewaysFriction;
                    wfc.stiffness = driftStifness;
                    axleInfo.leftWheel.sidewaysFriction = wfc;

                    WheelFrictionCurve wfc2 = axleInfo.rightWheel.sidewaysFriction;
                    wfc2.stiffness = driftStifness;
                    axleInfo.rightWheel.sidewaysFriction = wfc2;
                }

                {
                    WheelFrictionCurve wfc = axleInfo.leftWheel.forwardFriction;
                    wfc.stiffness = driftStifness;
                    axleInfo.leftWheel.forwardFriction = wfc;

                    WheelFrictionCurve wfc2 = axleInfo.rightWheel.forwardFriction;
                    wfc2.stiffness = driftStifness;
                    axleInfo.rightWheel.forwardFriction = wfc2;
                }
            }
        }
        else
        {
            foreach (AxleInfo axleInfo in axleInfos)
            {
                {
                    WheelFrictionCurve wfc = axleInfo.leftWheel.sidewaysFriction;
                    wfc.stiffness = normalStifness;
                    axleInfo.leftWheel.sidewaysFriction = wfc;

                    WheelFrictionCurve wfc2 = axleInfo.rightWheel.sidewaysFriction;
                    wfc2.stiffness = normalStifness;
                    axleInfo.rightWheel.sidewaysFriction = wfc2;
                }

                {
                    WheelFrictionCurve wfc = axleInfo.leftWheel.forwardFriction;
                    wfc.stiffness = normalStifness;
                    axleInfo.leftWheel.forwardFriction = wfc;

                    WheelFrictionCurve wfc2 = axleInfo.rightWheel.forwardFriction;
                    wfc2.stiffness = normalStifness;
                    axleInfo.rightWheel.forwardFriction = wfc2;
                }
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
        visualWheel.transform.rotation = rotation * Quaternion.Euler(180, 0, 0);
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
            if (axleInfo.leftWheel.GetGroundHit(out wh))
            {
                onGround = true;
            }
        }

        if (!onGround)
        {
            // Not on ground
            if (!Input.GetButton("Fire3"))
            {
                rb.AddRelativeTorque(new Vector3(0, airSpinPowerHorizontal * Input.GetAxis("Horizontal"), 0));
            }
            else
            {
                rb.AddRelativeTorque(new Vector3(0, 0, -airSpinPowerPitch * Input.GetAxis("Horizontal")));
            }

            rb.AddRelativeTorque(new Vector3(airSpinPowerHorizontal * Input.GetAxis("Vertical"), 0, 0));
        }
        else
        {
            // On ground

            //Turn helper
            rb.AddRelativeTorque(new Vector3(0, groundSpinForce * Input.GetAxis("Horizontal"), 0));

            // Turn helper drift
            if (Input.GetButton("Fire3"))
            {
                rb.AddRelativeTorque(new Vector3(0, groundSpinForceDrift * Input.GetAxis("Horizontal"), 0));
            }

            // Downforce helper
            rb.AddRelativeForce(-Vector3.up * (50f + speed) * downforce);

            // Break helper
            float t = 1 + Mathf.Min(Input.GetAxis("Speed"), 0);
            float newDrag = Mathf.Lerp(breakDrag, defaultDrag, t);
            rb.drag = newDrag;

            // Speed helper
            float t2 = Mathf.Max(Input.GetAxis("Speed"), 0);

            rb.AddForce(this.transform.forward * speedHelper * t2);
        }

        bool didBoost = false;
        // Boost
        if (Input.GetButton("Fire2"))
        {
            if (currentBoost >= boostUsage)
            {
                rb.AddForce(this.transform.forward * maxBoostPower);
                didBoost = true;
                currentBoost -= boostUsage;
            }
        }

        if (!didBoost)
        {
            if (onGround)
            {
                currentBoost = Mathf.Min(maxBoost, currentBoost + boostRegen);
            }
        }





        // Speedometer
        float distance = Vector3.Distance(lastFramePos, transform.position);

        speed = Mathf.Floor(distance / Time.deltaTime * speedometerMultiplier);
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