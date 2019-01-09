using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FadeCameraObstruction : MonoBehaviour {

    public float fadeSpeed = 1f;
    [Range(0,1)]
    public float minFade = 0.1f;

    public float fadePercentage { get; private set; }

    private List<Renderer> renderers;
    private List<MeshCollider> colliders;
    private List<GameObject> objects;

    // Use this for initialization
    void Start() {
        renderers = new List<Renderer>();
        colliders = new List<MeshCollider>();
        objects = new List<GameObject>();
        objects.Add(gameObject);

        MeshFilter meshFilter = GetComponent<MeshFilter>();
        Renderer r = GetComponent<Renderer>();
        if (r != null) {
            renderers.Add(r);
        }

        MeshCollider c = GetComponent<MeshCollider>();
        if(meshFilter && r && !c)
        {
            c = gameObject.AddComponent<MeshCollider>();
            c.sharedMesh = meshFilter.sharedMesh;
        }

        colliders.Add(c);

        for(int i = 0; i < transform.childCount; i++)
        {
            GameObject child = transform.GetChild(i).gameObject;
            Renderer renderer = child.GetComponent<Renderer>();

            if(renderer != null)
            {
                renderers.Add(renderer);
            }

            MeshCollider collider = child.GetComponent<MeshCollider>();
            if(collider != null)
            {
                colliders.Add(collider);
            }

            objects.Add(child);
        }
    }
	
	// Update is called once per frame
	void Update () {
        RaycastHit hit;
        int layerMask = ~0;

        Vector3 origin = Camera.main.gameObject.transform.position;

        if (Physics.Raycast(origin, (GameManager.instance.playerObject.transform.position - origin).normalized, out hit, Vector3.Distance(origin, GameManager.instance.playerObject.transform.position), layerMask))
        {
            GameObject hitObject = hit.collider.gameObject;

            if(hitObject == gameObject)
            {
                Fade(true);
            }
            else
            {
                Debug.DrawLine(GameManager.instance.playerObject.transform.position + new Vector3(0, 2, 0), GameManager.instance.playerObject.transform.position + new Vector3(0, 2, 0) + (-(GameManager.instance.playerObject.transform.position - origin).normalized * 20));
                if (Physics.Raycast(GameManager.instance.playerObject.transform.position + new Vector3(0, 2, 0), -(GameManager.instance.playerObject.transform.position - origin).normalized, out hit, Vector3.Distance(origin, GameManager.instance.playerObject.transform.position), layerMask))
                {
                    GameObject hitObject2 = hit.collider.gameObject;

                    if (objects.Contains(hitObject2))
                    {
                        Fade(true);
                    }
                    else
                    {
                        Fade(false);
                    }
                }
                else
                {
                    Fade(false);
                }
            }
        }
        else
        {
            if (Physics.Raycast(GameManager.instance.playerObject.transform.position + new Vector3(0,2,0), -(GameManager.instance.playerObject.transform.position - origin).normalized, out hit, Vector3.Distance(origin, GameManager.instance.playerObject.transform.position), layerMask))
            {
                GameObject hitObject = hit.collider.gameObject;

                if (hitObject == gameObject)
                {
                    Fade(true);
                }
                else
                {
                    Fade(false);
                }
            }
            else
            {
                Fade(false);
            }
        }
    }

    public void Fade(bool fade)
    {
        if (renderers.Count == 0) { return; }

        if (fade)
        {
            fadePercentage = Mathf.Min(1 - minFade, fadePercentage + (Time.deltaTime * fadeSpeed));
        }
        else
        {
            fadePercentage = Mathf.Max(0, fadePercentage - (Time.deltaTime * fadeSpeed * 2f));
        }

        foreach(Renderer renderer in renderers)
        {
            Color color = renderer.material.color;
            color.a = (1 - fadePercentage);
            renderer.material.color = color;
        }
    }
}
