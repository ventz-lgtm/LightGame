using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickUpItem : BaseInteractable {

    public enum items {TORCH, GENERATOR_PIECE, FUEL};

    public items itemDefinition;

    public Holding playerHeld;

    protected override void Start()
    {
        base.Start();

        playerHeld = GameObject.Find("Character").GetComponent<Holding>();
        if (!playerHeld)
        {
            Debug.Log("Could not get inventory");
        }

    }

    public void pickUpRequest()
    {
        if (playerHeld.getHolding() == -1) { 

            playerHeld.getItem((int)itemDefinition);
            Destroy(gameObject);
        }
       
    }


    protected override void Update()
    {
        base.Update();
    }

}
