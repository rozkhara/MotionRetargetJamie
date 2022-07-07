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
    public class Bone
    {
        public Transform Transform = null;
        public Vector3 orientation;
        public float BoneLength;
        public Bone Child = null;
    }

    public Bone BaseBone;
    public Bone[] SourceBones;

    public FirstPosition a;

    void Start()
    {
        SourceBones = new Bone[(int)SourceBoneIndex.count];
        for (int i = 0; i < (int)SourceBoneIndex.count; i++) SourceBones[i] = new Bone();

        a = GameObject.Find("sourcePositionWise").GetComponent<FirstPosition>();
        Init();
    }

    void Update()
    {
        Debug.Log(SourceBones[(int)SourceBoneIndex.spine].orientation);
        for (int i = 0; i < (int)SourceBoneIndex.count; i++)
        {
            //Debug.Log(SourceBones[i].BoneLength); 
        }
        
    }

    void Init()
    {
        //BaseBone.Transform.position = a.newTransform[0];

        SourceBones[(int)SourceBoneIndex.spine].BoneLength = Vector3.Distance(a.newTransform[0], a.newTransform[7]);
        SourceBones[(int)SourceBoneIndex.chest].BoneLength = Vector3.Distance(a.newTransform[7], a.newTransform[8]);
        SourceBones[(int)SourceBoneIndex.neck].BoneLength = Vector3.Distance(a.newTransform[8], a.newTransform[9]);
        SourceBones[(int)SourceBoneIndex.head].BoneLength = Vector3.Distance(a.newTransform[9], a.newTransform[10]);

        SourceBones[(int)SourceBoneIndex.rUpperLeg].BoneLength = Vector3.Distance(a.newTransform[0], a.newTransform[1]);
        SourceBones[(int)SourceBoneIndex.rLowerLeg].BoneLength = Vector3.Distance(a.newTransform[1], a.newTransform[2]);
        SourceBones[(int)SourceBoneIndex.rFoot].BoneLength = Vector3.Distance(a.newTransform[2], a.newTransform[3]);

        SourceBones[(int)SourceBoneIndex.lUpperLeg].BoneLength = Vector3.Distance(a.newTransform[0], a.newTransform[4]);
        SourceBones[(int)SourceBoneIndex.lLowerLeg].BoneLength = Vector3.Distance(a.newTransform[4], a.newTransform[5]);
        SourceBones[(int)SourceBoneIndex.lFoot].BoneLength = Vector3.Distance(a.newTransform[5], a.newTransform[6]);

        SourceBones[(int)SourceBoneIndex.rUpperArm].BoneLength = Vector3.Distance(a.newTransform[8], a.newTransform[14]);
        SourceBones[(int)SourceBoneIndex.rLowerArm].BoneLength = Vector3.Distance(a.newTransform[14], a.newTransform[15]);
        SourceBones[(int)SourceBoneIndex.rHand].BoneLength = Vector3.Distance(a.newTransform[15], a.newTransform[16]);

        SourceBones[(int)SourceBoneIndex.lUpperArm].BoneLength = Vector3.Distance(a.newTransform[8], a.newTransform[11]);
        SourceBones[(int)SourceBoneIndex.lLowerArm].BoneLength = Vector3.Distance(a.newTransform[11], a.newTransform[12]);
        SourceBones[(int)SourceBoneIndex.lHand].BoneLength = Vector3.Distance(a.newTransform[12], a.newTransform[13]);

        //target skeleton의 정보를 받아와서 bone의 orientation 반영 -> 그런데 어떻게 받아오는지 모르겠다. 찾아보기
        SourceBones[(int)SourceBoneIndex.spine].orientation = transform.Find("Cha_Hips").GetChild(0).position - transform.Find("Cha_Hips").position;
        /*
        SourceBones[(int)SourceBoneIndex.chest]
        SourceBones[(int)SourceBoneIndex.neck];
        SourceBones[(int)SourceBoneIndex.head];

        SourceBones[(int)SourceBoneIndex.rUpperLeg].orientation = transform.Find("Cha_Hips").GetChild(2).position - transform.Find("Cha_Hips").position;
        SourceBones[(int)SourceBoneIndex.rLowerLeg];
        SourceBones[(int)SourceBoneIndex.rFoot];

        SourceBones[(int)SourceBoneIndex.lUpperLeg].orientation = transform.Find("Cha_Hips").GetChild(1).position - transform.Find("Cha_Hips").position;
        SourceBones[(int)SourceBoneIndex.lLowerLeg];
        SourceBones[(int)SourceBoneIndex.lFoot];

        SourceBones[(int)SourceBoneIndex.rUpperArm];
        SourceBones[(int)SourceBoneIndex.rLowerArm];
        SourceBones[(int)SourceBoneIndex.rHand];

        SourceBones[(int)SourceBoneIndex.lUpperArm];
        SourceBones[(int)SourceBoneIndex.lLowerArm];
        SourceBones[(int)SourceBoneIndex.lHand];
        */
    }

}
