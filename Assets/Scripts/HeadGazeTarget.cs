using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeadGazeTarget : MonoBehaviour
{
    public List<Transform> trackableObjects = null;
    private readonly float threshold = 1f;
    private bool isTracking = false;
    private float timeElapsedTracking = 0f;

    private Vector3 Nose, SelectedVector, BlendVector;
    private void Start()
    {
        trackableObjects = new();
        foreach (Transform item in GameObject.Find("TrackableObjects").transform)
        {
            trackableObjects.Add(item);
        }
    }

    public Quaternion GetTrackObjViewVector(Vector3 nose, Vector3 v, Vector3 bodyV, Vector3 antLeft, Vector3 antRight, Vector3 neck)
    {
        Nose = nose.normalized;
        float minValue = int.MaxValue;
        SelectedVector = Vector3.zero;
        foreach (Transform item in trackableObjects)
        {
            Vector3 antVector = item.position - TriangleIncenter(antLeft, antRight, neck);
            antVector.Normalize();
            float value = EuclideanNorm(Nose, antVector);
            if (value < minValue)
            {
                minValue = value;
                SelectedVector = antVector;
            }
        }

        Debug.Log(minValue);

        if (minValue < threshold && EuclideanNorm(bodyV, SelectedVector) < threshold * 1.2f)
        {
            if (!isTracking)
            {
                isTracking = true;
            }
            timeElapsedTracking += Time.deltaTime * 5f;
        }
        else
        {
            if (isTracking)
            {
                isTracking = false;
            }
            timeElapsedTracking -= Time.deltaTime * 5f;
        }
        timeElapsedTracking = Mathf.Clamp(timeElapsedTracking, 0f, 5f);
        float sigValue = Sigmoid(timeElapsedTracking);
        BlendVector = SelectedVector * sigValue + Nose * (1f - sigValue);
        BlendVector.Normalize();
        return Quaternion.LookRotation(BlendVector, v);
    }

    private float EuclideanNorm(Vector3 v1, Vector3 v2)
    {
        return Mathf.Sqrt((v1.x - v2.x) * (v1.x - v2.x) + (v1.y - v2.y) * (v1.y - v2.y) + (v1.z - v2.z) * (v1.z - v2.z));
    }

    private float Sigmoid(float f)
    {
        float inputF = f - 3f;
        float denom = 1f + Mathf.Exp(-inputF);
        return 1f / denom;
    }

    private Vector3 TriangleIncenter(Vector3 p1, Vector3 p2, Vector3 p3)
    {
        float AB = Vector3.Magnitude(p2 - p1);
        float BC = Vector3.Magnitude(p3 - p2);
        float CA = Vector3.Magnitude(p1 - p3);

        Vector3 temp = AB * p1 + BC * p2 + CA * p3;
        return temp / (AB + BC + CA);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawLine(Vector3.zero, Nose);
        Gizmos.color = Color.red;
        Gizmos.DrawLine(Vector3.zero, SelectedVector);
        Gizmos.color = Color.green;
        Gizmos.DrawLine(Vector3.zero, BlendVector);
    }

}
