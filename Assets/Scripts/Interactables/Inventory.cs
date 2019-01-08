using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Inventory : MonoBehaviour {

    public Transform holdPosition;
    public GameObject heldObject;
    private Rigidbody rb_heldObject;
    public bool holding;

    public int batteryCount;
    public int maxBatteries;

    public void GetItem(GameObject pickUpObject)
    {
        if (!holding)
        {
            pickUpObject.transform.SetParent(holdPosition);
            pickUpObject.transform.position = holdPosition.transform.position;
            pickUpObject.transform.rotation = holdPosition.transform.rotation;
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
        if (Input.GetKeyDown(KeyCode.P))
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
}
