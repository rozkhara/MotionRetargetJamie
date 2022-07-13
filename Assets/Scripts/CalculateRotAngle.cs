using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CalculateRotAngle : MonoBehaviour
{
    private static readonly int _JOINTCOUNT = Constants.JOINTCOUNT;
    private static readonly int _TARGETFRAME = Constants.TARGETFRAME;
    private static readonly int _SCALEFACTOR = Constants.SCALEFACTOR;

    public string[][] sData = new string[_JOINTCOUNT * _TARGETFRAME][];

    public Transform rootNode;
    public Transform[] childNodes;
    public Vector3[] nodeRotations;
    public static Transform[] nodesInitVal;

    private JointPoint[] jointPoints;
    public  JointPoint[] JointPoints { get { return jointPoints; } }

    private bool flag = true;

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

    private void OnDrawGizmos()
    {
        PopulateChildren();
        nodesInitVal = childNodes;
    }

    private void Awake()
    {
        //PopulateChildren();
        //nodesInitVal = childNodes;
    }
    private void Update()
    {
        if (flag)
        {
            //try
            //{
                Init();
            //}
            //catch(System.Exception e)
            //{
            //    Debug.Log(e.Message);
            //}
            flag = false;
        }
        RotUpdate();
    }

    public JointPoint[] Init()
    {
        jointPoints = new JointPoint[(int)Constants.TargetPositionIndex.Count];
        for (int i = 0; i < (int)Constants.TargetPositionIndex.Count; i++)
        {
            JointPoints[i] = new JointPoint();
        }

        //Right Leg
        jointPoints[(int)Constants.TargetPositionIndex.Cha_UpperLegR].boneTransform = childNodes[(int)Constants.TargetPositionIndex.Cha_UpperLegR];
        jointPoints[(int)Constants.TargetPositionIndex.Cha_LowerLegR].boneTransform = childNodes[(int)Constants.TargetPositionIndex.Cha_LowerLegR];
        jointPoints[(int)Constants.TargetPositionIndex.Cha_FootR].boneTransform = childNodes[(int)Constants.TargetPositionIndex.Cha_FootR];
        //jointPoints[(int)Constants.TargetPositionIndex.Cha_ToeR].boneTransform = childNodes[(int)Constants.TargetPositionIndex.Cha_ToeR];

        //Left Leg
        jointPoints[(int)Constants.TargetPositionIndex.Cha_UpperLegL].boneTransform = childNodes[(int)Constants.TargetPositionIndex.Cha_UpperLegL];
        jointPoints[(int)Constants.TargetPositionIndex.Cha_LowerLegL].boneTransform = childNodes[(int)Constants.TargetPositionIndex.Cha_LowerLegL];
        jointPoints[(int)Constants.TargetPositionIndex.Cha_FootL].boneTransform = childNodes[(int)Constants.TargetPositionIndex.Cha_FootL];
        //jointPoints[(int)Constants.TargetPositionIndex.Cha_ToeL].boneTransform = childNodes[(int)Constants.TargetPositionIndex.Cha_ToeL];

        //Spinal
        jointPoints[(int)Constants.TargetPositionIndex.Cha_Hips].boneTransform = childNodes[(int)Constants.TargetPositionIndex.Cha_Hips];
        jointPoints[(int)Constants.TargetPositionIndex.Cha_Spine].boneTransform = childNodes[(int)Constants.TargetPositionIndex.Cha_Spine];
        jointPoints[(int)Constants.TargetPositionIndex.Cha_Chest].boneTransform = childNodes[(int)Constants.TargetPositionIndex.Cha_Chest];
        jointPoints[(int)Constants.TargetPositionIndex.Face].boneTransform = childNodes[(int)Constants.TargetPositionIndex.Face];
        //jointPoints[(int)Constants.TargetPositionIndex.Cha_HeadDummy].boneTransform =

        //Left Arm
        jointPoints[(int)Constants.TargetPositionIndex.Cha_UpperArmL].boneTransform = childNodes[(int)Constants.TargetPositionIndex.Cha_UpperArmL];
        jointPoints[(int)Constants.TargetPositionIndex.Cha_LowerArmL].boneTransform = childNodes[(int)Constants.TargetPositionIndex.Cha_LowerArmL];
        jointPoints[(int)Constants.TargetPositionIndex.Cha_HandL].boneTransform = childNodes[(int)Constants.TargetPositionIndex.Cha_HandL];
        //jointPoints[(int)Constants.TargetPositionIndex.BodyHandL].boneTransform =

        //Right Arm
        jointPoints[(int)Constants.TargetPositionIndex.Cha_UpperArmR].boneTransform = childNodes[(int)Constants.TargetPositionIndex.Cha_UpperArmR];
        jointPoints[(int)Constants.TargetPositionIndex.Cha_LowerArmR].boneTransform = childNodes[(int)Constants.TargetPositionIndex.Cha_LowerArmR];
        jointPoints[(int)Constants.TargetPositionIndex.Cha_HandR].boneTransform = childNodes[(int)Constants.TargetPositionIndex.Cha_HandR];
        //jointPoints[(int)Constants.TargetPositionIndex.BodyHandR].boneTransform = 

        //Child Settings
        //Right Leg
        jointPoints[(int)Constants.TargetPositionIndex.Cha_UpperLegR].child = jointPoints[(int)Constants.TargetPositionIndex.Cha_LowerLegR];
        jointPoints[(int)Constants.TargetPositionIndex.Cha_LowerLegR].child = jointPoints[(int)Constants.TargetPositionIndex.Cha_FootR];
        //jointPoints[(int)Constants.TargetPositionIndex.Cha_FootR].child = jointPoints[(int)Constants.TargetPositionIndex.Cha_ToeR];
        //jointPoints[(int)Constants.TargetPositionIndex.Cha_FootR].parent = jointPoints[(int)Constants.TargetPositionIndex.Cha_LowerLegR];
        jointPoints[(int)Constants.TargetPositionIndex.Cha_LowerLegR].parent = jointPoints[(int)Constants.TargetPositionIndex.Cha_UpperLegR];

        //Left Leg
        jointPoints[(int)Constants.TargetPositionIndex.Cha_UpperLegL].child = jointPoints[(int)Constants.TargetPositionIndex.Cha_LowerLegL];
        jointPoints[(int)Constants.TargetPositionIndex.Cha_LowerLegL].child = jointPoints[(int)Constants.TargetPositionIndex.Cha_FootL];
        //jointPoints[(int)Constants.TargetPositionIndex.Cha_FootL].child = jointPoints[(int)Constants.TargetPositionIndex.Cha_ToeL];
        //jointPoints[(int)Constants.TargetPositionIndex.Cha_FootL].parent = jointPoints[(int)Constants.TargetPositionIndex.Cha_LowerLegL];
        jointPoints[(int)Constants.TargetPositionIndex.Cha_LowerLegL].parent = jointPoints[(int)Constants.TargetPositionIndex.Cha_UpperLegL];

        //Spinal
        jointPoints[(int)Constants.TargetPositionIndex.Cha_Spine].child = jointPoints[(int)Constants.TargetPositionIndex.Cha_Chest];
        jointPoints[(int)Constants.TargetPositionIndex.Cha_Chest].child = jointPoints[(int)Constants.TargetPositionIndex.Face];

        //Left Arm
        jointPoints[(int)Constants.TargetPositionIndex.Cha_UpperArmL].child = jointPoints[(int)Constants.TargetPositionIndex.Cha_LowerArmL];
        jointPoints[(int)Constants.TargetPositionIndex.Cha_LowerArmL].child = jointPoints[(int)Constants.TargetPositionIndex.Cha_HandL];
        jointPoints[(int)Constants.TargetPositionIndex.Cha_LowerArmL].parent = jointPoints[(int)Constants.TargetPositionIndex.Cha_UpperArmL];

        //Right Arm
        jointPoints[(int)Constants.TargetPositionIndex.Cha_UpperArmR].child = jointPoints[(int)Constants.TargetPositionIndex.Cha_LowerArmR];
        jointPoints[(int)Constants.TargetPositionIndex.Cha_LowerArmR].child = jointPoints[(int)Constants.TargetPositionIndex.Cha_HandR];
        jointPoints[(int)Constants.TargetPositionIndex.Cha_LowerArmR].parent = jointPoints[(int)Constants.TargetPositionIndex.Cha_UpperArmR];

        //Set Inverse
        Vector3 forward = TriangleNormal(jointPoints[(int)Constants.TargetPositionIndex.Cha_Spine].boneTransform.position,
            jointPoints[(int)Constants.TargetPositionIndex.Cha_UpperLegL].boneTransform.position,
            jointPoints[(int)Constants.TargetPositionIndex.Cha_UpperLegR].boneTransform.position);
        foreach (JointPoint jp in JointPoints)
        {
            if (jp.boneTransform != null)
            {
                jp.initRotation = jp.boneTransform.rotation;
            }

            if (jp.child != null)
            {
                jp.inverse = GetInverse(jp, jp.child, forward);
                jp.inverseRotation = jp.inverse * jp.initRotation;
            }
        }

        return JointPoints;
    }

    public void RotUpdate()
    {
        GetNT();
        Vector3 forward = TriangleNormal(jointPoints[(int)Constants.TargetPositionIndex.Cha_Spine].jointPosition,
            jointPoints[(int)Constants.TargetPositionIndex.Cha_UpperLegL].jointPosition,
           jointPoints[(int)Constants.TargetPositionIndex.Cha_UpperLegR].jointPosition);

        foreach (JointPoint jp in jointPoints)
        {
            if (jp.parent != null)
            {
                Vector3 fv = jp.parent.jointPosition - jp.jointPosition;
                jp.boneTransform.rotation = Quaternion.LookRotation(jp.jointPosition - jp.child.jointPosition, fv) * jp.inverseRotation;
            }
            else if (jp.child != null)
            {
                jp.boneTransform.rotation = Quaternion.LookRotation(jp.jointPosition - jp.child.jointPosition, forward) * jp.inverseRotation;
            }
        }

        //Right Leg
        childNodes[(int)Constants.TargetPositionIndex.Cha_UpperLegR] = jointPoints[(int)Constants.TargetPositionIndex.Cha_UpperLegR].boneTransform;
        childNodes[(int)Constants.TargetPositionIndex.Cha_LowerLegR] = jointPoints[(int)Constants.TargetPositionIndex.Cha_LowerLegR].boneTransform;
        childNodes[(int)Constants.TargetPositionIndex.Cha_FootR] = jointPoints[(int)Constants.TargetPositionIndex.Cha_FootR].boneTransform;
        childNodes[(int)Constants.TargetPositionIndex.Cha_ToeR] = //jointPoints[(int)Constants.TargetPositionIndex.Cha_ToeR].boneTransform;

        //Left Leg
        childNodes[(int)Constants.TargetPositionIndex.Cha_UpperLegL] = jointPoints[(int)Constants.TargetPositionIndex.Cha_UpperLegL].boneTransform;
        childNodes[(int)Constants.TargetPositionIndex.Cha_LowerLegL] = jointPoints[(int)Constants.TargetPositionIndex.Cha_LowerLegL].boneTransform;
        childNodes[(int)Constants.TargetPositionIndex.Cha_FootL] = jointPoints[(int)Constants.TargetPositionIndex.Cha_FootL].boneTransform;
        childNodes[(int)Constants.TargetPositionIndex.Cha_ToeL] = //jointPoints[(int)Constants.TargetPositionIndex.Cha_ToeL].boneTransform;

        //Spinal
        childNodes[(int)Constants.TargetPositionIndex.Cha_Hips] = jointPoints[(int)Constants.TargetPositionIndex.Cha_Hips].boneTransform;
        childNodes[(int)Constants.TargetPositionIndex.Cha_Spine] = jointPoints[(int)Constants.TargetPositionIndex.Cha_Spine].boneTransform;
        childNodes[(int)Constants.TargetPositionIndex.Cha_Chest] = jointPoints[(int)Constants.TargetPositionIndex.Cha_Chest].boneTransform;
        childNodes[(int)Constants.TargetPositionIndex.Face] = jointPoints[(int)Constants.TargetPositionIndex.Face].boneTransform;
        //jointPoints[(int)Constants.TargetPositionIndex.Cha_HeadDummy].boneTransform =

        //Left Arm
        childNodes[(int)Constants.TargetPositionIndex.Cha_UpperArmL] = jointPoints[(int)Constants.TargetPositionIndex.Cha_UpperArmL].boneTransform;
        childNodes[(int)Constants.TargetPositionIndex.Cha_LowerArmL] = jointPoints[(int)Constants.TargetPositionIndex.Cha_LowerArmL].boneTransform;
        childNodes[(int)Constants.TargetPositionIndex.Cha_HandL] = jointPoints[(int)Constants.TargetPositionIndex.Cha_HandL].boneTransform;
        //jointPoints[(int)Constants.TargetPositionIndex.BodyHandL].boneTransform =

        //Right Arm
        childNodes[(int)Constants.TargetPositionIndex.Cha_UpperArmR] = jointPoints[(int)Constants.TargetPositionIndex.Cha_UpperArmR].boneTransform;
        childNodes[(int)Constants.TargetPositionIndex.Cha_LowerArmR] = jointPoints[(int)Constants.TargetPositionIndex.Cha_LowerArmR].boneTransform;
        childNodes[(int)Constants.TargetPositionIndex.Cha_HandR] = jointPoints[(int)Constants.TargetPositionIndex.Cha_HandR].boneTransform;
        //jointPoints[(int)Constants.TargetPositionIndex.BodyHandR].boneTransform = 

        //Vector3 forward = TriangleNormal(jointPoints[(int)Constants.TargetPositionIndex.Cha_Spine], jointPoints[(int)Constants.TargetPositionIndex.Cha_UpperLegL], )
    }

    public Quaternion GetInverse(JointPoint p1, JointPoint p2, Vector3 vec)
    {
        return Quaternion.Inverse(Quaternion.LookRotation(p1.boneTransform.position - p2.boneTransform.position, vec));
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

    private void GetNT()
    {
        sData = DataProcess.Instance.GetSData();

        //Right Leg
        jointPoints[(int)Constants.TargetPositionIndex.Cha_UpperLegR].jointPosition =
            new Vector3(float.Parse(sData[DataProcess.Instance.chunkIndex + (int)Constants.SourcePositionIndex.right_hip][2]) / _SCALEFACTOR,
                    float.Parse(sData[DataProcess.Instance.chunkIndex + (int)Constants.SourcePositionIndex.right_hip][4]) / _SCALEFACTOR,
                    float.Parse(sData[DataProcess.Instance.chunkIndex + (int)Constants.SourcePositionIndex.right_hip][3]) / _SCALEFACTOR);
        jointPoints[(int)Constants.TargetPositionIndex.Cha_LowerLegR].jointPosition =
            new Vector3(float.Parse(sData[DataProcess.Instance.chunkIndex + (int)Constants.SourcePositionIndex.right_knee][2]) / _SCALEFACTOR,
                    float.Parse(sData[DataProcess.Instance.chunkIndex + (int)Constants.SourcePositionIndex.right_knee][4]) / _SCALEFACTOR,
                    float.Parse(sData[DataProcess.Instance.chunkIndex + (int)Constants.SourcePositionIndex.right_knee][3]) / _SCALEFACTOR);
        jointPoints[(int)Constants.TargetPositionIndex.Cha_FootR].jointPosition =
            new Vector3(float.Parse(sData[DataProcess.Instance.chunkIndex + (int)Constants.SourcePositionIndex.right_foot][2]) / _SCALEFACTOR,
                    float.Parse(sData[DataProcess.Instance.chunkIndex + (int)Constants.SourcePositionIndex.right_foot][4]) / _SCALEFACTOR,
                    float.Parse(sData[DataProcess.Instance.chunkIndex + (int)Constants.SourcePositionIndex.right_foot][3]) / _SCALEFACTOR);

        //Left Leg
        jointPoints[(int)Constants.TargetPositionIndex.Cha_UpperLegL].jointPosition =
            new Vector3(float.Parse(sData[DataProcess.Instance.chunkIndex + (int)Constants.SourcePositionIndex.left_hip][2]) / _SCALEFACTOR,
                    float.Parse(sData[DataProcess.Instance.chunkIndex + (int)Constants.SourcePositionIndex.left_hip][4]) / _SCALEFACTOR,
                    float.Parse(sData[DataProcess.Instance.chunkIndex + (int)Constants.SourcePositionIndex.left_hip][3]) / _SCALEFACTOR);
        jointPoints[(int)Constants.TargetPositionIndex.Cha_LowerLegL].jointPosition =
            new Vector3(float.Parse(sData[DataProcess.Instance.chunkIndex + (int)Constants.SourcePositionIndex.left_knee][2]) / _SCALEFACTOR,
                    float.Parse(sData[DataProcess.Instance.chunkIndex + (int)Constants.SourcePositionIndex.left_knee][4]) / _SCALEFACTOR,
                    float.Parse(sData[DataProcess.Instance.chunkIndex + (int)Constants.SourcePositionIndex.left_knee][3]) / _SCALEFACTOR);
        jointPoints[(int)Constants.TargetPositionIndex.Cha_FootL].jointPosition =
            new Vector3(float.Parse(sData[DataProcess.Instance.chunkIndex + (int)Constants.SourcePositionIndex.left_foot][2]) / _SCALEFACTOR,
                    float.Parse(sData[DataProcess.Instance.chunkIndex + (int)Constants.SourcePositionIndex.left_foot][4]) / _SCALEFACTOR,
                    float.Parse(sData[DataProcess.Instance.chunkIndex + (int)Constants.SourcePositionIndex.left_foot][3]) / _SCALEFACTOR);

        //Spinal
        jointPoints[(int)Constants.TargetPositionIndex.Cha_Hips].jointPosition =
            new Vector3(float.Parse(sData[DataProcess.Instance.chunkIndex + (int)Constants.SourcePositionIndex.bottom_torso][2]) / _SCALEFACTOR,
                    float.Parse(sData[DataProcess.Instance.chunkIndex + (int)Constants.SourcePositionIndex.bottom_torso][4]) / _SCALEFACTOR,
                    float.Parse(sData[DataProcess.Instance.chunkIndex + (int)Constants.SourcePositionIndex.bottom_torso][3]) / _SCALEFACTOR);
        jointPoints[(int)Constants.TargetPositionIndex.Cha_Spine].jointPosition =
            new Vector3(float.Parse(sData[DataProcess.Instance.chunkIndex + (int)Constants.SourcePositionIndex.center_torso][2]) / _SCALEFACTOR,
                    float.Parse(sData[DataProcess.Instance.chunkIndex + (int)Constants.SourcePositionIndex.center_torso][4]) / _SCALEFACTOR,
                    float.Parse(sData[DataProcess.Instance.chunkIndex + (int)Constants.SourcePositionIndex.center_torso][3]) / _SCALEFACTOR);
        jointPoints[(int)Constants.TargetPositionIndex.Cha_Chest].jointPosition =
            new Vector3(float.Parse(sData[DataProcess.Instance.chunkIndex + (int)Constants.SourcePositionIndex.upper_torso][2]) / _SCALEFACTOR,
                    float.Parse(sData[DataProcess.Instance.chunkIndex + (int)Constants.SourcePositionIndex.upper_torso][4]) / _SCALEFACTOR,
                    float.Parse(sData[DataProcess.Instance.chunkIndex + (int)Constants.SourcePositionIndex.upper_torso][3]) / _SCALEFACTOR);
        jointPoints[(int)Constants.TargetPositionIndex.Face].jointPosition =
            new Vector3(float.Parse(sData[DataProcess.Instance.chunkIndex + (int)Constants.SourcePositionIndex.center_head][2]) / _SCALEFACTOR,
                    float.Parse(sData[DataProcess.Instance.chunkIndex + (int)Constants.SourcePositionIndex.center_head][4]) / _SCALEFACTOR,
                    float.Parse(sData[DataProcess.Instance.chunkIndex + (int)Constants.SourcePositionIndex.center_head][3]) / _SCALEFACTOR);

        //Left Arm
        jointPoints[(int)Constants.TargetPositionIndex.Cha_UpperArmL].jointPosition =
            new Vector3(float.Parse(sData[DataProcess.Instance.chunkIndex + (int)Constants.SourcePositionIndex.left_shoulder][2]) / _SCALEFACTOR,
                    float.Parse(sData[DataProcess.Instance.chunkIndex + (int)Constants.SourcePositionIndex.left_shoulder][4]) / _SCALEFACTOR,
                    float.Parse(sData[DataProcess.Instance.chunkIndex + (int)Constants.SourcePositionIndex.left_shoulder][3]) / _SCALEFACTOR);
        jointPoints[(int)Constants.TargetPositionIndex.Cha_LowerArmL].jointPosition =
            new Vector3(float.Parse(sData[DataProcess.Instance.chunkIndex + (int)Constants.SourcePositionIndex.left_elbow][2]) / _SCALEFACTOR,
                    float.Parse(sData[DataProcess.Instance.chunkIndex + (int)Constants.SourcePositionIndex.left_elbow][4]) / _SCALEFACTOR,
                    float.Parse(sData[DataProcess.Instance.chunkIndex + (int)Constants.SourcePositionIndex.left_elbow][3]) / _SCALEFACTOR);
        jointPoints[(int)Constants.TargetPositionIndex.Cha_HandL].jointPosition =
            new Vector3(float.Parse(sData[DataProcess.Instance.chunkIndex + (int)Constants.SourcePositionIndex.left_hand][2]) / _SCALEFACTOR,
                    float.Parse(sData[DataProcess.Instance.chunkIndex + (int)Constants.SourcePositionIndex.left_hand][4]) / _SCALEFACTOR,
                    float.Parse(sData[DataProcess.Instance.chunkIndex + (int)Constants.SourcePositionIndex.left_hand][3]) / _SCALEFACTOR);

        //Right Arm
        jointPoints[(int)Constants.TargetPositionIndex.Cha_UpperArmR].jointPosition =
            new Vector3(float.Parse(sData[DataProcess.Instance.chunkIndex + (int)Constants.SourcePositionIndex.right_shoulder][2]) / _SCALEFACTOR,
                    float.Parse(sData[DataProcess.Instance.chunkIndex + (int)Constants.SourcePositionIndex.right_shoulder][4]) / _SCALEFACTOR,
                    float.Parse(sData[DataProcess.Instance.chunkIndex + (int)Constants.SourcePositionIndex.right_shoulder][3]) / _SCALEFACTOR);
        jointPoints[(int)Constants.TargetPositionIndex.Cha_LowerArmR].jointPosition =
            new Vector3(float.Parse(sData[DataProcess.Instance.chunkIndex + (int)Constants.SourcePositionIndex.right_elbow][2]) / _SCALEFACTOR,
                    float.Parse(sData[DataProcess.Instance.chunkIndex + (int)Constants.SourcePositionIndex.right_elbow][4]) / _SCALEFACTOR,
                    float.Parse(sData[DataProcess.Instance.chunkIndex + (int)Constants.SourcePositionIndex.right_elbow][3]) / _SCALEFACTOR);
        jointPoints[(int)Constants.TargetPositionIndex.Cha_HandR].jointPosition =
            new Vector3(float.Parse(sData[DataProcess.Instance.chunkIndex + (int)Constants.SourcePositionIndex.right_hand][2]) / _SCALEFACTOR,
                    float.Parse(sData[DataProcess.Instance.chunkIndex + (int)Constants.SourcePositionIndex.right_hand][4]) / _SCALEFACTOR,
                    float.Parse(sData[DataProcess.Instance.chunkIndex + (int)Constants.SourcePositionIndex.right_hand][3]) / _SCALEFACTOR);
    }
}
