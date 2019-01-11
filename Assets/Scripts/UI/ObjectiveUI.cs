using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ObjectiveUI : MonoBehaviour {

    public static ObjectiveUI instance;

    private Text text;
    
    private void Awake()
    {
        instance = this;
        text = transform.Find("Text").GetComponent<Text>();
    }

    public void SetObjective(string message)
    {
        text.text = message;

        TextGenerator textGen = new TextGenerator();
        TextGenerationSettings generationSettings = text.GetGenerationSettings(text.rectTransform.rect.size);

        float height = textGen.GetPreferredHeight(message, generationSettings);

        RectTransform transform = GetComponent<RectTransform>();
        transform.sizeDelta = new Vector2(transform.sizeDelta.x, height + 50);
    }
}
