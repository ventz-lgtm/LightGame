using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fire : BaseInteractable {

    public float fireIntensity = 1;
    public float fireRange = 5;
    public float flickerAmount = 0.1f;

    public bool fireOn { get; private set; }

    private GameObject particleObject;
    private ParticleSystem particleSystem;
    private GameObject lightObject;
    private Light light;
    private float fireAlpha = 0;
    private float flicker = 0;
    private float flickerChange = 0;

    protected override void Start()
    {
        ExtinguishFire();

        base.Start();

        Transform lightTransform = transform.Find("Light");
        if (lightTransform)
        {
            lightObject = lightTransform.gameObject;
            light = lightObject.GetComponent<Light>();
        }

        Transform particleTransform = transform.Find("Particles");
        if (particleTransform)
        {
            particleObject = particleTransform.gameObject;
            particleSystem = particleObject.GetComponent<ParticleSystem>();
        }
    }
    protected override void Update()
    {
        base.Update();

        if (fireOn)
        {
            fireAlpha = Mathf.Min(1, fireAlpha + (Time.deltaTime * 4));
        }
        else
        {
            fireAlpha = Mathf.Max(0, fireAlpha - Time.deltaTime);
        }

        flickerChange += Random.Range(-flickerAmount, flickerAmount);
        flickerChange = Mathf.Clamp(flickerChange, -flickerAmount, flickerAmount);
        if(flickerChange == flickerAmount || flickerChange == flickerAmount)
        {
            flickerChange = 0;
        }

        flicker = Mathf.Clamp(flicker + flickerChange, -flickerAmount, flickerAmount);

        if (light)
        {
            if(fireAlpha > 0)
            {
                light.intensity = fireAlpha + flicker;
            }
            else
            {
                light.intensity = 0;
            }
            
            light.range = fireRange;
        }
    }

    protected override void OnInteractableStart(GameObject invokerObject)
    {
        base.OnInteractableStart(invokerObject);

        LightFire();
    }

    protected override void OnInteractableEnd(GameObject invokerObject)
    {
        base.OnInteractableEnd(invokerObject);

        ExtinguishFire();
    }

    void LightFire()
    {
        fireOn = true;

        if (particleSystem)
        {
            particleSystem.Play();
        }
    }

    void ExtinguishFire()
    {
        fireOn = false;

        if (particleSystem)
        {
            particleSystem.Stop();
        }
    }
}
