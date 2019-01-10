using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Character : MonoBehaviour {

    public enum CameraMovementType { NONE, QE, MOUSE }

    [Header("Movement")]
    public float movementForce = 10;
    public float cameraHeight = 10;
    public float movementDampen = 4;
    public GameObject visualObject;

    [Header("Camera")]
    public CameraMovementType cameraMovementType = CameraMovementType.QE;
    public float cameraSensitivityYaw = 60;
    public float cameraSensitivityPitch = 60;
    public float cameraYaw = 0;
    public float cameraPitch = 40;
    public float minCameraPitch = 20;
    public float maxCameraPitch = 80;
    public float originDampening = 0.2f;

    [Header("Items")]
    public int maxFlares = 5;
    public GameObject flarePrefab;
    public int inventorySize = 12;

    public Action onInventoryChanged;
    public List<InventoryItemType> inventoryItems { get; private set; }
    public int flareCount { get; protected set; }
    private Vector3 targetCameraLocation;
    private Camera camera;
    private Rigidbody rb;
    private float velocityX = 0;
    private float velocityZ = 0;
    private Vector3 currentOrigin;
    private Vector3 movementDirection = Vector3.forward;
    private Vector3 currentMovementDirection = Vector3.forward;
    private float lastHintCheck = 0;
    private bool darknessHintDone = false;

    AudioSource footStepSound;

    Animator playerAnimation;
    AnimatorControllerParameter isRunning;

    private void Awake()
    {
        inventoryItems = new List<InventoryItemType>();
    }

    // Use this for initialization
    void Start () {
        rb = GetComponent<Rigidbody>();
        camera = Camera.main;
        camera.transform.parent = transform;

        currentOrigin = transform.position;
        playerAnimation = transform.Find("Visual").transform.Find("Mesh").GetComponent<Animator>();
        footStepSound = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update() {
        currentOrigin += (transform.position - currentOrigin) * originDampening;

        Debug.DrawLine(transform.position, currentOrigin);

        switch (cameraMovementType)
        {
            case CameraMovementType.QE:
                HandleQECameraMovement();
                break;
            case CameraMovementType.MOUSE:
                HandleMouseCameraMovement();
                break;
        }

        if(Time.time - lastHintCheck > 2)
        {
            lastHintCheck = Time.time;

            float intensity = GetLightIntensity();

            if (!darknessHintDone)
            {
                if (intensity < 0.2f)
                {
                    Debug.Log("hint");
                    darknessHintDone = true;
                    GameManager.instance.ShowHint("Stay in the light.\n\nWandering into darkness reduces your sanity and exposes you to monsters!", "The Darkness");
                }
            }
        }        

        UpdateCamera();
        UpdateVisibleObject();

        if (velocityX > 0.1f || velocityZ > 0.1f || velocityX < -0.1f || velocityZ < -0.1f)
        {
            if (!footStepSound.isPlaying)
            {
                footStepSound.Play();
            }

            playerAnimation.SetBool("isRunning", true);

        }
        else
        {
            if (footStepSound.isPlaying)
            {
                footStepSound.Stop();
            }

            playerAnimation.SetBool("isRunning", false);
        }
	}


    private void FixedUpdate()
    {
        HandleMovement();
    }

    void UpdateCamera()
    {
        if(camera != null)
        {
            targetCameraLocation = transform.position;
            Quaternion rotation = Quaternion.Euler(90 - cameraPitch, cameraYaw, 0);

            float pitchPercentage = 1 - Mathf.Clamp(cameraPitch / maxCameraPitch, 0, 1);

            Vector3 cameraForward = rotation * Vector3.forward;

            targetCameraLocation -= (cameraForward * cameraHeight);


            Vector3 relativePosition = targetCameraLocation - camera.transform.position;

            camera.transform.position += (relativePosition * 0.3f);
            camera.transform.rotation = Quaternion.LookRotation((transform.position - camera.transform.position).normalized, Vector3.up);
        }
    }

    void UpdateVisibleObject()
    {
        if (visualObject)
        {
            Inventory inv = GetComponent<Inventory>();
            Transform transform = inv.GetHeldItem();
            GameObject heldObject = transform != null ? transform.gameObject : null;
            bool aiming = false;

            if (heldObject)
            {
                PickUpItem pickup = heldObject.GetComponent<PickUpItem>();
                if (pickup && pickup.aimable && pickup.ShouldAim())
                {
                    aiming = true;
                }
            }

            if (aiming)
            {
                // Face towards the mouse for holdables which should be aimed

                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

                Vector3 endPos = ray.origin + (ray.direction * Vector3.Distance(Camera.main.gameObject.transform.position, visualObject.transform.position));
                Vector3 rPos = endPos - visualObject.transform.position;
                rPos.y = 0;
                rPos.Normalize();
                visualObject.transform.rotation = Quaternion.LookRotation(rPos, Vector3.up);
            }
            else {
                // Face in direction of movement

                Vector3 direction = new Vector3(movementDirection.x, 0, movementDirection.z);

                if (direction != Vector3.zero)
                {
                    Vector3 diff = direction - currentMovementDirection;
                    currentMovementDirection += (diff * 10f * Time.deltaTime);

                    visualObject.transform.rotation = Quaternion.LookRotation(currentMovementDirection, Vector3.up);
                }
            }
        }
    }

    void HandleMouseCameraMovement()
    {
        float mouseX = Input.GetAxis("Mouse X");
        float mouseY = Input.GetAxis("Mouse Y");

        if (mouseX != 0)
        {
            cameraYaw += (mouseX * Time.deltaTime * cameraSensitivityYaw);
        }

        if (mouseY != 0)
        {
            cameraPitch = Mathf.Clamp(cameraPitch + (mouseY * Time.deltaTime * cameraSensitivityPitch), 0, maxCameraPitch);
        }
    }

    void HandleQECameraMovement()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            cameraYaw += 45;
        }else if (Input.GetKeyDown(KeyCode.Q))
        {
            cameraYaw -= 45;
        }
    }

    void HandleMovement()
    {
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");

        if (horizontal != 0)
        {
            velocityX = Mathf.Clamp(horizontal, -1, 1);
        }
        else
        {
            velocityX = Mathf.MoveTowards(velocityX, 0, Time.deltaTime * movementDampen);
        }

        if (vertical != 0)
        {
            velocityZ = Mathf.Clamp(vertical, -1, 1);
        }
        else
        {
            velocityZ = Mathf.MoveTowards(velocityZ, 0, (Time.deltaTime * movementDampen));
        }

        Vector3 direction = new Vector3(velocityX, 0, velocityZ).normalized;

        Vector3 rPos = transform.position - camera.transform.position;
        rPos.y = 0;
        rPos.Normalize();

        Vector3 newVel = new Vector3(0, rb.velocity.y, 0);
        newVel -= Vector3.Cross(rPos, Vector3.up) * Mathf.Abs(velocityX) * direction.x * movementForce;
        newVel += rPos * Mathf.Abs(velocityZ) * direction.z * movementForce;
        rb.velocity = newVel;

        movementDirection = newVel.normalized;
    }

    public float GetLightIntensity()
    {
        float intensity = 0f;
        intensity = LightUtil.instance.SampleLightIntensity(transform.position + new Vector3(0, 0.5f, 0), gameObject);
        intensity = Mathf.Max(intensity, LightUtil.instance.SampleLightIntensity(transform.position + new Vector3(0, 0.1f, 0), gameObject));

        return intensity;
    }

    public bool PickupFlare()
    {
        if(flareCount >= maxFlares) { return false; }

        flareCount++;

        return true;
    }

    public bool ThrowFlare()
    {
        if(flareCount <= 0) { return false; }
        if(flarePrefab == null) { return false; }

        GameObject flare = Instantiate(flarePrefab);
        if(flare == null) { return false; }

        flare.transform.position = transform.position + (transform.forward * 0.5f);
        DroppedFlare flareComponent = flare.GetComponent<DroppedFlare>();
        flareComponent.Ignite();

        return true;
    }

    public bool InventoryPickup(InventoryItemType type)
    {
        if(inventoryItems.Count >= inventorySize) { return false; }

        inventoryItems.Add(type);

        if (onInventoryChanged != null)
        {
            onInventoryChanged();
        }

        return true;
    }

    public GameObject InventoryDrop(int index, bool dontDrop = false)
    {
        if(inventoryItems.Count <= index) { return null; }
        
        InventoryItemType item = inventoryItems[index];
        if (item == null) { return null; }

        GameObject droppedItem = item.instance;
        if (droppedItem == null) { return null; }
        
        if (dontDrop)
        {
            Destroy(droppedItem);
        }
        else
        {
            droppedItem.SetActive(true);
            droppedItem.transform.position = transform.position + (visualObject.transform.forward * 0.5f);
        }
        
        inventoryItems.RemoveAt(index);

        if (onInventoryChanged != null)
        {
            onInventoryChanged();
        }

        return droppedItem;
    }

    public GameObject InventoryDrop(string name, bool dontDrop = false)
    {
        for(int i = 0; i < inventoryItems.Count; i++)
        {
            InventoryItemType type = inventoryItems[i];
            
            if (type.name == name)
            {
                return InventoryDrop(i, dontDrop);
            }
        }

        return null;
    }

    public InventoryItemType InventoryItemAt(int index)
    {
        if(inventoryItems.Count <= index) { return null; }
        return inventoryItems[index];
    }

    public int GetInventoryItemCount(string name)
    {
        int count = 0;

        foreach (InventoryItemType type in inventoryItems)
        {
            if(type.name == name)
            {
                count++;
            }
        }

        return count;
    }
}
