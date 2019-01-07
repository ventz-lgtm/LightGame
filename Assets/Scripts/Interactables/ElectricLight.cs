using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ElectricLight : BaseElectricInteractable {

    public float lightIntensity = 3;
    public float lightRange = 10;
    public float spotlightAngle = 35;
    [Range(0,1)]
    public float flickerChance = 0.1f;

    public bool lightOn { get; private set; }

    private GameObject lightObject;
    private Light electricLight;
    private float lastFlicker = 0;
    private bool flickering = false;

    protected override void Start()
    {
        base.Start();

        Transform lightTransform = transform.Find("Light");
        if (lightTransform)
        {
            lightObject = lightTransform.gameObject;
            electricLight = lightObject.GetComponent<Light>();
        }
    }

    protected override void Update()
    {
        base.Update();

        if (electricLight)
        {
            if (lightOn && IsPowered())
            {
                electricLight.intensity = lightIntensity;
                electricLight.range = lightRange;
                electricLight.spotAngle = spotlightAngle;

                if (flickering)
                {
                    electricLight.intensity = electricLight.intensity * Random.Range(0f, 1f);
                }
            }
            else
            {
                electricLight.intensity = 0;
            }
        }

        if(Time.time - lastFlicker > 1)
        {
            lastFlicker = Time.time;
            flickering = false;

            if (Random.Range(0f, 1f) <= flickerChance)
            {
                flickering = true;
            }
        }
    }

    protected override void OnInteractableStart(GameObject invokerObject)
    {
        base.OnInteractableStart(invokerObject);

        ActivateLight();
    }

    protected override void OnInteractableEnd(GameObject invokerObject)
    {
        base.OnInteractableEnd(invokerObject);

        DeactivateLight();
    }

    public void ActivateLight()
    {
        lightOn = true;
    }

    public void DeactivateLight()
    {
        lightOn = false;
    }

    public void ToggleLight()
    {
        if (lightOn)
        {
            DeactivateLight();
        }
        else
        {
            ActivateLight();
        }
    }
}
