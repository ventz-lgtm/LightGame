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

	// Use this for initialization
	void Start () {
        Transform doorTransform = transform.Find("Door");
        if (doorTransform)
        {
            doorGameObject = doorTransform.gameObject;
            doorMeshRenderer = doorGameObject.GetComponent<MeshRenderer>();

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
                float diff = 90 - yaw;
                Quaternion rotation = Quaternion.Euler(0, diff * doorDampen, 0);
                doorGameObject.transform.rotation = doorGameObject.transform.rotation * rotation;
            }
            else
            {
                float diff = -yaw;
                Quaternion rotation = Quaternion.Euler(0, diff * doorDampen, 0);
                doorGameObject.transform.rotation = doorGameObject.transform.rotation * rotation;
            }

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

        ToggleDoor();
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

    public void OpenDoor()
    {
        lastDoorOpen = Time.time;
        doorOpen = true;
    }

    public void CloseDoor()
    {
        doorOpen = false;
    }

    public void ToggleDoor()
    {
        if (doorOpen)
        {
            CloseDoor();
        }
        else
        {
            OpenDoor();
        }
    }
}
