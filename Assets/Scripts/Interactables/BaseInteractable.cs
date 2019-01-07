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
    public Material idleMaterial;
    public Material hoveredMaterial;
    public Material pressedMaterial;

    [Header("Interaction")]
    public InteractableType interactableType;
    public InteractableEvent onInteractStart;
    public InteractableEvent onInteractEnd;

    public bool pressed {get; private set;}

    protected GameObject textMeshObject;
    protected TextMesh textMesh;
    protected MeshRenderer meshRenderer;
    protected MeshFilter meshFilter;
    private float lastActive = 0;
    private float textMeshAlpha = 0;
    private bool checkedStartActive = false;

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
        }
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

        bool hovered = IsHovered();

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

            transform.rotation = Quaternion.LookRotation(forward, Vector3.up);
        }

        if (textMesh)
        {
            textMesh.text = interactableName;

            if (hovered)
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

        if (Physics.Raycast(ray, out hit))
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

        OnInteractableStart(invokerObject);
        onInteractStart.Invoke(invokerObject);

        pressed = true;
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
}
