using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TeddyInventoryitem : BaseInventoryItem {

    protected override void Start()
    {
        base.Start();
        onDrop.AddListener(Heal);
        
    }

    void Heal()
    {
        Debug.Log("PlayStart");
        GameManager.instance.HealSanity(1f);
    }

    protected override void OnInteractableStart(GameObject invokerObject)
    {
        base.OnInteractableStart(invokerObject);
    }
}
