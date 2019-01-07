using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Character : MonoBehaviour {

    public enum CameraMovementType { NONE, QE, MOUSE }

    [Header("Movement")]
    public float movementForce = 10;
    public float cameraHeight = 10;
    public float movementDampen = 4;

    [Header("Camera")]
    public CameraMovementType cameraMovementType = CameraMovementType.QE;
    public float cameraSensitivityYaw = 60;
    public float cameraSensitivityPitch = 60;
    public float cameraYaw = 0;
    public float cameraPitch = 40;
    public float minCameraPitch = 20;
    public float maxCameraPitch = 80;
    public float originDampening = 0.2f;

    private Vector3 targetCameraLocation;
    private Camera camera;
    private Rigidbody rb;
    private float velocityX = 0;
    private float velocityZ = 0;
    private Vector3 currentOrigin;

	// Use this for initialization
	void Start () {
        rb = GetComponent<Rigidbody>();
        camera = Camera.main;
        camera.transform.parent = transform;

        currentOrigin = transform.position;

    }
	
	// Update is called once per frame
	void Update () {
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

        UpdateCamera();

        float lightIntensity = LightUtil.instance.SampleLightIntensity(transform.position + new Vector3(0, 0.5f, 0));
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
            Vector3 relativePosition = transform.position - targetCameraLocation;
            relativePosition.y = 0;
            Vector3 right = -Vector3.Cross(relativePosition.normalized, Vector3.up);

            velocityX = Mathf.Clamp(horizontal, -1, 1);
            //rb.AddForce(right * horizontal * movementForce);
        }
        else
        {
            velocityX = Mathf.MoveTowards(velocityX, 0, Time.deltaTime * movementDampen);
        }

        if (vertical != 0)
        {
            Vector3 relativePosition = transform.position - targetCameraLocation;
            relativePosition.y = 0;
            Vector3 forward = relativePosition.normalized;

            velocityZ = Mathf.Clamp(vertical, -1, 1);
            //rb.AddForce(forward * vertical * movementForce);
            rb.AddForce(forward * vertical * movementForce);
        }
        else
        {
            velocityZ = Mathf.MoveTowards(velocityZ, 0, (Time.deltaTime * movementDampen));
        }

        Vector3 direction = new Vector3(velocityX, 0, velocityZ).normalized;

        Vector3 vel = rb.velocity;
        vel.x = Mathf.Abs(velocityX) * direction.x * movementForce;
        vel.z = Mathf.Abs(velocityZ) * direction.z * movementForce;
        rb.velocity = vel;
    }
}
