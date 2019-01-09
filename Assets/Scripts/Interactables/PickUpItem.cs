using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickUpItem : BaseInteractable {

    public enum items {TORCH, FIRE_TORCH, WOODPILE, AXE, FLARE, CONTROL_PANEL, BATTERY_CHARGER, EXHAUST, RADIATOR, FUEL};

    public items itemDefinition;
    public bool aimable = false;

    public Inventory playerHeld;

    private Character player;
    private Inventory inventory;

    protected override void Start()
    {
        base.Start();

        playerHeld = GameManager.instance.playerObject.GetComponent<Inventory>();
        if (!playerHeld)
        {
            Debug.Log("Could not get inventory");
        }

        player = GameManager.instance.playerCharacter;
        inventory = player.GetComponent<Inventory>();
    }

    protected override void Update()
    {
        base.Update();

        if (Input.GetButtonDown("Use") && inventory.heldObject == gameObject)
        {
            OnUse();
        }
    }

    protected override void OnInteractableStart(GameObject invokerObject)
    {
        base.OnInteractableStart(invokerObject);

        pickUpRequest();
    }

    public int getItemDefinition()
    {
        return (int)itemDefinition;
    }

    public string getPartDescription()
    {
        if(itemDefinition == items.CONTROL_PANEL)
        {
            return "Control Panel";
        }

        if (itemDefinition == items.BATTERY_CHARGER)
        {
            return "Battery Charger";
        }

        if (itemDefinition == items.EXHAUST)
        {
            return "Exhaust";
        }

        if (itemDefinition == items.RADIATOR)
        {
            return "Radiator";
        }

        return null;
    }

    public void pickUpRequest()
    {
        if (!playerHeld.GetHolding())
        {
            playerHeld.GetItem(gameObject);
        }
       
    }

    public virtual bool ShouldAim()
    {
        return true;
    }

    public virtual void OnUse()
    {

    }
}
