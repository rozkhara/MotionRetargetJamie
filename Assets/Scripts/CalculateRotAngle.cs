using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CalculateRotAngle : MonoBehaviour
{
    private GameObject wrapper;
    public Transform rootNode;
    public Transform[] childNodes;
    public Vector3[] nodeRotations;
    public static Transform[] nodesInitVal;

    private JointPoint[] jp;
    public JointPoint[] JointPoints { get; }

    private TposeAlignment TPA;

    public class JointPoint
    {
        public Vector3 jointPosition = new Vector3();
        public Transform boneTransform = null;
        public Quaternion initRotation;
        public Quaternion inverse;
        public Quaternion inverseRotation;

        public JointPoint child = null;
        public JointPoint parent = null;
    }


    private void Awake()
    {
        PopulateChildren();
        nodesInitVal = childNodes;
    }
    private void Update()
    {
        TPA = GameObject.Find("sourceTpose").GetComponent<TposeAlignment>();
        if (!TPA.flag)
        {

        }
    }

    public JointPoint[] Init()
    {
        jp = new JointPoint[(int)Constants.SourcePositionIndex.Count];
        for (int i = 0; i < (int)Constants.SourcePositionIndex.Count; i++)
        {
            JointPoints[i] = new JointPoint();
        }

        //Right Leg
        JointPoints[(int)Constants.SourcePositionIndex.right_hip].boneTransform = TPA.childNodes[(int)SourceBoneIndex.rUpperLeg];
        JointPoints[(int)Constants.SourcePositionIndex.right_knee].boneTransform = TPA.childNodes[(int)SourceBoneIndex.rLowerLeg];
        JointPoints[(int)Constants.SourcePositionIndex.right_foot].boneTransform = TPA.childNodes[(int)SourceBoneIndex.rFoot];

        //Left Leg
        JointPoints[(int)Constants.SourcePositionIndex.left_hip].boneTransform = TPA.childNodes[(int)SourceBoneIndex.lUpperLeg];
        JointPoints[(int)Constants.SourcePositionIndex.left_knee].boneTransform = TPA.childNodes[(int)SourceBoneIndex.lLowerLeg];
        JointPoints[(int)Constants.SourcePositionIndex.left_foot].boneTransform = TPA.childNodes[(int)SourceBoneIndex.lFoot];

        //Spinal
        JointPoints[(int)Constants.SourcePositionIndex.center_torso].boneTransform = TPA.childNodes[(int)SourceBoneIndex.spine];
        JointPoints[(int)Constants.SourcePositionIndex.upper_torso].boneTransform = TPA.childNodes[(int)SourceBoneIndex.chest];
        JointPoints[(int)Constants.SourcePositionIndex.neck_base].boneTransform = TPA.childNodes[(int)SourceBoneIndex.neck];
        JointPoints[(int)Constants.SourcePositionIndex.center_head].boneTransform = TPA.childNodes[(int)SourceBoneIndex.head];

        //Left Arm
        JointPoints[(int)Constants.SourcePositionIndex.left_shoulder].boneTransform = TPA.childNodes[(int)SourceBoneIndex.lUpperArm];
        JointPoints[(int)Constants.SourcePositionIndex.left_elbow].boneTransform = TPA.childNodes[(int)SourceBoneIndex.lLowerArm];
        JointPoints[(int)Constants.SourcePositionIndex.left_hand].boneTransform = TPA.childNodes[(int)SourceBoneIndex.lHand];

        //Right Arm
        JointPoints[(int)Constants.SourcePositionIndex.right_shoulder].boneTransform = TPA.childNodes[(int)SourceBoneIndex.rUpperArm];
        JointPoints[(int)Constants.SourcePositionIndex.right_elbow].boneTransform = TPA.childNodes[(int)SourceBoneIndex.rLowerArm];
        JointPoints[(int)Constants.SourcePositionIndex.right_hand].boneTransform = TPA.childNodes[(int)SourceBoneIndex.rHand];

        //Child Settings
        //Right Leg


        //temporary return value as unity just doesnt want to run without it
        return new JointPoint[0];
    }

    public void CalCurRot()
    {
        Vector3 forward = TriangleNormal(childNodes[0].position, childNodes[20].position, childNodes[24].position);

    }

    public Quaternion GetInverse(Transform p1, Transform p2, Vector3 vec)
    {
        return Quaternion.Inverse(Quaternion.LookRotation(p1.position - p2.position, vec));
    }

    Vector3 TriangleNormal(Vector3 posA, Vector3 posB, Vector3 posC)
    {
        Vector3 dist1 = posA - posB;
        Vector3 dist2 = posA - posC;

        Vector3 returnVal = Vector3.Cross(dist1, dist2);
        return Vector3.Normalize(returnVal);
    }

    public void PopulateChildren()
    {
        childNodes = rootNode.GetComponentsInChildren<Transform>();
    }
}
