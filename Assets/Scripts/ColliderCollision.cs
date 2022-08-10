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
    private List<Collision> go = new();

    private Vector3 ParentPos = new();
    private Vector3 ChildPos = new();
    private Vector3 PoL = new();
    private float length = new();

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

    private void OnCollisionEnter(Collision collision)
    {
        if (!collision.gameObject.name.Contains(self) && !collision.gameObject.name.Contains(parent) && collision.gameObject.name.Contains("Virtual"))       //if two colliders do not share a joint
        {
            if (!go.Contains(collision))
            {
                go.Add(collision);
            }
            gameObject.GetComponent<MeshRenderer>().material = SetCollider.GetComponent<MeshRenderer>().materials[1];
            collision.gameObject.GetComponent<MeshRenderer>().material = SetCollider.GetComponent<MeshRenderer>().materials[3];
            Debug.Log(string.Format("{0} collides with {1} at ({2}, {3}, {4})", gameObject.name, collision.gameObject.name, collision.GetContact(0).point.x, collision.GetContact(0).point.y, collision.GetContact(0).point.z));
            colPoint = Vector3.zero;
            for (int i = 0; i < collision.contactCount; i++)
            {
                colPoint += collision.GetContact(i).point;
            }
            colPoint /= collision.contactCount;
            StartCoroutine(CustomGizmo());
            //Debug.DrawRay(collision.GetContact(0).point, collision.GetContact(0).normal, Color.red);
            tempObject.name = string.Format("{0} - {1}", gameObject.name, collision.gameObject.name);
            Debug.Log(gameObject.name + " collides with " + collision.gameObject.name);
            FindDistance(collision.gameObject, colPoint, out ParentPos, out ChildPos, out PoL, out length);
            Debug.DrawLine(ParentPos, PoL, Color.green);
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
        foreach (var item in go)
        {
            if (item.gameObject == collision.gameObject)
            {
                if (!collision.gameObject.name.Contains(self) && !collision.gameObject.name.Contains(parent) && collision.gameObject.name.Contains("Virtual"))       //if two colliders do not share a joint
                {
                    Vector3 tempV = new();
                    for (int i = 0; i < collision.contactCount; i++)
                    {
                        tempV += collision.GetContact(i).point;
                    }
                    tempV /= collision.contactCount;
                    tempObject.transform.position = tempV;
                    FindDistance(collision.gameObject, tempV, out ParentPos, out ChildPos, out PoL, out length);
                    Debug.DrawLine(ParentPos, PoL, Color.green);
                    Debug.DrawLine(PoL, tempV, Color.green);
                }
            }
        }

    }


    private void OnCollisionExit(Collision collision)
    {
        if (go.Contains(collision))
        {
            go.Remove(collision);
        }
        Destroy(tempObject);
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

    private IEnumerator CustomGizmo()
    {
        if (tempObject != null)
        {
            Destroy(tempObject);
        }
        tempObject = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        tempObject.SetActive(true);
        tempObject.transform.position = colPoint;
        tempObject.transform.localScale = new Vector3(0.02f, 0.02f, 0.02f);
        tempObject.GetComponent<Collider>().enabled = false;
        //tempObject.AddComponent<ColliderCollision>();
        yield return null;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="go">The virtualCapsule object</param>
    /// <param name="pointA">Collision Point</param>
    /// <param name="pos1">Parent Joint Position</param>
    /// <param name="pos2">Child Joint Position</param>
    /// <param name="pointOnLine">Point on line pos1->pos2 which makes the distance between line and pointA minimum</param>
    /// <param name="length">Bone Length</param>
    private void FindDistance(GameObject go, Vector3 pointA, out Vector3 pos1, out Vector3 pos2, out Vector3 pointOnLine, out float length)
    {
        foreach (var item in SetCollider.virtualCapsules)
        {
            if (item.go == go)
            {
                pos1 = item.parentTransform.position;
                pos2 = item.childTransform.position;
                Vector3 line = pos2 - pos1;
                length = Vector3.Distance(pos1, pos2);
                pointOnLine = GetClosestPoint(pos1, pos2, pointA);
                return;
            }
        }
        pos1 = Vector3.zero;
        pos2 = Vector3.zero;
        pointOnLine = Vector3.zero;
        length = 0.0f;
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
}
