using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Torchbehaviour : MonoBehaviour {


    //LightIntensity value between 0 and 8.
    public float maxLightIntensity;
    public float currentLightIntenisity;
    public float batteryDrain;
    public int maxBatteries;
    public int batteryCount;
    private int batteryPercent;
    Light light;

    Inventory playerInventory;

    public bool torchOn;
    public bool hasBattery;

	// Use this for initialization
	void Start () {
        light = GetComponent<Light>();
        playerInventory = GameManager.instance.playerObject.GetComponent<Inventory>();
        currentLightIntenisity = maxLightIntensity;
	}

    

    /*public int GetBatteryCount()
    {
        return batteryCount;
    }*/

    public int GetMaxBatteries()
    {
        return maxBatteries;
    }

    public int GetBatteryLife()
    {
        batteryPercent = (int)((100.0f / maxLightIntensity) * currentLightIntenisity);
        return batteryPercent;
    }

    public void replaceBatteries()
    {
        hasBattery = true;
        currentLightIntenisity = maxLightIntensity;
    }

    public void TurnOnLight()
    {
        if (hasBattery)
        {
            torchOn = !torchOn;
        }
    }


	// Update is called once per frame
	void Update () {

        

        if (torchOn)
        {
            currentLightIntenisity -= batteryDrain;
            light.intensity = currentLightIntenisity;
            if(light.intensity <= 0.0)
            {
                hasBattery = false;
            }
        }
        else
        {
            light.intensity = 0.0f;
        }
	}
}
