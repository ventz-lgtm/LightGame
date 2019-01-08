using UnityEngine;
using System.Collections;

public class InfoRenderer : MonoBehaviour
{
    float deltaTime = 0.0f;

    void Update()
    {
        deltaTime += (Time.unscaledDeltaTime - deltaTime) * 0.1f;
    }

    void OnGUI()
    {
        int w = Screen.width, h = Screen.height;

        GUIStyle style = new GUIStyle();

        Rect rect = new Rect(0, 0, w, h * 2 / 100);
        style.alignment = TextAnchor.UpperLeft;
        style.fontSize = h * 2 / 100;
        style.normal.textColor = new Color(1f, 1f, 1f, 1.0f);
        float msec = deltaTime * 1000.0f;
        float fps = 1.0f / deltaTime;
        string text = string.Format("{0:0.0} ms ({1:0.} fps)", msec, fps);
        GUI.Label(rect, text, style);

        Character character = GameManager.instance.playerObject.GetComponent<Character>();
        if (character)
        {
            rect = new Rect(0, 25, w, (h * 2 / 100));
            text = string.Format("Light: {0}", character.GetLightIntensity());
            GUI.Label(rect, text, style);

            rect = new Rect(0, 50, w, (h * 2 / 100));
            text = string.Format("Danger: {0}", GameManager.instance.dangerLevel);
            GUI.Label(rect, text, style);

            rect = new Rect(0, 75, w, (h * 2 / 100));
            text = string.Format("Sanity: {0}", GameManager.instance.sanity);
            GUI.Label(rect, text, style);
        }
    }
}