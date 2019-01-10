using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HintPanel : MonoBehaviour {

    
    private Text title;
    private Text text;

    private void Awake()
    {
        title = transform.Find("Title").Find("Text").GetComponent<Text>();
        text = transform.Find("Text").GetComponent<Text>();
    }

    public void SetTitle(string titleText)
    {
        title.text = titleText;
    }

    public void SetText(string contents)
    {
        text.text = contents;

        TextGenerator textGen = new TextGenerator();
        TextGenerationSettings generationSettings = text.GetGenerationSettings(text.rectTransform.rect.size);

        float height = textGen.GetPreferredHeight(contents, generationSettings);

        RectTransform transform = GetComponent<RectTransform>();
        transform.sizeDelta = new Vector2(transform.sizeDelta.x, height + 100);
    }
}
