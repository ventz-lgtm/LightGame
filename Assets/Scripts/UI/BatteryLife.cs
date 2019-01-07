using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BatteryLife : MonoBehaviour {

    private Text batteryTextItem;
    private Torchbehaviour torch;
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
        
        torch = GameObject.Find("torch").GetComponent<Torchbehaviour>();
        if (!torch)
        {
            Debug.Log("No torch instance found");
        }

        maxBatteries = torch.GetMaxBatteries();

    }

    // Use this for initialization
    void Start () {
        
	}
	
	// Update is called once per frame
	void Update () {

        if (torch.GetBatteryLife() > 0)
        {
            batteryTextItem.text = string.Format("Battery: {0}%", torch.GetBatteryLife());
        }
        else
        {
            batteryTextItem.text = ("Battery: 0%");
        }
        batteryCount.fillAmount = (float) (1.0f / maxBatteries * torch.GetBatteryCount());
	}
}
