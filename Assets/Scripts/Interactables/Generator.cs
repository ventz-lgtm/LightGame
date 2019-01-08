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

    public bool partProvided { get; private set; }

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

        //Checks if the player is currently holding an item.
        if (playerInventory.GetHolding())
        {
            //Gets a reference to the item being held by the player.
            PickUpItem currentItem = player.transform.GetChild(0).GetChild(0).GetComponent<PickUpItem>();
            //getItemDefinition returns the int for the enum of the pickUpItem held by the player.
            if (currentItem.getItemDefinition() == 0)
            {
                bool success = ProvidePart(requiredPart.name); // TODO: Use part which the player is holding
            }
        }
    }

    public bool ProvidePart(string partName)
    {
        if (partProvided) { return false; }
        if(partName != requiredPart.name) { return false; }

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
