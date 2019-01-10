using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Generator : BaseElectricInteractable {

    [Header("Generator Settings")]
    public GeneratorPart requiredPart;
    public Color offColor = Color.red;
    public Color onColor = Color.green;
    public Material onLightMaterial;
    public Material offLightMaterial;

    public bool partProvided;

    private GameObject lightObject;
    private Light generatorLight;
    private MeshRenderer generatorLightMeshRenderer;
    private bool partPicked = false;

    private Inventory playerInventory;
    private GameObject player;

    protected override void Start()
    {
        base.Start();

        Transform lightTransform = transform.Find("Light");
        if (lightTransform)
        {
            lightObject = lightTransform.gameObject;
            generatorLight = lightObject.GetComponent<Light>();
            generatorLightMeshRenderer = lightObject.GetComponent<MeshRenderer>();
        }

        Transform lightMeshTransform = transform.Find("LightMesh");
        if (lightMeshTransform)
        {
            generatorLightMeshRenderer = lightMeshTransform.gameObject.GetComponent<MeshRenderer>();
        }

        playerInventory = GameManager.instance.playerObject.GetComponent<Inventory>();
        if (!playerInventory)
        {
            Debug.Log("Could not get player inventory instance.");
        }

        player = GameManager.instance.playerObject;
        if (!player)
        {
            Debug.Log("Could not get player instance.");
        }


    }

    protected override void Update()
    {
        base.Update();

        if (!partPicked)
        {
            partPicked = true;
            PickPartName();
        }

        if (IsPowered())
        {
            if (generatorLight)
                generatorLight.color = onColor;

            if(generatorLightMeshRenderer)
                generatorLightMeshRenderer.sharedMaterial = onLightMaterial;
        }
        else
        {
            if (generatorLight)
                generatorLight.color = offColor;

            if (generatorLightMeshRenderer)
                generatorLightMeshRenderer.sharedMaterial = offLightMaterial;
        }
    }

    protected override void OnInteractableStart(GameObject invokerObject)
    {
        base.OnInteractableStart(invokerObject);
        
        if (playerInventory.GetHolding())
        {
            
            PickUpItem currentItem = player.transform.Find("Visual").gameObject.transform.Find("HoldPosition").GetChild(0).GetComponent<PickUpItem>();
            
            bool success = ProvidePart(currentItem.itemDefinition);
            Debug.Log(success);
            if (success)
            {
                playerInventory.DestroyHeldItem();
            }
            else
            {
                GameManager.instance.Notify("Generator needs a " + requiredPart.name + "!", 4f);
            }
        }
        else
        {
            GameManager.instance.Notify("Generator needs a " + requiredPart.name + "!", 4f);
        }
    }

    public bool ProvidePart(PickUpItem.items itemType)
    {
        if (partProvided) { return false; }
        if(itemType != requiredPart.pickupType) { return false; }

        GameManager.instance.SetLocationActive(locationType, true);
        partProvided = true;

        return true;
    }

    public override bool IsPowered()
    {
        if (!base.IsPowered()) { return false; }

        return partProvided;
    }

    private void PickPartName()
    {
        int index = Random.Range(0, GameManager.instance.generatorParts.Length);
        requiredPart = (GameManager.instance.generatorParts[index]);

        meshFilter.sharedMesh = requiredPart.generatorMesh;
    }
}
