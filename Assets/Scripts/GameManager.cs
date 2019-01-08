using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour {

    public enum LocationType { NONE, TOWN, CAMP, WATER_TOWER, TRAIN_STATION }

    public static GameManager instance;

    public float darknessWarningThreshold = 0.5f;
    public float darknessDeathThreshold = 0.2f;
    public GameObject playerObject;
    public MonsterPrefabType[] monsterPrefabs;

    public GeneratorPart[] generatorParts;

    [HideInInspector]
    Dictionary<LocationType, bool> activatedLocations = new Dictionary<LocationType, bool>();

    public float dangerLevel { get; private set; }
    public float sanity { get; private set; }

    public Character playerCharacter;

    private float lastMonsterSpawn = 0;

	// Use this for initialization
	void Start () {
        if(instance == null)
        {
            instance = this;
        }else if(instance != this)
        {
            Destroy(gameObject);
            return;
        }

        if(playerObject != null)
        {
            playerCharacter = playerObject.GetComponent<Character>();
        }

        dangerLevel = 0;
    }
	
	// Update is called once per frame
	void Update () {
		if(playerCharacter != null)
        {
            float lightLevel = playerCharacter.GetLightIntensity();
            float valueChange = (((1 - lightLevel) - 0.5f) * 0.01f);
            if(valueChange > 0)
            {
                valueChange *= 0.03f;
            }

            dangerLevel = Mathf.Clamp(dangerLevel + valueChange, 0, 1);
        }
	}

    public GameObject SpawnMonster(GameObject monsterPrefab = null)
    {
        if(monsterPrefabs.Length == 0) { return null; }

        if (monsterPrefab == null)
        {
            MonsterPrefabType prefabType = monsterPrefabs[Mathf.Clamp(Random.Range(0, monsterPrefabs.Length - 1), 0, monsterPrefabs.Length - 1)];
            monsterPrefab = prefabType.prefab;
        }

        GameObject monster = Instantiate(monsterPrefab);

        return monster;
    }

    public bool IsLocationActive(LocationType type)
    {
        if (!activatedLocations.ContainsKey(type)) { return false; }
        return activatedLocations[type] == true;
    }

    public void SetLocationActive(LocationType type, bool active)
    {
        activatedLocations[type] = active;
    }
}

[System.Serializable]
public class GeneratorPart
{
    public string name;
    public Mesh mesh;
    public Material material;
}

[System.Serializable]
public class MonsterPrefabType
{
    public GameObject prefab;
    public int spawnAfterSeconds = 0;
    public int spawnChance = 100;
}