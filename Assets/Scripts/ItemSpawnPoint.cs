using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemSpawnPoint : MonoBehaviour {

    public float spawnChance = 1f;
    public ItemSpawnPointSettings settings;

    private void Start()
    {
        if(GameManager.instance.playerObject == null) { return; }

        SpawnItem();
        Destroy(gameObject);
    }

    public GameObject SpawnItem()
    {
        if(settings == null) { return null; }
        if(Random.Range(0f, 1f) > Mathf.Min(settings.maxSpawnChance, spawnChance)) { return null; }

        GameObject parent = GameObject.Find("SpawnedItems");
        if(parent == null)
        {
            parent = new GameObject("SpawnedItems");
        }

        int total = 0;
        foreach(SpawnPointItem item in settings.items)
        {
            total += item.chance;
        }

        int selection = Random.Range(0, total - 1);

        total = 0;
        foreach(SpawnPointItem item in settings.items)
        {
            total += item.chance;

            if(item.prefab == null) { continue; }

            if(total > selection)
            {
                GameObject obj = Instantiate(item.prefab);
                if(obj == null) { return null; }
                obj.transform.position = transform.position + item.offset + new Vector3(0,0.6f,0);
                obj.transform.rotation = Quaternion.Euler(item.rotation.x, item.rotation.y, item.rotation.z);
                obj.transform.parent = parent.transform;

                return obj;
            }
        }

        return null;
    }
}

[CreateAssetMenu]
public class ItemSpawnPointSettings : ScriptableObject
{
    public float maxSpawnChance = 1f;
    public SpawnPointItem[] items;
}

[System.Serializable]
public class SpawnPointItem
{
    public int chance = 100;
    public GameObject prefab;
    public Vector3 offset;
    public Vector3 rotation;
}