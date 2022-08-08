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
            Debug.Log(string.Format("{0} collides with {1} at ({2}, {3}, {4})", gameObject.name, collision.gameObject.name, collision.GetContact(0).point.x, collision.GetContact(0).point.y, collision.GetContact(0).point.z));
            for (int i = 0; i < collision.contactCount; i++)
            {
                colPoint += collision.GetContact(i).point;
            }
            colPoint /= collision.contactCount;
            StartCoroutine(CustomGizmo());
            tempObject.name = string.Format("{0} - {1}", gameObject.name, collision.gameObject.name);
            //Debug.Log(gameObject.name + " collides with " + collision.gameObject.name);
        }
    }

    private void OnCollisionStay(Collision collision)
    {
        if (!collision.gameObject.name.Contains(self) && !collision.gameObject.name.Contains(parent) && collision.gameObject.name.Contains("Virtual"))       //if two colliders do not share a joint
        {
            Debug.DrawRay(collision.GetContact(0).point, collision.GetContact(0).normal, Color.red);
        }
        foreach (var item in go)
        {
            if (item.gameObject == collision.gameObject)
            {
                Vector3 tempV = new();
                for (int i = 0; i < collision.contactCount; i++)
                {
                    tempV += collision.GetContact(i).point;
                }
                tempV /= collision.contactCount;
                tempObject.transform.position = tempV;
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
}
