using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DarknessLurker : MonoBehaviour {

    public float maxDuration = 0;
    public float dissapearThreshold = 0.05f;
    public float movement = 0.3f;

    private Renderer rendererComponent;
    private ParticleSystem particleSystem;
    private float darknessLevel = 0;
    private float lastDarknessCheck = 0;
    private float created;
    private Vector3 originalPosition;
    private float timeOffset = 0;

	// Use this for initialization
	void Start () {
        created = Time.time;
        rendererComponent = GetComponent<Renderer>();
        particleSystem = GetComponent<ParticleSystem>();
        SampleLight();

        originalPosition = transform.position;
        timeOffset = Random.Range(0f, 10f);
    }
	
	// Update is called once per frame
	void Update () {
        SampleLight();

        if(darknessLevel > dissapearThreshold)
        {
            GameManager.instance.lurkers.Remove(gameObject);
            Destroy(gameObject);
        }

        

        if(maxDuration > 0 && Time.time - created > maxDuration)
        {
            if (particleSystem != null && particleSystem.isPlaying)
            {
                particleSystem.Stop();
            }

            if(Time.time - created > maxDuration + 2)
            {
                GameManager.instance.lurkers.Remove(gameObject);
                Destroy(gameObject);
            }
        }
        else if(Vector3.Distance(transform.position, GameManager.instance.playerObject.transform.position) < 5)
            {
            maxDuration = Time.time - created - 0.1f;
        }

        if(movement > 0)
        {
            transform.position = originalPosition + (new Vector3(0, Mathf.Cos(Time.time + timeOffset), 0) * movement);
        }
    }

    void SampleLight()
    {
        if (Time.time - lastDarknessCheck > 0.2f)
        {
            lastDarknessCheck = Time.time;
            darknessLevel = LightUtil.instance.SampleLightIntensity(transform.position, gameObject);
        }
    }
}
