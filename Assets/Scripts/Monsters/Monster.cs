using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Monster : MonoBehaviour {

    public float visibleLightThreshold = 0.3f;
    public bool avoidLight = true;
    public float escapeAfterSeconds = 0;

    [Header("Movement")]
    public float moveSpeed = 0.5f;
    public bool staggerMovement = false;
    public float staggerDuration = 0.5f;
    public float staggerPause = 1f;
    public float waitUntilEscapeLight = 8f;
    public float contactDamage = 0.2f;
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
    private bool visible = false;
    private float lastGrowl = 0;
    private float lastSurprise = 0;
    private float predictSample;
    private float created = 0;

    AudioSource[] audioSources;
    AudioSource monsterSuprise;
    AudioSource monsterGrowl;

    AudioClip monsterSupriseClip1;
    AudioClip monsterSupriseClip2;

    GameObject player;
	// Use this for initialization
	void Start () {
        if (particleObject)
        {
            particles = particleObject.GetComponent<ParticleSystem>();
        }

        audioSources = GetComponents<AudioSource>();

        monsterSuprise = audioSources[0];
        monsterGrowl = audioSources[1];

        monsterSupriseClip1 = (AudioClip)Resources.Load("Audio/horror_sound_one");
        monsterSupriseClip2 = (AudioClip)Resources.Load("Audio/horror_sound_two");

        player = GameManager.instance.playerObject;
        created = Time.time;
    }
	
	// Update is called once per frame
	void Update () {

        if (player)
        {
            Debug.Log(player.name);
        }

        if(Vector3.Distance(player.transform.position, gameObject.transform.position) <= 1.0f)
        {
            GameManager.instance.DamageSanity(contactDamage);
            Destroy(gameObject);
        }


        if (Time.time - lastGrowl > 18)
        {
            lastGrowl = Time.time;
            if (Vector3.Distance(player.transform.position, gameObject.transform.position) < 6.0f && !visible)
            {
                StartCoroutine(GrowlAfterSeconds());
            }
        }

        if(escapeAfterSeconds > 0 && Time.time - created > escapeAfterSeconds)
        {
            escapingLight = true;
        }

        if (visible && !escapingLight && Time.time - lastSurprise > 30)
        {
            RaycastHit hit;

            bool playSound = false;
            Vector3 origin = Camera.main.gameObject.transform.position;
            if (Physics.Raycast(origin, (transform.position - origin).normalized, out hit, Vector3.Distance(origin, transform.position) - 1, ~0))
            {
                Debug.Log("hit " + hit.transform);
                Transform objectHit = hit.transform;

                playSound = objectHit.gameObject == gameObject;
            }
            else
            {
                Debug.Log("no hit");
                playSound = true;
            }

            if (playSound)
            {
                lastSurprise = Time.time;

                if (!monsterSuprise.isPlaying)
                {
                    if (Random.Range(0.0f, 1.0f) > 0.5f)
                    {
                        monsterSuprise.PlayOneShot(monsterSupriseClip1);
                    }
                    else
                    {
                        monsterSuprise.PlayOneShot(monsterSupriseClip2);
                    }
                }
            }
        }

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

    public IEnumerator GrowlAfterSeconds()
    {
        
        yield return new WaitForSeconds(Mathf.Floor(Random.Range(0.0f, 8.0f)));

        if (!monsterGrowl.isPlaying)
        {
            monsterGrowl.Play();
        }

    }

    private void FixedUpdate()
    {
        visible = IsVisible();
        Vector3 predictPosition = transform.position + (transform.forward * moveSpeed) + new Vector3(0, 0.5f, 0);
        predictSample = LightUtil.instance.SampleLightIntensity(predictPosition, false, gameObject);

        if (visible || stopped)
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

        if (!escaping && avoidLight)
        {
            if (predictSample >= visibleLightThreshold - 0.05f)
            {
                if (vel != Vector3.zero)
                {
                    transform.position += vel * Time.deltaTime;
                    vel -= vel * 2f * Time.deltaTime;
                }

                if ((visible && Time.time - invisibleSince > 1f) || (stopped && Time.time - stoppedSince > waitUntilEscapeLight))
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
