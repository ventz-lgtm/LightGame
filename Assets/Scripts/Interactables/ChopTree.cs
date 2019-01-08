using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ChopTree : BaseInteractable {

    public Image treeProgression;
    private Image treeInstance;

    public GameObject treeStump;
    public GameObject woodPile;

    Canvas m_Canvas;
    Vector3 pos;
    Vector3 treePosition;
    public Vector3 groundHeight;
    float chopTime;
    public int chopDuration;
    private bool chopping;
    private bool chopped;


    // Use this for initialization
    protected override void Start () {
        base.Start();
        m_Canvas = Camera.main.GetComponentInChildren<Canvas>();
        chopping = false;
        treePosition = gameObject.transform.position;
    }

    // Update is called once per frame
    protected override void Update()
    {
        base.Update();

        if (chopping)
        {
            treeInstance.fillAmount = Mathf.Lerp(1.0f, 0.0f, ((Time.time - chopTime) / chopDuration));
            if(treeInstance.fillAmount == 0)
            {
                
                Destroy(gameObject);
                Instantiate(treeStump, treePosition + groundHeight, treeStump.transform.rotation);
                Instantiate(woodPile, treePosition + new Vector3(0, 0, 1) + groundHeight, woodPile.transform.rotation);
                ChopEnd();
            }
        }
    }

    public void Chop()
    {
        
        chopTime = Time.time;
        pos = Input.mousePosition;
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
