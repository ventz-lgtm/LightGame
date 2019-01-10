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
    }
}
