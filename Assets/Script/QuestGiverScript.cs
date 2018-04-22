using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class QuestGiverScript : MonoBehaviour {
    public float objectiveTimePassed;
    public GameObject topSecretPrefab;
    public Transform[] placesToPlaceObjectives;

    public static bool hasObjective = false;
    public static int money = 1000;

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
                money += 150;

                GameObject o = Instantiate(topSecretPrefab);
                o.transform.position = placesToPlaceObjectives[Random.Range(0, placesToPlaceObjectives.Length)].position;

                hasObjective = true;
                objectiveTimePassed = 0;

                pointer.target = o;
            }
        }
    }
}
