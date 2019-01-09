using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour {

    public enum LocationType { NONE, TOWN, CAMP, WATER_TOWER, TRAIN_STATION }
    public enum InventoryItem { STICK }

    public static GameManager instance;

    [Header("Game")]
    public float darknessWarningThreshold = 0.5f;
    public float darknessDeathThreshold = 0.2f;
    public GameObject playerObject;
    public GameObject generatorPrefab;

    [Header("Monsters")]
    public int maxMonsters = 10;
    public float monsterSpawnCooldown = 10f;
    public int minMonsterSpawnChance = 10;
    public MonsterPrefabType[] monsterPrefabs;
    public GeneratorPart[] generatorParts;

    [Header("Crafting")]
    public InventoryRecipe[] recipes;

    [HideInInspector]
    Dictionary<LocationType, bool> activatedLocations = new Dictionary<LocationType, bool>();

    public float dangerLevel { get; private set; }
    public float sanity { get; private set; }
    public float minimumSanity { get; private set; }
    public ArrayList monsters { get; private set; }
    public Character playerCharacter { get; private set; }

    private float lastMonsterSpawn = 0;
    private float lastMonsterTrySpawn = 0;
    private float lastNotify = 0;
    private float notifyTime = 0;
    private string notifyText = "";
    private float notifyAlpha = 0;

    AudioSource[] audioSources;
    AudioSource backgroundAmbience;
    AudioSource sanityWhispers;
    AudioListener audioListener;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else if (instance != this)
        {
            Destroy(gameObject);
            return;
        }

        if (playerObject != null)
        {
            playerCharacter = playerObject.GetComponent<Character>();
        }

        audioListener = playerObject.GetComponent<AudioListener>();

        audioSources = GetComponents<AudioSource>();
        backgroundAmbience = audioSources[0];
        backgroundAmbience.volume = 0.2f;
        

        sanityWhispers = audioSources[1];
        sanityWhispers.volume = 0.0f;

        backgroundAmbience.Play();

    }

    // Use this for initialization
    void Start () {
        dangerLevel = 0;
        monsters = new ArrayList();

        Camera.main.farClipPlane = 30f;
    }
	
	// Update is called once per frame
	void Update () {
		if(playerCharacter != null)
        {
            float lightLevel = playerCharacter.GetLightIntensity();

            if(lightLevel <= 0.2f)
            {
                sanity = Mathf.Clamp(sanity + (0.02f * Time.deltaTime), minimumSanity, 1f);
            }
            else
            {
                sanity = Mathf.Clamp(sanity - (lightLevel * Time.deltaTime * 0.07f), minimumSanity, 1f);
            }

            float valueChange = (((1 - lightLevel) - 0.5f));
            if(valueChange > 0)
            {
                valueChange *= 0.03f;
            }
            else
            {
                valueChange *= 0.06f;
            }

            sanityWhispers.volume = sanity;

            dangerLevel = Mathf.Clamp(dangerLevel + valueChange * Time.deltaTime, 0, 1);

            if(Time.time - lastMonsterTrySpawn > 1 && Time.time - lastMonsterSpawn > monsterSpawnCooldown && monsters.Count < maxMonsters)
            {
                lastMonsterTrySpawn = Time.time;

                int chance = Random.Range(0, 100);
                float threshold = Mathf.Max(minMonsterSpawnChance, dangerLevel * 20);

                if (chance < threshold)
                {
                    GameObject monster = SpawnMonster();
                    if(monster != null)
                    {
                        monster.transform.position = PickMonsterSpawnLocation();
                        monsters.Add(monster);
                        monster.transform.parent = transform;
                        lastMonsterSpawn = Time.time;
                    }
                }
            }
        }
	}

    public float GetDangerLevel()
    {
        return Mathf.Max(sanity, dangerLevel);
    }

    public void DamageSanity(float amount)
    {
        minimumSanity = Mathf.Clamp(minimumSanity + amount, 0f, 1f);
    }

    public void RemoveMonster(GameObject monster)
    {
        if (monsters.Contains(monster))
        {
            monsters.Remove(monster);
        }

        Destroy(monster);
    }

    public Vector3 PickMonsterSpawnLocation()
    {
        Vector3 location = playerObject.transform.position;

        float degree = Random.Range(0f, 360f) * Mathf.Deg2Rad;
        float distance = Random.Range(15f, 30f);

        location += new Vector3(Mathf.Cos(degree), 0, Mathf.Sin(degree)) * distance;

        return location;
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

    public void Notify(string message, float time)
    {
        lastNotify = Time.time;
        notifyTime = time;
        notifyText = message;
    }

    void OnGUI()
    {
        if(Time.time - lastNotify < notifyTime)
        {
            notifyAlpha = Mathf.Min(1, notifyAlpha + Time.deltaTime);
        }
        else
        {
            notifyAlpha = Mathf.Max(0, notifyAlpha - Time.deltaTime);
        }

        if(notifyAlpha <= 0) { return; }

        int w = Screen.width, h = Screen.height;

        GUIStyle style = new GUIStyle();

        Rect rect = new Rect(0, 0, w, h * 0.3f);
        style.alignment = TextAnchor.MiddleCenter;
        style.fontSize = h * 2 / 100;
        style.normal.textColor = new Color(1f, 1f, 1f, notifyAlpha);
        GUI.Label(rect, notifyText, style);
    }
}

[System.Serializable]
public class GeneratorPart
{
    public string name;
    public Mesh mesh;
    public Material material;
    public Mesh generatorMesh;
    public PickUpItem.items pickupType;
}

[System.Serializable]
public class MonsterPrefabType
{
    public GameObject prefab;
    public int spawnAfterSeconds = 0;
    public int spawnChance = 100;
}

[System.Serializable]
public class LocationConfig
{
    public string name;
    public GameManager.LocationType locationType;
    public float locationRadius;
    public GeneratorSpawnInfo[] generatorSpawns;
}

[System.Serializable]
public class GeneratorSpawnInfo
{
    public Vector3 location;
    public Vector3 rotation;
}

[System.Serializable]
public class InventoryItemType
{
    public string name;
    public Sprite icon;
    public GameObject instance;
}

[System.Serializable]
public class InventoryRecipe
{
    public string name;
    public Sprite icon;
    public GameObject prefab;
    public string[] ingredients;
}