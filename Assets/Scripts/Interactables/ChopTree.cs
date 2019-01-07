using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ChopTree : BaseInteractable {

    public Image treeProgression;
    Canvas m_Canvas = Camera.main.GetComponentInChildren<Canvas>();
    Vector3 pos;

    float completePercent;

    // Use this for initialization
    protected override void Start () {
        base.Start();
        m_Canvas = Camera.main.GetComponentInChildren<Canvas>();
        
    }

    // Update is called once per frame
    protected override void Update()
    {
        base.Update();
    }

    public void Chop()
    {
        
        pos =Input.mousePosition;
        treeProgression = Instantiate(treeProgression,m_Canvas.transform);
        treeProgression.transform.position = pos;
    }

}
