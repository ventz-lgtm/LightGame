using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FadeCameraObstruction : MonoBehaviour {

    public float fadeSpeed = 1f;
    [Range(0,1)]
    public float minFade = 0.1f;

    public float fadePercentage { get; private set; }

    private Renderer renderer;
    private MeshCollider collider;

	// Use this for initialization
	void Start () {
        MeshFilter meshFilter = GetComponent<MeshFilter>();
        renderer = GetComponent<Renderer>();
        collider = GetComponent<MeshCollider>();
        if(meshFilter && renderer && !collider)
        {
            collider = gameObject.AddComponent<MeshCollider>();
            collider.sharedMesh = meshFilter.sharedMesh;
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
                Fade(false);
            }
        }
        else
        {
            Fade(false);
        }
    }

    public void Fade(bool fade)
    {
        if (!renderer) { return; }

        if (fade)
        {
            fadePercentage = Mathf.Min(1 - minFade, fadePercentage + (Time.deltaTime * fadeSpeed));
        }
        else
        {
            fadePercentage = Mathf.Max(0, fadePercentage - (Time.deltaTime * fadeSpeed));
        }

        Color color = renderer.material.color;
        color.a = (1 - fadePercentage);
        renderer.material.color = color;
    }
}
