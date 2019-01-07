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

        bool success = ProvidePart(requiredPart.name); // TODO: Use part which the player is holding
        Debug.Log(success);
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
        int index = Random.Range(0, GameManager.instance.generatorParts.Length - 1);
        requiredPart = (GameManager.instance.generatorParts[index]);
    }
}
