using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UpgradeScript : MonoBehaviour
{
    public CarController car;
    public float distance;
    public QuestGiverScript questGiver;

    public GameObject panel;

    public Text[] buttonTexts;

    private int[] levels = { 0, 0, 0, 0 };
    private int[] maxLevels = { 7, 3, 2, 4 };
    private int[] basePrice = { 1000, 2000, 10000, 5000 };
    private int[] priceAdder = { 500, 500, 10000, 2000 };
    private float[] upgradeValue = { 10, 0.05f, 1, 0.05f };

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (Vector3.Distance(car.transform.position, transform.position) < distance)
        {
            panel.SetActive(true);

            for (int i = 0; i < buttonTexts.Length; i++)
            {
                if (levels[i] == maxLevels[i])
                {
                    buttonTexts[i].text = "Fully upgraded (level " + levels[i] + ")";
                }
                else
                {
                    buttonTexts[i].text = "Upgrade to level " + (levels[i] + 1) + " for " + (basePrice[i] + levels[i] * priceAdder[i]) + " $";
                }

            }
        }
        else
        {
            panel.SetActive(false);
        }
    }

    public void Upgrade(int id)
    {
        if (levels[id] < maxLevels[id])
        {
            if (QuestGiverScript.money >= (basePrice[id] + levels[id] * priceAdder[id]))
            {
                QuestGiverScript.money -= (basePrice[id] + levels[id] * priceAdder[id]);
                levels[id]++;

                switch (id)
                {
                    case 0:
                        car.maxBoost += upgradeValue[id];
                        break;
                    case 1:
                        car.boostRegen += upgradeValue[id];
                        break;
                    case 2:
                        car.maxJumps += upgradeValue[id];
                        break;
                    case 3:
                        car.jumpRegen += upgradeValue[id];
                        break;
                }
            }
        }
    }
}
