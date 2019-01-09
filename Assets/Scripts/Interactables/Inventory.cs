using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Inventory : MonoBehaviour {

    public Transform holdPosition;
    public GameObject heldObject;
    private Rigidbody rb_heldObject;
    public bool holding;

    Torchbehaviour torch;

    public int batteryCount;
    public int maxBatteries;

    public int batteryPercent;

    public void GetItem(GameObject pickUpObject)
    {
        if (!holding)
        {
            pickUpObject.transform.SetParent(holdPosition);
            pickUpObject.transform.position = holdPosition.transform.position;
            pickUpObject.transform.rotation = holdPosition.transform.rotation;

            Collider collider = pickUpObject.GetComponent<Collider>();
            if (collider)
            {
                collider.enabled = false;
            }

            rb_heldObject = pickUpObject.GetComponent<Rigidbody>();
            rb_heldObject.isKinematic = true;

            holding = true;
        }
    }

    public void DropItem()
    {
        if (holding)
        {
            rb_heldObject.isKinematic = false;

            Collider collider = rb_heldObject.GetComponent<Collider>();
            if (collider)
            {
                collider.enabled = true;
            }

            rb_heldObject = null;
            holdPosition.GetChild(0).SetParent(null);
            holding = false;
        }
    }
    

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {

        if (holding)
        {
            PickUpItem itemInHand = holdPosition.GetChild(0).GetComponent<PickUpItem>();

            //If player is holding a torch
            if (itemInHand && itemInHand.getItemDefinition() == 0)
            {

                
                torch = holdPosition.GetChild(0).Find("Bulb").GetComponent<Torchbehaviour>();
                SetCurrentBatteryLife(torch);
                if (Input.GetKeyDown(KeyCode.T) && holding)
                {
                    torch.TurnOnLight();
                }

                if (Input.GetKeyDown(KeyCode.R) && batteryCount > 0 && holding)
                {
                    torch.replaceBatteries();
                    batteryCount--;
                }
            }
        }

        //Any Object to drop
        if (Input.GetMouseButtonDown(1))
        {
            DropItem();
        }
    }

    public Transform GetHeldItem()
    {
        if (holding)
        {
            return holdPosition.GetChild(0);   
        }

        return null;

    }

    public void DestroyHeldItem()
    {
        if (holding)
        {
            Destroy(holdPosition.GetChild(0).gameObject);
            holding = false;
        }
    }

    public bool GetHolding()
    {
        return holding;
    }

    public int GetMaxBatteries()
    {
        return maxBatteries;
    }

    public void SetCurrentBatteryLife(Torchbehaviour torch)
    {
        batteryPercent = torch.GetBatteryLife();
    }

    public int GetCurrentBatteryLife()
    {
        return batteryPercent;
    }

    public int GetBatteryCount()
    {
        return batteryCount;
    }
}
