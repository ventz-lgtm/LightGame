using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickUpItem : MonoBehaviour {

    public enum items {TORCH, GENERATOR_PIECE, FUEL};

    public items itemDefinition;

    public int itemType;

    public Holding inventory;

	// Use this for initialization
	void Start () {
        
	}

    public int pickUpRequest()
    {
        Destroy(gameObject);
        return (int)itemDefinition;
    }


	// Update is called once per frame
	void Update () {
        
	}
}
