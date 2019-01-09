using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BatteryLife : MonoBehaviour {

    private Text batteryTextItem;
    private Torchbehaviour torch;
    private Inventory playerInventory;
    Image batteryCount;
    Image sanityMeter;

    GameManager gameManager;

    private int maxBatteries;

    Color tempColor;

    private void Start()
    {

        batteryTextItem = transform.GetChild(0).GetComponent<Text>();
        if(batteryTextItem.name != "BatteryMeter")
        {
            Debug.Log("Not found battery meter UI component.");
        }

        batteryCount = transform.Find("BatteryDisplay").GetComponent<Image>();

        if(batteryCount.name != "BatteryDisplay")
        {
            Debug.Log("Not found battery display UI component");
        }

        sanityMeter = transform.Find("SanityMeter").GetComponent<Image>();
        if(sanityMeter.name != "SanityMeter")
        {
            Debug.Log("Not found SanityMeter display UI component");
        }
        
        playerInventory = GameObject.FindGameObjectWithTag("Player").GetComponent<Inventory>();
        if (!playerInventory)
        {
            Debug.Log("No player Inventory instance found");
        }

        gameManager = GameManager.instance;

        maxBatteries = playerInventory.GetMaxBatteries();
    }
	
	// Update is called once per frame
	void Update () {

        if (playerInventory.GetHoldingTorch())
        {

            if (playerInventory.GetCurrentBatteryLife() > 0)
            {
                batteryTextItem.text = string.Format("Battery: {0}%", playerInventory.GetCurrentBatteryLife());
            }
            else
            {
                batteryTextItem.text = ("Battery: 0%");
            }
        }
        else
        {
            batteryTextItem.text = "Battery:";
        }
        batteryCount.fillAmount = (float) (1.0f / maxBatteries * playerInventory.GetBatteryCount());

        tempColor = sanityMeter.color;
        tempColor.a = gameManager.sanity;
        sanityMeter.color = tempColor;


	}
}
