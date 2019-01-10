using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrainControlBox : MonoBehaviour {

    public Material offMaterial;
    public Material onMaterial;

    private MeshRenderer[] buttonRenderers;
    private Generator[] generators;
    private bool foundGenerators = false;

    private void Start()
    {
        buttonRenderers = new MeshRenderer[4];
        for(int i = 0; i < 4; i++)
        {
            buttonRenderers[i] = transform.Find("Light" + i).gameObject.GetComponent<MeshRenderer>();
        }
    }

    private void Update()
    {
        if (!foundGenerators)
        {
            foundGenerators = true;

            generators = FindObjectsOfType<Generator>();
        }

        for(int i = 0; i < 4; i++)
        {
            if (IsGeneratorPowered(i))
            {
                buttonRenderers[i].sharedMaterial = onMaterial;
            }
            else
            {
                buttonRenderers[i].sharedMaterial = offMaterial;
            }
        }
    }

    public bool IsGeneratorPowered(int index)
    {
        if(index >= 4) { return false; }
        if(index >= generators.Length) { return false; }

        Generator g = generators[index];
        return g.IsPowered();
    }
}
