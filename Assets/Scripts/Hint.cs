using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hint : MonoBehaviour {

    public string text = "Hint";
    public float hintTime = 6;

    private bool triggered = false;

    private void OnTriggerEnter(Collider other)
    {
        if(!triggered && other.gameObject == GameManager.instance.playerObject)
        {
            triggered = true;
            GameManager.instance.Notify(text, hintTime);
        }
    }
}
