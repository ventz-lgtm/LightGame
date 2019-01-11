using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireTorchHoldable : PickUpItem {

    public float lightFlicker = 0;
    public float fuelUsage = 1 / 60;
    public float fuelPercentage { get; private set; }
    public bool fireLightOn = false;

    protected Light fireLight;
    protected ParticleSystem particles;
    protected float baseLightIntensity = 0;
    protected float fireAlpha = 0;
    
    protected override void Start()
    {
        base.Start();

        fuelPercentage = 1f;

        Transform fireTransform = transform.Find("Light");
        if (fireTransform)
        {
            fireLight = fireTransform.gameObject.GetComponent<Light>();
            baseLightIntensity = fireLight.intensity;
        }

        Transform particleTransform = transform.Find("Particles");
        if (particleTransform)
        {
            particles = particleTransform.gameObject.GetComponent<ParticleSystem>();
        }

        Extinguish();
    }

    protected override void Update()
    {
        base.Update();

        if (fireLightOn)
        {
            fuelPercentage = Mathf.Max(0, fuelPercentage - (fuelUsage * Time.deltaTime));
        }

        if(fireLight != null)
        {
            fireLight.gameObject.transform.position = transform.position + new Vector3(0, 1.5f, 0);
        }
    }

    public void LateUpdate()
    {
        if (fireLightOn)
        {
            fireAlpha = Mathf.Min(fuelPercentage, fireAlpha + Time.deltaTime);
        }
        else
        {
            fireAlpha = Mathf.Max(0, fireAlpha - Time.deltaTime);
        }

        if(fireAlpha > 0)
        {
            if (fireLight)
            {
                fireLight.intensity = (baseLightIntensity + Random.Range(-lightFlicker, lightFlicker)) * fireAlpha;
            }
        }
    }

    public void Light()
    {
        fireLightOn = true;

        if (particles)
        {
            particles.Play();
        }
    }

    public void Extinguish()
    {
        fireLightOn = false;

        if (particles)
        {
            particles.Stop();
        }
    }

    public void Toggle()
    {
        if (fireLightOn)
        {
            Extinguish();
        }
        else
        {
            Light();
        }
    }

    public override void OnUse()
    {
        base.OnUse();

        Toggle();
    }

    public override string GetHoldText()
    {
        if (fireLightOn)
        {
            return "Torch: " + Mathf.Round(fuelPercentage * 100) + "%";
        }
        else
        {
            return "";
        }
    }
}
