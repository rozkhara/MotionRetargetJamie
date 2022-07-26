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
        //capsules = new Transform[childNodes.Length;
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
                    capsule.transform.position = Vector3.Lerp(child.transform.position, child.parent.transform.position, 0.5f);
                    capsule.transform.rotation = Quaternion.LookRotation(child.transform.position - child.parent.transform.position);
                    float length = Vector3.Distance(child.transform.position, child.parent.transform.position);
                    capsule.transform.localScale = new Vector3(0.05f, length / 2.0f, 0.05f);
                    //capsule.transform.parent = capsules[System.Array.IndexOf(childNodes, child.parent)];
                    //capsule.transform.SetParent(capsules[System.Array.IndexOf(childNodes, child.parent)], true);
                    capsule.name = child.name;
                    capsules[i] = capsule;
                    i++;
                }
            }

        }
        /*
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
        */
        flag = false;
    }

    private void Update()
    {
        Quaternion rot = Quaternion.AngleAxis(90, Vector3.right);

        capsules[0].transform.rotation = childNodes[0].rotation;

        capsules[1].transform.SetPositionAndRotation(Vector3.Lerp(childNodes[1].transform.position , childNodes[0].transform.position, 0.5f), 
                                                    Quaternion.LookRotation(childNodes[1].transform.position - childNodes[0].transform.position) * rot);
        capsules[2].transform.SetPositionAndRotation(Vector3.Lerp(childNodes[2].transform.position , childNodes[1].transform.position, 0.5f), 
                                                    Quaternion.LookRotation(childNodes[2].transform.position - childNodes[1].transform.position) * rot);

        capsules[3].transform.SetPositionAndRotation(Vector3.Lerp(childNodes[6].transform.position, childNodes[2].transform.position, 0.5f), 
                                                    Quaternion.LookRotation(childNodes[6].transform.position - childNodes[2].transform.position) * rot);
        capsules[4].transform.SetPositionAndRotation(Vector3.Lerp(childNodes[7].transform.position, childNodes[6].transform.position, 0.5f), 
                                                    Quaternion.LookRotation(childNodes[7].transform.position - childNodes[6].transform.position) * rot);
        capsules[5].transform.SetPositionAndRotation(Vector3.Lerp(childNodes[8].transform.position, childNodes[7].transform.position, 0.5f), 
                                                    Quaternion.LookRotation(childNodes[8].transform.position - childNodes[7].transform.position) * rot);
        capsules[6].transform.SetPositionAndRotation(Vector3.Lerp(childNodes[9].transform.position, childNodes[8].transform.position, 0.5f), 
                                                    Quaternion.LookRotation(childNodes[9].transform.position - childNodes[8].transform.position) * rot);

        capsules[7].transform.SetPositionAndRotation(Vector3.Lerp(childNodes[10].transform.position, childNodes[2].transform.position, 0.5f),   
                                                    Quaternion.LookRotation(childNodes[10].transform.position - childNodes[2].transform.position) * rot);
        capsules[8].transform.SetPositionAndRotation(Vector3.Lerp(childNodes[11].transform.position, childNodes[10].transform.position, 0.5f),  
                                                    Quaternion.LookRotation(childNodes[11].transform.position - childNodes[10].transform.position) * rot);
        capsules[9].transform.SetPositionAndRotation(Vector3.Lerp(childNodes[12].transform.position, childNodes[11].transform.position, 0.5f),  
                                                    Quaternion.LookRotation(childNodes[12].transform.position - childNodes[11].transform.position) * rot);
        capsules[10].transform.SetPositionAndRotation(Vector3.Lerp(childNodes[13].transform.position, childNodes[12].transform.position, 0.5f), 
                                                    Quaternion.LookRotation(childNodes[13].transform.position - childNodes[12].transform.position) * rot);

        capsules[11].transform.SetPositionAndRotation(Vector3.Lerp(childNodes[16].transform.position, childNodes[2].transform.position, 0.5f), 
                                                    Quaternion.LookRotation(childNodes[16].transform.position - childNodes[2].transform.position) * rot);

        capsules[12].transform.SetPositionAndRotation(Vector3.Lerp(childNodes[20].transform.position, childNodes[0].transform.position, 0.5f),  
                                                    Quaternion.LookRotation(childNodes[20].transform.position - childNodes[0].transform.position) * rot);
        capsules[13].transform.SetPositionAndRotation(Vector3.Lerp(childNodes[21].transform.position, childNodes[20].transform.position, 0.5f), 
                                                    Quaternion.LookRotation(childNodes[21].transform.position - childNodes[20].transform.position) * rot);
        capsules[14].transform.SetPositionAndRotation(Vector3.Lerp(childNodes[22].transform.position, childNodes[21].transform.position, 0.5f), 
                                                    Quaternion.LookRotation(childNodes[22].transform.position - childNodes[21].transform.position) * rot);
        capsules[15].transform.SetPositionAndRotation(Vector3.Lerp(childNodes[23].transform.position, childNodes[22].transform.position, 0.5f), 
                                                    Quaternion.LookRotation(childNodes[23].transform.position - childNodes[22].transform.position) * rot);

        capsules[16].transform.SetPositionAndRotation(Vector3.Lerp(childNodes[24].transform.position, childNodes[0].transform.position, 0.5f),  
                                                    Quaternion.LookRotation(childNodes[24].transform.position - childNodes[0].transform.position) * rot);
        capsules[17].transform.SetPositionAndRotation(Vector3.Lerp(childNodes[25].transform.position, childNodes[24].transform.position, 0.5f), 
                                                    Quaternion.LookRotation(childNodes[25].transform.position - childNodes[24].transform.position) * rot);
        capsules[18].transform.SetPositionAndRotation(Vector3.Lerp(childNodes[26].transform.position, childNodes[25].transform.position, 0.5f), 
                                                    Quaternion.LookRotation(childNodes[26].transform.position - childNodes[25].transform.position) * rot);
        capsules[19].transform.SetPositionAndRotation(Vector3.Lerp(childNodes[27].transform.position, childNodes[26].transform.position, 0.5f), 
                                                    Quaternion.LookRotation(childNodes[27].transform.position - childNodes[26].transform.position) * rot);



    }

    public void PopulateChildren()
    {
        childNodes = rootNode.GetComponentsInChildren<Transform>();
    }
}
