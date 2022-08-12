using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetCollider : MonoBehaviour
{
    public Transform rootNode;
    public Transform[] childNodes;

    public GameObject[] capsules;
    public VirtualFamily[] virtualCapsules;

    public struct VirtualFamily
    {
        public GameObject go;
        public Vector3 curParentPosition;
        public Quaternion curParentRotation;
        public Vector3 curChildPosition;
        public Quaternion curChildRotation;
        public Vector3 prevParentPosition;
        public Quaternion prevParentRotation;
        public Vector3 prevChildPosition;
        public Quaternion prevChildRotation;
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
                virtualCapsules[i].curParentPosition = child.transform.position;
                virtualCapsules[i].curParentRotation = child.transform.rotation;
                virtualCapsules[i].curChildPosition = child.transform.position;
                virtualCapsules[i].curChildRotation = child.transform.rotation;
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
                    virtualCapsules[i].curParentPosition = child.parent.transform.position;
                    virtualCapsules[i].curParentRotation = child.parent.transform.rotation;
                    virtualCapsules[i].curChildPosition = child.transform.position;
                    virtualCapsules[i].curChildRotation = child.transform.rotation;
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
        virtualCapsules[i].curParentPosition = childNodes[20].transform.position;
        virtualCapsules[i].curParentRotation = childNodes[20].transform.rotation;
        virtualCapsules[i].curChildPosition = childNodes[6].transform.position;
        virtualCapsules[i].curChildRotation = childNodes[6].transform.rotation;

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
        virtualCapsules[i].curParentPosition = childNodes[24].transform.position;
        virtualCapsules[i].curParentRotation = childNodes[24].transform.rotation;
        virtualCapsules[i].curChildPosition = childNodes[10].transform.position;
        virtualCapsules[i].curChildRotation = childNodes[10].transform.rotation;
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
        virtualCapsules[i].curParentPosition = childNodes[16].transform.position;
        virtualCapsules[i].curParentRotation = childNodes[16].transform.rotation;
        virtualCapsules[i].curChildPosition = childNodes[16].transform.position;
        virtualCapsules[i].curChildRotation = childNodes[16].transform.rotation;
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
        for (int k = 0; k < virtualCapsules.Length; k++)
        {
            virtualCapsules[k].go.transform.SetParent(vParentObject.transform);
            virtualCapsules[k].go.GetComponent<MeshRenderer>().material = this.gameObject.GetComponent<MeshRenderer>().materials[2];
            virtualCapsules[k].prevParentPosition = virtualCapsules[k].curParentPosition;
            virtualCapsules[k].prevParentRotation = virtualCapsules[k].curParentRotation;
            virtualCapsules[k].prevChildPosition = virtualCapsules[k].curChildPosition;
            virtualCapsules[k].prevChildRotation = virtualCapsules[k].curChildRotation;
        }


        flag = false;
    }

    private void Update()
    {
        Quaternion rot = Quaternion.AngleAxis(90, Vector3.right);

        for (int k = 0; k < virtualCapsules.Length; k++)
        {
            virtualCapsules[k].prevParentPosition = virtualCapsules[k].curParentPosition;
            virtualCapsules[k].prevParentRotation = virtualCapsules[k].curParentRotation;
            virtualCapsules[k].prevChildPosition = virtualCapsules[k].curChildPosition;
            virtualCapsules[k].prevChildRotation = virtualCapsules[k].curChildRotation;
        }
        capsules[0].transform.SetPositionAndRotation(childNodes[0].position, childNodes[0].rotation * rot);
        virtualCapsules[0].curParentPosition = childNodes[0].transform.position;
        virtualCapsules[0].curParentRotation = childNodes[0].transform.rotation;
        virtualCapsules[0].curChildPosition = childNodes[0].transform.position;
        virtualCapsules[0].curChildRotation = childNodes[0].transform.rotation;
        capsules[1].transform.SetPositionAndRotation(Vector3.Lerp(childNodes[1].transform.position, childNodes[0].transform.position, 0.5f),
                                                    Quaternion.LookRotation(childNodes[1].transform.position - childNodes[0].transform.position) * rot);
        virtualCapsules[1].curParentPosition = childNodes[0].transform.position;
        virtualCapsules[1].curParentRotation = childNodes[0].transform.rotation;
        virtualCapsules[1].curChildPosition = childNodes[1].transform.position;
        virtualCapsules[1].curChildRotation = childNodes[1].transform.rotation;
        capsules[2].transform.SetPositionAndRotation(Vector3.Lerp(childNodes[2].transform.position, childNodes[1].transform.position, 0.5f),
                                                    Quaternion.LookRotation(childNodes[2].transform.position - childNodes[1].transform.position) * rot);
        virtualCapsules[2].curParentPosition = childNodes[1].transform.position;
        virtualCapsules[2].curParentRotation = childNodes[1].transform.rotation;
        virtualCapsules[2].curChildPosition = childNodes[2].transform.position;
        virtualCapsules[2].curChildRotation = childNodes[2].transform.rotation;

        capsules[3].transform.SetPositionAndRotation(Vector3.Lerp(childNodes[6].transform.position, childNodes[2].transform.position, 0.5f),
                                                    Quaternion.LookRotation(childNodes[6].transform.position - childNodes[2].transform.position) * rot);
        virtualCapsules[3].curParentPosition = childNodes[2].transform.position;
        virtualCapsules[3].curParentRotation = childNodes[2].transform.rotation;
        virtualCapsules[3].curChildPosition = childNodes[6].transform.position;
        virtualCapsules[3].curChildRotation = childNodes[6].transform.rotation;
        capsules[4].transform.SetPositionAndRotation(Vector3.Lerp(childNodes[7].transform.position, childNodes[6].transform.position, 0.5f),
                                                    Quaternion.LookRotation(childNodes[7].transform.position - childNodes[6].transform.position) * rot);
        virtualCapsules[4].curParentPosition = childNodes[6].transform.position;
        virtualCapsules[4].curParentRotation = childNodes[6].transform.rotation;
        virtualCapsules[4].curChildPosition = childNodes[7].transform.position;
        virtualCapsules[4].curChildRotation = childNodes[7].transform.rotation;
        capsules[5].transform.SetPositionAndRotation(Vector3.Lerp(childNodes[8].transform.position, childNodes[7].transform.position, 0.5f),
                                                    Quaternion.LookRotation(childNodes[8].transform.position - childNodes[7].transform.position) * rot);
        virtualCapsules[5].curParentPosition = childNodes[7].transform.position;
        virtualCapsules[5].curParentRotation = childNodes[7].transform.rotation;
        virtualCapsules[5].curChildPosition = childNodes[8].transform.position;
        virtualCapsules[5].curChildRotation = childNodes[8].transform.rotation;
        capsules[6].transform.SetPositionAndRotation(Vector3.Lerp(childNodes[9].transform.position, childNodes[8].transform.position, 0.5f),
                                                    Quaternion.LookRotation(childNodes[9].transform.position - childNodes[8].transform.position) * rot);
        virtualCapsules[6].curParentPosition = childNodes[8].transform.position;
        virtualCapsules[6].curParentRotation = childNodes[8].transform.rotation;
        virtualCapsules[6].curChildPosition = childNodes[9].transform.position;
        virtualCapsules[6].curChildRotation = childNodes[9].transform.rotation;

        capsules[7].transform.SetPositionAndRotation(Vector3.Lerp(childNodes[10].transform.position, childNodes[2].transform.position, 0.5f),
                                                    Quaternion.LookRotation(childNodes[10].transform.position - childNodes[2].transform.position) * rot);
        virtualCapsules[7].curParentPosition = childNodes[2].transform.position;
        virtualCapsules[7].curParentRotation = childNodes[2].transform.rotation;
        virtualCapsules[7].curChildPosition = childNodes[10].transform.position;
        virtualCapsules[7].curChildRotation = childNodes[10].transform.rotation;
        capsules[8].transform.SetPositionAndRotation(Vector3.Lerp(childNodes[11].transform.position, childNodes[10].transform.position, 0.5f),
                                                    Quaternion.LookRotation(childNodes[11].transform.position - childNodes[10].transform.position) * rot);
        virtualCapsules[8].curParentPosition = childNodes[10].transform.position;
        virtualCapsules[8].curParentRotation = childNodes[10].transform.rotation;
        virtualCapsules[8].curChildPosition = childNodes[11].transform.position;
        virtualCapsules[8].curChildRotation = childNodes[11].transform.rotation;
        capsules[9].transform.SetPositionAndRotation(Vector3.Lerp(childNodes[12].transform.position, childNodes[11].transform.position, 0.5f),
                                                    Quaternion.LookRotation(childNodes[12].transform.position - childNodes[11].transform.position) * rot);
        virtualCapsules[9].curParentPosition = childNodes[11].transform.position;
        virtualCapsules[9].curParentRotation = childNodes[11].transform.rotation;
        virtualCapsules[9].curChildPosition = childNodes[12].transform.position;
        virtualCapsules[9].curChildRotation = childNodes[12].transform.rotation;
        capsules[10].transform.SetPositionAndRotation(Vector3.Lerp(childNodes[13].transform.position, childNodes[12].transform.position, 0.5f),
                                                    Quaternion.LookRotation(childNodes[13].transform.position - childNodes[12].transform.position) * rot);
        virtualCapsules[10].curParentPosition = childNodes[12].transform.position;
        virtualCapsules[10].curParentRotation = childNodes[12].transform.rotation;
        virtualCapsules[10].curChildPosition = childNodes[13].transform.position;
        virtualCapsules[10].curChildRotation = childNodes[13].transform.rotation;

        capsules[11].transform.SetPositionAndRotation(Vector3.Lerp(childNodes[16].transform.position, childNodes[2].transform.position, 0.5f),
                                                    Quaternion.LookRotation(childNodes[16].transform.position - childNodes[2].transform.position) * rot);
        virtualCapsules[11].curParentPosition = childNodes[2].transform.position;
        virtualCapsules[11].curParentRotation = childNodes[2].transform.rotation;
        virtualCapsules[11].curChildPosition = childNodes[16].transform.position;
        virtualCapsules[11].curChildRotation = childNodes[16].transform.rotation;

        capsules[12].transform.SetPositionAndRotation(Vector3.Lerp(childNodes[20].transform.position, childNodes[0].transform.position, 0.5f),
                                                    Quaternion.LookRotation(childNodes[20].transform.position - childNodes[0].transform.position) * rot);
        virtualCapsules[12].curParentPosition = childNodes[0].transform.position;
        virtualCapsules[12].curParentRotation = childNodes[0].transform.rotation;
        virtualCapsules[12].curChildPosition = childNodes[20].transform.position;
        virtualCapsules[12].curChildRotation = childNodes[20].transform.rotation;
        capsules[13].transform.SetPositionAndRotation(Vector3.Lerp(childNodes[21].transform.position, childNodes[20].transform.position, 0.5f),
                                                    Quaternion.LookRotation(childNodes[21].transform.position - childNodes[20].transform.position) * rot);
        virtualCapsules[13].curParentPosition = childNodes[20].transform.position;
        virtualCapsules[13].curParentRotation = childNodes[20].transform.rotation;
        virtualCapsules[13].curChildPosition = childNodes[21].transform.position;
        virtualCapsules[13].curChildRotation = childNodes[21].transform.rotation;
        capsules[14].transform.SetPositionAndRotation(Vector3.Lerp(childNodes[22].transform.position, childNodes[21].transform.position, 0.5f),
                                                    Quaternion.LookRotation(childNodes[22].transform.position - childNodes[21].transform.position) * rot);
        virtualCapsules[14].curParentPosition = childNodes[21].transform.position;
        virtualCapsules[14].curParentRotation = childNodes[21].transform.rotation;
        virtualCapsules[14].curChildPosition = childNodes[22].transform.position;
        virtualCapsules[14].curChildRotation = childNodes[22].transform.rotation;
        capsules[15].transform.SetPositionAndRotation(Vector3.Lerp(childNodes[23].transform.position, childNodes[22].transform.position, 0.5f),
                                                    Quaternion.LookRotation(childNodes[23].transform.position - childNodes[22].transform.position) * rot);
        virtualCapsules[15].curParentPosition = childNodes[22].transform.position;
        virtualCapsules[15].curParentRotation = childNodes[22].transform.rotation;
        virtualCapsules[15].curChildPosition = childNodes[23].transform.position;
        virtualCapsules[15].curChildRotation = childNodes[23].transform.rotation;

        capsules[16].transform.SetPositionAndRotation(Vector3.Lerp(childNodes[24].transform.position, childNodes[0].transform.position, 0.5f),
                                                    Quaternion.LookRotation(childNodes[24].transform.position - childNodes[0].transform.position) * rot);
        virtualCapsules[16].curParentPosition = childNodes[0].transform.position;
        virtualCapsules[16].curParentRotation = childNodes[0].transform.rotation;
        virtualCapsules[16].curChildPosition = childNodes[24].transform.position;
        virtualCapsules[16].curChildRotation = childNodes[24].transform.rotation;
        capsules[17].transform.SetPositionAndRotation(Vector3.Lerp(childNodes[25].transform.position, childNodes[24].transform.position, 0.5f),
                                                    Quaternion.LookRotation(childNodes[25].transform.position - childNodes[24].transform.position) * rot);
        virtualCapsules[17].curParentPosition = childNodes[24].transform.position;
        virtualCapsules[17].curParentRotation = childNodes[24].transform.rotation;
        virtualCapsules[17].curChildPosition = childNodes[25].transform.position;
        virtualCapsules[17].curChildRotation = childNodes[25].transform.rotation;
        capsules[18].transform.SetPositionAndRotation(Vector3.Lerp(childNodes[26].transform.position, childNodes[25].transform.position, 0.5f),
                                                    Quaternion.LookRotation(childNodes[26].transform.position - childNodes[25].transform.position) * rot);
        virtualCapsules[18].curParentPosition = childNodes[25].transform.position;
        virtualCapsules[18].curParentRotation = childNodes[25].transform.rotation;
        virtualCapsules[18].curChildPosition = childNodes[26].transform.position;
        virtualCapsules[18].curChildRotation = childNodes[26].transform.rotation;
        capsules[19].transform.SetPositionAndRotation(Vector3.Lerp(childNodes[27].transform.position, childNodes[26].transform.position, 0.5f),
                                                    Quaternion.LookRotation(childNodes[27].transform.position - childNodes[26].transform.position) * rot);
        virtualCapsules[19].curParentPosition = childNodes[26].transform.position;
        virtualCapsules[19].curParentRotation = childNodes[26].transform.rotation;
        virtualCapsules[19].curChildPosition = childNodes[27].transform.position;
        virtualCapsules[19].curChildRotation = childNodes[27].transform.rotation;

        capsules[20].transform.SetPositionAndRotation(Vector3.Lerp(childNodes[6].transform.position, childNodes[20].transform.position, 0.5f),
                                                    Quaternion.LookRotation(childNodes[6].transform.position - childNodes[20].transform.position) * rot);
        virtualCapsules[20].curParentPosition = childNodes[20].transform.position;
        virtualCapsules[20].curParentRotation = childNodes[20].transform.rotation;
        virtualCapsules[20].curChildPosition = childNodes[6].transform.position;
        virtualCapsules[20].curChildRotation = childNodes[6].transform.rotation;
        capsules[21].transform.SetPositionAndRotation(Vector3.Lerp(childNodes[10].transform.position, childNodes[24].transform.position, 0.5f),
                                                    Quaternion.LookRotation(childNodes[10].transform.position - childNodes[24].transform.position) * rot);
        virtualCapsules[21].curParentPosition = childNodes[24].transform.position;
        virtualCapsules[21].curParentRotation = childNodes[24].transform.rotation;
        virtualCapsules[21].curChildPosition = childNodes[10].transform.position;
        virtualCapsules[21].curChildRotation = childNodes[10].transform.rotation;
        capsules[22].transform.SetPositionAndRotation(Vector3.Lerp(childNodes[16].position, childNodes[19].position, headPosition), childNodes[16].rotation);
        virtualCapsules[22].curParentPosition = childNodes[16].transform.position;
        virtualCapsules[22].curParentRotation = childNodes[16].transform.rotation;
        virtualCapsules[22].curChildPosition = childNodes[16].transform.position;
        virtualCapsules[22].curChildRotation = childNodes[16].transform.rotation;

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
