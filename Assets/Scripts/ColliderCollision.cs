using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColliderCollision : MonoBehaviour
{
    public SetCollider SetCollider;
    private string[] textBuffer = new string[2];
    private string self;
    private string parent;
    private Vector3 colPoint;
    private GameObject tempObject;
    private List<Collision> CollisionList = new();
    private List<GameObject> GoList = new();

    private Vector3 curParentPosition = new();
    private Vector3 curChildPosition = new();
    private Vector3 prevParentPosition = new();
    private Quaternion prevParentRotation = new();
    private Vector3 prevChildPosition = new();
    private Quaternion prevChildRotation = new();
    private Vector3 PoL = new();
    private float length = new();

    private GameObject RealParentObject;
    private Transform[] RealObjects;

    private void Awake()
    {
        SetCollider = GameObject.Find("SetCollider").GetComponent<SetCollider>();
        if (gameObject.name.Contains(" - "))
        {
            textBuffer = gameObject.name.Split(" - ");
            self = textBuffer[0];
            parent = textBuffer[1];
        }
        else
        {
            self = gameObject.name;
            parent = "Placeholder";
        }

    }
    private void Start()
    {
        if (RealParentObject == null)
        {
            RealParentObject = GameObject.Find("Bounding Volume");
            RealObjects = RealParentObject.GetComponentsInChildren<Transform>();
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (!collision.gameObject.name.Contains(self) && !collision.gameObject.name.Contains(parent) && collision.gameObject.name.Contains("Virtual"))       //if two colliders do not share a joint
        {
            if (!CollisionList.Contains(collision))
            {
                CollisionList.Add(collision);
            }
            gameObject.GetComponent<MeshRenderer>().material = SetCollider.GetComponent<MeshRenderer>().materials[1];
            collision.gameObject.GetComponent<MeshRenderer>().material = SetCollider.GetComponent<MeshRenderer>().materials[3];
            Debug.Log(string.Format("{0} collides with {1} at ({2}, {3}, {4})", gameObject.name, collision.gameObject.name, collision.GetContact(0).point.x, collision.GetContact(0).point.y, collision.GetContact(0).point.z));
            colPoint = collision.GetContact(0).point;
            //StartCoroutine(CustomGizmo());
            //Debug.DrawRay(collision.GetContact(0).point, collision.GetContact(0).normal, Color.red);
            //tempObject.name = string.Format("{0} - {1}", gameObject.name, collision.gameObject.name);
            //Debug.Log(gameObject.name + " collides with " + collision.gameObject.name);
            GetValues(collision.gameObject, colPoint);
            GameObject realVolume = RealObjects[collision.gameObject.transform.GetSiblingIndex() + 1].gameObject;
            //Debug.Log("ColGameObject = " + collision.gameObject.name + " realVolume = " + realVolume.name);
            Vector3 pointA = GetPointA(realVolume, colPoint);
            Vector3 prevPointA = GetPrevPointA(pointA);
            //Debug.Log("curPos: " + curParentPosition.ToString() + " prevPos: " + prevParentPosition.ToString());
            Debug.DrawLine(pointA, curParentPosition, Color.blue);
            Debug.DrawLine(prevPointA, prevParentPosition, Color.blue);
            Debug.DrawLine(curParentPosition, PoL, Color.green);
            Debug.DrawLine(PoL, colPoint, Color.green);
            Debug.Break();
        }
    }

    private void OnCollisionStay(Collision collision)
    {
        //if (!collision.gameObject.name.Contains(self) && !collision.gameObject.name.Contains(parent) && collision.gameObject.name.Contains("Virtual"))       //if two colliders do not share a joint
        //{
        //    Debug.DrawRay(collision.GetContact(0).point, collision.GetContact(0).normal, Color.red);
        //}
        foreach (var item in CollisionList)
        {
            if (item.gameObject == collision.gameObject)
            {
                if (!collision.gameObject.name.Contains(self) && !collision.gameObject.name.Contains(parent) && collision.gameObject.name.Contains("Virtual"))       //if two colliders do not share a joint
                {
                    var tempV = collision.GetContact(0).point;
                    //tempObject.transform.position = tempV;
                    GetValues(collision.gameObject, tempV);
                    GameObject realVolume = RealObjects[collision.gameObject.transform.GetSiblingIndex() + 1].gameObject;
                    Vector3 pointA = GetPointA(realVolume, tempV);
                    Vector3 prevPointA = GetPrevPointA(pointA);
                    Debug.DrawLine(pointA, curParentPosition, Color.blue);
                    Debug.DrawLine(prevPointA, prevParentPosition, Color.blue);
                    Debug.DrawLine(curParentPosition, PoL, Color.green);
                    Debug.DrawLine(PoL, tempV, Color.green);
                }
            }
        }

    }


    private void OnCollisionExit(Collision collision)
    {
        if (CollisionList.Contains(collision))
        {
            CollisionList.Remove(collision);
        }
        //Destroy(tempObject);
        gameObject.GetComponent<MeshRenderer>().material = SetCollider.GetComponent<MeshRenderer>().materials[0];
        collision.gameObject.GetComponent<MeshRenderer>().material = SetCollider.GetComponent<MeshRenderer>().materials[2];
    }

    //private void OnTriggerEnter(Collider other)
    //{
    //    if (!other.gameObject.name.Contains(self) && !other.gameObject.name.Contains(parent))       //if two colliders do not share a joint
    //    {
    //        gameObject.GetComponent<MeshRenderer>().material = SetCollider.GetComponent<MeshRenderer>().materials[1];
    //        Debug.Log(gameObject.name + " collides with " + other.gameObject.name);
    //        var collisionPoint = other.ClosestPoint(this.transform.position);
    //        Debug.Log(string.Format("collision point ({0}, {1}, {2})", collisionPoint.x, collisionPoint.y, collisionPoint.z));
    //        colPoint = collisionPoint;
    //        StartCoroutine(CustomGizmo());
    //    }
    //}

    //private void OnTriggerStay(Collider other)
    //{
    //    var collisionPoint = other.ClosestPoint(this.transform.position); 
    //    colPoint = collisionPoint;
    //}

    //private void OnTriggerExit(Collider other)
    //{
    //    Destroy(tempObject);
    //    gameObject.GetComponent<MeshRenderer>().material = SetCollider.GetComponent<MeshRenderer>().materials[0];
    //}

    //private IEnumerator CustomGizmo()
    //{
    //    if (tempObject != null)
    //    {
    //        Destroy(tempObject);
    //    }
    //    tempObject = GameObject.CreatePrimitive(PrimitiveType.Sphere);
    //    tempObject.SetActive(true);
    //    tempObject.transform.position = colPoint;
    //    tempObject.transform.localScale = new Vector3(0.02f, 0.02f, 0.02f);
    //    tempObject.GetComponent<Collider>().enabled = false;
    //    //tempObject.AddComponent<ColliderCollision>();
    //    yield return null;
    //}

    private void GetValues(GameObject go, Vector3 pointA)
    {
        foreach (var item in SetCollider.virtualCapsules)   //can replace this with sibling index
        {
            if (item.go == go)
            {
                curParentPosition = item.curParentPosition;
                curChildPosition = item.curChildPosition;
                prevParentPosition = item.prevParentPosition;
                prevParentRotation = item.prevParentRotation;
                prevChildPosition = item.prevChildPosition;
                prevChildRotation = item.prevChildRotation;
                length = Vector3.Distance(curParentPosition, curChildPosition);
                PoL = GetClosestPoint(curParentPosition, curChildPosition, pointA);
                return;
            }
        }
        Debug.Log("Object Not Found");
    }

    private Vector3 GetClosestPoint(Vector3 A, Vector3 B, Vector3 P)
    {
        Vector3 line = B - A;
        float length = line.magnitude;
        line.Normalize();

        Vector3 v = P - A;
        float d = Vector3.Dot(v, line);
        d = Mathf.Clamp(d, 0f, length);
        return (A + line * d);
    }

    private Vector3 GetPointA(GameObject realVolume, Vector3 PoS)
    {
        if (realVolume.name.Contains("Face") || realVolume.name.Contains("Hips"))
        {
            var collider = realVolume.GetComponent<SphereCollider>();
            return collider.ClosestPoint(PoS);
        }
        else
        {
            var collider = realVolume.GetComponent<CapsuleCollider>();
            return collider.ClosestPoint(PoS);
        }
    }

    private Vector3 GetPrevPointA(Vector3 pointA)
    {
        Vector3 prevBone = prevChildPosition - prevParentPosition;
        Vector3 curBone = curChildPosition - curParentPosition;
        Vector3 inverseTranspose = prevParentPosition - curParentPosition;
        Quaternion inverseRotation = Quaternion.FromToRotation(curBone, prevBone);
        Vector3 returnVal = pointA + inverseTranspose;
        returnVal = inverseRotation * (returnVal - prevParentPosition) + prevParentPosition;
        return returnVal;
    }
}
