using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Holding : MonoBehaviour {

    public int currentlyHeldObject;
    public GameObject[] itemList;
    public Transform holdPosition;
    private GameObject heldObject;
    private Rigidbody heldObjectRB;

    public int batteryCount;
    public int maxBatteries;
    public float currentBatteryLife;

	// Use this for initialization
	void Start () {
        // -1 is no object in hands
        currentlyHeldObject = -1;
	}

    void putDown()
    {
        if (currentlyHeldObject != -1)
        {
            Instantiate(itemList[currentlyHeldObject], gameObject.transform.position + new Vector3(0, 0, 1), Quaternion.identity);
            Destroy(heldObject);
            currentlyHeldObject = -1;
            
        }      
    }
    
    public int getHolding()
    {
        return currentlyHeldObject;
    }

    public void getItem(int item)
    {
        if (item != -1)
        {
            heldObject = Instantiate(itemList[item], holdPosition.position, holdPosition.rotation);
            heldObjectRB = heldObject.GetComponent<Rigidbody>();
            heldObjectRB.isKinematic = true;
            heldObject.transform.SetParent(holdPosition);
            currentlyHeldObject = item;
        }
        else
        {
            Destroy(heldObject);
            currentlyHeldObject = item;
        }
    }

    void replacebattery()
    {
       
       if (batteryCount > 0 && currentlyHeldObject == 0)
       {
                
            heldObject.GetComponentInChildren<Torchbehaviour>().replaceBatteries();
            batteryCount--;
       }
            
        
    }

    public int GetMaxBatteries()
    {
        return maxBatteries;
    }

    public float GetCurrentBatteryLife()
    {
        return currentBatteryLife;
    }

    public int GetCurrentBatteries()
    {
        return batteryCount;
    }

    // Update is called once per frame
    void Update () {
        if (Input.GetKeyDown(KeyCode.R))
        {
            replacebattery();
        }

        if (Input.GetKeyDown(KeyCode.P))
        {
            putDown();
        }
    }
}
