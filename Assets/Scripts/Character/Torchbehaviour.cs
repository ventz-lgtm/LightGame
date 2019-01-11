using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Torchbehaviour : MonoBehaviour {


    public float torchLightDistance = 1;
    //LightIntensity value between 0 and 8.
    public float maxLightIntensity;
    public float currentLightIntenisity;
    public float batteryDrain;
    public int maxBatteries;
    public int batteryCount;
    private int batteryPercent;
    Light directionalLight;
    Light glowLight;
    Vector3 glowPosition;
    GameObject glowObject;

    Inventory playerInventory;

    public bool torchOn;
    public bool hasBattery;

    AudioSource flashLightOnOff;
    PickUpItem torchItem;

    Character playerCharacter;

	// Use this for initialization
	void Start () {
        directionalLight = GetComponent<Light>();
        Transform glowTransform = transform.Find("GlowArea");
        if(glowTransform != null)
        {
            glowLight = glowTransform.GetComponent<Light>();

        }

        if(glowTransform != null)
        {
            glowObject = glowTransform.gameObject;
        }

        playerInventory = GameManager.instance.playerObject.GetComponent<Inventory>();
        currentLightIntenisity = maxLightIntensity;

        flashLightOnOff = GetComponent<AudioSource>();

        playerCharacter = GameManager.instance.playerObject.GetComponent<Character>();

        torchItem = transform.parent.GetComponent<PickUpItem>();

    }

    
    
    

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
        if (playerCharacter.GetInventoryItemCount("Battery") > 0)
        {
            playerCharacter.InventoryDrop("Battery", true);
            currentLightIntenisity = maxLightIntensity;
        }
    }

    public void TurnOnLight()
    {
        if (hasBattery)
        {
            torchOn = !torchOn;
            flashLightOnOff.Play();
        }
    }


	// Update is called once per frame
	void Update () {

        if (torchOn)
        {
            currentLightIntenisity -= (batteryDrain / maxLightIntensity) * Time.deltaTime * 10;
            directionalLight.intensity = currentLightIntenisity;
            glowLight.intensity = currentLightIntenisity *0.25f;
            if(directionalLight.intensity <= 0.0)
            {
                hasBattery = false;
            }

            /*RaycastHit hit;
            Ray downRay = new Ray(gameObject.transform.position , Vector3.down);

            if(Physics.Raycast(downRay, out hit))
            {
                Debug.Log(hit.distance);
                Vector3 localForward = transform.rotation * Vector3.up;
                //gameObject.transform.position = gameObject.transform.forward + new Vector3(0, 0, hit.distance * gameObject.transform.forward.magnitude);
                glowObject.transform.position = gameObject.transform.position - transform.InverseTransformVector(new Vector3(hit.distance, 0, hit.distance));
                //glowObject.transform.position = Transform.Translate(gameObject.transform.position,new Vector3(0, 0, hit.distance)); gameObject.transform.position + new Vector3(0, 0, hit.distance);
            }*/

            Transform heldItem = playerInventory.GetHeldItem();
            if (heldItem == torchItem.gameObject.transform)
            {
                glowObject.transform.position = transform.position + (transform.forward * torchLightDistance);
            }
            else
            {
                glowObject.transform.position = transform.position;
            }
            
        }
        else
        {
            directionalLight.intensity = 0.0f;
            if(glowLight != null)
            {
                glowLight.intensity = 0.0f;
            }
            
        }
	}
}
