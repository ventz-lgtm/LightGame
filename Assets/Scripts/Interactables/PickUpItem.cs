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

    AudioSource pickupSoundSource;
    AudioClip pickupSound;

    protected override void Start()
    {
        base.Start();

        gameObject.AddComponent<AudioSource>();
        pickupSoundSource = GetComponent<AudioSource>();
        pickupSound = (AudioClip)Resources.Load("Audio/snatch");

        pickupSoundSource.clip = pickupSound;
        pickupSoundSource.volume = 1.0f;

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

        if (Input.GetButtonDown("Use"))
        {
            Transform item = inventory.GetHeldItem();
            if(item != null && item.gameObject == gameObject)
            {
                OnUse();
            }
        }
    }

    private void OnGUI()
    {
        Transform item = inventory.GetHeldItem();
        if (item == null || item.gameObject != gameObject) { return; }

        string text = GetHoldText();
        if(text == "") { return; }

        int w = Screen.width, h = Screen.height;

        GUIStyle style = new GUIStyle();

        Rect rect = new Rect(0, 0, w, 50);
        style.alignment = TextAnchor.MiddleCenter;
        style.fontSize = h * 2 / 100;
        style.normal.textColor = new Color(1f, 1f, 1f, 1.0f);
        GUI.Label(rect, text, style);
    }

    protected override string GetHoverText(GameObject invokerObject)
    {
        return "Hold " + interactableName;
    }

    protected override void OnInteractableStart(GameObject invokerObject)
    {
        base.OnInteractableStart(invokerObject);

        pickUpRequest();
        pickupSoundSource.Play();
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
        if (playerHeld.GetHolding())
        {
            playerHeld.DropItem();
        }

        playerHeld.GetItem(gameObject);
    }

    public virtual bool ShouldAim()
    {
        return true;
    }

    public virtual void OnUse()
    {

    }

    public virtual string GetHoldText()
    {
        return "";
    }
}
