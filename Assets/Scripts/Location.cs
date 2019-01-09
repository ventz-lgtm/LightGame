using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class Location : MonoBehaviour {

    public LocationConfig locationConfig;

    private bool spawnedGenerator = false;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        if (!Application.isPlaying) {
            spawnedGenerator = false;

            LocationConfig cfg = locationConfig;
            if (cfg == null) { return; }

            Vector3 origin = transform.position;
            float radius = cfg.locationRadius;

            Debug.DrawLine(origin + new Vector3(radius, 0, 0), origin - new Vector3(radius, 0, 0));
            Debug.DrawLine(origin + new Vector3(0, radius, 0), origin - new Vector3(0, radius, 0));
            Debug.DrawLine(origin + new Vector3(0, 0, radius), origin - new Vector3(0, 0, radius));

            foreach (GeneratorSpawnInfo generatorSpawn in cfg.generatorSpawns)
            {
                Debug.DrawLine(origin, origin + generatorSpawn.location);
            }

            return;
        }

        if (!spawnedGenerator)
        {
            spawnedGenerator = true;

            if (locationConfig.generatorSpawns.Length >= 0)
            {
                GeneratorSpawnInfo location = locationConfig.generatorSpawns[Random.Range(0, locationConfig.generatorSpawns.Length)];
                GameObject generator = Instantiate(GameManager.instance.generatorPrefab);
                generator.transform.position = transform.position + location.location;
                generator.transform.rotation = Quaternion.Euler(location.rotation);
                generator.transform.parent = transform;

                Generator generatorComponent = generator.GetComponent<Generator>();
                generatorComponent.locationType = locationConfig.locationType;
            }
        }
    }
}
