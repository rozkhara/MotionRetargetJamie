using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CalculateRotAngle : MonoBehaviour
{
    private GameObject wrapper;
    public Transform rootNode;
    public Transform[] childNodes;
    public Vector3[] nodeRotations;

    private void Awake()
    {
        wrapper = GameObject.FindGameObjectWithTag("Player");
    }
    //private void Update()
    //{
    //    if (rootNode != null)
    //    {
    //        if (childNodes == null || childNodes.Length == 0)
    //        {
    //            //get all joints to draw
    //            PopulateChildren();
    //        }

    //        foreach (Transform child in childNodes)
    //        {

    //        }
    //    }
    //}
    public void PopulateChildren()
    {
        childNodes = rootNode.GetComponentsInChildren<Transform>();
    }
}
