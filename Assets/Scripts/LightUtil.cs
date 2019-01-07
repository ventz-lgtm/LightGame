using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightUtil : MonoBehaviour {

    public static LightUtil instance;

    private bool lightsFound = false;
    private Light[] lights;


    private void Start()
    {
        instance = this;
    }

    public float SampleLightIntensity(Vector3 location)
    {
        FindLights();

        float intensity = 0;
        RaycastHit hit;

        foreach (Light l in lights)
        {
            if (l.gameObject.activeSelf)
            {
                Vector3 lightPosition = l.gameObject.transform.position;
                float distance = Vector3.Distance(location, lightPosition);
                float range = l.range;
                if (distance > range) { continue; }

                Vector3 relativePosition = location - lightPosition;

                int layerMask = 0;
                layerMask = ~layerMask;

                switch (l.type)
                {
                    case LightType.Point:
                        if (Physics.Raycast(lightPosition, relativePosition.normalized, out hit, range, layerMask))
                        {
                            intensity += GetLightIntensity(distance, range, l.intensity);
                        }
                        break;
                    case LightType.Spot:
                        float dot = (1 - Vector3.Dot(relativePosition.normalized, l.transform.forward)) * 180 * 4;
                        float dotLimit = l.spotAngle;

                        if(dot < dotLimit && dot > -dotLimit)
                        {
                            if (Physics.Raycast(lightPosition, relativePosition.normalized, out hit, range, layerMask))
                            {
                                intensity += GetLightIntensity(distance, range, l.intensity);
                            }
                        }
                        break;
                }
                
            }
        }

        return Mathf.Clamp(intensity, 0, 1);
    }

    public float GetLightIntensity(float distance, float range, float intensity)
    {
        float value = 1 - (distance / range);

        //return Mathf.Clamp(value + ((1 - value) * intensity * 0.3f), 0, 1);
        value *= Mathf.Clamp(intensity, 0f, 1f);

        if(value < 0.7f)
        {
            value = value * value;
        }

        return value;
    }

    public void FindLights()
    {
        if (lightsFound) { return; }

        lights = FindObjectsOfType(typeof(Light)) as Light[];
    }
}
