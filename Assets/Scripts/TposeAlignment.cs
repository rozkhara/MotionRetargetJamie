using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum SourceBoneIndex : int
{
    spine = 0,
    chest,
    neck,
    head,

    rUpperLeg,
    rLowerLeg,
    rFoot,

    lUpperLeg,
    lLowerLeg,
    lFoot,

    rUpperArm,
    rLowerArm,
    rHand,

    lUpperArm,
    lLowerArm,
    lHand,

    count,
}

public class TposeAlignment : MonoBehaviour
{
    public Transform rootNode;           // transform of source skeleton
    public Transform[] childNodes;
    public bool flag = true;

    public Transform t_rootNode;         // transform of target skeleton (Chr_Hips)
    public Transform[] t_childNodes;

    public class Bone
    {
        public Quaternion rotation;
        public Vector3 orientation;
        public float BoneLength;
    }

    public Bone BaseBone;
    public Bone[] SourceBones;

    public PositionWiseUpdate a;

    public bool globalBasis;

    void Start()
    {
        SourceBones = new Bone[(int)SourceBoneIndex.count];
        for (int i = 0; i < (int)SourceBoneIndex.count; i++) SourceBones[i] = new Bone();
        a = GameObject.Find("sourcePositionWise").GetComponent<PositionWiseUpdate>();
        if (rootNode != null)
        {
            if (childNodes == null || childNodes.Length == 0)
            {
                //get all joints to draw
                childNodes = rootNode.GetComponentsInChildren<Transform>();
            }
        }
        if (t_rootNode != null)
        {
            if (t_childNodes == null || t_childNodes.Length == 0)
            {
                //get all joints to draw
                t_childNodes = t_rootNode.GetComponentsInChildren<Transform>();
            }
        }
    }

    void Update()
    {
        if (flag)
        {
            if (a.firstTransform.Count != 0)
            {
                Init();
                flag = false;
            }
        }

    }

    void Init()
    {
        // get the length of the bone from the pose text
        SourceBones[(int)SourceBoneIndex.spine].BoneLength = Vector3.Distance(a.firstTransform[0], a.firstTransform[7]);
        SourceBones[(int)SourceBoneIndex.chest].BoneLength = Vector3.Distance(a.firstTransform[7], a.firstTransform[8]);
        SourceBones[(int)SourceBoneIndex.neck].BoneLength = Vector3.Distance(a.firstTransform[8], a.firstTransform[9]);
        SourceBones[(int)SourceBoneIndex.head].BoneLength = Vector3.Distance(a.firstTransform[9], a.firstTransform[10]);

        SourceBones[(int)SourceBoneIndex.rUpperLeg].BoneLength = Vector3.Distance(a.firstTransform[0], a.firstTransform[1]);
        SourceBones[(int)SourceBoneIndex.rLowerLeg].BoneLength = Vector3.Distance(a.firstTransform[1], a.firstTransform[2]);
        SourceBones[(int)SourceBoneIndex.rFoot].BoneLength = Vector3.Distance(a.firstTransform[2], a.firstTransform[3]);

        SourceBones[(int)SourceBoneIndex.lUpperLeg].BoneLength = Vector3.Distance(a.firstTransform[0], a.firstTransform[4]);
        SourceBones[(int)SourceBoneIndex.lLowerLeg].BoneLength = Vector3.Distance(a.firstTransform[4], a.firstTransform[5]);
        SourceBones[(int)SourceBoneIndex.lFoot].BoneLength = Vector3.Distance(a.firstTransform[5], a.firstTransform[6]);

        SourceBones[(int)SourceBoneIndex.rUpperArm].BoneLength = Vector3.Distance(a.firstTransform[8], a.firstTransform[14]);
        SourceBones[(int)SourceBoneIndex.rLowerArm].BoneLength = Vector3.Distance(a.firstTransform[14], a.firstTransform[15]);
        SourceBones[(int)SourceBoneIndex.rHand].BoneLength = Vector3.Distance(a.firstTransform[15], a.firstTransform[16]);

        SourceBones[(int)SourceBoneIndex.lUpperArm].BoneLength = Vector3.Distance(a.firstTransform[8], a.firstTransform[11]);
        SourceBones[(int)SourceBoneIndex.lLowerArm].BoneLength = Vector3.Distance(a.firstTransform[11], a.firstTransform[12]);
        SourceBones[(int)SourceBoneIndex.lHand].BoneLength = Vector3.Distance(a.firstTransform[12], a.firstTransform[13]);

        if (globalBasis)
        {
            // reflect the orientation of the bone by receiving the target skeleton information
            // (using localPosition to receive the parent relative position of the child)
            SourceBones[(int)SourceBoneIndex.spine].orientation = Vector3.Normalize(t_childNodes[(int)Constants.TargetPositionIndex.Cha_Hips].GetChild(0).position - t_childNodes[(int)Constants.TargetPositionIndex.Cha_Hips].position);
            SourceBones[(int)SourceBoneIndex.chest].orientation = Vector3.Normalize(t_childNodes[(int)Constants.TargetPositionIndex.Cha_Spine].GetChild(0).position - t_childNodes[(int)Constants.TargetPositionIndex.Cha_Spine].position);
            SourceBones[(int)SourceBoneIndex.neck].orientation = Vector3.Normalize(t_childNodes[(int)Constants.TargetPositionIndex.Cha_Chest].GetChild(5).position - t_childNodes[(int)Constants.TargetPositionIndex.Cha_Chest].position);  //원본에서 cape, wing 때문에 face가 5번 index
            SourceBones[(int)SourceBoneIndex.head].orientation = Vector3.Normalize(t_childNodes[(int)Constants.TargetPositionIndex.Cha_Chest].GetChild(5).position - t_childNodes[(int)Constants.TargetPositionIndex.Cha_Chest].position);

            SourceBones[(int)SourceBoneIndex.rUpperLeg].orientation = Vector3.Normalize(t_childNodes[(int)Constants.TargetPositionIndex.Cha_Hips].GetChild(2).position - t_childNodes[(int)Constants.TargetPositionIndex.Cha_Hips].position);
            SourceBones[(int)SourceBoneIndex.rLowerLeg].orientation = Vector3.Normalize(t_childNodes[(int)Constants.TargetPositionIndex.Cha_UpperLegR].GetChild(0).position - t_childNodes[(int)Constants.TargetPositionIndex.Cha_UpperLegR].position);
            SourceBones[(int)SourceBoneIndex.rFoot].orientation = Vector3.Normalize(t_childNodes[(int)Constants.TargetPositionIndex.Cha_LowerLegR].GetChild(0).position - t_childNodes[(int)Constants.TargetPositionIndex.Cha_LowerLegR].position);

            SourceBones[(int)SourceBoneIndex.lUpperLeg].orientation = Vector3.Normalize(t_childNodes[(int)Constants.TargetPositionIndex.Cha_Hips].GetChild(1).position - t_childNodes[(int)Constants.TargetPositionIndex.Cha_Hips].position);
            SourceBones[(int)SourceBoneIndex.lLowerLeg].orientation = Vector3.Normalize(t_childNodes[(int)Constants.TargetPositionIndex.Cha_UpperLegL].GetChild(0).position - t_childNodes[(int)Constants.TargetPositionIndex.Cha_UpperLegL].position);
            SourceBones[(int)SourceBoneIndex.lFoot].orientation = Vector3.Normalize(t_childNodes[(int)Constants.TargetPositionIndex.Cha_LowerLegL].GetChild(0).position - t_childNodes[(int)Constants.TargetPositionIndex.Cha_LowerLegL].position);

            SourceBones[(int)SourceBoneIndex.rUpperArm].orientation = Vector3.Normalize(t_childNodes[(int)Constants.TargetPositionIndex.Cha_UpperArmR].position
                                                                                        - t_childNodes[(int)Constants.TargetPositionIndex.Cha_UpperArmL].position);
            SourceBones[(int)SourceBoneIndex.rLowerArm].orientation = Vector3.Normalize(t_childNodes[(int)Constants.TargetPositionIndex.Cha_UpperArmR].GetChild(0).position - t_childNodes[(int)Constants.TargetPositionIndex.Cha_UpperArmR].position);
            SourceBones[(int)SourceBoneIndex.rHand].orientation = Vector3.Normalize(t_childNodes[(int)Constants.TargetPositionIndex.Cha_LowerArmR].GetChild(0).position - t_childNodes[(int)Constants.TargetPositionIndex.Cha_LowerArmR].position);

            SourceBones[(int)SourceBoneIndex.lUpperArm].orientation = Vector3.Normalize(t_childNodes[(int)Constants.TargetPositionIndex.Cha_UpperArmL].position
                                                                                        - t_childNodes[(int)Constants.TargetPositionIndex.Cha_UpperArmR].position);
            SourceBones[(int)SourceBoneIndex.lLowerArm].orientation = Vector3.Normalize(t_childNodes[(int)Constants.TargetPositionIndex.Cha_UpperArmL].GetChild(0).position - t_childNodes[(int)Constants.TargetPositionIndex.Cha_UpperArmL].position);
            SourceBones[(int)SourceBoneIndex.lHand].orientation = Vector3.Normalize(t_childNodes[(int)Constants.TargetPositionIndex.Cha_LowerArmL].GetChild(0).position - t_childNodes[(int)Constants.TargetPositionIndex.Cha_LowerArmL].position);
            // since the target and source skeleton have different topology, both UpperArms are obtained using each other's coordinates.
        }
        else
        {
            // reflect target skeleton to the orientation of the bone
            // (using localPosition to receive the parent relative position of the child)
            SourceBones[(int)SourceBoneIndex.spine].orientation = Vector3.Normalize(t_childNodes[(int)Constants.TargetPositionIndex.Cha_Hips].GetChild(0).localPosition);
            SourceBones[(int)SourceBoneIndex.chest].orientation = Vector3.Normalize(t_childNodes[(int)Constants.TargetPositionIndex.Cha_Spine].GetChild(0).localPosition);
            SourceBones[(int)SourceBoneIndex.neck].orientation = Vector3.Normalize(t_childNodes[(int)Constants.TargetPositionIndex.Cha_Chest].GetChild(5).localPosition);  //원본에서 cape, wing 때문에 face가 5번 index
            SourceBones[(int)SourceBoneIndex.head].orientation = Vector3.Normalize(t_childNodes[(int)Constants.TargetPositionIndex.Cha_Chest].GetChild(5).localPosition);

            SourceBones[(int)SourceBoneIndex.rUpperLeg].orientation = Vector3.Normalize(t_childNodes[(int)Constants.TargetPositionIndex.Cha_Hips].GetChild(2).localPosition);
            SourceBones[(int)SourceBoneIndex.rLowerLeg].orientation = Vector3.Normalize(t_childNodes[(int)Constants.TargetPositionIndex.Cha_LowerLegR].GetChild(0).localPosition);
            SourceBones[(int)SourceBoneIndex.rFoot].orientation = Vector3.Normalize(t_childNodes[(int)Constants.TargetPositionIndex.Cha_FootR].GetChild(0).localPosition);

            SourceBones[(int)SourceBoneIndex.lUpperLeg].orientation = Vector3.Normalize(t_childNodes[(int)Constants.TargetPositionIndex.Cha_Hips].GetChild(1).localPosition);
            SourceBones[(int)SourceBoneIndex.lLowerLeg].orientation = Vector3.Normalize(t_childNodes[(int)Constants.TargetPositionIndex.Cha_LowerLegL].GetChild(0).localPosition);
            SourceBones[(int)SourceBoneIndex.lFoot].orientation = Vector3.Normalize(t_childNodes[(int)Constants.TargetPositionIndex.Cha_FootL].GetChild(0).localPosition);

            SourceBones[(int)SourceBoneIndex.rUpperArm].orientation = Vector3.Normalize(t_childNodes[(int)Constants.TargetPositionIndex.Cha_Chest].GetChild(2).localPosition
                                                                                        - t_childNodes[(int)Constants.TargetPositionIndex.Cha_Chest].GetChild(1).localPosition);
            SourceBones[(int)SourceBoneIndex.rLowerArm].orientation = Vector3.Normalize(t_childNodes[(int)Constants.TargetPositionIndex.Cha_LowerArmR].GetChild(0).localPosition);
            SourceBones[(int)SourceBoneIndex.rHand].orientation = Vector3.Normalize(t_childNodes[(int)Constants.TargetPositionIndex.Cha_HandR].GetChild(0).localPosition);

            SourceBones[(int)SourceBoneIndex.lUpperArm].orientation = Vector3.Normalize(t_childNodes[(int)Constants.TargetPositionIndex.Cha_Chest].GetChild(1).localPosition
                                                                                        - t_childNodes[(int)Constants.TargetPositionIndex.Cha_Chest].GetChild(2).localPosition);
            SourceBones[(int)SourceBoneIndex.lLowerArm].orientation = Vector3.Normalize(t_childNodes[(int)Constants.TargetPositionIndex.Cha_LowerArmL].GetChild(0).localPosition);
            SourceBones[(int)SourceBoneIndex.lHand].orientation = Vector3.Normalize(t_childNodes[(int)Constants.TargetPositionIndex.Cha_HandL].GetChild(0).localPosition);  //target과 source skeleton의 위상이 다르므로, 양 UpperArm은 서로의 좌표를 이용해 구함

            // get rotation value for basis match
            rootNode.rotation = t_childNodes[(int)Constants.TargetPositionIndex.Cha_Hips].rotation;
            SourceBones[(int)SourceBoneIndex.spine].rotation = t_childNodes[(int)Constants.TargetPositionIndex.Cha_Spine].rotation;
            SourceBones[(int)SourceBoneIndex.chest].rotation = t_childNodes[(int)Constants.TargetPositionIndex.Cha_Chest].rotation;
            SourceBones[(int)SourceBoneIndex.neck].rotation = t_childNodes[(int)Constants.TargetPositionIndex.Face].rotation;
            SourceBones[(int)SourceBoneIndex.head].rotation = t_childNodes[(int)Constants.TargetPositionIndex.Face].rotation;

            SourceBones[(int)SourceBoneIndex.rUpperLeg].rotation = t_childNodes[(int)Constants.TargetPositionIndex.Cha_UpperLegR].rotation;
            SourceBones[(int)SourceBoneIndex.rLowerLeg].rotation = t_childNodes[(int)Constants.TargetPositionIndex.Cha_LowerLegR].rotation;
            SourceBones[(int)SourceBoneIndex.rFoot].rotation = t_childNodes[(int)Constants.TargetPositionIndex.Cha_FootR].rotation;

            SourceBones[(int)SourceBoneIndex.lUpperLeg].rotation = t_childNodes[(int)Constants.TargetPositionIndex.Cha_UpperLegL].rotation;
            SourceBones[(int)SourceBoneIndex.lLowerLeg].rotation = t_childNodes[(int)Constants.TargetPositionIndex.Cha_LowerLegL].rotation;
            SourceBones[(int)SourceBoneIndex.lFoot].rotation = t_childNodes[(int)Constants.TargetPositionIndex.Cha_FootL].rotation;

            SourceBones[(int)SourceBoneIndex.rUpperArm].rotation = t_childNodes[(int)Constants.TargetPositionIndex.Cha_UpperArmR].rotation;
            SourceBones[(int)SourceBoneIndex.rLowerArm].rotation = t_childNodes[(int)Constants.TargetPositionIndex.Cha_LowerArmR].rotation;
            SourceBones[(int)SourceBoneIndex.rHand].rotation = t_childNodes[(int)Constants.TargetPositionIndex.Cha_HandR].rotation;

            SourceBones[(int)SourceBoneIndex.lUpperArm].rotation = t_childNodes[(int)Constants.TargetPositionIndex.Cha_UpperArmL].rotation;
            SourceBones[(int)SourceBoneIndex.lLowerArm].rotation = t_childNodes[(int)Constants.TargetPositionIndex.Cha_LowerArmL].rotation;
            SourceBones[(int)SourceBoneIndex.lHand].rotation = t_childNodes[(int)Constants.TargetPositionIndex.Cha_HandL].rotation;
        }


        // determine the position of each transform using bone length and orientation information
        foreach (Transform child in childNodes)
        {
            if (!child.name.Contains("Tpose") && !child.name.Contains("base"))
            {
                SourceBoneIndex s = (SourceBoneIndex)System.Enum.Parse(typeof(SourceBoneIndex), child.name);
                calcTransform((int)s, child);
            }
        }
    }

    void calcTransform(int bone, Transform child)
    {
        float x = SourceBones[bone].orientation.x * SourceBones[bone].BoneLength;
        float y = SourceBones[bone].orientation.y * SourceBones[bone].BoneLength;
        float z = SourceBones[bone].orientation.z * SourceBones[bone].BoneLength;
        child.localPosition = new Vector3(x, y, z);

        child.rotation = SourceBones[bone].rotation;
    }
}

