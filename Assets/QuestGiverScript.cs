using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuestGiverScript : MonoBehaviour {
    public GameObject topSecretPrefab;
    public Transform[] placesToPlaceObjectives;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            GameObject o = Instantiate(topSecretPrefab);
            o.transform.position = placesToPlaceObjectives[Random.Range(0, placesToPlaceObjectives.Length)].position;
        }
    }
}
