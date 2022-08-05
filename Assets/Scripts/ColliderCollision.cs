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
        if (!collision.gameObject.name.Contains(self) && !collision.gameObject.name.Contains(parent))       //if two colliders do not share a joint
        {
            gameObject.GetComponent<MeshRenderer>().material = SetCollider.GetComponent<MeshRenderer>().materials[1];
            Debug.Log(string.Format("{0} collides with {1} at ({2}, {3}, {4})", gameObject.name, collision.gameObject.name, collision.GetContact(0).point.x, collision.GetContact(0).point.y, collision.GetContact(0).point.z));
            colPoint = collision.GetContact(0).point;
            StartCoroutine(CustomGizmo());
            tempObject.name = string.Format("{0} - {1}", gameObject.name, collision.gameObject.name);
            //Debug.Log(gameObject.name + " collides with " + collision.gameObject.name);
        }
    }

    private void OnCollisionStay(Collision collision)
    {
        if (!collision.gameObject.name.Contains(self) && !collision.gameObject.name.Contains(parent))       //if two colliders do not share a joint
        {
            Debug.DrawRay(collision.GetContact(0).point, collision.GetContact(0).normal, Color.red);
        }

    }


    private void OnCollisionExit(Collision collision)
    {
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
    //        Debug.Log(string.Format("collision point ({0},{1},{2})", collisionPoint.x, collisionPoint.y, collisionPoint.z));
    //    }
    //}

    //private void OnTriggerExit(Collider other)
    //{
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
        tempObject.AddComponent<ColliderCollision>();
        yield return null;
    }
}
