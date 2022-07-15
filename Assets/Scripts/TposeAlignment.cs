using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum SourceBoneIndex : int //부모에서 해당 joint까지의 뼈
{
    spine = 0,
    chest,
    neck,
    head,

    lUpperArm,
    lLowerArm,
    lHand,

    rUpperArm,
    rLowerArm,
    rHand,

    lUpperLeg,
    lLowerLeg,
    lFoot,

    rUpperLeg,
    rLowerLeg,
    rFoot,

    count,
}

public class TposeAlignment : MonoBehaviour
{
    public Transform rootNode;           //최종 source skeleton의 transform
    public Transform[] childNodes;
    public List<Quaternion> perJointRotSourceGlobal;     //Rr G
    public List<Quaternion> perJointRotSourceLocal;    //qr init
    public List<Quaternion> perJointRotTargetGlobal;     //Rd G
    public List<Quaternion> perJointRotTargetLocal;    //qd init
    public bool flag = true;

    public List<Quaternion> qPre;
    public List<Quaternion> qPost;

    //임시
    private int[] targetPair = {0, 1, 2, -1, -1, -1, 5, 6, 7, -1, 8, 9, 10, -1, -1, -1, 3, -1, -1, -1, 11, 12, 13, -1, 14, 15, 16, -1 };

    public Transform t_rootNode;         //target skeleton의 transform (원본 Chr_Hips)
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

        //target skeleton의 정보를 받아와서 bone의 orientation 반영 (localPosition을 이용해 자식의 부모기준 위치를 받아옴)
        foreach (Transform child in t_childNodes)
        {
            if (child.name.Contains("Hips"))
            {
                SourceBones[(int)SourceBoneIndex.spine].orientation = Vector3.Normalize(child.GetChild(0).localPosition);
                SourceBones[(int)SourceBoneIndex.lUpperLeg].orientation = Vector3.Normalize(child.GetChild(1).localPosition);
                SourceBones[(int)SourceBoneIndex.rUpperLeg].orientation = Vector3.Normalize(child.GetChild(2).localPosition);
            }
            else if (child.name.Contains("Spine"))
            {
                SourceBones[(int)SourceBoneIndex.chest].orientation = Vector3.Normalize(child.GetChild(0).localPosition);
            }
            else if (child.name.Contains("Chest"))
            {
                //target과 source skeleton의 위상이 다르므로, 양 UpperArm은 서로의 좌표를 이용해 구함
                SourceBones[(int)SourceBoneIndex.lUpperArm].orientation = Vector3.Normalize(child.GetChild(1).localPosition - child.GetChild(2).localPosition);
                SourceBones[(int)SourceBoneIndex.rUpperArm].orientation = Vector3.Normalize(child.GetChild(2).localPosition - child.GetChild(1).localPosition);
                //원본에서 cape, wing 때문에 face가 5번 index
                SourceBones[(int)SourceBoneIndex.neck].orientation = Vector3.Normalize(child.GetChild(5).localPosition);
                SourceBones[(int)SourceBoneIndex.head].orientation = Vector3.Normalize(child.GetChild(5).localPosition);
            }
            else if (child.name.Contains("UpperArmL"))
            {
                SourceBones[(int)SourceBoneIndex.lLowerArm].orientation = Vector3.Normalize(child.GetChild(0).localPosition);
            }
            else if (child.name.Contains("UpperArmR"))
            {
                SourceBones[(int)SourceBoneIndex.rLowerArm].orientation = Vector3.Normalize(child.GetChild(0).localPosition);
            }
            else if (child.name.Contains("UpperLegL"))
            {
                SourceBones[(int)SourceBoneIndex.lLowerLeg].orientation = Vector3.Normalize(child.GetChild(0).localPosition);
            }
            else if (child.name.Contains("UpperLegR"))
            {
                SourceBones[(int)SourceBoneIndex.rLowerLeg].orientation = Vector3.Normalize(child.GetChild(0).localPosition);
            }
            else if (child.name.Contains("LowerArmL"))
            {
                SourceBones[(int)SourceBoneIndex.lHand].orientation = Vector3.Normalize(child.GetChild(0).localPosition);
            }
            else if (child.name.Contains("LowerArmR"))
            {
                SourceBones[(int)SourceBoneIndex.rHand].orientation = Vector3.Normalize(child.GetChild(0).localPosition);
            }
            else if (child.name.Contains("LowerLegL"))
            {
                SourceBones[(int)SourceBoneIndex.lFoot].orientation = Vector3.Normalize(child.GetChild(0).localPosition);
            }
            else if (child.name.Contains("LowerLegR"))
            {
                SourceBones[(int)SourceBoneIndex.rFoot].orientation = Vector3.Normalize(child.GetChild(0).localPosition);
            }
        }

        //basis 일치를 위해 rotation 값 가져오기
        foreach (Transform child in t_childNodes)
        {
            if (child.name.Contains("Hips"))
            {
                rootNode.rotation = child.rotation;
            }
            else if (child.name.Contains("Spine"))
            {
                SourceBones[(int)SourceBoneIndex.spine].rotation = child.rotation;
            }
            else if (child.name.Contains("Chest"))
            {
                SourceBones[(int)SourceBoneIndex.chest].rotation = child.rotation;
            }
            else if (child.name.Contains("UpperArmL"))
            {
                SourceBones[(int)SourceBoneIndex.lUpperArm].rotation = child.rotation;
            }
            else if (child.name.Contains("UpperArmR"))
            {
                SourceBones[(int)SourceBoneIndex.rUpperArm].rotation = child.rotation;
            }
            else if (child.name.Contains("UpperLegL"))
            {
                SourceBones[(int)SourceBoneIndex.lUpperLeg].rotation = child.rotation;
            }
            else if (child.name.Contains("UpperLegR"))
            {
                SourceBones[(int)SourceBoneIndex.rUpperLeg].rotation = child.rotation;
            }
            else if (child.name.Contains("LowerArmL"))
            {
                SourceBones[(int)SourceBoneIndex.lLowerArm].rotation = child.rotation;
            }
            else if (child.name.Contains("LowerArmR"))
            {
                SourceBones[(int)SourceBoneIndex.rLowerArm].rotation = child.rotation;
            }
            else if (child.name.Contains("LowerLegL"))
            {
                SourceBones[(int)SourceBoneIndex.lLowerLeg].rotation = child.rotation;
            }
            else if (child.name.Contains("LowerLegR"))
            {
                SourceBones[(int)SourceBoneIndex.rLowerLeg].rotation = child.rotation;
            }
            else if (child.name.Contains("FootL"))
            {
                SourceBones[(int)SourceBoneIndex.lFoot].rotation = child.rotation;
            }
            else if (child.name.Contains("FootR"))
            {
                SourceBones[(int)SourceBoneIndex.rFoot].rotation = child.rotation;
            }
            else if (child.name.Contains("HandL"))
            {
                SourceBones[(int)SourceBoneIndex.lHand].rotation = child.rotation;
            }
            else if (child.name.Contains("HandR"))
            {
                SourceBones[(int)SourceBoneIndex.rHand].rotation = child.rotation;
            }
            else if (child.name.Contains("Face"))
            {
                SourceBones[(int)SourceBoneIndex.neck].rotation = child.rotation;
                SourceBones[(int)SourceBoneIndex.head].rotation = child.rotation;
            }
        }

        //뼈의 길이와 방향 정보를 이용해 각 transform의 position 결정
        foreach (Transform child in childNodes)
        {
            if (child.name != "Tpose" && child.name != "base")
            {
                SourceBoneIndex s = (SourceBoneIndex)System.Enum.Parse(typeof(SourceBoneIndex), child.name);
                calcTransform((int)s, child);
            }
        }

        for (int i = 0; i < childNodes.Length; i++)
        {
            perJointRotSourceGlobal.Add(childNodes[i].rotation);
            perJointRotSourceLocal.Add(childNodes[i].localRotation);
        }
        for (int i = 0; i < t_childNodes.Length; i++)
        {
            perJointRotTargetGlobal.Add(t_childNodes[i].rotation);
            perJointRotTargetLocal.Add(t_childNodes[i].localRotation);

            //임시
            int j = targetPair[i];
            if (j != -1)
            {
                qPre.Add(perJointRotTargetLocal[i] * Quaternion.Inverse(perJointRotTargetGlobal[i]) * perJointRotSourceGlobal[j] * Quaternion.Inverse(perJointRotSourceLocal[j]));
                qPost.Add(Quaternion.Inverse(perJointRotSourceGlobal[j]) * perJointRotTargetGlobal[i]);
                //Debug.LogFormat("qPre : {0}*{1}*{2}*{3}\nqPost : {4}*{5}", perJointRotTargetLocal[i], Quaternion.Inverse(perJointRotTargetGlobal[i]), perJointRotSourceGlobal[j], Quaternion.Inverse(perJointRotSourceLocal[j]), Quaternion.Inverse(perJointRotSourceGlobal[j]), perJointRotTargetGlobal[i]);
            }
            else
            {
                qPre.Add(Quaternion.identity);
                qPost.Add(Quaternion.identity);
            }
            //Debug.LogFormat("qPre[{0}] : {1} qPost[{0}] : {2} qPre[{0}] * qPost[{0}] : {3}\nj : {4}", i, qPre[i], qPost[i], qPre[i] * qPost[i], j);
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

