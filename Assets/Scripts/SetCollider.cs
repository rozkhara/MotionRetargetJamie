using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetCollider : MonoBehaviour
{
    public Transform rootNode;
    public Transform[] childNodes;

    public GameObject[] capsules;
    public VirtualFamily[] virtualCapsules;

    public class VirtualFamily
    {
        public GameObject go;
        public Transform parentTransform = null;
        public Transform childTransform = null;
        public Transform prevParentTransform = null;
    }

    private readonly float headPosition = 0.31f;
    private readonly float headSize = 0.4f;
    private readonly Vector3 virtualScaleCapsule = new(1.5f, 1.1f, 1.5f);
    private readonly Vector3 virtualScaleSphere = new(1.3f, 1.3f, 1.3f);

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
        virtualCapsules = new VirtualFamily[23];
        for (int k = 0; k < virtualCapsules.Length; k++)
        {
            virtualCapsules[k] = new();
        }
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
                virtualCapsules[i].go = sphereVirtual;
                virtualCapsules[i].parentTransform = child.transform;
                virtualCapsules[i].childTransform = child.transform;
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
                    virtualCapsules[i].go = capsuleVirtual;
                    virtualCapsules[i].parentTransform = child.parent.transform;
                    virtualCapsules[i].childTransform = child.transform;
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
        virtualCapsules[i].go = capsuleVirtual1;
        virtualCapsules[i].parentTransform = childNodes[20].transform;
        virtualCapsules[i].childTransform = childNodes[6].transform;
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
        virtualCapsules[i].go = capsuleVirtual2;
        virtualCapsules[i].parentTransform = childNodes[24].transform;
        virtualCapsules[i].childTransform = childNodes[10].transform;
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
        virtualCapsules[i].go = sphereVirtual1;
        virtualCapsules[i].parentTransform = childNodes[16].transform;
        virtualCapsules[i].childTransform = childNodes[16].transform;
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
            item.go.transform.SetParent(vParentObject.transform);
            item.go.GetComponent<MeshRenderer>().material = this.gameObject.GetComponent<MeshRenderer>().materials[2];
            item.prevParentTransform = item.parentTransform;
        }


        flag = false;
    }

    private void Update()
    {
        Quaternion rot = Quaternion.AngleAxis(90, Vector3.right);

        foreach(var item in virtualCapsules)
        {
            item.prevParentTransform = item.parentTransform;
        }
        capsules[0].transform.SetPositionAndRotation(childNodes[0].position, childNodes[0].rotation * rot);
        virtualCapsules[0].parentTransform = childNodes[0].transform;
        virtualCapsules[0].childTransform = childNodes[0].transform;
        capsules[1].transform.SetPositionAndRotation(Vector3.Lerp(childNodes[1].transform.position, childNodes[0].transform.position, 0.5f),
                                                    Quaternion.LookRotation(childNodes[1].transform.position - childNodes[0].transform.position) * rot);
        virtualCapsules[1].parentTransform = childNodes[0].transform;
        virtualCapsules[1].childTransform = childNodes[1].transform;
        capsules[2].transform.SetPositionAndRotation(Vector3.Lerp(childNodes[2].transform.position, childNodes[1].transform.position, 0.5f),
                                                    Quaternion.LookRotation(childNodes[2].transform.position - childNodes[1].transform.position) * rot);
        virtualCapsules[2].parentTransform = childNodes[1].transform;
        virtualCapsules[2].childTransform = childNodes[2].transform;

        capsules[3].transform.SetPositionAndRotation(Vector3.Lerp(childNodes[6].transform.position, childNodes[2].transform.position, 0.5f),
                                                    Quaternion.LookRotation(childNodes[6].transform.position - childNodes[2].transform.position) * rot);
        virtualCapsules[3].parentTransform = childNodes[2].transform;
        virtualCapsules[3].childTransform = childNodes[6].transform;
        capsules[4].transform.SetPositionAndRotation(Vector3.Lerp(childNodes[7].transform.position, childNodes[6].transform.position, 0.5f),
                                                    Quaternion.LookRotation(childNodes[7].transform.position - childNodes[6].transform.position) * rot);
        virtualCapsules[4].parentTransform = childNodes[6].transform;
        virtualCapsules[4].childTransform = childNodes[7].transform;
        capsules[5].transform.SetPositionAndRotation(Vector3.Lerp(childNodes[8].transform.position, childNodes[7].transform.position, 0.5f),
                                                    Quaternion.LookRotation(childNodes[8].transform.position - childNodes[7].transform.position) * rot);
        virtualCapsules[5].parentTransform = childNodes[7].transform;
        virtualCapsules[5].childTransform = childNodes[8].transform;
        capsules[6].transform.SetPositionAndRotation(Vector3.Lerp(childNodes[9].transform.position, childNodes[8].transform.position, 0.5f),
                                                    Quaternion.LookRotation(childNodes[9].transform.position - childNodes[8].transform.position) * rot);
        virtualCapsules[6].parentTransform = childNodes[8].transform;
        virtualCapsules[6].childTransform = childNodes[9].transform;

        capsules[7].transform.SetPositionAndRotation(Vector3.Lerp(childNodes[10].transform.position, childNodes[2].transform.position, 0.5f),
                                                    Quaternion.LookRotation(childNodes[10].transform.position - childNodes[2].transform.position) * rot);
        virtualCapsules[7].parentTransform = childNodes[2].transform;
        virtualCapsules[7].childTransform = childNodes[10].transform;
        capsules[8].transform.SetPositionAndRotation(Vector3.Lerp(childNodes[11].transform.position, childNodes[10].transform.position, 0.5f),
                                                    Quaternion.LookRotation(childNodes[11].transform.position - childNodes[10].transform.position) * rot);
        virtualCapsules[8].parentTransform = childNodes[10].transform;
        virtualCapsules[8].childTransform = childNodes[11].transform;
        capsules[9].transform.SetPositionAndRotation(Vector3.Lerp(childNodes[12].transform.position, childNodes[11].transform.position, 0.5f),
                                                    Quaternion.LookRotation(childNodes[12].transform.position - childNodes[11].transform.position) * rot);
        virtualCapsules[9].parentTransform = childNodes[11].transform;
        virtualCapsules[9].childTransform = childNodes[12].transform;
        capsules[10].transform.SetPositionAndRotation(Vector3.Lerp(childNodes[13].transform.position, childNodes[12].transform.position, 0.5f),
                                                    Quaternion.LookRotation(childNodes[13].transform.position - childNodes[12].transform.position) * rot);
        virtualCapsules[10].parentTransform = childNodes[12].transform;
        virtualCapsules[10].childTransform = childNodes[13].transform;

        capsules[11].transform.SetPositionAndRotation(Vector3.Lerp(childNodes[16].transform.position, childNodes[2].transform.position, 0.5f),
                                                    Quaternion.LookRotation(childNodes[16].transform.position - childNodes[2].transform.position) * rot);
        virtualCapsules[11].parentTransform = childNodes[2].transform;
        virtualCapsules[11].childTransform = childNodes[16].transform;

        capsules[12].transform.SetPositionAndRotation(Vector3.Lerp(childNodes[20].transform.position, childNodes[0].transform.position, 0.5f),
                                                    Quaternion.LookRotation(childNodes[20].transform.position - childNodes[0].transform.position) * rot);
        virtualCapsules[12].parentTransform = childNodes[0].transform;
        virtualCapsules[12].childTransform = childNodes[20].transform;
        capsules[13].transform.SetPositionAndRotation(Vector3.Lerp(childNodes[21].transform.position, childNodes[20].transform.position, 0.5f),
                                                    Quaternion.LookRotation(childNodes[21].transform.position - childNodes[20].transform.position) * rot);
        virtualCapsules[13].parentTransform = childNodes[20].transform;
        virtualCapsules[13].childTransform = childNodes[21].transform;
        capsules[14].transform.SetPositionAndRotation(Vector3.Lerp(childNodes[22].transform.position, childNodes[21].transform.position, 0.5f),
                                                    Quaternion.LookRotation(childNodes[22].transform.position - childNodes[21].transform.position) * rot);
        virtualCapsules[14].parentTransform = childNodes[21].transform;
        virtualCapsules[14].childTransform = childNodes[22].transform;
        capsules[15].transform.SetPositionAndRotation(Vector3.Lerp(childNodes[23].transform.position, childNodes[22].transform.position, 0.5f),
                                                    Quaternion.LookRotation(childNodes[23].transform.position - childNodes[22].transform.position) * rot);
        virtualCapsules[15].parentTransform = childNodes[22].transform;
        virtualCapsules[15].childTransform = childNodes[23].transform;

        capsules[16].transform.SetPositionAndRotation(Vector3.Lerp(childNodes[24].transform.position, childNodes[0].transform.position, 0.5f),
                                                    Quaternion.LookRotation(childNodes[24].transform.position - childNodes[0].transform.position) * rot);
        virtualCapsules[16].parentTransform = childNodes[0].transform;
        virtualCapsules[16].childTransform = childNodes[24].transform;
        capsules[17].transform.SetPositionAndRotation(Vector3.Lerp(childNodes[25].transform.position, childNodes[24].transform.position, 0.5f),
                                                    Quaternion.LookRotation(childNodes[25].transform.position - childNodes[24].transform.position) * rot);
        virtualCapsules[17].parentTransform = childNodes[24].transform;
        virtualCapsules[17].childTransform = childNodes[25].transform;
        capsules[18].transform.SetPositionAndRotation(Vector3.Lerp(childNodes[26].transform.position, childNodes[25].transform.position, 0.5f),
                                                    Quaternion.LookRotation(childNodes[26].transform.position - childNodes[25].transform.position) * rot);
        virtualCapsules[18].parentTransform = childNodes[25].transform;
        virtualCapsules[18].childTransform = childNodes[26].transform;
        capsules[19].transform.SetPositionAndRotation(Vector3.Lerp(childNodes[27].transform.position, childNodes[26].transform.position, 0.5f),
                                                    Quaternion.LookRotation(childNodes[27].transform.position - childNodes[26].transform.position) * rot);
        virtualCapsules[19].parentTransform = childNodes[26].transform;
        virtualCapsules[19].childTransform = childNodes[27].transform;

        capsules[20].transform.SetPositionAndRotation(Vector3.Lerp(childNodes[6].transform.position, childNodes[20].transform.position, 0.5f),
                                                    Quaternion.LookRotation(childNodes[6].transform.position - childNodes[20].transform.position) * rot);
        virtualCapsules[20].parentTransform = childNodes[20].transform;
        virtualCapsules[20].childTransform = childNodes[6].transform;
        capsules[21].transform.SetPositionAndRotation(Vector3.Lerp(childNodes[10].transform.position, childNodes[24].transform.position, 0.5f),
                                                    Quaternion.LookRotation(childNodes[10].transform.position - childNodes[24].transform.position) * rot);
        virtualCapsules[21].parentTransform = childNodes[24].transform;
        virtualCapsules[21].childTransform = childNodes[10].transform;
        capsules[22].transform.SetPositionAndRotation(Vector3.Lerp(childNodes[16].position, childNodes[19].position, headPosition), childNodes[16].rotation);
        virtualCapsules[22].parentTransform = childNodes[16].transform;
        virtualCapsules[22].childTransform = childNodes[16].transform;

        for (int i = 0; i < virtualCapsules.Length; i++)
        {
            virtualCapsules[i].go.transform.SetPositionAndRotation(capsules[i].transform.position, capsules[i].transform.rotation);
        }
    }

    public void PopulateChildren()
    {
        childNodes = rootNode.GetComponentsInChildren<Transform>();
    }
}
