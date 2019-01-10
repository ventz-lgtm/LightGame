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

    private bool holdingTorch;
    private Character character;

    public int batteryPercent;

    public void GetItem(GameObject pickUpObject)
    {
        if (!holding)
        {
            GameObject handObject = character.handObject;

            pickUpObject.transform.SetParent(holdPosition);
            pickUpObject.transform.position = handObject.transform.position;
            pickUpObject.transform.rotation = holdPosition.transform.rotation;

            Collider collider = pickUpObject.GetComponent<Collider>();
            if (collider)
            {
                collider.enabled = false;
            }

            rb_heldObject = pickUpObject.GetComponent<Rigidbody>();
            rb_heldObject.isKinematic = true;

            holding = true;

            if(pickUpObject.GetComponent<PickUpItem>().getItemDefinition() == 0)
            {
                holdingTorch = true;
            }
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

            if (holdPosition.GetChild(0).GetComponent<PickUpItem>().getItemDefinition() == 0)
            {
                holdingTorch = false;
            }

            rb_heldObject = null;
            holdPosition.GetChild(0).SetParent(null);
            holding = false;

           
        }
    }
    
    public bool GetHoldingTorch()
    {
        return holdingTorch;
    }

    // Use this for initialization
    void Start () {
        character = GameManager.instance.playerObject.GetComponent<Character>();
	}
	
	// Update is called once per frame
	void Update () {

        if (holding)
        {
            PickUpItem itemInHand = holdPosition.GetChild(0).GetComponent<PickUpItem>();

            GameObject handObject = character.handObject;
            rb_heldObject.gameObject.transform.position = handObject.transform.position - (handObject.transform.forward * 0.1f);
            if (itemInHand.ignoreHandRotation)
            {

            }
            else
            {
                rb_heldObject.gameObject.transform.rotation = handObject.transform.rotation;
            }

            //If player is holding a torch
            if (itemInHand && itemInHand.getItemDefinition() == 0)
            {

                
                torch = holdPosition.GetChild(0).Find("Bulb").GetComponent<Torchbehaviour>();
                SetCurrentBatteryLife(torch);
                if (Input.GetKeyDown(KeyCode.F))
                {
                    torch.TurnOnLight();
                }

                if (Input.GetKeyDown(KeyCode.R))
                {
                    torch.replaceBatteries();
                }
            }
        }

        //Any Object to drop
        if (Input.GetMouseButton(1))

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
