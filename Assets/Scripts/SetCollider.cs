using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetCollider : MonoBehaviour
{
    public Transform rootNode;
    public Transform[] childNodes;

    public GameObject[] capsules;
    public GameObject[] virtualCapsules;

    private readonly float headPosition = 0.31f;
    private readonly float headSize = 0.4f;
    private readonly Vector3 virtualScaleCapsule = new Vector3(1.5f, 1.3f, 1.5f);
    private readonly Vector3 virtualScaleSphere = new Vector3(1.3f, 1.3f, 1.3f);

    public bool flag = true;
    private readonly bool isKinematic = true;
    private readonly bool isTrigger = false;
    public bool isRenderBody = false;
    public bool isRenderVirtual = false;

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
        float length;
        capsules = new GameObject[23];
        virtualCapsules = new GameObject[23];
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
                sphere.GetComponent<Rigidbody>().isKinematic = isKinematic;
                sphere.GetComponent<Collider>().isTrigger = isTrigger;
                sphere.GetComponent<MeshRenderer>().enabled = isRenderBody;
                GameObject sphereVirtual = Instantiate(sphere);
                sphere.AddComponent<ColliderCollision>();
                capsules[i] = sphere;
                sphereVirtual.transform.localScale = Vector3.Scale(sphere.transform.localScale, virtualScaleSphere);
                sphereVirtual.GetComponent<MeshRenderer>().enabled = isRenderVirtual;
                sphereVirtual.name = "Virtual " + child.name;
                virtualCapsules[i] = sphereVirtual;
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
                    capsule.GetComponent<Rigidbody>().isKinematic = isKinematic;
                    capsule.GetComponent<Collider>().isTrigger = isTrigger;
                    capsule.GetComponent<MeshRenderer>().enabled = isRenderBody;
                    GameObject capsuleVirtual = Instantiate(capsule);
                    capsule.AddComponent<ColliderCollision>();
                    capsules[i] = capsule;
                    capsuleVirtual.transform.localScale = Vector3.Scale(capsule.transform.localScale, virtualScaleCapsule);
                    capsuleVirtual.GetComponent<MeshRenderer>().enabled = isRenderVirtual;
                    capsuleVirtual.name = "Virtual " + child.name + " - " + child.parent.name;
                    virtualCapsules[i] = capsuleVirtual;
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
        capsule1.GetComponent<Rigidbody>().isKinematic = isKinematic;
        capsule1.GetComponent<Collider>().isTrigger = isTrigger;
        capsule1.GetComponent<MeshRenderer>().enabled = isRenderBody;
        GameObject capsuleVirtual1 = Instantiate(capsule1);
        capsule1.AddComponent<ColliderCollision>();
        capsules[i] = capsule1;
        capsuleVirtual1.transform.localScale = Vector3.Scale(capsule1.transform.localScale, virtualScaleCapsule);
        capsuleVirtual1.GetComponent<MeshRenderer>().enabled = isRenderVirtual;
        capsuleVirtual1.name = "Virtual " + childNodes[6].name + " - " + childNodes[20].name;
        virtualCapsules[i] = capsuleVirtual1;
        i++;

        GameObject capsule2 = GameObject.CreatePrimitive(PrimitiveType.Capsule);
        capsule2.SetActive(true);
        capsule2.transform.SetPositionAndRotation(Vector3.Lerp(childNodes[10].transform.position, childNodes[24].transform.position, 0.5f),
                                                    Quaternion.LookRotation(childNodes[10].transform.position - childNodes[24].transform.position));
        length = Vector3.Distance(childNodes[10].transform.position, childNodes[24].transform.position);
        capsule2.transform.localScale = new Vector3(0.05f, length / 2.0f, 0.05f);
        capsule2.name = childNodes[10].name + " - " + childNodes[24].name;
        capsule2.AddComponent<Rigidbody>();
        capsule2.GetComponent<Rigidbody>().isKinematic = isKinematic;
        capsule2.GetComponent<Collider>().isTrigger = isTrigger;
        capsule2.GetComponent<MeshRenderer>().enabled = isRenderBody;
        GameObject capsuleVirtual2 = Instantiate(capsule2);
        capsule2.AddComponent<ColliderCollision>();
        capsules[i] = capsule2;
        capsuleVirtual2.transform.localScale = Vector3.Scale(capsule2.transform.localScale, virtualScaleCapsule);
        capsuleVirtual2.GetComponent<MeshRenderer>().enabled = isRenderVirtual;
        capsuleVirtual2.name = "Virtual " + childNodes[10].name + " - " + childNodes[24].name;
        virtualCapsules[i] = capsuleVirtual2;
        i++;

        GameObject sphere1 = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        sphere1.SetActive(true);
        sphere1.transform.SetPositionAndRotation(Vector3.Lerp(childNodes[16].position, childNodes[19].position, headPosition), childNodes[16].rotation);
        sphere1.transform.localScale = new Vector3(headSize, headSize, headSize);
        sphere1.name = childNodes[16].name;
        sphere1.AddComponent<Rigidbody>();
        sphere1.GetComponent<Rigidbody>().isKinematic = isKinematic;
        sphere1.GetComponent<Collider>().isTrigger = isTrigger;
        sphere1.GetComponent<MeshRenderer>().enabled = isRenderBody;
        GameObject sphereVirtual1 = Instantiate(sphere1);
        sphere1.AddComponent<ColliderCollision>();
        capsules[i] = sphere1;
        sphereVirtual1.transform.localScale = Vector3.Scale(sphere1.transform.localScale, virtualScaleSphere);
        sphereVirtual1.GetComponent<MeshRenderer>().enabled = isRenderVirtual;
        sphereVirtual1.name = "Virtual " + childNodes[16].name;
        virtualCapsules[i] = sphereVirtual1;
        #endregion

        GameObject parentObject = new();
        parentObject.transform.position = childNodes[0].transform.parent.position;
        parentObject.name = "Bounding Volume";

        GameObject vParentObject = new();
        vParentObject.transform.position = childNodes[0].transform.parent.position;
        vParentObject.name = "Virtual Bounding Volume";

        foreach (var item in capsules)
        {
            item.transform.SetParent(parentObject.transform);
        }
        foreach (var item in virtualCapsules)
        {
            item.transform.SetParent(vParentObject.transform);
            item.GetComponent<MeshRenderer>().material = this.gameObject.GetComponent<MeshRenderer>().materials[2];
        }


        flag = false;
    }

    private void Update()
    {
        Quaternion rot = Quaternion.AngleAxis(90, Vector3.right);

        capsules[0].transform.SetPositionAndRotation(childNodes[0].position, childNodes[0].rotation * rot);
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

        for (int i = 0; i < virtualCapsules.Length; i++)
        {
            virtualCapsules[i].transform.SetPositionAndRotation(capsules[i].transform.position, capsules[i].transform.rotation);
        }
    }

    public void PopulateChildren()
    {
        childNodes = rootNode.GetComponentsInChildren<Transform>();
    }
}
