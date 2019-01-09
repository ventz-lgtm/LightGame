using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryUI : MonoBehaviour {

    public bool inventoryOpen = false;

    private GameObject slotsObject;
    private Character player;
    private GameObject backgroundObject;
    private List<InventoryUISlot> slots;

    // Use this for initialization
    void Start () {
        player = GameManager.instance.playerCharacter;
        player.onInventoryChanged += UpdateUI;

        backgroundObject = transform.Find("Background").gameObject;

        slotsObject = backgroundObject.transform.Find("Slots").gameObject;
        slots = new List<InventoryUISlot>();
        for(int i = 0; i < slotsObject.transform.childCount; i++)
        {
            GameObject child = slotsObject.transform.GetChild(i).gameObject;
            InventoryUISlot slot = child.GetComponent<InventoryUISlot>();
            slot.slotIndex = slotsObject.transform.childCount - i - 1;
            slots.Insert(0, slot);
        }
    }
	
	// Update is called once per frame
	void Update () {
        if (Input.GetButtonDown("Inventory"))
        {
            inventoryOpen = !inventoryOpen;
        }

        if (inventoryOpen)
        {
            if (!backgroundObject.activeSelf)
            {
                backgroundObject.SetActive(true);
            }
        }
        else
        {
            if (backgroundObject.activeSelf)
            {
                backgroundObject.SetActive(false);
            }
        }
	}

    void UpdateUI()
    {
        int index = 0;
        foreach(InventoryUISlot slot in slots)
        {
            InventoryItemType itemType = player.InventoryItemAt(index);
            if(itemType == null)
            {
                slot.hasItem = false;
            }
            else
            {
                slot.itemType = itemType;
                slot.hasItem = true;
            }

            slot.UpdateSlot();
            index++;
        }
    }
}
