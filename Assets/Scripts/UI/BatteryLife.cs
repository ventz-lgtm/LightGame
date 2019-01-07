using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BatteryLife : MonoBehaviour {

    private Text batteryTextItem;
    private Torchbehaviour torch;
    Image batteryCount;
    Image healthbar;

    private void Awake()
    {
        //Font arial;
        

        
        //Text textItem = GetComponent<Text>();
        
        

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

        batteryTextItem.text = "BatteryLife: 100%";
        torch = GameObject.Find("torch").GetComponent<Torchbehaviour>();
       

    }

    // Use this for initialization
    void Start () {
        
	}
	
	// Update is called once per frame
	void Update () {

        batteryTextItem.text = string.Format("Battery: {0}%", torch.GetIntensity());
        batteryCount.fillAmount = (float) (1.0f / 3.0f * torch.GetBatteryCount());
	}
}
