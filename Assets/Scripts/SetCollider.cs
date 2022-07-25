using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetCollider : MonoBehaviour
{
    public Transform rootNode;
    public Transform[] childNodes;

    public Dictionary<int, Transform> capsules = new();

    private void OnDrawGizmos()
    {
        if (rootNode != null)
        {
            if (childNodes == null || childNodes.Length == 0)
            {
                //get all joints to draw
                PopulateChildren();
            }
        }
    }

    private void Awake()
    {
        int i = 0;
        //capsules = new Transform[childNodes.Length];
        foreach (Transform child in childNodes)
        {
            if (child == rootNode)
            {
                GameObject sphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                sphere.SetActive(true);
                sphere.transform.SetPositionAndRotation(child.position, child.rotation);
                sphere.transform.localScale = new Vector3(0.05f, 0.05f, 0.05f);
                capsules[-1] = sphere.transform;

            }
            else
            {
                if (!child.name.Contains("Cape") && !child.name.Contains("Wing") && !child.name.Contains("Antenna") && !child.name.Contains("Dummy"))
                {
                    GameObject capsule = GameObject.CreatePrimitive(PrimitiveType.Capsule);
                    capsule.SetActive(true);
                    capsule.transform.position = (child.transform.position + child.parent.transform.position) / 2;
                    capsule.transform.rotation = Quaternion.LookRotation(Vector3.forward, child.transform.position - child.parent.transform.position);
                    float length = Vector3.Distance(child.transform.position, child.parent.transform.position);
                    capsule.transform.localScale = new Vector3(0.05f, length * 0.5f, 0.05f);
                    //capsule.transform.parent = capsules[System.Array.IndexOf(childNodes, child.parent)];
                    //capsule.transform.SetParent(capsules[System.Array.IndexOf(childNodes, child.parent)], true);
                    capsules[System.Array.IndexOf(childNodes, child.parent.transform)] = capsule.transform;
                }
            }
            i++;
        }
        //foreach (var item in capsules)    //this doesnt work for some reason
        //{
        //    if (item.Key >= 0)
        //    {
        //        item.Value.SetParent(capsules[item.Key]);

        //    }
        //}
    }


    public void PopulateChildren()
    {
        childNodes = rootNode.GetComponentsInChildren<Transform>();
    }
}
