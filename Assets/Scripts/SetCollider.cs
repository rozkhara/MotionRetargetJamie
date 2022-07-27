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
                    capsule.transform.SetPositionAndRotation(Vector3.Lerp(child.transform.position, child.parent.transform.position, 0.5f), 
                                                                Quaternion.LookRotation(child.transform.position - child.parent.transform.position));
                    float length = Vector3.Distance(child.transform.position, child.parent.transform.position);
                    capsule.transform.localScale = new Vector3(1f, length / 2.0f, 1f);
                    //capsule.transform.parent = capsules[System.Array.IndexOf(childNodes, child.parent)];
                    //capsule.transform.SetParent(capsules[System.Array.IndexOf(childNodes, child.parent)], true);
                    capsule.name = child.name;
                    capsules[i] = capsule;
                    i++;
                }
            }

        }

        bool tag = false;
        capsules[1].transform.SetParent(capsules[0].transform, tag);
        capsules[2].transform.SetParent(capsules[1].transform, tag);

        capsules[3].transform.SetParent(capsules[2].transform, tag);
        capsules[4].transform.SetParent(capsules[3].transform, tag);
        capsules[5].transform.SetParent(capsules[4].transform, tag);
        capsules[6].transform.SetParent(capsules[5].transform, tag);

        capsules[7].transform.SetParent(capsules[2].transform, tag);
        capsules[8].transform.SetParent(capsules[7].transform, tag);
        capsules[9].transform.SetParent(capsules[8].transform, tag);
        capsules[10].transform.SetParent(capsules[9].transform, tag);

        capsules[11].transform.SetParent(capsules[2].transform, tag);

        capsules[12].transform.SetParent(capsules[0].transform, tag);
        capsules[13].transform.SetParent(capsules[12].transform, tag);
        capsules[14].transform.SetParent(capsules[13].transform, tag);
        capsules[15].transform.SetParent(capsules[14].transform, tag);

        capsules[16].transform.SetParent(capsules[0].transform, tag);
        capsules[17].transform.SetParent(capsules[16].transform, tag);
        capsules[18].transform.SetParent(capsules[17].transform, tag);
        capsules[19].transform.SetParent(capsules[18].transform, tag);

        bool[] scaleSet = new bool[capsules.Length];
        int j = 0;
        foreach (GameObject item in capsules)
        {
            if (item.transform.parent != null)
            {
                scaleSet[j] = false;
                if (scaleSet[System.Array.IndexOf(capsules, item.transform.parent.gameObject)] != true)
                {
                    var inverse = new Vector3(1 / item.transform.parent.localScale.x, 1 / item.transform.parent.localScale.y, 1 / item.transform.parent.localScale.z);
                    item.transform.localScale = Vector3.Scale(item.transform.localScale, inverse);
                    scaleSet[j] = true;
                }
            }
            j++;
        }

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
