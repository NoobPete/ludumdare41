using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class QuestGiverScript : MonoBehaviour
{
    public float objectiveTimePassed;
    public GameObject topSecretPrefab;
    public Transform[] placesToPlaceObjectives;
    public float distanceToPoint;
    public int baseMoney;
    public float distanceMultiplierMoney;

    public static bool hasObjective = false;
    public static int money = 500;

    public Text timePassedUI;
    public Text moneyUI;

    public QuestPointerScript pointer;

    public bool ignoreNext = true;

    public float showTime;
    public float currentTime = -1;

    public GameObject panel;

    public Text[] resultText;

    private float[] records;

    // Use this for initialization
    void Start()
    {
        records = new float[placesToPlaceObjectives.Length + 1];

        for (int i = 0; i < records.Length; i++)
        {
            records[i] = Mathf.Infinity;
        }
    }

    // Update is called once per frame
    void Update()
    {
        objectiveTimePassed += Time.deltaTime;

        timePassedUI.text = objectiveTimePassed.ToString("n2");

        moneyUI.text = money + " $";

        currentTime -= Time.deltaTime;

        if (currentTime >= 0)
        {
            panel.SetActive(true);
        }
        else
        {
            panel.SetActive(false);
        }

    }

    private GameObject o;
    private int trackID = 0;

    void OnTriggerEnter(Collider other)
    {
        if (!hasObjective)
        {
            if (other.gameObject.CompareTag("Player"))
            {
                if (!ignoreNext)
                {
                    int distanceMoney = (int)(distanceToPoint * distanceMultiplierMoney);

                    int speedBonus = (int)Mathf.Lerp(100, 1000, distanceToPoint / objectiveTimePassed / 15);
                    int total = (baseMoney + distanceMoney + speedBonus);

                    money += total;

                    currentTime = showTime;

                    bool newRecord = false;

                    if (objectiveTimePassed < records[trackID])
                    {
                        newRecord = true;
                        records[trackID] = objectiveTimePassed;
                    }

                    resultText[1].text = "Track ID: " + trackID;
                    resultText[2].text = "Your time: " + objectiveTimePassed.ToString("n2");
                    resultText[3].text = "Best time: " + records[trackID].ToString("n2");

                    resultText[5].text = "Base: " + baseMoney;
                    resultText[6].text = "Distance: " + distanceMoney;
                    resultText[7].text = "Speed: " + speedBonus;
                    resultText[8].text = "Total: " + total;


                }

                o = Instantiate(topSecretPrefab);

                trackID = Random.Range(0, placesToPlaceObjectives.Length);
                o.transform.position = placesToPlaceObjectives[trackID].position;

                distanceToPoint = Vector3.Distance(transform.position, o.transform.position);


                hasObjective = true;
                objectiveTimePassed = 0;

                pointer.target = o;

                ignoreNext = false;
            }
        }
    }

    public void hasRespawned()
    {
        hasObjective = false;
        ignoreNext = true;

        Destroy(o);
    }
}
