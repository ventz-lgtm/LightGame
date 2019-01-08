using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BatteryLife : MonoBehaviour {

    private Text batteryTextItem;
    private Torchbehaviour torch;
    private Inventory playerInventory;
    Image batteryCount;
    Image healthbar;
    private int maxBatteries;

    private void Awake()
    {

        batteryTextItem = transform.GetChild(0).GetComponent<Text>();
        if(batteryTextItem.name != "BatteryMeter")
        {
            Debug.Log("Not found battery meter UI component.");
        }

        batteryCount = transform.GetChild(1).GetComponent<Image>();

        if(batteryCount.name != "BatteryDisplay")
        {
            Debug.Log("Not found battery display UI component");
        }

        healthbar = transform.GetChild(2).GetComponent<Image>();
        if(healthbar.name != "healthbar")
        {
            Debug.Log("Not found healthbar display UI component");
        }
        
        playerInventory = GameObject.FindGameObjectWithTag("Player").GetComponent<Inventory>();
        if (!torch)
        {
            Debug.Log("No player Inventory instance found");
        }

        

    }

    // Use this for initialization
    void Start () {
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
	}
}
