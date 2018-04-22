using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuestPointerScript : MonoBehaviour
{
    public GameObject car;
    public Vector3 offset;
    public GameObject target;
    public GameObject questGiver;

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        transform.position = car.transform.position + offset;
        if (target != null)
        {
            transform.LookAt(target.transform.position);
        }
        else
        {
            transform.LookAt(questGiver.transform.position);
        }
    }
}
