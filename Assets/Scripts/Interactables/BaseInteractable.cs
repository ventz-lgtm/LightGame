using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class BaseInteractable : MonoBehaviour {

    public enum InteractableType { PRESS, TOGGLE, HOLD }

    [System.Serializable]
    public class InteractableEvent : UnityEvent<GameObject> { }

    public bool startActive = false;

    [Header("Appearance")]
    public string interactableName = "Placeholder";
    public string tooltip = "";
    public Color textColor = Color.white;
    public float textMeshFadeMultiplier = 4;
    public bool textTowardsCamera = false;
    public Material idleMaterial;
    public Material hoveredMaterial;
    public Material pressedMaterial;

    [Header("Interaction")]
    public bool touchActivates;
    public InteractableType interactableType;
    public InteractableEvent onInteractStart;
    public InteractableEvent onInteractEnd;
    public AudioClip useSound;

    public bool pressed {get; private set;}

    protected GameObject textMeshObject;
    protected TextMesh textMesh;
    protected MeshRenderer meshRenderer;
    protected MeshFilter meshFilter;
    private float lastActive = 0;
    private float textMeshAlpha = 0;
    private bool checkedStartActive = false;
    private float textMeshDistance = 0;
    private Vector3 originalTextPosition;
    private float lastCollisionTrigger = 0;

    private GameObject hoverLightObject;
    private Light hoverLight;
    
    protected virtual void Start()
    {
        pressed = false;
        meshRenderer = GetComponent<MeshRenderer>();
        meshFilter = GetComponent<MeshFilter>();
        Transform textMeshTransform = transform.Find("TextMesh");
        if (textMeshTransform)
        {
            textMeshObject = textMeshTransform.gameObject;
            textMesh = textMeshObject.GetComponent<TextMesh>();

            originalTextPosition = textMeshObject.transform.position - transform.position;
        }
        else
        {
            textMeshObject = new GameObject("TextMesh");
            textMeshObject.transform.parent = transform;
            textMeshObject.transform.position = transform.position + new Vector3(0, 0.7f, 0);
            textMesh = textMeshObject.AddComponent<TextMesh>();
            textMesh.characterSize = 0.05f;
            textMesh.fontSize = 70;
            textMesh.alignment = TextAlignment.Center;
            textMesh.anchor = TextAnchor.MiddleCenter;
        }

        textMeshDistance = Vector3.Distance(transform.position, textMeshObject.transform.position);
        originalTextPosition = textMeshObject.transform.position - transform.position;

        int layer = LayerMask.NameToLayer("Interactable");
        gameObject.layer = layer;
        for (int i = 0; i < transform.childCount; i++)
        {
            transform.GetChild(i).gameObject.layer = layer;
        }

        hoverLightObject = new GameObject("HoverLight");
        hoverLightObject.transform.parent = transform;
        hoverLight = hoverLightObject.AddComponent<Light>();
        hoverLight.range = 2f;
        hoverLight.color = new Color(255, 255, 200);
        hoverLight.shadows = LightShadows.Hard;
        hoverLight.renderMode = LightRenderMode.ForcePixel;
    }

    protected virtual void Update()
    {
        if (!checkedStartActive)
        {
            checkedStartActive = true;
            if (startActive)
            {
                DoInteractStart(null);
            }
        }

        if(hoverLightObject != null)
        {
            hoverLightObject.transform.position = transform.position + new Vector3(0, 1, 0);
        }

        if (textMeshObject)
        {
            if (textTowardsCamera)
            {
                textMeshObject.transform.position = transform.position + originalTextPosition + ((Camera.main.gameObject.transform.position - transform.position).normalized);
            }
            else
            {
                textMeshObject.transform.position = transform.position + (Vector3.up * textMeshDistance);
            }
        }

        bool hovered = IsHovered();

        if (hoverLight != null)
        {
            if (hovered)
            {
                hoverLight.intensity = 0.02f;
                hoverLight.enabled = true;
            }
            else
            {
                hoverLight.intensity = 0;
                hoverLight.enabled = false;
            }
        }

        if (InventoryUI.instance == null || !InventoryUI.instance.inventoryOpen)
        if(hovered && Input.GetMouseButtonDown(0))
        {
            switch (interactableType) {
                case InteractableType.PRESS:
                    DoInteractStart(GameManager.instance.playerObject);
                    DoInteractEnd(GameManager.instance.playerObject);
                    break;
                case InteractableType.TOGGLE:
                    if (pressed)
                    {
                        DoInteractEnd(GameManager.instance.playerObject);
                    }
                    else
                    {
                        DoInteractStart(GameManager.instance.playerObject);
                    }

                    break;
                case InteractableType.HOLD:
                    if (Input.GetMouseButton(0))
                    {
                        if (!pressed)
                        {
                            DoInteractStart(GameManager.instance.playerObject);
                        }
                    }
                    break;
            }
        }else if((!hovered || !Input.GetMouseButton(0)) && interactableType == InteractableType.HOLD)
        {
            if (pressed)
            {
                DoInteractEnd(GameManager.instance.playerObject);
            }
        }



        if (pressed || Time.time - lastActive < 0.3)
        {
            SetMaterial(pressedMaterial);
        }
        else
        {
            if (hovered)
            {
                SetMaterial(hoveredMaterial);
            }
            else
            {
                SetMaterial(idleMaterial);
            }
        }

        if (textMeshObject)
        {
            Vector3 forward = -(Camera.main.transform.position - transform.position).normalized;

            textMeshObject.transform.rotation = Quaternion.LookRotation(forward, Vector3.up);
        }

        if (textMesh)
        {
            string text = interactableName;
            if(GetHoverText(GameManager.instance.playerObject) != "")
            {
                text = GetHoverText(GameManager.instance.playerObject);
            }

            textMesh.text = text;

            if (hovered && GameManager.instance.playerObject != null)
            {
                textMeshAlpha = Mathf.Min(textMeshAlpha + (Time.deltaTime * textMeshFadeMultiplier), 1);
            }
            else
            {
                textMeshAlpha = Mathf.Max(textMeshAlpha - (Time.deltaTime * textMeshFadeMultiplier), 0);
            }

            textMesh.color = new Color(textColor.r, textColor.g, textColor.b, textMeshAlpha);
        }
    }

    bool IsPressed()
    {
        return pressed;
    }

    bool IsHovered()
    {
        RaycastHit hit;
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        Debug.DrawLine(ray.origin, ray.direction * 100);

        int layerMask = 1 << LayerMask.NameToLayer("Interactable");
        float extents = 0.2f;

        if(Physics.BoxCast(ray.origin,new Vector3(extents, extents, extents),ray.direction, out hit, Quaternion.identity, 100f, layerMask))
        {
            Transform objectHit = hit.transform;

            if(objectHit == gameObject.transform || objectHit.transform.parent == gameObject.transform)
            {
                return true;
            }
        }

        return false;
    }

    void DoInteractStart(GameObject invokerObject)
    {
        lastActive = Time.time;

        if(useSound != null)
        {
            AudioPlayer.instance.PlaySound(useSound, 0.6f, Random.Range(0.9f, 1.1f));
        }

        OnInteractableStart(invokerObject);
        onInteractStart.Invoke(invokerObject);

        pressed = true;
    }

    public float GetLastActive()
    {
        return lastActive;
    }

    void DoInteractEnd(GameObject invokerObject)
    {
        OnInteractableEnd(invokerObject);
        onInteractEnd.Invoke(invokerObject);

        pressed = false;
    }

    protected virtual void SetMaterial(Material material)
    {
        if (!meshRenderer || !material) { return; }

        meshRenderer.sharedMaterial = material;
    }

    protected virtual void OnInteractableStart(GameObject invokerObject)
    {

    }

    protected virtual void OnInteractableEnd(GameObject invokerObject)
    {

    }

    protected virtual string GetHoverText(GameObject invokerObject)
    {
        return "";
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (!touchActivates) { return; }
        if(Time.time - lastCollisionTrigger < 1) { return; }
        GameObject collisionObject = collision.rigidbody.gameObject;
        if(collisionObject != GameManager.instance.playerObject) { return; }

        DoInteractStart(collisionObject);
        lastCollisionTrigger = Time.time;
    }
}
