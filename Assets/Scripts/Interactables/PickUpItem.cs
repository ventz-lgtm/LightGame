using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickUpItem : BaseInteractable {

    public enum items {TORCH, WOODPILE, AXE, FLARE, GENERATOR_PIECE, FUEL};

    public items itemDefinition;

    public Inventory playerHeld;

    protected override void Start()
    {
        base.Start();

        playerHeld = GameObject.FindGameObjectWithTag("Player").GetComponent<Inventory>();
        if (!playerHeld)
        {
            Debug.Log("Could not get inventory");
        }

    }

    public void pickUpRequest()
    {
        playerHeld.GetItem(gameObject);
       
    }


    protected override void Update()
    {
        base.Update();
    }

}
