using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Holding : MonoBehaviour {

    public int inventory;
    GameObject targetObject;
    PickUpItem target;

    public GameObject[] itemList;
    public Transform holdPosition;

    public GameObject heldObject;

	// Use this for initialization
	void Start () {
        inventory = -1;

        targetObject = GameObject.Find("Generator");
        target = targetObject.GetComponent<PickUpItem>();
        //dropPosition = transform.GetChild(0);
	}
	

    void pickUp()
    {
        if(inventory == -1)
        {
            inventory = target.pickUpRequest();

            //If Picking up the torch
            if(inventory == 0)
            {
                heldObject = Instantiate(itemList[0], holdPosition);
            }

        }
        
    }

    void putDown()
    {
        if (inventory != -1)
        {
            Instantiate(itemList[0], gameObject.transform.position + new Vector3(0, 0, 1), Quaternion.identity);
            Destroy(heldObject);
            inventory = -1;
            
        }
            
    }
    


	// Update is called once per frame
	void Update () {
        if (Input.GetKeyDown(KeyCode.O))
        {
            pickUp();
        }

        if(Input.GetKeyDown(KeyCode.P))
        {
            putDown();
        }
    }
}
