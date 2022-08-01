using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetCollider : MonoBehaviour
{
    public Transform rootNode;
    public Transform[] childNodes;

    public GameObject[] capsules;

    private readonly float headPosition = 0.31f;
    private readonly float headSize = 0.4f;

    private Material[] materials = new Material[2];

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
        materials = gameObject.GetComponent<MeshRenderer>().materials;
        int i = 0;
        float length;
        capsules = new GameObject[23];
        foreach (Transform child in childNodes)
        {
            if (child == rootNode)
            {
                GameObject sphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                sphere.SetActive(true);
                sphere.transform.SetPositionAndRotation(child.position, child.rotation);
                sphere.transform.localScale = new Vector3(0.05f, 0.05f, 0.05f);
                sphere.name = child.name;
                sphere.AddComponent<Rigidbody>();
                sphere.AddComponent<ColliderCollision>();
                sphere.GetComponent<Collider>().isTrigger = true;
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
                    length = Vector3.Distance(child.transform.position, child.parent.transform.position);
                    capsule.transform.localScale = new Vector3(0.05f, length / 2.0f, 0.05f);
                    capsule.name = child.name + " - " + child.parent.name;
                    capsule.AddComponent<Rigidbody>();
                    capsule.AddComponent<ColliderCollision>();
                    capsule.GetComponent<Collider>().isTrigger = true;
                    capsules[i] = capsule;
                    i++;
                }
            }

        }
        #region create body
        GameObject capsule1 = GameObject.CreatePrimitive(PrimitiveType.Capsule);
        capsule1.SetActive(true);
        capsule1.transform.SetPositionAndRotation(Vector3.Lerp(childNodes[6].transform.position, childNodes[20].transform.position, 0.5f),
                                                    Quaternion.LookRotation(childNodes[6].transform.position - childNodes[20].transform.position));
        length = Vector3.Distance(childNodes[6].transform.position, childNodes[20].transform.position);
        capsule1.transform.localScale = new Vector3(0.05f, length / 2.0f, 0.05f);
        capsule1.name = childNodes[6].name + " - " + childNodes[20].name;
        capsule1.AddComponent<Rigidbody>();
        capsule1.AddComponent<ColliderCollision>();
        capsule1.GetComponent<Collider>().isTrigger = true;
        capsules[i] = capsule1;
        i++;

        GameObject capsule2 = GameObject.CreatePrimitive(PrimitiveType.Capsule);
        capsule2.SetActive(true);
        capsule2.transform.SetPositionAndRotation(Vector3.Lerp(childNodes[10].transform.position, childNodes[24].transform.position, 0.5f),
                                                    Quaternion.LookRotation(childNodes[10].transform.position - childNodes[24].transform.position));
        length = Vector3.Distance(childNodes[10].transform.position, childNodes[24].transform.position);
        capsule2.transform.localScale = new Vector3(0.05f, length / 2.0f, 0.05f);
        capsule2.name = childNodes[10].name + " - " + childNodes[24].name;
        capsule2.AddComponent<Rigidbody>();
        capsule2.AddComponent<ColliderCollision>();
        capsule2.GetComponent<Collider>().isTrigger = true;
        capsules[i] = capsule2;
        i++;

        GameObject sphere1 = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        sphere1.SetActive(true);
        sphere1.transform.SetPositionAndRotation(Vector3.Lerp(childNodes[16].position, childNodes[19].position, headPosition), childNodes[16].rotation);
        sphere1.transform.localScale = new Vector3(headSize, headSize, headSize);
        sphere1.name = childNodes[16].name;
        sphere1.AddComponent<Rigidbody>();
        sphere1.AddComponent<ColliderCollision>();
        sphere1.GetComponent<Collider>().isTrigger = true;
        capsules[i] = sphere1;
        i++;
        #endregion

        GameObject parentObject = new();
        parentObject.transform.position = childNodes[0].transform.parent.position;
        parentObject.name = "Bounding Volume";

        foreach (var item in capsules)
        {
            item.transform.SetParent(parentObject.transform);
        }


        flag = false;
    }

    private void Update()
    {
        Quaternion rot = Quaternion.AngleAxis(90, Vector3.right);

        capsules[0].transform.SetPositionAndRotation(childNodes[0].position, childNodes[0].rotation);

        capsules[1].transform.SetPositionAndRotation(Vector3.Lerp(childNodes[1].transform.position, childNodes[0].transform.position, 0.5f),
                                                    Quaternion.LookRotation(childNodes[1].transform.position - childNodes[0].transform.position) * rot);
        capsules[2].transform.SetPositionAndRotation(Vector3.Lerp(childNodes[2].transform.position, childNodes[1].transform.position, 0.5f),
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

        capsules[20].transform.SetPositionAndRotation(Vector3.Lerp(childNodes[6].transform.position, childNodes[20].transform.position, 0.5f),
                                                    Quaternion.LookRotation(childNodes[6].transform.position - childNodes[20].transform.position) * rot);
        capsules[21].transform.SetPositionAndRotation(Vector3.Lerp(childNodes[10].transform.position, childNodes[24].transform.position, 0.5f),
                                                    Quaternion.LookRotation(childNodes[10].transform.position - childNodes[24].transform.position) * rot);
        capsules[22].transform.SetPositionAndRotation(Vector3.Lerp(childNodes[16].position, childNodes[19].position, headPosition), childNodes[16].rotation);
    }

    public void PopulateChildren()
    {
        childNodes = rootNode.GetComponentsInChildren<Transform>();
    }
}
