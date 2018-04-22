using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PanRotateCycleScript : MonoBehaviour {
    public Transform objectToRotate;
    public float rotationSpeed;
    public float rotateTime;

    private float time = 0;
    private float direction = 1;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {

        objectToRotate.transform.Rotate(Vector3.up, rotationSpeed * Time.deltaTime * direction);

        time += Time.deltaTime * direction;

        if (direction == 1)
        {
            if (time >= rotateTime)
            {
                direction = -1;
            }
        } else
        {
            if (time <= -rotateTime)
            {
                direction = 1;
            }
        }
	}
}
