using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hint : MonoBehaviour {

    [TextArea]
    public string text = "Placeholder";
    public string title = "Hint";
    public float hintTime = 6;
    public bool showHint = true;
    [TextArea]
    public string objective = "";

    private bool triggered = false;

    private void OnTriggerEnter(Collider other)
    {
        if(showHint && !triggered && other.gameObject == GameManager.instance.playerObject)
        {
            GameManager.instance.ShowHint(text, title);
        }

        if(!triggered && objective != "" && ObjectiveUI.instance != null && other.gameObject == GameManager.instance.playerObject)
        {
            ObjectiveUI.instance.SetObjective(objective);
        }

        triggered = true;
    }
}
