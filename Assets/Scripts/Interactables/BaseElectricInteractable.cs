using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseElectricInteractable : BaseInteractable {

    public GameManager.LocationType locationType = GameManager.LocationType.NONE;
    public Material poweredMaterial;
    public Material unpoweredMaterial;

    private GameObject poweredLightObject;
    private MeshRenderer poweredLightRenderer;

    protected override void Start()
    {
        base.Start();

        Transform poweredLighTransform = transform.Find("PoweredLight");

        if (poweredLighTransform)
        {
            poweredLightObject = poweredLighTransform.gameObject;
            poweredLightRenderer = poweredLightObject.GetComponent<MeshRenderer>();
        }
    }
    protected override void OnInteractableStart(GameObject invokerObject)
    {
        if (!IsPowered()) { return; }

        base.OnInteractableStart(invokerObject);
    }

    protected override void Update()
    {
        base.Update();

        //Debug.Log(this);
       // Debug.Log(poweredLightRenderer);

        if (IsPowered())
        {
            if (poweredLightRenderer)
            {
                poweredLightRenderer.sharedMaterial = poweredMaterial;
            }
        }
        else
        {
            if (poweredLightRenderer)
            {
                poweredLightRenderer.sharedMaterial = unpoweredMaterial;
            }
        }
    }

    public virtual bool IsPowered()
    {
        if(locationType == GameManager.LocationType.NONE) { return true; }

        return GameManager.instance.IsLocationActive(locationType);
    }
}
