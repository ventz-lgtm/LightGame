using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class BaseInventoryItem : BaseInteractable {

    [System.Serializable]
    public class InventoryItemCallback : UnityEvent { }

    public InventoryItemCallback onDrop;
    public bool deleteOnDrop = false;

    public InventoryItemType itemType;

    protected override void Start()
    {
        startActive = false;
        base.Start();

        if (!textMeshObject)
        {
            textMeshObject = new GameObject("TextMesh");
            textMeshObject.transform.SetParent(transform);
            textMesh = textMeshObject.AddComponent<TextMesh>();
            textMesh.characterSize = 0.05f;
            textMesh.fontSize = 45;
            textMesh.alignment = TextAlignment.Center;
            textMesh.anchor = TextAnchor.MiddleCenter;
        }
    }

    protected override void OnInteractableStart(GameObject invokerObject)
    {
        base.OnInteractableStart(invokerObject);

        Character player = GameManager.instance.playerCharacter;

        itemType.instance = gameObject;
        if (player.InventoryPickup(itemType))
        {
            GameObject parent = GameObject.Find("InventoryItems");
            if(parent == null)
            {
                parent = new GameObject("InventoryItems");
            }

            gameObject.transform.parent = parent.transform;
            gameObject.SetActive(false);
        }
        else
        {
            GameManager.instance.Notify("Inventory full!", 2);
        }
    }

    protected override string GetHoverText(GameObject invokerObject)
    {
        if(itemType == null) { return "Pickup NULL"; }
        return "Pickup " + itemType.name;
    }
}
