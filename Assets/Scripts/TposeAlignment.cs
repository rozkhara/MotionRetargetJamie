using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum SourceBoneIndex : int //부모에서 해당 joint까지의 뼈
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
    public Transform rootNode;           //최종 source skeleton의 transform
    public Transform[] childNodes;

    public Transform t_rootNode;         //target skeleton의 transform (원본 Chr_Hips)
    public Transform[] t_childNodes;

    public class Bone
    {
        public Vector3 orientation;
        public float BoneLength;
    }

    public Bone BaseBone;
    public Bone[] SourceBones;

    public FirstPosition a;

    void Start()
    {
        SourceBones = new Bone[(int)SourceBoneIndex.count];
        for (int i = 0; i < (int)SourceBoneIndex.count; i++) SourceBones[i] = new Bone();

        a = GameObject.Find("sourcePositionWise").GetComponent<FirstPosition>();
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
        if(a.firstTransform.Count != 0)
        {
            Init();
        }
    }

    void Init()
    {

        //pose text에서 bone의 길이 가져오기
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

        //target skeleton의 정보를 받아와서 bone의 orientation 반영
        foreach (Transform child in t_childNodes)
        {
            if (child.name.Contains("Hips"))
            {
                SourceBones[(int)SourceBoneIndex.spine].orientation = Vector3.Normalize(child.GetChild(0).position - child.position);
                SourceBones[(int)SourceBoneIndex.lUpperLeg].orientation = Vector3.Normalize(child.GetChild(1).position - child.position);
                SourceBones[(int)SourceBoneIndex.rUpperLeg].orientation = Vector3.Normalize(child.GetChild(2).position - child.position);
            }
            else if (child.name.Contains("Spine"))
            {
                SourceBones[(int)SourceBoneIndex.chest].orientation = Vector3.Normalize(child.GetChild(0).position - child.position);
            }
            else if (child.name.Contains("Chest"))
            {
                //target과 source skeleton의 위상이 다르므로, 양 UpperArm은 서로의 좌표를 이용해 구함
                SourceBones[(int)SourceBoneIndex.lUpperArm].orientation = Vector3.Normalize(child.GetChild(1).position - child.GetChild(2).position);
                SourceBones[(int)SourceBoneIndex.rUpperArm].orientation = Vector3.Normalize(child.GetChild(2).position - child.GetChild(1).position);
                //원본에서 cape, wing 때문에 face가 5번 index
                SourceBones[(int)SourceBoneIndex.neck].orientation = Vector3.Normalize(child.GetChild(5).position - child.position);
                SourceBones[(int)SourceBoneIndex.head].orientation = Vector3.Normalize(child.GetChild(5).position - child.position);
            }
            else if (child.name.Contains("UpperArmL"))
            {
                SourceBones[(int)SourceBoneIndex.lLowerArm].orientation = Vector3.Normalize(child.GetChild(0).position - child.position);
            }
            else if (child.name.Contains("UpperArmR"))
            {
                SourceBones[(int)SourceBoneIndex.rLowerArm].orientation = Vector3.Normalize(child.GetChild(0).position - child.position);
            }
            else if (child.name.Contains("UpperLegL"))
            {
                SourceBones[(int)SourceBoneIndex.lLowerLeg].orientation = Vector3.Normalize(child.GetChild(0).position - child.position);
            }
            else if (child.name.Contains("UpperLegR"))
            {
                SourceBones[(int)SourceBoneIndex.rLowerLeg].orientation = Vector3.Normalize(child.GetChild(0).position - child.position);
            }
            else if (child.name.Contains("LowerArmL"))
            {
                SourceBones[(int)SourceBoneIndex.lHand].orientation = Vector3.Normalize(child.GetChild(0).position - child.position);
            }
            else if (child.name.Contains("LowerArmR"))
            {
                SourceBones[(int)SourceBoneIndex.rHand].orientation = Vector3.Normalize(child.GetChild(0).position - child.position);
            }
            else if (child.name.Contains("LowerLegL"))
            {
                SourceBones[(int)SourceBoneIndex.lFoot].orientation = Vector3.Normalize(child.GetChild(0).position - child.position);
            }
            else if (child.name.Contains("LowerLegR"))
            {
                SourceBones[(int)SourceBoneIndex.rFoot].orientation = Vector3.Normalize(child.GetChild(0).position - child.position);
            }
        }

        //뼈의 길이와 방향 정보를 이용해 각 transform의 position 결정
        foreach(Transform child in childNodes)
        {
            if (child.name != "Tpose" && child.name != "base")
            {
                SourceBoneIndex s = (SourceBoneIndex)System.Enum.Parse(typeof(SourceBoneIndex), child.name);
                calcPosition((int)s, child);
            }

        }

        //basis 일치 ?

    }

    void calcPosition(int bone, Transform child)
    {
        float x = SourceBones[bone].orientation.x * SourceBones[bone].BoneLength;
        float y = SourceBones[bone].orientation.y * SourceBones[bone].BoneLength;
        float z = SourceBones[bone].orientation.z * SourceBones[bone].BoneLength;
        child.localPosition = new Vector3(x, y, z);
    }
}

