using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class PrefabScatter : MonoBehaviour {

    public float width = 5f;
    public float length = 5f;
    public float height = 5f;

    public int seed = 1;

    public ScatterPrefabSettings settings;

    private bool hasSpawned = false;
    private int lastSeed = 0;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        if (!Application.isPlaying)
        {
            Debug.DrawLine(transform.position, transform.position + new Vector3(width, 0, 0));
            Debug.DrawLine(transform.position, transform.position + new Vector3(0, 0, length));
            Debug.DrawLine(transform.position + new Vector3(0,0,length), transform.position + new Vector3(width, 0, length));
            Debug.DrawLine(transform.position + new Vector3(width,0,0), transform.position + new Vector3(width, 0, length));

            Debug.DrawLine(transform.position + new Vector3(0,-height,0), transform.position + new Vector3(width, -height, 0));
            Debug.DrawLine(transform.position + new Vector3(0, -height, 0), transform.position + new Vector3(0, -height, length));
            Debug.DrawLine(transform.position + new Vector3(0, -height, length), transform.position + new Vector3(width, -height, length));
            Debug.DrawLine(transform.position + new Vector3(width, -height, 0), transform.position + new Vector3(width, -height, length));
        }

        if (!hasSpawned || lastSeed != seed)
        {
            lastSeed = seed;
            hasSpawned = true;
            SpawnItems();
        }
	}

    public void SpawnItems()
    {
        RemoveItems();

        ScatterPrefabConfig[] prefabs = settings.prefabs;
        Random.InitState(seed);

        foreach (ScatterPrefabConfig cfg in prefabs)
        {
            int amount = Random.Range(cfg.minPerSegment, cfg.maxPerSegment);
            amount = amount * (int)(width * 0.1f) * (int)(length * 0.1f);

            if(amount > 0)
            {
                Vector3[] locations = new Vector3[amount];

                for(int i = 0; i < amount; i++)
                {
                    Vector3 location = PickLocation();
                    if(location != Vector3.zero)
                    {
                        bool cont = false;
                        for(int j = 0; j < locations.Length; j++)
                        {
                            if(locations[j] != null && Vector3.Distance(location, locations[j]) < cfg.radius)
                            {
                                cont = true;
                                break;
                            }
                        }
                        if (cont) { continue; }

                        GameObject item = Instantiate(cfg.prefab);
                        if(item != null)
                        {
                            item.transform.position = location + cfg.offset;
                            item.transform.parent = transform;
                            locations[i] = location + cfg.offset;

                            Vector3 angle = cfg.angle;
                            angle += new Vector3(Random.Range(cfg.randomizePitch.x, cfg.randomizePitch.y), Random.Range(cfg.randomizeYaw.x, cfg.randomizeYaw.y), Random.Range(cfg.randomizeRoll.x, cfg.randomizeRoll.y));
                            item.transform.rotation = Quaternion.Euler(angle.x, angle.y, angle.z);

                            float scale = Mathf.Max(1, Random.Range(cfg.scale.x, cfg.scale.y));
                            item.transform.localScale = new Vector3(item.transform.localScale.x * scale, item.transform.localScale.y * scale, item.transform.localScale.z * scale);

                            if (cfg.addCollider)
                            {
                                Collider c = item.GetComponent<Collider>();
                                if (!c)
                                {
                                    item.AddComponent<MeshCollider>();
                                }
                            }
                        }
                    }
                }
            }
        }

        Random.InitState(System.DateTime.Now.Second);
    }

    public void RemoveItems()
    {
        for(int i = transform.childCount - 1; i >= 0; i--)
        {
            Transform child = transform.GetChild(i);
            DestroyImmediate(child.gameObject);
        }
    }

    public Vector3 PickLocation()
    {
        Vector3 location = transform.position + new Vector3(Random.Range(0f, width), 0, Random.Range(0f, length));

        RaycastHit hit;
        int layerMask = ~0;

        if (Physics.Raycast(location, -Vector3.up, out hit, height, layerMask))
        {
            return hit.point;
        }

        return Vector3.zero;
    }
}

[System.Serializable]
public class ScatterPrefabConfig
{
    public GameObject prefab;
    public int maxPerSegment;
    public int minPerSegment;
    public float radius;
    public bool addCollider;
    public Vector2 scale;
    public Vector3 offset;
    public Vector3 angle;
    public Vector2 randomizePitch;
    public Vector2 randomizeYaw;
    public Vector2 randomizeRoll;
}

[CreateAssetMenu]
public class ScatterPrefabSettings : ScriptableObject
{
    public ScatterPrefabConfig[] prefabs;
}