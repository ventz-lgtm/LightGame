﻿using System.Collections;
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
        return SampleLightIntensity(location, true);
    }

    public float SampleLightIntensity(Vector3 location, GameObject traceObject)
    {
        return SampleLightIntensity(location, true, traceObject);
    }

    public float SampleLightIntensity(Vector3 location, bool useTrace, GameObject traceObject = null)
    {
        FindLights();

        float intensity = 0;

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
                        if (LightHit(lightPosition, location, useTrace, traceObject))
                        {
                            intensity += GetLightIntensity(distance, range, l.intensity);
                        }
                        break;
                    case LightType.Spot:
                        float dot = (1 - Vector3.Dot(relativePosition.normalized, l.transform.forward)) * 180 * 1.5f;
                        float dotLimit = l.spotAngle;

                        if(dot < dotLimit && dot > -dotLimit)
                        {
                            if (LightHit(lightPosition, location, useTrace, traceObject))
                            {
                                intensity += GetLightIntensity(distance, range, l.intensity) * 1.8f;
                            }
                        }
                        break;
                }
                
            }
        }

        return Mathf.Clamp(intensity, 0, 1);
    }

    public bool LightHit(Vector3 lightPosition, Vector3 location, bool useTrace, GameObject traceObject)
    {
        if (!useTrace)
        {
            return true;
        }

        Vector3 relativePosition = location - lightPosition;
        float range = relativePosition.magnitude;
        RaycastHit hit;
        int layerMask = 0;
        layerMask = ~layerMask;

        if (Physics.Raycast(lightPosition, relativePosition.normalized, out hit, range, layerMask))
        {
            if(traceObject == null) { return false; }
            if(traceObject != hit.collider.gameObject && hit.collider.gameObject.transform.parent != transform.transform) { return false; }

            return true;
        }

        return true;
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
