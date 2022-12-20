using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeadGazeTarget : MonoBehaviour
{
    public List<Transform> trackableObjects = null;
    private readonly float threshold = 1f;
    private void Start()
    {
        trackableObjects = new();
        foreach (Transform item in GameObject.Find("TrackableObjects").transform)
        {
            trackableObjects.Add(item);
        }
    }

    public Quaternion GetTrackObjViewVector(Vector3 nose, Vector3 v, Vector3 antLeft, Vector3 antRight)
    {
        nose.Normalize();
        float minValue = (float)int.MaxValue;
        Vector3 selectedVector = new();
        foreach (Transform item in trackableObjects)
        {
            Vector3 antVector = (item.position - antLeft + item.position - antRight) / 2;
            antVector.Normalize();
            float value = GetENorm(nose, antVector);
            if (value < minValue)
            {
                minValue = value;
                selectedVector = antVector;
            }
        }
        Debug.Log(minValue);
        if (minValue < threshold)
        {
            return Quaternion.LookRotation(selectedVector, v);
        }
        else
        {
            return Quaternion.LookRotation(nose, v);
        }
    }

    private float GetENorm(Vector3 v1, Vector3 v2)
    {
        return Mathf.Sqrt((v1.x - v2.x) * (v1.x - v2.x) + (v1.y - v2.y) * (v1.y - v2.y) + (v1.z - v2.z) * (v1.z - v2.z));
    }
}
