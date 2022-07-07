using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CalculateRotAngle : MonoBehaviour
{
    private GameObject wrapper;
    public Transform rootNode;
    public Transform[] childNodes;
    public Vector3[] nodeRotations;
    public static Transform[] nodesInitVal;

    private void Awake()
    {
        PopulateChildren();
        nodesInitVal = childNodes;
    }

    public void CalCurRot()
    {
        foreach (Transform child in childNodes)
        {
            if (child.childCount == 0)  //terminal node
            {
                if (child.parent != null)   //parent exists
                {
                    //Do I really have to subdivide it into these tiny cases? Find a more general way
                }
            }
        }
    }

    public Quaternion GetInverse(Transform p1, Transform p2, Vector3 vec)
    {
        vec = Vector3.forward;
        return Quaternion.Inverse(Quaternion.LookRotation(p1.position - p2.position, Vector3.forward));
    }

    public void PopulateChildren()
    {
        childNodes = rootNode.GetComponentsInChildren<Transform>();
    }
}
