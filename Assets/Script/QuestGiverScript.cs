using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class QuestGiverScript : MonoBehaviour {
    public float objectiveTimePassed;
    public GameObject topSecretPrefab;
    public Transform[] placesToPlaceObjectives;
    public float distanceToPoint;
    public float baseMoney;
    public float distanceMultiplierMoney;

    public static bool hasObjective = false;
    public static int money = 500;

    public Text timePassedUI;
    public Text moneyUI;

    public QuestPointerScript pointer;

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        objectiveTimePassed += Time.deltaTime;

        timePassedUI.text = objectiveTimePassed.ToString("n2");

        moneyUI.text = money + " $";
    }

    void OnTriggerEnter(Collider other)
    {
        if (!hasObjective)
        {
            if (other.gameObject.CompareTag("Player"))
            {
                money += (int)(baseMoney + distanceToPoint * distanceMultiplierMoney);

                GameObject o = Instantiate(topSecretPrefab);
                o.transform.position = placesToPlaceObjectives[Random.Range(0, placesToPlaceObjectives.Length)].position;

                distanceToPoint = Vector3.Distance(transform.position, o.transform.position);


                hasObjective = true;
                objectiveTimePassed = 0;

                pointer.target = o;
            }
        }
    }
}
