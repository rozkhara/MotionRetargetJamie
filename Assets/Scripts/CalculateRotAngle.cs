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

    public Transform s_rootNode;
    public Transform[] s_childNodes;

    private Vector3 initPosition;
    private Vector3 s_initPosition;

    public Transform parentTransform;
    public Transform s_parentTransform;

    private TposeAlignment TPA;

    private JointPoint[] jointPoints;
    public JointPoint[] JointPoints { get { return jointPoints; } }

    private JointPoint[] s_jointPoints;
    public JointPoint[] s_JointPoints { get { return s_jointPoints; } }


    private bool flag = true;
    private bool t_flag = true;

    public class JointPoint
    {
        public Vector3 jointPosition = new();
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
    }

    private void Awake()
    {
        TPA = GameObject.Find("sourceTpose").GetComponent<TposeAlignment>();
    }

    private void Update()
    {
        if (flag)
        {
            Init();
            flag = false;
        }
        if (t_flag)
        {
            if (!TPA.flag)
            {
                Init_s();
                t_flag = false;
            }
        }
        if (DataProcess.Instance.parseFlag)
        {
            if (!flag)
            {
                RotUpdate();
            }
            if (!t_flag)
            {
                RotUpdate_s();
            }
        }
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
        jointPoints[(int)Constants.TargetPositionIndex.Cha_FootR].parent = jointPoints[(int)Constants.TargetPositionIndex.Cha_LowerLegR];

        //Left Leg
        jointPoints[(int)Constants.TargetPositionIndex.Cha_UpperLegL].child = jointPoints[(int)Constants.TargetPositionIndex.Cha_LowerLegL];
        jointPoints[(int)Constants.TargetPositionIndex.Cha_LowerLegL].child = jointPoints[(int)Constants.TargetPositionIndex.Cha_FootL];
        //jointPoints[(int)Constants.TargetPositionIndex.Cha_FootL].child = jointPoints[(int)Constants.TargetPositionIndex.Cha_ToeL];
        //jointPoints[(int)Constants.TargetPositionIndex.Cha_FootL].parent = jointPoints[(int)Constants.TargetPositionIndex.Cha_LowerLegL];
        jointPoints[(int)Constants.TargetPositionIndex.Cha_FootL].parent = jointPoints[(int)Constants.TargetPositionIndex.Cha_LowerLegR];

        //Spinal
        jointPoints[(int)Constants.TargetPositionIndex.Cha_Spine].child = jointPoints[(int)Constants.TargetPositionIndex.Cha_Chest];
        jointPoints[(int)Constants.TargetPositionIndex.Cha_Chest].child = jointPoints[(int)Constants.TargetPositionIndex.Face];

        //Left Arm
        jointPoints[(int)Constants.TargetPositionIndex.Cha_UpperArmL].child = jointPoints[(int)Constants.TargetPositionIndex.Cha_LowerArmL];
        jointPoints[(int)Constants.TargetPositionIndex.Cha_LowerArmL].child = jointPoints[(int)Constants.TargetPositionIndex.Cha_HandL];
        jointPoints[(int)Constants.TargetPositionIndex.Cha_HandL].parent = jointPoints[(int)Constants.TargetPositionIndex.Cha_LowerArmL];

        //Right Arm
        jointPoints[(int)Constants.TargetPositionIndex.Cha_UpperArmR].child = jointPoints[(int)Constants.TargetPositionIndex.Cha_LowerArmR];
        jointPoints[(int)Constants.TargetPositionIndex.Cha_LowerArmR].child = jointPoints[(int)Constants.TargetPositionIndex.Cha_HandR];
        jointPoints[(int)Constants.TargetPositionIndex.Cha_HandR].parent = jointPoints[(int)Constants.TargetPositionIndex.Cha_LowerArmR];

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
        initPosition = jointPoints[(int)Constants.TargetPositionIndex.Cha_Hips].boneTransform.position;
        jointPoints[(int)Constants.TargetPositionIndex.Cha_Hips].inverse = Quaternion.Inverse(Quaternion.LookRotation(forward));
        jointPoints[(int)Constants.TargetPositionIndex.Cha_Hips].inverseRotation = jointPoints[(int)Constants.TargetPositionIndex.Cha_Hips].inverse * jointPoints[(int)Constants.TargetPositionIndex.Cha_Hips].initRotation;

        return JointPoints;
    }

    public JointPoint[] Init_s()
    {
        s_jointPoints = new JointPoint[(int)Constants.SourcePositionIndex.Count];
        for (int i = 0; i < (int)Constants.SourcePositionIndex.Count; i++)
        {
            s_JointPoints[i] = new JointPoint();
        }

        //Right Leg
        s_jointPoints[(int)Constants.SourcePositionIndex.right_hip].boneTransform = s_childNodes[(int)Constants.SourcePositionIndex.right_hip];
        s_jointPoints[(int)Constants.SourcePositionIndex.right_knee].boneTransform = s_childNodes[(int)Constants.SourcePositionIndex.right_knee];
        s_jointPoints[(int)Constants.SourcePositionIndex.right_foot].boneTransform = s_childNodes[(int)Constants.SourcePositionIndex.right_foot];

        //Left Leg
        s_jointPoints[(int)Constants.SourcePositionIndex.left_hip].boneTransform = s_childNodes[(int)Constants.SourcePositionIndex.left_hip];
        s_jointPoints[(int)Constants.SourcePositionIndex.left_knee].boneTransform = s_childNodes[(int)Constants.SourcePositionIndex.left_knee];
        s_jointPoints[(int)Constants.SourcePositionIndex.left_foot].boneTransform = s_childNodes[(int)Constants.SourcePositionIndex.left_foot];

        //Spinal
        s_jointPoints[(int)Constants.SourcePositionIndex.bottom_torso].boneTransform = s_childNodes[(int)Constants.SourcePositionIndex.bottom_torso];
        s_jointPoints[(int)Constants.SourcePositionIndex.center_torso].boneTransform = s_childNodes[(int)Constants.SourcePositionIndex.center_torso];
        s_jointPoints[(int)Constants.SourcePositionIndex.upper_torso].boneTransform = s_childNodes[(int)Constants.SourcePositionIndex.upper_torso];
        s_jointPoints[(int)Constants.SourcePositionIndex.neck_base].boneTransform = s_childNodes[(int)Constants.SourcePositionIndex.neck_base];
        s_jointPoints[(int)Constants.SourcePositionIndex.center_head].boneTransform = s_childNodes[(int)Constants.SourcePositionIndex.center_head];
        //s_jointPoints[(int)Constants.SourcePositionIndex.Cha_HeadDummy].boneTransform =

        //Left Arm
        s_jointPoints[(int)Constants.SourcePositionIndex.left_shoulder].boneTransform = s_childNodes[(int)Constants.SourcePositionIndex.left_shoulder];
        s_jointPoints[(int)Constants.SourcePositionIndex.left_elbow].boneTransform = s_childNodes[(int)Constants.SourcePositionIndex.left_elbow];
        s_jointPoints[(int)Constants.SourcePositionIndex.left_hand].boneTransform = s_childNodes[(int)Constants.SourcePositionIndex.left_hand];
        //s_jointPoints[(int)Constants.SourcePositionIndex.BodyHandL].boneTransform =

        //Right Arm
        s_jointPoints[(int)Constants.SourcePositionIndex.right_shoulder].boneTransform = s_childNodes[(int)Constants.SourcePositionIndex.right_shoulder];
        s_jointPoints[(int)Constants.SourcePositionIndex.right_elbow].boneTransform = s_childNodes[(int)Constants.SourcePositionIndex.right_elbow];
        s_jointPoints[(int)Constants.SourcePositionIndex.right_hand].boneTransform = s_childNodes[(int)Constants.SourcePositionIndex.right_hand];
        //s_jointPoints[(int)Constants.SourcePositionIndex.BodyHandR].boneTransform = 

        //s_child Settings
        //Right Leg
        s_jointPoints[(int)Constants.SourcePositionIndex.right_hip].child = s_jointPoints[(int)Constants.SourcePositionIndex.right_knee];
        s_jointPoints[(int)Constants.SourcePositionIndex.right_knee].child = s_jointPoints[(int)Constants.SourcePositionIndex.right_foot];
        //s_jointPoints[(int)Constants.SourcePositionIndex.Cha_FootR].child = s_jointPoints[(int)Constants.SourcePositionIndex.Cha_ToeR];
        //s_jointPoints[(int)Constants.SourcePositionIndex.Cha_FootR].parent = s_jointPoints[(int)Constants.SourcePositionIndex.Cha_LowerLegR];
        s_jointPoints[(int)Constants.SourcePositionIndex.right_foot].parent = s_jointPoints[(int)Constants.SourcePositionIndex.right_knee];

        //Left Leg
        s_jointPoints[(int)Constants.SourcePositionIndex.left_hip].child = s_jointPoints[(int)Constants.SourcePositionIndex.left_knee];
        s_jointPoints[(int)Constants.SourcePositionIndex.left_knee].child = s_jointPoints[(int)Constants.SourcePositionIndex.left_foot];
        //s_jointPoints[(int)Constants.SourcePositionIndex.Cha_FootL].child = s_jointPoints[(int)Constants.SourcePositionIndex.Cha_ToeL];
        //s_jointPoints[(int)Constants.SourcePositionIndex.Cha_FootL].parent = s_jointPoints[(int)Constants.SourcePositionIndex.Cha_LowerLegL];
        s_jointPoints[(int)Constants.SourcePositionIndex.left_foot].parent = s_jointPoints[(int)Constants.SourcePositionIndex.left_knee];

        //Spinal
        s_jointPoints[(int)Constants.SourcePositionIndex.center_torso].child = s_jointPoints[(int)Constants.SourcePositionIndex.upper_torso];
        s_jointPoints[(int)Constants.SourcePositionIndex.upper_torso].child = s_jointPoints[(int)Constants.SourcePositionIndex.neck_base];
        s_jointPoints[(int)Constants.SourcePositionIndex.neck_base].child = s_jointPoints[(int)Constants.SourcePositionIndex.center_head];

        //Left Arm
        s_jointPoints[(int)Constants.SourcePositionIndex.left_shoulder].child = s_jointPoints[(int)Constants.SourcePositionIndex.left_elbow];
        s_jointPoints[(int)Constants.SourcePositionIndex.left_elbow].child = s_jointPoints[(int)Constants.SourcePositionIndex.left_hand];
        s_jointPoints[(int)Constants.SourcePositionIndex.left_hand].parent = s_jointPoints[(int)Constants.SourcePositionIndex.left_elbow];

        //Right Arm
        s_jointPoints[(int)Constants.SourcePositionIndex.right_shoulder].child = s_jointPoints[(int)Constants.SourcePositionIndex.right_elbow];
        s_jointPoints[(int)Constants.SourcePositionIndex.right_elbow].child = s_jointPoints[(int)Constants.SourcePositionIndex.right_hand];
        s_jointPoints[(int)Constants.SourcePositionIndex.right_hand].parent = s_jointPoints[(int)Constants.SourcePositionIndex.right_elbow];

        //Set Inverse
        Vector3 forward = TriangleNormal(s_jointPoints[(int)Constants.SourcePositionIndex.center_torso].boneTransform.position,
            s_jointPoints[(int)Constants.SourcePositionIndex.left_hip].boneTransform.position,
            s_jointPoints[(int)Constants.SourcePositionIndex.right_hip].boneTransform.position);
        foreach (JointPoint jp in s_jointPoints)
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
        s_initPosition = s_jointPoints[(int)Constants.SourcePositionIndex.center_torso].boneTransform.position;
        s_jointPoints[(int)Constants.SourcePositionIndex.bottom_torso].inverse = Quaternion.Inverse(Quaternion.LookRotation(forward));
        s_jointPoints[(int)Constants.SourcePositionIndex.bottom_torso].inverseRotation = s_jointPoints[(int)Constants.SourcePositionIndex.bottom_torso].inverse * s_jointPoints[(int)Constants.SourcePositionIndex.bottom_torso].initRotation;


        return s_jointPoints;
    }

    public void RotUpdate()
    {
        GetNT();
        Vector3 forward = TriangleNormal(jointPoints[(int)Constants.TargetPositionIndex.Cha_Spine].jointPosition,
           jointPoints[(int)Constants.TargetPositionIndex.Cha_UpperLegL].jointPosition,
           jointPoints[(int)Constants.TargetPositionIndex.Cha_UpperLegR].jointPosition);
        jointPoints[(int)Constants.TargetPositionIndex.Cha_Hips].boneTransform.SetPositionAndRotation(jointPoints[(int)Constants.TargetPositionIndex.Cha_Hips].jointPosition + initPosition,
                                                                                                        Quaternion.LookRotation(forward) * jointPoints[(int)Constants.TargetPositionIndex.Cha_Hips].inverseRotation);
        foreach (JointPoint jp in jointPoints)
        {
            if (jp.parent != null)
            {
                if (jp.child != null)
                {
                    Vector3 fv = jp.parent.jointPosition - jp.jointPosition;
                    jp.boneTransform.rotation = Quaternion.LookRotation(jp.jointPosition - jp.child.jointPosition, fv) * jp.inverseRotation;
                }
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
        //childNodes[(int)Constants.TargetPositionIndex.Cha_ToeR] = jointPoints[(int)Constants.TargetPositionIndex.Cha_ToeR].boneTransform;

        //Left Leg
        childNodes[(int)Constants.TargetPositionIndex.Cha_UpperLegL] = jointPoints[(int)Constants.TargetPositionIndex.Cha_UpperLegL].boneTransform;
        childNodes[(int)Constants.TargetPositionIndex.Cha_LowerLegL] = jointPoints[(int)Constants.TargetPositionIndex.Cha_LowerLegL].boneTransform;
        childNodes[(int)Constants.TargetPositionIndex.Cha_FootL] = jointPoints[(int)Constants.TargetPositionIndex.Cha_FootL].boneTransform;
        //childNodes[(int)Constants.TargetPositionIndex.Cha_ToeL] = jointPoints[(int)Constants.TargetPositionIndex.Cha_ToeL].boneTransform;

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

    public void RotUpdate_s()
    {
        GetNT_s();
        Vector3 forward = TriangleNormal(s_jointPoints[(int)Constants.SourcePositionIndex.center_torso].jointPosition,
           s_jointPoints[(int)Constants.SourcePositionIndex.left_hip].jointPosition,
           s_jointPoints[(int)Constants.SourcePositionIndex.right_hip].jointPosition);
        s_jointPoints[(int)Constants.SourcePositionIndex.bottom_torso].boneTransform.SetPositionAndRotation(s_jointPoints[(int)Constants.SourcePositionIndex.bottom_torso].jointPosition + s_initPosition,
                                                                                                        Quaternion.LookRotation(forward) * s_jointPoints[(int)Constants.SourcePositionIndex.bottom_torso].inverseRotation);

        foreach (JointPoint jp in s_jointPoints)
        {
            if (jp.parent != null)
            {
                if (jp.child != null)
                {
                    Vector3 fv = jp.parent.jointPosition - jp.jointPosition;
                    jp.boneTransform.rotation = Quaternion.LookRotation(jp.jointPosition - jp.child.jointPosition, fv) * jp.inverseRotation;
                }
            }
            else if (jp.child != null)
            {
                jp.boneTransform.rotation = Quaternion.LookRotation(jp.jointPosition - jp.child.jointPosition, forward) * jp.inverseRotation;
            }
        }

        //Right Leg
        s_childNodes[(int)Constants.SourcePositionIndex.right_hip] = s_jointPoints[(int)Constants.SourcePositionIndex.right_hip].boneTransform;
        s_childNodes[(int)Constants.SourcePositionIndex.right_knee] = s_jointPoints[(int)Constants.SourcePositionIndex.right_knee].boneTransform;
        s_childNodes[(int)Constants.SourcePositionIndex.right_foot] = s_jointPoints[(int)Constants.SourcePositionIndex.right_foot].boneTransform;
        //s_childNodes[(int)Constants.SourcePositionIndex.Cha_ToeR] = s_jointPoints[(int)Constants.SourcePositionIndex.Cha_ToeR].boneTransform;

        //Left Leg
        s_childNodes[(int)Constants.SourcePositionIndex.left_hip] = s_jointPoints[(int)Constants.SourcePositionIndex.left_hip].boneTransform;
        s_childNodes[(int)Constants.SourcePositionIndex.left_knee] = s_jointPoints[(int)Constants.SourcePositionIndex.left_knee].boneTransform;
        s_childNodes[(int)Constants.SourcePositionIndex.left_foot] = s_jointPoints[(int)Constants.SourcePositionIndex.left_foot].boneTransform;
        //s_childNodes[(int)Constants.SourcePositionIndex.Cha_ToeL] = s_jointPoints[(int)Constants.SourcePositionIndex.Cha_ToeL].boneTransform;

        //Spinal
        s_childNodes[(int)Constants.SourcePositionIndex.bottom_torso] = s_jointPoints[(int)Constants.SourcePositionIndex.bottom_torso].boneTransform;
        s_childNodes[(int)Constants.SourcePositionIndex.center_torso] = s_jointPoints[(int)Constants.SourcePositionIndex.center_torso].boneTransform;
        s_childNodes[(int)Constants.SourcePositionIndex.upper_torso] = s_jointPoints[(int)Constants.SourcePositionIndex.upper_torso].boneTransform;
        s_childNodes[(int)Constants.SourcePositionIndex.neck_base] = s_jointPoints[(int)Constants.SourcePositionIndex.neck_base].boneTransform;
        s_childNodes[(int)Constants.SourcePositionIndex.center_head] = s_jointPoints[(int)Constants.SourcePositionIndex.center_head].boneTransform;
        //s_jointPoints[(int)Constants.SourcePositionIndex.Cha_HeadDummy].boneTransform =

        //Left Arm
        s_childNodes[(int)Constants.SourcePositionIndex.left_shoulder] = s_jointPoints[(int)Constants.SourcePositionIndex.left_shoulder].boneTransform;
        s_childNodes[(int)Constants.SourcePositionIndex.left_elbow] = s_jointPoints[(int)Constants.SourcePositionIndex.left_elbow].boneTransform;
        s_childNodes[(int)Constants.SourcePositionIndex.left_hand] = s_jointPoints[(int)Constants.SourcePositionIndex.left_hand].boneTransform;
        //s_jointPoints[(int)Constants.SourcePositionIndex.BodyHandL].boneTransform =

        //Right Arm
        s_childNodes[(int)Constants.SourcePositionIndex.right_shoulder] = s_jointPoints[(int)Constants.SourcePositionIndex.right_shoulder].boneTransform;
        s_childNodes[(int)Constants.SourcePositionIndex.right_elbow] = s_jointPoints[(int)Constants.SourcePositionIndex.right_elbow].boneTransform;
        s_childNodes[(int)Constants.SourcePositionIndex.right_hand] = s_jointPoints[(int)Constants.SourcePositionIndex.right_hand].boneTransform;
        //s_jointPoints[(int)Constants.SourcePositionIndex.BodyHandR].boneTransform = 

        //Vector3 forward = TriangleNormal(s_jointPoints[(int)Constants.SourcePositionIndex.Cha_Spine], s_jointPoints[(int)Constants.SourcePositionIndex.Cha_UpperLegL], )
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

    Vector3 RotateAround(Vector3 position, Vector3 pivot, Quaternion rotation)
    {
        return rotation * (position - pivot) + pivot;
    }

    public void PopulateChildren()
    {
        childNodes = rootNode.GetComponentsInChildren<Transform>();
        s_childNodes = s_rootNode.GetComponentsInChildren<Transform>();
    }

    private void GetNT()
    {
        sData = DataProcess.Instance.GetSData();
        var rotation = parentTransform.rotation;

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

        jointPoints[(int)Constants.TargetPositionIndex.Cha_UpperLegR].jointPosition = RotateAround(jointPoints[(int)Constants.TargetPositionIndex.Cha_UpperLegR].jointPosition, initPosition, rotation);
        jointPoints[(int)Constants.TargetPositionIndex.Cha_LowerLegR].jointPosition = RotateAround(jointPoints[(int)Constants.TargetPositionIndex.Cha_LowerLegR].jointPosition, initPosition, rotation);
        jointPoints[(int)Constants.TargetPositionIndex.Cha_FootR].jointPosition = RotateAround(jointPoints[(int)Constants.TargetPositionIndex.Cha_FootR].jointPosition, initPosition, rotation);

        jointPoints[(int)Constants.TargetPositionIndex.Cha_UpperLegL].jointPosition = RotateAround(jointPoints[(int)Constants.TargetPositionIndex.Cha_UpperLegL].jointPosition, initPosition, rotation);
        jointPoints[(int)Constants.TargetPositionIndex.Cha_LowerLegL].jointPosition = RotateAround(jointPoints[(int)Constants.TargetPositionIndex.Cha_LowerLegL].jointPosition, initPosition, rotation);
        jointPoints[(int)Constants.TargetPositionIndex.Cha_FootL].jointPosition = RotateAround(jointPoints[(int)Constants.TargetPositionIndex.Cha_FootL].jointPosition, initPosition, rotation);

        jointPoints[(int)Constants.TargetPositionIndex.Cha_Spine].jointPosition = RotateAround(jointPoints[(int)Constants.TargetPositionIndex.Cha_Spine].jointPosition, initPosition, rotation);
        jointPoints[(int)Constants.TargetPositionIndex.Cha_Chest].jointPosition = RotateAround(jointPoints[(int)Constants.TargetPositionIndex.Cha_Chest].jointPosition, initPosition, rotation);
        jointPoints[(int)Constants.TargetPositionIndex.Face].jointPosition = RotateAround(jointPoints[(int)Constants.TargetPositionIndex.Face].jointPosition, initPosition, rotation);

        jointPoints[(int)Constants.TargetPositionIndex.Cha_UpperArmL].jointPosition = RotateAround(jointPoints[(int)Constants.TargetPositionIndex.Cha_UpperArmL].jointPosition, initPosition, rotation);
        jointPoints[(int)Constants.TargetPositionIndex.Cha_LowerArmL].jointPosition = RotateAround(jointPoints[(int)Constants.TargetPositionIndex.Cha_LowerArmL].jointPosition, initPosition, rotation);
        jointPoints[(int)Constants.TargetPositionIndex.Cha_HandL].jointPosition = RotateAround(jointPoints[(int)Constants.TargetPositionIndex.Cha_HandL].jointPosition, initPosition, rotation);

        jointPoints[(int)Constants.TargetPositionIndex.Cha_UpperArmR].jointPosition = RotateAround(jointPoints[(int)Constants.TargetPositionIndex.Cha_UpperArmR].jointPosition, initPosition, rotation);
        jointPoints[(int)Constants.TargetPositionIndex.Cha_LowerArmR].jointPosition = RotateAround(jointPoints[(int)Constants.TargetPositionIndex.Cha_LowerArmR].jointPosition, initPosition, rotation);
        jointPoints[(int)Constants.TargetPositionIndex.Cha_HandR].jointPosition = RotateAround(jointPoints[(int)Constants.TargetPositionIndex.Cha_HandR].jointPosition, initPosition, rotation);
    }

    private void GetNT_s()
    {
        sData = DataProcess.Instance.GetSData();
        var rotation = s_parentTransform.rotation;

        //Right Leg
        s_jointPoints[(int)Constants.SourcePositionIndex.right_hip].jointPosition =
            new Vector3(float.Parse(sData[DataProcess.Instance.chunkIndex + (int)Constants.SourcePositionIndex.right_hip][2]) / _SCALEFACTOR,
                    float.Parse(sData[DataProcess.Instance.chunkIndex + (int)Constants.SourcePositionIndex.right_hip][4]) / _SCALEFACTOR,
                    float.Parse(sData[DataProcess.Instance.chunkIndex + (int)Constants.SourcePositionIndex.right_hip][3]) / _SCALEFACTOR);
        s_jointPoints[(int)Constants.SourcePositionIndex.right_knee].jointPosition =
            new Vector3(float.Parse(sData[DataProcess.Instance.chunkIndex + (int)Constants.SourcePositionIndex.right_knee][2]) / _SCALEFACTOR,
                    float.Parse(sData[DataProcess.Instance.chunkIndex + (int)Constants.SourcePositionIndex.right_knee][4]) / _SCALEFACTOR,
                    float.Parse(sData[DataProcess.Instance.chunkIndex + (int)Constants.SourcePositionIndex.right_knee][3]) / _SCALEFACTOR);
        s_jointPoints[(int)Constants.SourcePositionIndex.right_foot].jointPosition =
            new Vector3(float.Parse(sData[DataProcess.Instance.chunkIndex + (int)Constants.SourcePositionIndex.right_foot][2]) / _SCALEFACTOR,
                    float.Parse(sData[DataProcess.Instance.chunkIndex + (int)Constants.SourcePositionIndex.right_foot][4]) / _SCALEFACTOR,
                    float.Parse(sData[DataProcess.Instance.chunkIndex + (int)Constants.SourcePositionIndex.right_foot][3]) / _SCALEFACTOR);

        //Left Leg
        s_jointPoints[(int)Constants.SourcePositionIndex.left_hip].jointPosition =
            new Vector3(float.Parse(sData[DataProcess.Instance.chunkIndex + (int)Constants.SourcePositionIndex.left_hip][2]) / _SCALEFACTOR,
                    float.Parse(sData[DataProcess.Instance.chunkIndex + (int)Constants.SourcePositionIndex.left_hip][4]) / _SCALEFACTOR,
                    float.Parse(sData[DataProcess.Instance.chunkIndex + (int)Constants.SourcePositionIndex.left_hip][3]) / _SCALEFACTOR);
        s_jointPoints[(int)Constants.SourcePositionIndex.left_knee].jointPosition =
            new Vector3(float.Parse(sData[DataProcess.Instance.chunkIndex + (int)Constants.SourcePositionIndex.left_knee][2]) / _SCALEFACTOR,
                    float.Parse(sData[DataProcess.Instance.chunkIndex + (int)Constants.SourcePositionIndex.left_knee][4]) / _SCALEFACTOR,
                    float.Parse(sData[DataProcess.Instance.chunkIndex + (int)Constants.SourcePositionIndex.left_knee][3]) / _SCALEFACTOR);
        s_jointPoints[(int)Constants.SourcePositionIndex.left_foot].jointPosition =
            new Vector3(float.Parse(sData[DataProcess.Instance.chunkIndex + (int)Constants.SourcePositionIndex.left_foot][2]) / _SCALEFACTOR,
                    float.Parse(sData[DataProcess.Instance.chunkIndex + (int)Constants.SourcePositionIndex.left_foot][4]) / _SCALEFACTOR,
                    float.Parse(sData[DataProcess.Instance.chunkIndex + (int)Constants.SourcePositionIndex.left_foot][3]) / _SCALEFACTOR);

        //Spinal
        s_jointPoints[(int)Constants.SourcePositionIndex.bottom_torso].jointPosition =
            new Vector3(float.Parse(sData[DataProcess.Instance.chunkIndex + (int)Constants.SourcePositionIndex.bottom_torso][2]) / _SCALEFACTOR,
                    float.Parse(sData[DataProcess.Instance.chunkIndex + (int)Constants.SourcePositionIndex.bottom_torso][4]) / _SCALEFACTOR,
                    float.Parse(sData[DataProcess.Instance.chunkIndex + (int)Constants.SourcePositionIndex.bottom_torso][3]) / _SCALEFACTOR);
        s_jointPoints[(int)Constants.SourcePositionIndex.center_torso].jointPosition =
            new Vector3(float.Parse(sData[DataProcess.Instance.chunkIndex + (int)Constants.SourcePositionIndex.center_torso][2]) / _SCALEFACTOR,
                    float.Parse(sData[DataProcess.Instance.chunkIndex + (int)Constants.SourcePositionIndex.center_torso][4]) / _SCALEFACTOR,
                    float.Parse(sData[DataProcess.Instance.chunkIndex + (int)Constants.SourcePositionIndex.center_torso][3]) / _SCALEFACTOR);
        s_jointPoints[(int)Constants.SourcePositionIndex.upper_torso].jointPosition =
            new Vector3(float.Parse(sData[DataProcess.Instance.chunkIndex + (int)Constants.SourcePositionIndex.upper_torso][2]) / _SCALEFACTOR,
                    float.Parse(sData[DataProcess.Instance.chunkIndex + (int)Constants.SourcePositionIndex.upper_torso][4]) / _SCALEFACTOR,
                    float.Parse(sData[DataProcess.Instance.chunkIndex + (int)Constants.SourcePositionIndex.upper_torso][3]) / _SCALEFACTOR);
        s_jointPoints[(int)Constants.SourcePositionIndex.neck_base].jointPosition =
            new Vector3(float.Parse(sData[DataProcess.Instance.chunkIndex + (int)Constants.SourcePositionIndex.neck_base][2]) / _SCALEFACTOR,
                    float.Parse(sData[DataProcess.Instance.chunkIndex + (int)Constants.SourcePositionIndex.neck_base][4]) / _SCALEFACTOR,
                    float.Parse(sData[DataProcess.Instance.chunkIndex + (int)Constants.SourcePositionIndex.neck_base][3]) / _SCALEFACTOR);
        s_jointPoints[(int)Constants.SourcePositionIndex.center_head].jointPosition =
            new Vector3(float.Parse(sData[DataProcess.Instance.chunkIndex + (int)Constants.SourcePositionIndex.center_head][2]) / _SCALEFACTOR,
                    float.Parse(sData[DataProcess.Instance.chunkIndex + (int)Constants.SourcePositionIndex.center_head][4]) / _SCALEFACTOR,
                    float.Parse(sData[DataProcess.Instance.chunkIndex + (int)Constants.SourcePositionIndex.center_head][3]) / _SCALEFACTOR);

        //Left Arm
        s_jointPoints[(int)Constants.SourcePositionIndex.left_shoulder].jointPosition =
            new Vector3(float.Parse(sData[DataProcess.Instance.chunkIndex + (int)Constants.SourcePositionIndex.left_shoulder][2]) / _SCALEFACTOR,
                    float.Parse(sData[DataProcess.Instance.chunkIndex + (int)Constants.SourcePositionIndex.left_shoulder][4]) / _SCALEFACTOR,
                    float.Parse(sData[DataProcess.Instance.chunkIndex + (int)Constants.SourcePositionIndex.left_shoulder][3]) / _SCALEFACTOR);
        s_jointPoints[(int)Constants.SourcePositionIndex.left_elbow].jointPosition =
            new Vector3(float.Parse(sData[DataProcess.Instance.chunkIndex + (int)Constants.SourcePositionIndex.left_elbow][2]) / _SCALEFACTOR,
                    float.Parse(sData[DataProcess.Instance.chunkIndex + (int)Constants.SourcePositionIndex.left_elbow][4]) / _SCALEFACTOR,
                    float.Parse(sData[DataProcess.Instance.chunkIndex + (int)Constants.SourcePositionIndex.left_elbow][3]) / _SCALEFACTOR);
        s_jointPoints[(int)Constants.SourcePositionIndex.left_hand].jointPosition =
            new Vector3(float.Parse(sData[DataProcess.Instance.chunkIndex + (int)Constants.SourcePositionIndex.left_hand][2]) / _SCALEFACTOR,
                    float.Parse(sData[DataProcess.Instance.chunkIndex + (int)Constants.SourcePositionIndex.left_hand][4]) / _SCALEFACTOR,
                    float.Parse(sData[DataProcess.Instance.chunkIndex + (int)Constants.SourcePositionIndex.left_hand][3]) / _SCALEFACTOR);

        //Right Arm
        s_jointPoints[(int)Constants.SourcePositionIndex.right_shoulder].jointPosition =
            new Vector3(float.Parse(sData[DataProcess.Instance.chunkIndex + (int)Constants.SourcePositionIndex.right_shoulder][2]) / _SCALEFACTOR,
                    float.Parse(sData[DataProcess.Instance.chunkIndex + (int)Constants.SourcePositionIndex.right_shoulder][4]) / _SCALEFACTOR,
                    float.Parse(sData[DataProcess.Instance.chunkIndex + (int)Constants.SourcePositionIndex.right_shoulder][3]) / _SCALEFACTOR);
        s_jointPoints[(int)Constants.SourcePositionIndex.right_elbow].jointPosition =
            new Vector3(float.Parse(sData[DataProcess.Instance.chunkIndex + (int)Constants.SourcePositionIndex.right_elbow][2]) / _SCALEFACTOR,
                    float.Parse(sData[DataProcess.Instance.chunkIndex + (int)Constants.SourcePositionIndex.right_elbow][4]) / _SCALEFACTOR,
                    float.Parse(sData[DataProcess.Instance.chunkIndex + (int)Constants.SourcePositionIndex.right_elbow][3]) / _SCALEFACTOR);
        s_jointPoints[(int)Constants.SourcePositionIndex.right_hand].jointPosition =
            new Vector3(float.Parse(sData[DataProcess.Instance.chunkIndex + (int)Constants.SourcePositionIndex.right_hand][2]) / _SCALEFACTOR,
                    float.Parse(sData[DataProcess.Instance.chunkIndex + (int)Constants.SourcePositionIndex.right_hand][4]) / _SCALEFACTOR,
                    float.Parse(sData[DataProcess.Instance.chunkIndex + (int)Constants.SourcePositionIndex.right_hand][3]) / _SCALEFACTOR);

        s_jointPoints[(int)Constants.SourcePositionIndex.right_hip].jointPosition = RotateAround(s_jointPoints[(int)Constants.SourcePositionIndex.right_hip].jointPosition, s_initPosition, rotation);
        s_jointPoints[(int)Constants.SourcePositionIndex.right_knee].jointPosition = RotateAround(s_jointPoints[(int)Constants.SourcePositionIndex.right_knee].jointPosition, s_initPosition, rotation);
        s_jointPoints[(int)Constants.SourcePositionIndex.right_foot].jointPosition = RotateAround(s_jointPoints[(int)Constants.SourcePositionIndex.right_foot].jointPosition, s_initPosition, rotation);

        s_jointPoints[(int)Constants.SourcePositionIndex.left_hip].jointPosition = RotateAround(s_jointPoints[(int)Constants.SourcePositionIndex.left_hip].jointPosition, s_initPosition, rotation);
        s_jointPoints[(int)Constants.SourcePositionIndex.left_knee].jointPosition = RotateAround(s_jointPoints[(int)Constants.SourcePositionIndex.left_knee].jointPosition, s_initPosition, rotation);
        s_jointPoints[(int)Constants.SourcePositionIndex.left_foot].jointPosition = RotateAround(s_jointPoints[(int)Constants.SourcePositionIndex.left_foot].jointPosition, s_initPosition, rotation);

        s_jointPoints[(int)Constants.SourcePositionIndex.center_torso].jointPosition = RotateAround(s_jointPoints[(int)Constants.SourcePositionIndex.center_torso].jointPosition, s_initPosition, rotation);
        s_jointPoints[(int)Constants.SourcePositionIndex.upper_torso].jointPosition = RotateAround(s_jointPoints[(int)Constants.SourcePositionIndex.upper_torso].jointPosition, s_initPosition, rotation);
        s_jointPoints[(int)Constants.SourcePositionIndex.neck_base].jointPosition = RotateAround(s_jointPoints[(int)Constants.SourcePositionIndex.neck_base].jointPosition, s_initPosition, rotation);
        s_jointPoints[(int)Constants.SourcePositionIndex.center_head].jointPosition = RotateAround(s_jointPoints[(int)Constants.SourcePositionIndex.center_head].jointPosition, s_initPosition, rotation);

        s_jointPoints[(int)Constants.SourcePositionIndex.left_shoulder].jointPosition = RotateAround(s_jointPoints[(int)Constants.SourcePositionIndex.left_shoulder].jointPosition, s_initPosition, rotation);
        s_jointPoints[(int)Constants.SourcePositionIndex.left_elbow].jointPosition = RotateAround(s_jointPoints[(int)Constants.SourcePositionIndex.left_elbow].jointPosition, s_initPosition, rotation);
        s_jointPoints[(int)Constants.SourcePositionIndex.left_hand].jointPosition = RotateAround(s_jointPoints[(int)Constants.SourcePositionIndex.left_hand].jointPosition, s_initPosition, rotation);

        s_jointPoints[(int)Constants.SourcePositionIndex.right_shoulder].jointPosition = RotateAround(s_jointPoints[(int)Constants.SourcePositionIndex.right_shoulder].jointPosition, s_initPosition, rotation);
        s_jointPoints[(int)Constants.SourcePositionIndex.right_elbow].jointPosition = RotateAround(s_jointPoints[(int)Constants.SourcePositionIndex.right_elbow].jointPosition, s_initPosition, rotation);
        s_jointPoints[(int)Constants.SourcePositionIndex.right_hand].jointPosition = RotateAround(s_jointPoints[(int)Constants.SourcePositionIndex.right_hand].jointPosition, s_initPosition, rotation);
    }
}
