using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Inventory : MonoBehaviour {

    public Transform holdPosition;
    public GameObject heldObject;
    private Rigidbody rb_heldObject;
    private bool holding;

    public int batteryCount;
    public int maxBatteries;

    public void GetItem(GameObject pickUpObject)
    {
        if (!holding)
        {
            pickUpObject.transform.SetParent(holdPosition);
        }
    }
    

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
