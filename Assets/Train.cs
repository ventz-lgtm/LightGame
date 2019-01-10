using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Train : MonoBehaviour {

    public static Train instance;

    public Vector3 trainMovement;
    public bool moving { get; private set; }

    private float velocity = 0;

    private void Awake()
    {
        instance = this;
    }

    private void Update()
    {
        if (moving)
        {
            velocity = Mathf.Min(5, velocity + (Time.deltaTime * 0.8f));
            transform.position += trainMovement * velocity * Time.deltaTime;
            GameManager.instance.playerObject.transform.position += trainMovement * velocity * Time.deltaTime;
        }
    }

    public void StartTrain()
    {
        if (false && !IsPowered())
        {
            GameManager.instance.Notify("Train is not powered!", 3f);
            return;
        }

        moving = true;
    }

    public bool IsPowered()
    {
        return TrainControlBox.instance.FullyPowered();
    }
}
