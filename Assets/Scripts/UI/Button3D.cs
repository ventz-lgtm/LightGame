using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Button3D : MonoBehaviour {

    public Material idleMaterial;
    public Material hoveredMaterial;

    [System.Serializable]
    public class Button3DEvent : UnityEvent<GameObject> { }
    public Button3DEvent onClicked;

    private GameObject meshObject;
    private MeshRenderer meshRenderer;
    private Vector3 originalLocation;
    private Vector3 originalRotation;
    private float timeOffset;

    public bool hovered { get; private set; }

	// Use this for initialization
	void Start () {
        meshObject = transform.Find("Mesh").gameObject;
        meshRenderer = meshObject.GetComponent<MeshRenderer>();

        originalLocation = transform.position;
        originalRotation = transform.rotation.eulerAngles;
        timeOffset = Random.Range(1, 10);
    }
	
	// Update is called once per frame
	void Update () {
        RaycastHit hit;
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out hit,100,~0)){
            hovered = hit.transform.gameObject == gameObject;
        }
        else
        {
            hovered = false;
        }

        float mult = 1.8f;
        transform.position = originalLocation + (new Vector3(0, Mathf.Sin(Time.time + timeOffset), 0) * 0.07f);
        transform.rotation = Quaternion.Euler(new Vector3(Mathf.Sin(Time.time + timeOffset) + Mathf.Cos(Time.time * 2) * mult, Mathf.Cos(Time.time * 1.5f) * mult, Mathf.Sin(Time.time * 3)) * mult);

        Debug.Log(hovered);

        if (hovered)
        {
            meshRenderer.sharedMaterial = hoveredMaterial;

            if (Input.GetMouseButtonDown(0))
            {
                if (onClicked != null)
                {
                    onClicked.Invoke(null);
                }
            }
        }
        else
        {
            meshRenderer.sharedMaterial = idleMaterial;
        }
	}
}
