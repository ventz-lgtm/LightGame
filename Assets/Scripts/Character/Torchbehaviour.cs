using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Torchbehaviour : MonoBehaviour {


    //LightIntensity value between 0 and 8.
    public float lightIntenisity;
    public float batteryDrain;
    public int batteryCount;
    public int batteryPercent;
    Light light;


	// Use this for initialization
	void Start () {
        light = GetComponent<Light>();
        lightIntenisity = 8;
        batteryCount = 3;
	}

    void replacebattery()
    {

        if (batteryCount > 0) {

            if (Input.GetKeyDown(KeyCode.T))
            {
                lightIntenisity = 8.0f;
                batteryCount--;
            }
        }
    }

    public int GetBatteryCount()
    {
        return batteryCount;
    }

    public int GetIntensity()
    {
        batteryPercent = (int)((100.0f / 8.0f) * lightIntenisity);

        return batteryPercent;
    }



	// Update is called once per frame
	void Update () {

        replacebattery();

        lightIntenisity -= batteryDrain;

        light.intensity = lightIntenisity;
		
	}
}
