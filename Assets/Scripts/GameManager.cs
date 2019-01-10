using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour {

    public enum LocationType { NONE, TOWN, CAMP, WATER_TOWER, CABIN }
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
    public int maxLurkers = 10;
    public MonsterPrefabType[] monsterPrefabs;
    public LurkerPrefabType[] lurkerPrefabs;
    public GeneratorPart[] generatorParts;

    [Header("Crafting")]
    public InventoryRecipe[] recipes;

    [Header("Audio")]
    public bool playSounds = true;
    public float windVolume = 0.14f;

    [Header("UI")]
    public GameObject hintPrefab;
    public GameObject escapeMenuPanel;
    public GameObject gameOverScreen;
    public GameObject fadeToBlackScreen;
    private Color tempColor;

    [HideInInspector]
    Dictionary<LocationType, bool> activatedLocations = new Dictionary<LocationType, bool>();

    public float dangerLevel { get; private set; }
    public float sanity { get; private set; }
    public float minimumSanity { get; private set; }
    public ArrayList monsters { get; private set; }
    public ArrayList lurkers { get; private set; }
    public Character playerCharacter { get; private set; }

    private float lastMonsterSpawn = 0;
    private float lastMonsterTrySpawn = 0;
    private float lastNotify = 0;
    private float notifyTime = 0;
    private string notifyText = "";
    private float notifyAlpha = 0;
    private Light dangerLight;
    private float lastLurker = 0;
    private GameObject hintPanel;
    private GameObject gameOverInstance;
    private GameObject fadeToBlackInstance;
    private bool gameOver = false;

    AudioSource[] audioSources;
    AudioSource backgroundAmbience;
    AudioSource windSource;
    AudioSource sanityWhispers;

    private float windSourceVolume = 0f;

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
            dangerLight = playerObject.transform.Find("DangerLight").GetComponent<Light>();
        }

        audioSources = GetComponents<AudioSource>();
        backgroundAmbience = audioSources[0];
        backgroundAmbience.volume = 0.2f;

        sanityWhispers = audioSources[1];
        sanityWhispers.volume = 0.0f;

        backgroundAmbience.Play();

        windSource = audioSources[2];
        windSource.volume = 0f;
        windSource.Play();

        if (!playSounds)
        {
            foreach(AudioSource source in audioSources)
            {
                source.volume = 0;
            }
        }
    }

    // Use this for initialization
    void Start () {
        dangerLevel = 0;
        monsters = new ArrayList();
        lurkers = new ArrayList();

        Camera.main.farClipPlane = 30f;
    }
	
	// Update is called once per frame
	void Update () {
        backgroundAmbience.volume = 0.2f * dangerLevel;

        if (Input.GetButtonDown("Pause"))
        {
            if (escapeMenuPanel.activeSelf)
            {
                escapeMenuPanel.SetActive(false);
            }
            else
            {
                escapeMenuPanel.SetActive(true);
            }
        }

        ////////////////
        // WIND SOUND //
        ////////////////

        if (playerObject != null)
        {
            int layerMask = ~0;
            RaycastHit hit;
            if (Physics.Raycast(playerObject.transform.position + new Vector3(0, 1, 0), Vector3.up, out hit, 20, layerMask))
            {
                windSourceVolume = windVolume * 0.15f * (1 - dangerLevel);
            }
            else
            {
                windSourceVolume = windVolume * (1 - dangerLevel);
            }

            windSource.volume = windSource.volume + ((windSourceVolume - windSource.volume) * 2f * Time.deltaTime);
        }
        if (!playSounds) { windSource.volume = 0f; }

        //////////////////
        // DANGER LIGHT //
        //////////////////

        float minDistance = Mathf.Infinity;
        for(int i = 0; i < transform.childCount; i++)
        {
            minDistance = Mathf.Min(minDistance, Vector3.Distance(playerObject.transform.position, transform.GetChild(i).transform.position));
        }

        if (dangerLight != null)
        {
            float dangerLightPercentage = Mathf.Clamp((3 - minDistance) / 3, 0, 1);
            dangerLight.range = 8;
            dangerLight.intensity = dangerLightPercentage * 0.38f;

            dangerLevel = Mathf.Max(dangerLevel, dangerLightPercentage);
        }

        /////////////
        // LURKERS //
        /////////////

        if(Time.time - lastLurker > Mathf.Max(0.1f, 1 - dangerLevel) * 1)
        {
            lastLurker = Time.time;

            foreach(LurkerPrefabType type in lurkerPrefabs)
            {
                if(type.dangerThreshold < dangerLevel)
                {
                    if(lurkers.Count >= maxLurkers) { break; }

                    GameObject lurker = Instantiate(type.prefab);
                    lurker.transform.position = PickMonsterSpawnLocation(Random.Range(8f, 15f));
                    lurkers.Add(lurker);
                }
            }
        }

        /////////////////////
        // SANITY & DANGER //
        /////////////////////

        if (playerCharacter != null)
        {
            float lightLevel = playerCharacter.GetLightIntensity();

            if(lightLevel <= 0.2f)
            {
                sanity = Mathf.Clamp(sanity + (0.01f * Time.deltaTime), minimumSanity, 1f);
            }
            else
            {
                sanity = Mathf.Clamp(sanity - (lightLevel * Time.deltaTime * 0.07f), minimumSanity, 1f);
            }

            float valueChange = (((1 - lightLevel) - 0.5f));
            if(valueChange > 0)
            {
                valueChange *= 0.02f;
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

            if(sanity >= 1.0f)
            {
                gameOver = true;
                fadeOut();
                StartCoroutine(waitThenMenu());
            }
        }

        //////////////////////
        // GameOver effects //
        //////////////////////
        else
        {
            tempColor.a += 0.005f;
            fadeToBlackInstance.GetComponent<Image>().color = tempColor;
        }
	}

    private void fadeOut()
    {
        gameOverInstance = Instantiate(gameOverScreen);
        gameOverInstance.transform.SetParent(Camera.main.transform.Find("Canvas"));
        gameOverInstance.GetComponent<RectTransform>().anchoredPosition = new Vector2(0.0f, 0.0f);
        fadeToBlackInstance = Instantiate(fadeToBlackScreen);
        tempColor = fadeToBlackInstance.GetComponent<Image>().color;
        tempColor.a = 0.0f;
        fadeToBlackInstance.GetComponent<Image>().color = tempColor;

        fadeToBlackInstance.transform.SetParent(Camera.main.transform.Find("Canvas"));
        fadeToBlackInstance.GetComponent<RectTransform>().anchoredPosition = new Vector2(0.0f, 0.0f);
    }
    IEnumerator waitThenMenu() { 
        yield return new WaitForSeconds(5);
        SceneManager.LoadScene("Menu");
    }

    public void LoadScene(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
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

    public Vector3 PickMonsterSpawnLocation(float distance = 0)
    {
        Vector3 location = playerObject.transform.position;

        float degree = Random.Range(0f, 360f) * Mathf.Deg2Rad;
        distance = distance == 0 ? Random.Range(15f, 30f) : distance;

        location += new Vector3(Mathf.Cos(degree), 0, Mathf.Sin(degree)) * distance;

        return location;
    }

    public GameObject SpawnMonster(GameObject monsterPrefab = null)
    {
        if(monsterPrefabs.Length == 0) { return null; }

        int total = 0;
        foreach(MonsterPrefabType type in monsterPrefabs)
        {
            total += type.spawnChance;
        }

        int selection = Random.Range(0, total);
        total = 0;

        foreach(MonsterPrefabType type in monsterPrefabs)
        {
            total += type.spawnChance;

            if(total >= selection)
            {
                monsterPrefab = type.prefab;
                GameObject monster = Instantiate(monsterPrefab);

                return monster;
            }
        }

        return null;
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

    public void ShowHint(string text, string title = "Hint")
    {
        if(hintPanel != null)
        {
            Destroy(hintPanel);
        }

        hintPanel = Instantiate(hintPrefab);
        hintPanel.transform.SetParent(Camera.main.transform.Find("Canvas").gameObject.transform);
        RectTransform rt = hintPanel.GetComponent<RectTransform>();
        rt.anchoredPosition = new Vector2(240, -300);
        HintPanel panelComponent = hintPanel.GetComponent<HintPanel>();
        panelComponent.SetTitle(title);
        panelComponent.SetText(text);
    }

    public void HealSanity(float amount)
    {
        sanity = Mathf.Max(0, sanity - amount);
    }

    public void ExitGame()
    {
        Application.Quit();
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
public class LurkerPrefabType
{
    public GameObject prefab;
    public float dangerThreshold = 0.5f;
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
    public bool deleteOnDrop = false;
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