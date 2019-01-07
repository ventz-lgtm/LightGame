using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ChopTree : BaseInteractable {

    public Image treeProgression;
    private Image treeInstance;

    Canvas m_Canvas;
    Vector3 pos;

    float chopTime;

    private bool chopping;
    private bool chopped;


    // Use this for initialization
    protected override void Start () {
        base.Start();
        m_Canvas = Camera.main.GetComponentInChildren<Canvas>();
        chopping = false;
    }

    // Update is called once per frame
    protected override void Update()
    {
        base.Update();

        if (chopping)
        {
            treeInstance.fillAmount = Mathf.Clamp01(chopTime - Time.time);
            if(treeInstance.fillAmount == 0)
            {
                Destroy(gameObject);
            }
        }
    }

    public void Chop()
    {
        
        chopTime = Time.time + 1.0f;
        pos =Input.mousePosition;
        treeInstance = Instantiate(treeProgression, m_Canvas.transform);
        treeInstance.transform.position = pos;
        chopping = true;
    } 

    public void ChopEnd()
    {
        chopping = false;
        treeInstance.fillAmount = 1.0f;
        Destroy(treeInstance);
    }
}
