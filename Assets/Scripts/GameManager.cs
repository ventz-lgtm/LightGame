using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour {

    public enum LocationType { NONE, TOWN, CAMP, WATER_TOWER, TRAIN_STATION }

    public static GameManager instance;
    public GameObject playerObject;

    public GeneratorPart[] generatorParts;

    [HideInInspector]
    Dictionary<LocationType, bool> activatedLocations = new Dictionary<LocationType, bool>();

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
	}
	
	// Update is called once per frame
	void Update () {
		
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