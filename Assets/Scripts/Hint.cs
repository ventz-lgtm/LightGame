using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hint : MonoBehaviour {

    [TextArea]
    public string text = "Placeholder";
    public string title = "Hint";
    public float hintTime = 6;

    private bool triggered = false;

    private void OnTriggerEnter(Collider other)
    {
        if(!triggered && other.gameObject == GameManager.instance.playerObject)
        {
            triggered = true;
            GameManager.instance.ShowHint(text, title);
        }
    }
}
