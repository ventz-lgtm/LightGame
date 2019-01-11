using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DroppedFlare : BaseInventoryItem {

    public float flareIntensity = 3f;
    public float flareRange = 5f;

    protected GameObject flareLightObject;
    protected Light flareLight;
    protected ParticleSystem flareParticles;
    protected bool flareOn = false;

    public float flareDuration;
    private float flareFuelRemaining;
    private float startTime;
    private float initialVolume;

    AudioSource flareSounds;
    AudioClip flareOnSound;
    AudioClip flareStartSound;
    AudioClip flareEndSound;


    protected override void Start()
    {
        base.Start();

        Transform flareLightTransform = transform.Find("Particles");
        if (flareLightTransform)
        {
            flareLightObject = flareLightTransform.gameObject;
            flareParticles = flareLightObject.GetComponent<ParticleSystem>();
        }

        flareLightTransform = transform.Find("Light");
        if (flareLightTransform)
        {
            flareLight = flareLightTransform.gameObject.GetComponent<Light>();
        }

        flareSounds = GetComponent<AudioSource>();
        flareOnSound = (AudioClip)Resources.Load("Audio/flare_loop");
        flareStartSound = (AudioClip)Resources.Load("Audio/RoadFlareStart");
        flareEndSound = (AudioClip)Resources.Load("Audio/RoadFlareEnd");
        flareSounds.clip = flareStartSound;
        initialVolume = flareSounds.volume;
        flareFuelRemaining = 1;
    }

    protected override void Update()
    {
        base.Update();

        if (flareOn)
        {
            flareFuelRemaining -= Time.deltaTime * 0.01f;

            if(flareLightObject && !flareLightObject.activeSelf)
            {
                flareLightObject.SetActive(true);
            }

            if (!flareSounds.isPlaying && flareSounds.enabled)
            {
                flareSounds.clip = flareOnSound;
                flareSounds.loop = true;
                flareSounds.Play();
            }

            float volumeDistance = 8f;
            flareSounds.volume = 0.3f * Mathf.Clamp(initialVolume * ((volumeDistance - Vector3.Distance(transform.position, GameManager.instance.playerObject.transform.position)) / volumeDistance), 0, initialVolume);
            
            if (flareLight)
            {
                flareLight.range = flareRange + Random.Range(-.5f, .5f);
                flareLight.intensity = flareIntensity;
            }
            if (flareParticles && !flareParticles.isPlaying)
            {
                flareParticles.Play();
            }

            if (flareFuelRemaining <= 0)
            {
                flareOn = false;
                flareSounds.Stop();
                flareSounds.loop = false;
                flareSounds.clip = flareEndSound;
                flareSounds.Play();
            }
        }
        else
        {
            if(flareLightObject && flareLightObject.activeSelf)
            {
                flareLightObject.SetActive(false);
            }

            if (flareLight)
            {
                flareLight.intensity = 0;
            }
        }

        if (flareLight)
        {
            flareLight.gameObject.transform.position = transform.position + new Vector3(0, 0.32f, 0);
        }
    }

    protected override void OnInteractableStart(GameObject invokerObject)
    {
        if (flareOn) { return; }
        if(invokerObject == null)
        {
            Ignite();
            return;
        }

        flareOn = true;
        flareFuelRemaining = 1;

        base.OnInteractableStart(invokerObject);
    }

    public void Ignite()
    {
        flareOn = true;
        startTime = Time.time;
        flareSounds.Play();
        
    }
}
