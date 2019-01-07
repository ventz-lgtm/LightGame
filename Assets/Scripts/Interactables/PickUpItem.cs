using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickUpItem : BaseInteractable {

    public enum items {TORCH, GENERATOR_PIECE, FUEL};

    public items itemDefinition;

    public int itemType;

    public Holding inventory;

    protected override void Start()
    {
        base.Start();
    }

    public int pickUpRequest()
    {
        Destroy(gameObject);
        return (int)itemDefinition;
    }

    protected override void Update()
    {
        base.Update();
    }

}
