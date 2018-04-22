using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DetectionScript : MonoBehaviour
{

    // Use this for initialization
    void Start()
    {

    }

    public GameObject car;

    // Update is called once per frame
    void Update()
    {
        if (LineOfSight(car.transform))
        {
            Debug.Log("Detected");
        }
    }

    public double fov = 0;
    private RaycastHit hit;

    public bool LineOfSight(Transform target)
    {
        if (Vector3.Angle(target.position - transform.position, transform.forward) <= fov &&
                Physics.Linecast(this.transform.position, target.position, out hit) &&
                hit.collider.transform == target)
        {
            return true;
        }
        return false;
    }
}
