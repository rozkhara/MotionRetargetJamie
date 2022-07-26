using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetCollider : MonoBehaviour
{
    public Transform rootNode;
    public Transform[] childNodes;

    public GameObject[] capsules;

    public bool flag = true;

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
        capsules = new GameObject[20];
        //capsules = new Transform[childNodes.Length];
        foreach (Transform child in childNodes)
        {
            if (child == rootNode)
            {
                GameObject sphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                sphere.SetActive(true);
                sphere.transform.SetPositionAndRotation(child.position, child.rotation);
                sphere.transform.localScale = new Vector3(0.05f, 0.05f, 0.05f);
                sphere.name = "bounding_hips";
                capsules[i] = sphere;
                i++;
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
                    capsule.name = child.name;
                    capsules[i] = capsule;
                    i++;
                }
            }

        }
        capsules[1].transform.parent = capsules[0].transform;
        capsules[2].transform.parent = capsules[1].transform;

        capsules[3].transform.parent = capsules[2].transform;
        capsules[4].transform.parent = capsules[3].transform;
        capsules[5].transform.parent = capsules[4].transform;
        capsules[6].transform.parent = capsules[5].transform;

        capsules[7].transform.parent = capsules[2].transform;
        capsules[8].transform.parent = capsules[7].transform;
        capsules[9].transform.parent = capsules[8].transform;
        capsules[10].transform.parent = capsules[9].transform;

        capsules[11].transform.parent = capsules[2].transform;

        capsules[12].transform.parent = capsules[0].transform;
        capsules[13].transform.parent = capsules[12].transform;
        capsules[14].transform.parent = capsules[13].transform;
        capsules[15].transform.parent = capsules[14].transform;

        capsules[16].transform.parent = capsules[0].transform;
        capsules[17].transform.parent = capsules[16].transform;
        capsules[18].transform.parent = capsules[17].transform;
        capsules[19].transform.parent = capsules[18].transform;
        flag = false;
    }

    

    public void PopulateChildren()
    {
        childNodes = rootNode.GetComponentsInChildren<Transform>();
    }
}
