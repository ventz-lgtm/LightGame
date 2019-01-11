using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemSpawnPoint : MonoBehaviour {

    public float spawnChance = 1f;

    private bool spawnedItem = false;
    private SpawnPointItem[] items;

    private void Start()
    {
        if(GameManager.instance.playerObject == null) { return; }       
    }

    private void Update()
    {
        if(items == null)
        {
            if(GameManager.instance == null) { return; }

            items = GameManager.instance.spawnPointItems;
            return;
        }

        if (!spawnedItem)
        {
            SpawnItem();
        }
        else
        {
            gameObject.SetActive(false);
        }
    }

    public GameObject SpawnItem()
    {
        /*if(Random.Range(0f, 1f) > Mathf.Min(0.1f, spawnChance)) {
            spawnedItem = true;
            return null;
        }*/

        GameObject parent = GameObject.Find("SpawnedItems");
        if(parent == null)
        {
            parent = new GameObject("SpawnedItems");
        }

        int total = 0;
        foreach(SpawnPointItem item in items)
        {
            total += item.chance;
        }

        int selection = Random.Range(0, total);

        total = 0;
        foreach(SpawnPointItem item in items)
        {
            total += item.chance;

            if(item.prefab == null) { continue; }

            if(total >= selection)
            {
                GameObject obj = Instantiate(item.prefab);
                if(obj == null) { Debug.Log(1); return null; }
                obj.transform.position = transform.position + item.offset + new Vector3(0,0.6f,0);
                obj.transform.rotation = Quaternion.Euler(item.rotation.x, item.rotation.y, item.rotation.z);
                obj.transform.parent = parent.transform;

                spawnedItem = true;

                return obj;
            }
        }

        Debug.Log(2);
        return null;
    }
}

[CreateAssetMenuAttribute]
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