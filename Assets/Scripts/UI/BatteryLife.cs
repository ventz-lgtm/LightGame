using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BatteryLife : MonoBehaviour {

    private Inventory playerInventory;
    Text batteryCount;
    Image sanityMeter;
    Image sanityPulse;

    Image batteryChargeMeter;
    Image batteryCasing;

    GameManager gameManager;

    private int maxBatteries;
    private float pulse = 0;
    private float lastPulse = 0;
    
    Color tempColor;

    Character playerCharacter;

    private void Start()
    {

        batteryCount = transform.Find("BatteryDisplay").GetComponent<Text>();

        if(batteryCount.name != "BatteryDisplay")
        {
            Debug.Log("Not found battery display UI component");
        }

        sanityMeter = transform.Find("SanityMeter").GetComponent<Image>();
        if(sanityMeter.name != "SanityMeter")
        {
            Debug.Log("Not found SanityMeter display UI component");
        }
        
        playerInventory = GameManager.instance.playerObject.GetComponent<Inventory>();
        if (!playerInventory)
        {
            Debug.Log("No player Inventory instance found");
        }

        batteryChargeMeter = gameObject.transform.Find("EnergyBar").GetComponent<Image>();

        batteryCasing = batteryChargeMeter.transform.Find("BatteryCasing").GetComponent<Image>();

        sanityPulse = transform.Find("SanityPulse").GetComponent<Image>();

        gameManager = GameManager.instance;

        playerCharacter = gameManager.playerObject.GetComponent<Character>();

        maxBatteries = 12;
    }
	
	// Update is called once per frame
	void Update () {

        if (playerInventory.GetHoldingTorch())
        {
            batteryCasing.enabled = true;
            batteryChargeMeter.enabled = true;
            batteryCount.enabled = true;

            if (playerInventory.GetCurrentBatteryLife() > 0)
            {
                batteryChargeMeter.fillAmount = (playerInventory.GetCurrentBatteryLife() / 100f);
            }
            else
            {
                batteryChargeMeter.fillAmount = 0.0f;
            }

            

        }
        else
        {
            batteryCasing.enabled = false;
            batteryChargeMeter.enabled = false;
            batteryCount.enabled = false;
        }

        batteryCount.text = string.Format("Batteries: {0}", playerCharacter.GetInventoryItemCount("Battery"));


        tempColor = sanityMeter.color;
        tempColor.a = gameManager.sanity * 0.25f;
        sanityMeter.color = tempColor;
        tempColor = sanityPulse.color;
        tempColor.a = pulse * gameManager.sanity * 0.3f;
        sanityPulse.color = tempColor;

        float pulseTime = 1.35f - gameManager.sanity;
        if(Time.time - lastPulse > pulseTime)
        {
            lastPulse = Time.time;

            pulse = gameManager.sanity;
        }
        else
        {
            pulse = Mathf.Max(0, pulse - (Time.deltaTime * 2f));
        }
    }
}
