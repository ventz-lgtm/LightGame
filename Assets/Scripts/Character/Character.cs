using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Character : MonoBehaviour {

    public float movementForce = 10;
    public float cameraHeight = 10;

    public float cameraYaw = 0;
    public float cameraPitch = 10;
    public float maxCameraPitch = 45;

    private Camera camera;
    private Rigidbody rb;

	// Use this for initialization
	void Start () {
        rb = GetComponent<Rigidbody>();
        camera = Camera.main;
	}
	
	// Update is called once per frame
	void Update () {
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");

        if (horizontal != 0)
        {
            rb.AddForce(transform.right * horizontal * movementForce);
        }

        if(vertical != 0)
        {
            rb.AddForce(transform.forward * vertical * movementForce);
        }
	}

    void UpdateCamera()
    {
        if(camera != null)
        {
            Vector3 cameraPosition = transform.position;
            Quaternion rotation = Quaternion.Euler(cameraPitch, cameraYaw, 0);

            float pitchPercentage = Mathf.Clamp(cameraPitch / maxCameraPitch, 0, 1);
            float distance = Mathf.Max(1, cameraHeight * pitchPercentage);

            // cameraPosition += rotation
        }
    }
}
