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

    public float KalmanParamQ = 0.001f;
    public float KalmanParamR = 0.0015f;

    private bool flag = true;
    private bool t_flag = true;
    public bool f_flag;

    private float LowPassParam = 0.1f;

    public bool n_flag;
    private Vector3 sNeckJoint;

    public class JointPoint
    {
        public Vector3 inputJointPosition = new();
        public Vector3[] prevJointPosition = new Vector3[6];
        public Vector3 estimatedJointPosition = new();
        public Transform boneTransform = null;
        public Quaternion initRotation;
        public Quaternion inverse;
        public Quaternion inverseRotation;

        public JointPoint child = null;
        public JointPoint parent = null;

        // For Kalman filter
        public Vector3 P = new Vector3();
        public Vector3 X = new Vector3();
        public Vector3 K = new Vector3();

        public Vector3[] PrevPos3D = new Vector3[6];
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

        //Assumes Tpose

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

        JointPoint head = jointPoints[(int)Constants.TargetPositionIndex.Face];
        head.initRotation = jointPoints[(int)Constants.TargetPositionIndex.Face].boneTransform.rotation;

        Vector3 v = jointPoints[(int)Constants.TargetPositionIndex.Face].boneTransform.position - jointPoints[(int)Constants.TargetPositionIndex.Cha_Chest].boneTransform.position;
        head.inverse = Quaternion.Inverse(Quaternion.LookRotation(forward, v));
        head.inverseRotation = head.inverse * head.initRotation;

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
        Vector3 forward = TriangleNormal(jointPoints[(int)Constants.TargetPositionIndex.Cha_Spine].inputJointPosition,
           jointPoints[(int)Constants.TargetPositionIndex.Cha_UpperLegL].inputJointPosition,
           jointPoints[(int)Constants.TargetPositionIndex.Cha_UpperLegR].inputJointPosition);
        jointPoints[(int)Constants.TargetPositionIndex.Cha_Hips].boneTransform.SetPositionAndRotation(jointPoints[(int)Constants.TargetPositionIndex.Cha_Hips].inputJointPosition + initPosition,
                                                                                                        Quaternion.LookRotation(forward) * jointPoints[(int)Constants.TargetPositionIndex.Cha_Hips].inverseRotation);

        foreach (JointPoint jp in jointPoints)
        {
            if (jp.parent != null)
            {
                if (jp.child != null)
                {
                    Vector3 fv = jp.parent.inputJointPosition - jp.inputJointPosition;
                    jp.boneTransform.rotation = Quaternion.LookRotation(jp.inputJointPosition - jp.child.inputJointPosition, fv) * jp.inverseRotation;
                }
            }
            else if (jp.child != null)
            {
                jp.boneTransform.rotation = Quaternion.LookRotation(jp.inputJointPosition - jp.child.inputJointPosition, forward) * jp.inverseRotation;
            }
        }

        if (n_flag)
        {
            Vector3 v = jointPoints[(int)Constants.TargetPositionIndex.Face].inputJointPosition - jointPoints[(int)Constants.TargetPositionIndex.Cha_Chest].inputJointPosition;
            Vector3 s = sNeckJoint - jointPoints[(int)Constants.TargetPositionIndex.Cha_Chest].inputJointPosition;
            Vector3 nose = s - Vector3.Project(s, v);
            jointPoints[(int)Constants.TargetPositionIndex.Face].boneTransform.rotation = Quaternion.LookRotation(nose, v) * jointPoints[(int)Constants.TargetPositionIndex.Face].inverseRotation;
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
        Vector3 forward = TriangleNormal(s_jointPoints[(int)Constants.SourcePositionIndex.center_torso].inputJointPosition,
           s_jointPoints[(int)Constants.SourcePositionIndex.left_hip].inputJointPosition,
           s_jointPoints[(int)Constants.SourcePositionIndex.right_hip].inputJointPosition);
        s_jointPoints[(int)Constants.SourcePositionIndex.bottom_torso].boneTransform.SetPositionAndRotation(s_jointPoints[(int)Constants.SourcePositionIndex.bottom_torso].inputJointPosition + s_initPosition,
                                                                                                        Quaternion.LookRotation(forward) * s_jointPoints[(int)Constants.SourcePositionIndex.bottom_torso].inverseRotation);

        foreach (JointPoint jp in s_jointPoints)
        {
            if (jp.parent != null)
            {
                if (jp.child != null)
                {
                    Vector3 fv = jp.parent.inputJointPosition - jp.inputJointPosition;
                    jp.boneTransform.rotation = Quaternion.LookRotation(jp.inputJointPosition - jp.child.inputJointPosition, fv) * jp.inverseRotation;
                }
            }
            else if (jp.child != null)
            {
                jp.boneTransform.rotation = Quaternion.LookRotation(jp.inputJointPosition - jp.child.inputJointPosition, forward) * jp.inverseRotation;
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
        var rotation = parentTransform.rotation * Quaternion.AngleAxis(180, Vector3.up);

        //Right Leg
        jointPoints[(int)Constants.TargetPositionIndex.Cha_UpperLegR].inputJointPosition =
            new Vector3(float.Parse(sData[DataProcess.Instance.chunkIndex + (int)Constants.SourcePositionIndex.right_hip][2]) / _SCALEFACTOR,
                    float.Parse(sData[DataProcess.Instance.chunkIndex + (int)Constants.SourcePositionIndex.right_hip][4]) / _SCALEFACTOR,
                    float.Parse(sData[DataProcess.Instance.chunkIndex + (int)Constants.SourcePositionIndex.right_hip][3]) / _SCALEFACTOR);
        jointPoints[(int)Constants.TargetPositionIndex.Cha_LowerLegR].inputJointPosition =
                    new Vector3(float.Parse(sData[DataProcess.Instance.chunkIndex + (int)Constants.SourcePositionIndex.right_knee][2]) / _SCALEFACTOR,
                            float.Parse(sData[DataProcess.Instance.chunkIndex + (int)Constants.SourcePositionIndex.right_knee][4]) / _SCALEFACTOR,
                            float.Parse(sData[DataProcess.Instance.chunkIndex + (int)Constants.SourcePositionIndex.right_knee][3]) / _SCALEFACTOR);
        jointPoints[(int)Constants.TargetPositionIndex.Cha_FootR].inputJointPosition =
            new Vector3(float.Parse(sData[DataProcess.Instance.chunkIndex + (int)Constants.SourcePositionIndex.right_foot][2]) / _SCALEFACTOR,
                    float.Parse(sData[DataProcess.Instance.chunkIndex + (int)Constants.SourcePositionIndex.right_foot][4]) / _SCALEFACTOR,
                    float.Parse(sData[DataProcess.Instance.chunkIndex + (int)Constants.SourcePositionIndex.right_foot][3]) / _SCALEFACTOR);

        //Left Leg
        jointPoints[(int)Constants.TargetPositionIndex.Cha_UpperLegL].inputJointPosition =
            new Vector3(float.Parse(sData[DataProcess.Instance.chunkIndex + (int)Constants.SourcePositionIndex.left_hip][2]) / _SCALEFACTOR,
                    float.Parse(sData[DataProcess.Instance.chunkIndex + (int)Constants.SourcePositionIndex.left_hip][4]) / _SCALEFACTOR,
                    float.Parse(sData[DataProcess.Instance.chunkIndex + (int)Constants.SourcePositionIndex.left_hip][3]) / _SCALEFACTOR);
        jointPoints[(int)Constants.TargetPositionIndex.Cha_LowerLegL].inputJointPosition =
            new Vector3(float.Parse(sData[DataProcess.Instance.chunkIndex + (int)Constants.SourcePositionIndex.left_knee][2]) / _SCALEFACTOR,
                    float.Parse(sData[DataProcess.Instance.chunkIndex + (int)Constants.SourcePositionIndex.left_knee][4]) / _SCALEFACTOR,
                    float.Parse(sData[DataProcess.Instance.chunkIndex + (int)Constants.SourcePositionIndex.left_knee][3]) / _SCALEFACTOR);
        jointPoints[(int)Constants.TargetPositionIndex.Cha_FootL].inputJointPosition =
            new Vector3(float.Parse(sData[DataProcess.Instance.chunkIndex + (int)Constants.SourcePositionIndex.left_foot][2]) / _SCALEFACTOR,
                    float.Parse(sData[DataProcess.Instance.chunkIndex + (int)Constants.SourcePositionIndex.left_foot][4]) / _SCALEFACTOR,
                    float.Parse(sData[DataProcess.Instance.chunkIndex + (int)Constants.SourcePositionIndex.left_foot][3]) / _SCALEFACTOR);

        //Spinal
        jointPoints[(int)Constants.TargetPositionIndex.Cha_Hips].inputJointPosition =
            new Vector3(float.Parse(sData[DataProcess.Instance.chunkIndex + (int)Constants.SourcePositionIndex.bottom_torso][2]) / _SCALEFACTOR,
                    float.Parse(sData[DataProcess.Instance.chunkIndex + (int)Constants.SourcePositionIndex.bottom_torso][4]) / _SCALEFACTOR,
                    float.Parse(sData[DataProcess.Instance.chunkIndex + (int)Constants.SourcePositionIndex.bottom_torso][3]) / _SCALEFACTOR);
        jointPoints[(int)Constants.TargetPositionIndex.Cha_Spine].inputJointPosition =
            new Vector3(float.Parse(sData[DataProcess.Instance.chunkIndex + (int)Constants.SourcePositionIndex.center_torso][2]) / _SCALEFACTOR,
                    float.Parse(sData[DataProcess.Instance.chunkIndex + (int)Constants.SourcePositionIndex.center_torso][4]) / _SCALEFACTOR,
                    float.Parse(sData[DataProcess.Instance.chunkIndex + (int)Constants.SourcePositionIndex.center_torso][3]) / _SCALEFACTOR);
        jointPoints[(int)Constants.TargetPositionIndex.Cha_Chest].inputJointPosition =
            new Vector3(float.Parse(sData[DataProcess.Instance.chunkIndex + (int)Constants.SourcePositionIndex.upper_torso][2]) / _SCALEFACTOR,
                    float.Parse(sData[DataProcess.Instance.chunkIndex + (int)Constants.SourcePositionIndex.upper_torso][4]) / _SCALEFACTOR,
                    float.Parse(sData[DataProcess.Instance.chunkIndex + (int)Constants.SourcePositionIndex.upper_torso][3]) / _SCALEFACTOR);
        jointPoints[(int)Constants.TargetPositionIndex.Face].inputJointPosition =
            new Vector3(float.Parse(sData[DataProcess.Instance.chunkIndex + (int)Constants.SourcePositionIndex.center_head][2]) / _SCALEFACTOR,
                    float.Parse(sData[DataProcess.Instance.chunkIndex + (int)Constants.SourcePositionIndex.center_head][4]) / _SCALEFACTOR,
                    float.Parse(sData[DataProcess.Instance.chunkIndex + (int)Constants.SourcePositionIndex.center_head][3]) / _SCALEFACTOR);

        sNeckJoint =
            new Vector3(float.Parse(sData[DataProcess.Instance.chunkIndex + (int)Constants.SourcePositionIndex.neck_base][2]) / _SCALEFACTOR,
                    float.Parse(sData[DataProcess.Instance.chunkIndex + (int)Constants.SourcePositionIndex.neck_base][4]) / _SCALEFACTOR,
                    float.Parse(sData[DataProcess.Instance.chunkIndex + (int)Constants.SourcePositionIndex.neck_base][3]) / _SCALEFACTOR);

        //Left Arm
        jointPoints[(int)Constants.TargetPositionIndex.Cha_UpperArmL].inputJointPosition =
            new Vector3(float.Parse(sData[DataProcess.Instance.chunkIndex + (int)Constants.SourcePositionIndex.left_shoulder][2]) / _SCALEFACTOR,
                    float.Parse(sData[DataProcess.Instance.chunkIndex + (int)Constants.SourcePositionIndex.left_shoulder][4]) / _SCALEFACTOR,
                    float.Parse(sData[DataProcess.Instance.chunkIndex + (int)Constants.SourcePositionIndex.left_shoulder][3]) / _SCALEFACTOR);
        jointPoints[(int)Constants.TargetPositionIndex.Cha_LowerArmL].inputJointPosition =
            new Vector3(float.Parse(sData[DataProcess.Instance.chunkIndex + (int)Constants.SourcePositionIndex.left_elbow][2]) / _SCALEFACTOR,
                    float.Parse(sData[DataProcess.Instance.chunkIndex + (int)Constants.SourcePositionIndex.left_elbow][4]) / _SCALEFACTOR,
                    float.Parse(sData[DataProcess.Instance.chunkIndex + (int)Constants.SourcePositionIndex.left_elbow][3]) / _SCALEFACTOR);
        jointPoints[(int)Constants.TargetPositionIndex.Cha_HandL].inputJointPosition =
            new Vector3(float.Parse(sData[DataProcess.Instance.chunkIndex + (int)Constants.SourcePositionIndex.left_hand][2]) / _SCALEFACTOR,
                    float.Parse(sData[DataProcess.Instance.chunkIndex + (int)Constants.SourcePositionIndex.left_hand][4]) / _SCALEFACTOR,
                    float.Parse(sData[DataProcess.Instance.chunkIndex + (int)Constants.SourcePositionIndex.left_hand][3]) / _SCALEFACTOR);

        //Right Arm
        jointPoints[(int)Constants.TargetPositionIndex.Cha_UpperArmR].inputJointPosition =
            new Vector3(float.Parse(sData[DataProcess.Instance.chunkIndex + (int)Constants.SourcePositionIndex.right_shoulder][2]) / _SCALEFACTOR,
                    float.Parse(sData[DataProcess.Instance.chunkIndex + (int)Constants.SourcePositionIndex.right_shoulder][4]) / _SCALEFACTOR,
                    float.Parse(sData[DataProcess.Instance.chunkIndex + (int)Constants.SourcePositionIndex.right_shoulder][3]) / _SCALEFACTOR);
        jointPoints[(int)Constants.TargetPositionIndex.Cha_LowerArmR].inputJointPosition =
            new Vector3(float.Parse(sData[DataProcess.Instance.chunkIndex + (int)Constants.SourcePositionIndex.right_elbow][2]) / _SCALEFACTOR,
                    float.Parse(sData[DataProcess.Instance.chunkIndex + (int)Constants.SourcePositionIndex.right_elbow][4]) / _SCALEFACTOR,
                    float.Parse(sData[DataProcess.Instance.chunkIndex + (int)Constants.SourcePositionIndex.right_elbow][3]) / _SCALEFACTOR);
        jointPoints[(int)Constants.TargetPositionIndex.Cha_HandR].inputJointPosition =
            new Vector3(float.Parse(sData[DataProcess.Instance.chunkIndex + (int)Constants.SourcePositionIndex.right_hand][2]) / _SCALEFACTOR,
                    float.Parse(sData[DataProcess.Instance.chunkIndex + (int)Constants.SourcePositionIndex.right_hand][4]) / _SCALEFACTOR,
                    float.Parse(sData[DataProcess.Instance.chunkIndex + (int)Constants.SourcePositionIndex.right_hand][3]) / _SCALEFACTOR);

        jointPoints[(int)Constants.TargetPositionIndex.Cha_UpperLegR].inputJointPosition = RotateAround(jointPoints[(int)Constants.TargetPositionIndex.Cha_UpperLegR].inputJointPosition, initPosition, rotation);
        jointPoints[(int)Constants.TargetPositionIndex.Cha_LowerLegR].inputJointPosition = RotateAround(jointPoints[(int)Constants.TargetPositionIndex.Cha_LowerLegR].inputJointPosition, initPosition, rotation);
        jointPoints[(int)Constants.TargetPositionIndex.Cha_FootR].inputJointPosition = RotateAround(jointPoints[(int)Constants.TargetPositionIndex.Cha_FootR].inputJointPosition, initPosition, rotation);

        jointPoints[(int)Constants.TargetPositionIndex.Cha_UpperLegL].inputJointPosition = RotateAround(jointPoints[(int)Constants.TargetPositionIndex.Cha_UpperLegL].inputJointPosition, initPosition, rotation);
        jointPoints[(int)Constants.TargetPositionIndex.Cha_LowerLegL].inputJointPosition = RotateAround(jointPoints[(int)Constants.TargetPositionIndex.Cha_LowerLegL].inputJointPosition, initPosition, rotation);
        jointPoints[(int)Constants.TargetPositionIndex.Cha_FootL].inputJointPosition = RotateAround(jointPoints[(int)Constants.TargetPositionIndex.Cha_FootL].inputJointPosition, initPosition, rotation);

        jointPoints[(int)Constants.TargetPositionIndex.Cha_Spine].inputJointPosition = RotateAround(jointPoints[(int)Constants.TargetPositionIndex.Cha_Spine].inputJointPosition, initPosition, rotation);
        jointPoints[(int)Constants.TargetPositionIndex.Cha_Chest].inputJointPosition = RotateAround(jointPoints[(int)Constants.TargetPositionIndex.Cha_Chest].inputJointPosition, initPosition, rotation);
        jointPoints[(int)Constants.TargetPositionIndex.Face].inputJointPosition = RotateAround(jointPoints[(int)Constants.TargetPositionIndex.Face].inputJointPosition, initPosition, rotation);

        jointPoints[(int)Constants.TargetPositionIndex.Cha_UpperArmL].inputJointPosition = RotateAround(jointPoints[(int)Constants.TargetPositionIndex.Cha_UpperArmL].inputJointPosition, initPosition, rotation);
        jointPoints[(int)Constants.TargetPositionIndex.Cha_LowerArmL].inputJointPosition = RotateAround(jointPoints[(int)Constants.TargetPositionIndex.Cha_LowerArmL].inputJointPosition, initPosition, rotation);
        jointPoints[(int)Constants.TargetPositionIndex.Cha_HandL].inputJointPosition = RotateAround(jointPoints[(int)Constants.TargetPositionIndex.Cha_HandL].inputJointPosition, initPosition, rotation);

        jointPoints[(int)Constants.TargetPositionIndex.Cha_UpperArmR].inputJointPosition = RotateAround(jointPoints[(int)Constants.TargetPositionIndex.Cha_UpperArmR].inputJointPosition, initPosition, rotation);
        jointPoints[(int)Constants.TargetPositionIndex.Cha_LowerArmR].inputJointPosition = RotateAround(jointPoints[(int)Constants.TargetPositionIndex.Cha_LowerArmR].inputJointPosition, initPosition, rotation);
        jointPoints[(int)Constants.TargetPositionIndex.Cha_HandR].inputJointPosition = RotateAround(jointPoints[(int)Constants.TargetPositionIndex.Cha_HandR].inputJointPosition, initPosition, rotation);

        sNeckJoint = RotateAround(sNeckJoint, initPosition, rotation);

        // Low pass filter
        if (f_flag)
        {
            foreach (var jp in jointPoints)
            {
                //칼만 필터 적용을 위해서는 영상과 Barracuda를 이용해서 예측값을 estimatedJointPosition에 채워주어야 하는 것 같음!
                //KalmanUpdate(jp);

                jp.prevJointPosition[0] = jp.inputJointPosition;
                for (var i = 1; i < jp.prevJointPosition.Length; i++)
                {
                    jp.prevJointPosition[i] = jp.prevJointPosition[i] * LowPassParam + jp.prevJointPosition[i - 1] * (1f - LowPassParam);
                }
                jp.inputJointPosition = jp.prevJointPosition[jp.prevJointPosition.Length - 1];
            }
        }

    }

    private void GetNT_s()
    {
        sData = DataProcess.Instance.GetSData();
        var rotation = s_parentTransform.rotation;

        //Right Leg
        s_jointPoints[(int)Constants.SourcePositionIndex.right_hip].inputJointPosition =
            new Vector3(float.Parse(sData[DataProcess.Instance.chunkIndex + (int)Constants.SourcePositionIndex.right_hip][2]) / _SCALEFACTOR,
                    float.Parse(sData[DataProcess.Instance.chunkIndex + (int)Constants.SourcePositionIndex.right_hip][4]) / _SCALEFACTOR,
                    float.Parse(sData[DataProcess.Instance.chunkIndex + (int)Constants.SourcePositionIndex.right_hip][3]) / _SCALEFACTOR);
        s_jointPoints[(int)Constants.SourcePositionIndex.right_knee].inputJointPosition =
            new Vector3(float.Parse(sData[DataProcess.Instance.chunkIndex + (int)Constants.SourcePositionIndex.right_knee][2]) / _SCALEFACTOR,
                    float.Parse(sData[DataProcess.Instance.chunkIndex + (int)Constants.SourcePositionIndex.right_knee][4]) / _SCALEFACTOR,
                    float.Parse(sData[DataProcess.Instance.chunkIndex + (int)Constants.SourcePositionIndex.right_knee][3]) / _SCALEFACTOR);
        s_jointPoints[(int)Constants.SourcePositionIndex.right_foot].inputJointPosition =
            new Vector3(float.Parse(sData[DataProcess.Instance.chunkIndex + (int)Constants.SourcePositionIndex.right_foot][2]) / _SCALEFACTOR,
                    float.Parse(sData[DataProcess.Instance.chunkIndex + (int)Constants.SourcePositionIndex.right_foot][4]) / _SCALEFACTOR,
                    float.Parse(sData[DataProcess.Instance.chunkIndex + (int)Constants.SourcePositionIndex.right_foot][3]) / _SCALEFACTOR);

        //Left Leg
        s_jointPoints[(int)Constants.SourcePositionIndex.left_hip].inputJointPosition =
            new Vector3(float.Parse(sData[DataProcess.Instance.chunkIndex + (int)Constants.SourcePositionIndex.left_hip][2]) / _SCALEFACTOR,
                    float.Parse(sData[DataProcess.Instance.chunkIndex + (int)Constants.SourcePositionIndex.left_hip][4]) / _SCALEFACTOR,
                    float.Parse(sData[DataProcess.Instance.chunkIndex + (int)Constants.SourcePositionIndex.left_hip][3]) / _SCALEFACTOR);
        s_jointPoints[(int)Constants.SourcePositionIndex.left_knee].inputJointPosition =
            new Vector3(float.Parse(sData[DataProcess.Instance.chunkIndex + (int)Constants.SourcePositionIndex.left_knee][2]) / _SCALEFACTOR,
                    float.Parse(sData[DataProcess.Instance.chunkIndex + (int)Constants.SourcePositionIndex.left_knee][4]) / _SCALEFACTOR,
                    float.Parse(sData[DataProcess.Instance.chunkIndex + (int)Constants.SourcePositionIndex.left_knee][3]) / _SCALEFACTOR);
        s_jointPoints[(int)Constants.SourcePositionIndex.left_foot].inputJointPosition =
            new Vector3(float.Parse(sData[DataProcess.Instance.chunkIndex + (int)Constants.SourcePositionIndex.left_foot][2]) / _SCALEFACTOR,
                    float.Parse(sData[DataProcess.Instance.chunkIndex + (int)Constants.SourcePositionIndex.left_foot][4]) / _SCALEFACTOR,
                    float.Parse(sData[DataProcess.Instance.chunkIndex + (int)Constants.SourcePositionIndex.left_foot][3]) / _SCALEFACTOR);

        //Spinal
        s_jointPoints[(int)Constants.SourcePositionIndex.bottom_torso].inputJointPosition =
            new Vector3(float.Parse(sData[DataProcess.Instance.chunkIndex + (int)Constants.SourcePositionIndex.bottom_torso][2]) / _SCALEFACTOR,
                    float.Parse(sData[DataProcess.Instance.chunkIndex + (int)Constants.SourcePositionIndex.bottom_torso][4]) / _SCALEFACTOR,
                    float.Parse(sData[DataProcess.Instance.chunkIndex + (int)Constants.SourcePositionIndex.bottom_torso][3]) / _SCALEFACTOR);
        s_jointPoints[(int)Constants.SourcePositionIndex.center_torso].inputJointPosition =
            new Vector3(float.Parse(sData[DataProcess.Instance.chunkIndex + (int)Constants.SourcePositionIndex.center_torso][2]) / _SCALEFACTOR,
                    float.Parse(sData[DataProcess.Instance.chunkIndex + (int)Constants.SourcePositionIndex.center_torso][4]) / _SCALEFACTOR,
                    float.Parse(sData[DataProcess.Instance.chunkIndex + (int)Constants.SourcePositionIndex.center_torso][3]) / _SCALEFACTOR);
        s_jointPoints[(int)Constants.SourcePositionIndex.upper_torso].inputJointPosition =
            new Vector3(float.Parse(sData[DataProcess.Instance.chunkIndex + (int)Constants.SourcePositionIndex.upper_torso][2]) / _SCALEFACTOR,
                    float.Parse(sData[DataProcess.Instance.chunkIndex + (int)Constants.SourcePositionIndex.upper_torso][4]) / _SCALEFACTOR,
                    float.Parse(sData[DataProcess.Instance.chunkIndex + (int)Constants.SourcePositionIndex.upper_torso][3]) / _SCALEFACTOR);
        s_jointPoints[(int)Constants.SourcePositionIndex.neck_base].inputJointPosition =
            new Vector3(float.Parse(sData[DataProcess.Instance.chunkIndex + (int)Constants.SourcePositionIndex.neck_base][2]) / _SCALEFACTOR,
                    float.Parse(sData[DataProcess.Instance.chunkIndex + (int)Constants.SourcePositionIndex.neck_base][4]) / _SCALEFACTOR,
                    float.Parse(sData[DataProcess.Instance.chunkIndex + (int)Constants.SourcePositionIndex.neck_base][3]) / _SCALEFACTOR);
        s_jointPoints[(int)Constants.SourcePositionIndex.center_head].inputJointPosition =
            new Vector3(float.Parse(sData[DataProcess.Instance.chunkIndex + (int)Constants.SourcePositionIndex.center_head][2]) / _SCALEFACTOR,
                    float.Parse(sData[DataProcess.Instance.chunkIndex + (int)Constants.SourcePositionIndex.center_head][4]) / _SCALEFACTOR,
                    float.Parse(sData[DataProcess.Instance.chunkIndex + (int)Constants.SourcePositionIndex.center_head][3]) / _SCALEFACTOR);

        //Left Arm
        s_jointPoints[(int)Constants.SourcePositionIndex.left_shoulder].inputJointPosition =
            new Vector3(float.Parse(sData[DataProcess.Instance.chunkIndex + (int)Constants.SourcePositionIndex.left_shoulder][2]) / _SCALEFACTOR,
                    float.Parse(sData[DataProcess.Instance.chunkIndex + (int)Constants.SourcePositionIndex.left_shoulder][4]) / _SCALEFACTOR,
                    float.Parse(sData[DataProcess.Instance.chunkIndex + (int)Constants.SourcePositionIndex.left_shoulder][3]) / _SCALEFACTOR);
        s_jointPoints[(int)Constants.SourcePositionIndex.left_elbow].inputJointPosition =
            new Vector3(float.Parse(sData[DataProcess.Instance.chunkIndex + (int)Constants.SourcePositionIndex.left_elbow][2]) / _SCALEFACTOR,
                    float.Parse(sData[DataProcess.Instance.chunkIndex + (int)Constants.SourcePositionIndex.left_elbow][4]) / _SCALEFACTOR,
                    float.Parse(sData[DataProcess.Instance.chunkIndex + (int)Constants.SourcePositionIndex.left_elbow][3]) / _SCALEFACTOR);
        s_jointPoints[(int)Constants.SourcePositionIndex.left_hand].inputJointPosition =
            new Vector3(float.Parse(sData[DataProcess.Instance.chunkIndex + (int)Constants.SourcePositionIndex.left_hand][2]) / _SCALEFACTOR,
                    float.Parse(sData[DataProcess.Instance.chunkIndex + (int)Constants.SourcePositionIndex.left_hand][4]) / _SCALEFACTOR,
                    float.Parse(sData[DataProcess.Instance.chunkIndex + (int)Constants.SourcePositionIndex.left_hand][3]) / _SCALEFACTOR);

        //Right Arm
        s_jointPoints[(int)Constants.SourcePositionIndex.right_shoulder].inputJointPosition =
            new Vector3(float.Parse(sData[DataProcess.Instance.chunkIndex + (int)Constants.SourcePositionIndex.right_shoulder][2]) / _SCALEFACTOR,
                    float.Parse(sData[DataProcess.Instance.chunkIndex + (int)Constants.SourcePositionIndex.right_shoulder][4]) / _SCALEFACTOR,
                    float.Parse(sData[DataProcess.Instance.chunkIndex + (int)Constants.SourcePositionIndex.right_shoulder][3]) / _SCALEFACTOR);
        s_jointPoints[(int)Constants.SourcePositionIndex.right_elbow].inputJointPosition =
            new Vector3(float.Parse(sData[DataProcess.Instance.chunkIndex + (int)Constants.SourcePositionIndex.right_elbow][2]) / _SCALEFACTOR,
                    float.Parse(sData[DataProcess.Instance.chunkIndex + (int)Constants.SourcePositionIndex.right_elbow][4]) / _SCALEFACTOR,
                    float.Parse(sData[DataProcess.Instance.chunkIndex + (int)Constants.SourcePositionIndex.right_elbow][3]) / _SCALEFACTOR);
        s_jointPoints[(int)Constants.SourcePositionIndex.right_hand].inputJointPosition =
            new Vector3(float.Parse(sData[DataProcess.Instance.chunkIndex + (int)Constants.SourcePositionIndex.right_hand][2]) / _SCALEFACTOR,
                    float.Parse(sData[DataProcess.Instance.chunkIndex + (int)Constants.SourcePositionIndex.right_hand][4]) / _SCALEFACTOR,
                    float.Parse(sData[DataProcess.Instance.chunkIndex + (int)Constants.SourcePositionIndex.right_hand][3]) / _SCALEFACTOR);

        s_jointPoints[(int)Constants.SourcePositionIndex.right_hip].inputJointPosition = RotateAround(s_jointPoints[(int)Constants.SourcePositionIndex.right_hip].inputJointPosition, s_initPosition, rotation);
        s_jointPoints[(int)Constants.SourcePositionIndex.right_knee].inputJointPosition = RotateAround(s_jointPoints[(int)Constants.SourcePositionIndex.right_knee].inputJointPosition, s_initPosition, rotation);
        s_jointPoints[(int)Constants.SourcePositionIndex.right_foot].inputJointPosition = RotateAround(s_jointPoints[(int)Constants.SourcePositionIndex.right_foot].inputJointPosition, s_initPosition, rotation);

        s_jointPoints[(int)Constants.SourcePositionIndex.left_hip].inputJointPosition = RotateAround(s_jointPoints[(int)Constants.SourcePositionIndex.left_hip].inputJointPosition, s_initPosition, rotation);
        s_jointPoints[(int)Constants.SourcePositionIndex.left_knee].inputJointPosition = RotateAround(s_jointPoints[(int)Constants.SourcePositionIndex.left_knee].inputJointPosition, s_initPosition, rotation);
        s_jointPoints[(int)Constants.SourcePositionIndex.left_foot].inputJointPosition = RotateAround(s_jointPoints[(int)Constants.SourcePositionIndex.left_foot].inputJointPosition, s_initPosition, rotation);

        s_jointPoints[(int)Constants.SourcePositionIndex.center_torso].inputJointPosition = RotateAround(s_jointPoints[(int)Constants.SourcePositionIndex.center_torso].inputJointPosition, s_initPosition, rotation);
        s_jointPoints[(int)Constants.SourcePositionIndex.upper_torso].inputJointPosition = RotateAround(s_jointPoints[(int)Constants.SourcePositionIndex.upper_torso].inputJointPosition, s_initPosition, rotation);
        s_jointPoints[(int)Constants.SourcePositionIndex.neck_base].inputJointPosition = RotateAround(s_jointPoints[(int)Constants.SourcePositionIndex.neck_base].inputJointPosition, s_initPosition, rotation);
        s_jointPoints[(int)Constants.SourcePositionIndex.center_head].inputJointPosition = RotateAround(s_jointPoints[(int)Constants.SourcePositionIndex.center_head].inputJointPosition, s_initPosition, rotation);

        s_jointPoints[(int)Constants.SourcePositionIndex.left_shoulder].inputJointPosition = RotateAround(s_jointPoints[(int)Constants.SourcePositionIndex.left_shoulder].inputJointPosition, s_initPosition, rotation);
        s_jointPoints[(int)Constants.SourcePositionIndex.left_elbow].inputJointPosition = RotateAround(s_jointPoints[(int)Constants.SourcePositionIndex.left_elbow].inputJointPosition, s_initPosition, rotation);
        s_jointPoints[(int)Constants.SourcePositionIndex.left_hand].inputJointPosition = RotateAround(s_jointPoints[(int)Constants.SourcePositionIndex.left_hand].inputJointPosition, s_initPosition, rotation);

        s_jointPoints[(int)Constants.SourcePositionIndex.right_shoulder].inputJointPosition = RotateAround(s_jointPoints[(int)Constants.SourcePositionIndex.right_shoulder].inputJointPosition, s_initPosition, rotation);
        s_jointPoints[(int)Constants.SourcePositionIndex.right_elbow].inputJointPosition = RotateAround(s_jointPoints[(int)Constants.SourcePositionIndex.right_elbow].inputJointPosition, s_initPosition, rotation);
        s_jointPoints[(int)Constants.SourcePositionIndex.right_hand].inputJointPosition = RotateAround(s_jointPoints[(int)Constants.SourcePositionIndex.right_hand].inputJointPosition, s_initPosition, rotation);
    }

    private void KalmanUpdate(JointPoint measurement)
    {
        MeasurementUpdate(measurement);
        measurement.inputJointPosition.x = measurement.X.x + (measurement.estimatedJointPosition.x - measurement.X.x) * measurement.K.x;
        measurement.inputJointPosition.y = measurement.X.y + (measurement.estimatedJointPosition.y - measurement.X.y) * measurement.K.y;
        measurement.inputJointPosition.z = measurement.X.z + (measurement.estimatedJointPosition.z - measurement.X.z) * measurement.K.z;
        measurement.X = measurement.inputJointPosition;
    }

    private void MeasurementUpdate(JointPoint measurement)
    {
        measurement.K.x = (measurement.P.x + KalmanParamQ) / (measurement.P.x + KalmanParamQ + KalmanParamR);
        measurement.K.y = (measurement.P.y + KalmanParamQ) / (measurement.P.y + KalmanParamQ + KalmanParamR);
        measurement.K.z = (measurement.P.z + KalmanParamQ) / (measurement.P.z + KalmanParamQ + KalmanParamR);
        measurement.P.x = KalmanParamR * (measurement.P.x + KalmanParamQ) / (measurement.P.x + KalmanParamQ + KalmanParamR);
        measurement.P.y = KalmanParamR * (measurement.P.y + KalmanParamQ) / (measurement.P.y + KalmanParamQ + KalmanParamR);
        measurement.P.z = KalmanParamR * (measurement.P.z + KalmanParamQ) / (measurement.P.z + KalmanParamQ + KalmanParamR);
    }

}
