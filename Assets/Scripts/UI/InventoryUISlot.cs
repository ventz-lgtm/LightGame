using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InventoryUISlot : MonoBehaviour {

    public int slotIndex;
    public InventoryItemType itemType;
    public bool hasItem;

    private GameObject closeButtonObject;
    private Image image;
    private GameObject imageObject;

    private void Start()
    {
        closeButtonObject = transform.Find("CloseButton").gameObject;
        imageObject = transform.Find("SlotButton").Find("Image").gameObject;
        image = imageObject.GetComponent<Image>();

        Button closeButton = closeButtonObject.GetComponent<Button>();
        closeButton.onClick.AddListener(DropItem);
    }

    public void UpdateSlot()
    {
        if(!hasItem)
        {
            imageObject.SetActive(false);
            closeButtonObject.SetActive(false);
        }
        else
        {
            imageObject.SetActive(true);
            closeButtonObject.SetActive(true);
            image.sprite = itemType.icon;
        }
    }

    public void DropItem()
    {
        if(!hasItem) { return; }

        Character player = GameManager.instance.playerCharacter;
        player.InventoryDrop(slotIndex);
    }
}
