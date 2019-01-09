using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotatingDoor : BaseInteractable {

    public float doorDampen = 0.2f;
    public float autoCloseDuration = 0;

    public bool doorOpen { get; private set; }

    private GameObject doorGameObject;
    private MeshRenderer doorMeshRenderer;
    private float doorDistance;
    private float lastDoorOpen = 0;
    private bool rotateBackwards = false;
    private float currentYaw = 0;
    private float initialYaw = 0;

	// Use this for initialization
	protected override void Start () {
        base.Start();

        Transform doorTransform = transform.Find("Door");
        if (doorTransform)
        {
            doorGameObject = doorTransform.gameObject;
            doorMeshRenderer = doorGameObject.GetComponent<MeshRenderer>();
            Vector3 rotation = doorGameObject.transform.rotation.eulerAngles;
            initialYaw = rotation.y;

            doorDistance = Vector3.Distance(transform.position, doorGameObject.transform.position);
        }
	}
	
	// Update is called once per frame
	protected override void Update () {
        base.Update();

        if (doorGameObject)
        {
            Vector3 euler = doorGameObject.transform.rotation.eulerAngles;
            float yaw = euler.y;

            if (doorOpen)
            {
                float diff = 90 - currentYaw;
                currentYaw += diff * doorDampen * Time.deltaTime * 30;

                Quaternion rotation = Quaternion.Euler(0, (rotateBackwards ? -currentYaw : currentYaw) + initialYaw, 0);
                doorGameObject.transform.rotation = rotation;
            }
            else
            {
                float diff = currentYaw;
                currentYaw -= diff * doorDampen * Time.deltaTime * 30;

                Quaternion rotation = Quaternion.Euler(0, (rotateBackwards ? -currentYaw : currentYaw) + initialYaw, 0);
                doorGameObject.transform.rotation = rotation;
            }

            currentYaw = Mathf.Clamp(currentYaw, 0f, 90f);

            Vector3 forward = doorGameObject.transform.rotation * Vector3.forward;
            doorGameObject.transform.position = transform.position + (forward * doorDistance);
        }

        if(doorOpen && autoCloseDuration > 0)
        {
            if(Time.time - lastDoorOpen > autoCloseDuration)
            {
                CloseDoor();
            }
        }

    }

    protected override void OnInteractableStart(GameObject invokerObject)
    {
        base.OnInteractableStart(invokerObject);

        ToggleDoor(invokerObject);
    }

    protected override void OnInteractableEnd(GameObject invokerObject)
    {
        base.OnInteractableEnd(invokerObject);
    }

    protected override void SetMaterial(Material material)
    {
        base.SetMaterial(material);

        if (doorMeshRenderer)
        {
            doorMeshRenderer.sharedMaterial = material;
        }
    }

    public void OpenDoor(GameObject invokerObject)
    {
        if(invokerObject != null)
        {
            Vector3 rPos = (invokerObject.transform.position - transform.position).normalized;
            float dot = Vector3.Dot(rPos, transform.forward);
            rotateBackwards = dot < 0;
        }
        
        lastDoorOpen = Time.time;
        doorOpen = true;
    }

    public void CloseDoor()
    {
        doorOpen = false;
    }

    public void ToggleDoor(GameObject invokerObject = null)
    {
        if (doorOpen)
        {
            CloseDoor();
        }
        else
        {
            OpenDoor(invokerObject);
        }
    }
}
