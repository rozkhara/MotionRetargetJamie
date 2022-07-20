using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ViewSkeleton : MonoBehaviour
{
    public Transform rootNode;
    public Transform[] childNodes;
    public Color myColor;

    void OnDrawGizmos()
    {
        if (rootNode != null)
        {
            if (childNodes == null || childNodes.Length == 0)
            {
                //get all joints to draw
                PopulateChildren();
            }
            foreach (Transform child in childNodes)
            {
                if (child == rootNode)
                {
                    //list includes the root, if root then larger, green cube
                    Gizmos.color = myColor;
                    Gizmos.DrawSphere(child.position, 0.05f);
                }
                else
                {
                    Gizmos.color = myColor;
                    Gizmos.DrawLine(child.position, child.parent.position);
                    Gizmos.DrawSphere(child.position, 0.01f);
                }
            }

        }
    }

    public void PopulateChildren()
    {
        childNodes = rootNode.GetComponentsInChildren<Transform>();
    }
}
