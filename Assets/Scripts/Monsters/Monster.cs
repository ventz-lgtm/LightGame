using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Monster : MonoBehaviour {

    public float visibleLightThreshold = 0.3f;

    [Header("Movement")]
    public float moveSpeed = 0.5f;
    public bool staggerMovement = false;
    public float staggerDuration = 0.5f;
    public float staggerPause = 1f;
    public float waitUntilEscapeLight = 8f;
    public GameObject particleObject;

    protected ParticleSystem particles;
    private bool staggering = false;
    private float lastStagger = 0;
    private Vector3 vel = Vector3.zero;
    private bool stopped = false;
    private bool escapingLight = false;
    private Vector3 escapeLocation;
    private float stoppedSince = 0;
    private float invisibleSince = 0;

	// Use this for initialization
	void Start () {
        if (particleObject)
        {
            particles = particleObject.GetComponent<ParticleSystem>();
        }
	}
	
	// Update is called once per frame
	void Update () {
        if (escapingLight)
        {
            EscapeLight();
            MoveTowardsTargetLocation(true);
        }
        else
        {
            FaceTowardsPlayer();
            MoveTowardsTargetLocation(false);
        }
	}

    private void FixedUpdate()
    {
        if (IsVisible() || stopped)
        {
            if(particleObject && !particles.isPlaying)
            {
                particles.Play();
            }
        }
        else
        {
            if (particleObject && particles.isPlaying)
            {
                particles.Stop();
            }
        }
    }

    public void MoveTowardsTargetLocation(bool escaping = false)
    {
        if (staggerMovement)
        {
            if (staggering)
            {
                if(Time.time - lastStagger > staggerPause)
                {
                    staggering = false;
                    lastStagger = Time.time;
                }
            }
            else
            {
                if(Time.time - lastStagger > staggerDuration)
                {
                    staggering = true;
                    lastStagger = Time.time;
                }
            }

            if (staggering)
            {
                return;
            }
        }

        if (!escaping)
        {
            Vector3 predictPosition = transform.position + (transform.forward * moveSpeed) + new Vector3(0, 0.5f, 0);
            float predictSample = LightUtil.instance.SampleLightIntensity(predictPosition, false, gameObject);

            if (predictSample >= visibleLightThreshold - 0.05f)
            {
                if (vel != Vector3.zero)
                {
                    transform.position += vel * Time.deltaTime;
                    vel -= vel * 2f * Time.deltaTime;
                }

                if ((IsVisible() && Time.time - invisibleSince > 1f) || (stopped && Time.time - stoppedSince > waitUntilEscapeLight))
                {
                    escapingLight = true;
                    PickEscapeLocation();
                }
                else
                {
                    escapingLight = false;
                }

                if (!stopped)
                {
                    stoppedSince = Time.time;
                }

                stopped = true;
                return;
            }
            else
            {
                stopped = false;
            }

            escapingLight = false;
        }
        else
        {
            stopped = false;
        }

        vel = transform.forward * moveSpeed;
        transform.position += vel * Time.deltaTime;
    }

    public void EscapeLight()
    {
        if (!escapingLight)
        {
            escapingLight = true;
            PickEscapeLocation();
        }

        FaceTowardsLocation(escapeLocation);

        if(Vector3.Distance(escapeLocation, transform.position) < 1)
        {
            GameManager.instance.RemoveMonster(gameObject);
        }
    }

    public void PickEscapeLocation()
    {
        Vector3 rPos = transform.position - GameManager.instance.playerObject.transform.position;
        rPos.y = 0;

        Vector3 origin = transform.position + (rPos).normalized * 10f;
        bool found = false;

        for(int i = 0; i < 10; i++)
        {
            for(int j = 0; j < 8; j++)
            {
                int deg = (360 / 8) * j;
                Vector3 location = origin + (new Vector3(Mathf.Cos(Mathf.Deg2Rad * deg), 0, Mathf.Sin(Mathf.Deg2Rad * deg)) * i * 3f);

                float lightIntensity = LightUtil.instance.SampleLightIntensity(location);
                if(lightIntensity <= 0.1f)
                {
                    origin = location;
                    found = true;
                    break;
                }
            }

            if (found) { break; }
        }

        escapeLocation = origin;
    }

    public void FaceTowardsPlayer()
    {
        GameObject player = GameManager.instance.playerObject;
        if (!player) { return; }

        FaceTowardsLocation(player.transform.position);
    }

    public void FaceTowardsLocation(Vector3 location)
    {
        Vector3 rPos = location - transform.position - new Vector3(0, 0.7f, 0);

        transform.rotation = Quaternion.LookRotation(rPos.normalized, Vector3.up);
    }

    public bool IsVisible()
    {
        bool visible = GetLightingIntensity() >= visibleLightThreshold;

        if (!visible)
        {
            invisibleSince = Time.time;
        }

        return visible;
    }

    public float GetLightingIntensity()
    {
        return LightUtil.instance.SampleLightIntensity(transform.position + new Vector3(0,0.5f,0), gameObject);
    }
}
