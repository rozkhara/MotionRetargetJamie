using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColliderCollision : MonoBehaviour
{
    public SetCollider SetCollider;
    private string[] textBuffer = new string[2];
    private string self;
    private string parent;

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

    private void OnTriggerEnter(Collider other)
    {
        if (!other.gameObject.name.Contains(self) && !other.gameObject.name.Contains(parent))       //if two colliders do not share a joint
        {
            gameObject.GetComponent<MeshRenderer>().material = SetCollider.GetComponent<MeshRenderer>().materials[1];
            Debug.Log(gameObject.name + " collides with " + other.gameObject.name);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        gameObject.GetComponent<MeshRenderer>().material = SetCollider.GetComponent<MeshRenderer>().materials[0];
    }
}
