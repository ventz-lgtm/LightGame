using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TeddyInventoryitem : BaseInventoryItem {

    private void Start()
    {
        onDrop.AddListener(Heal);
    }

    void Heal()
    {
        GameManager.instance.HealSanity(1f);
    }
}
