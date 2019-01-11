using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Train : MonoBehaviour {

    public static Train instance;

    public Vector3 trainMovement;
    public bool moving { get; private set; }

    private float velocity = 0;
    private float fadeIntensity = 0;
    private Image fadeImage;
    private bool restarted = false;
    private float fadeSince = 0;

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        fadeImage = Camera.main.transform.Find("Canvas").Find("Fade").GetComponent<Image>();
    }

    private void Update()
    {
        if (moving)
        {
            if (!restarted)
            {
                velocity = Mathf.Min(5, velocity + (Time.deltaTime * 0.8f));
            }
            
            transform.position += trainMovement * velocity * Time.deltaTime;
            GameManager.instance.playerObject.transform.position += trainMovement * velocity * Time.deltaTime;

            if(GameManager.instance.playerObject.transform.position.z < -20)
            {
                fadeIntensity = Mathf.Min(1, fadeIntensity + Time.deltaTime);
            }
            else
            {
                if(Time.time - fadeSince > 5)
                {
                    fadeIntensity = Mathf.Max(0, fadeIntensity - Time.deltaTime);
                }
            }

            if (restarted)
            {
                if(transform.position.z < 90)
                {
                    velocity = Mathf.Max(0, velocity - (Time.deltaTime * 0.8f));
                    if(velocity <= 0)
                    {
                        restarted = false;
                        moving = false;
                    }
                }
            }
        }

        if (fadeImage != null)
        {
            if (fadeIntensity > 0)
            {
                if (!fadeImage.gameObject.activeSelf)
                {
                    fadeImage.gameObject.SetActive(true);
                }

                Color color = fadeImage.color;
                color.a = fadeIntensity;
                fadeImage.color = color;

                if (fadeIntensity >= 1 && !restarted)
                {
                    gameObject.transform.position = new Vector3(transform.position.x, transform.position.y, transform.position.z + 180);
                    Vector3 playerPos = GameManager.instance.playerObject.transform.position;
                    GameManager.instance.playerObject.transform.position = new Vector3(playerPos.x, playerPos.y, playerPos.z + 180);
                    restarted = true;
                    fadeSince = Time.time;
                }
            }
            else
            {
                if (fadeImage.gameObject.activeSelf)
                {
                    fadeImage.gameObject.SetActive(false);
                }
            }
        }
    }

    public void StartTrain()
    {
        if (!IsPowered())
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
