using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Reflection;

public class CreateEETarget : MonoBehaviour
{    
    LegacyMatrix JacobianA;
    LegacyMatrix InverseA;
    //LegacyMatrix SelectMat;
    private bool[] isSelect;
    public int JointCount;
    public GameObject[] testPoints;
    public Quaternion[] initLocalRot;
    public Quaternion[] initRot;

    public Transform Root; //targetRot의 Cha_Hips
    public Transform[] Joints;

    public Transform SRoot;
    public Transform[] SJoints;

    // Start is called before the first frame update
    void Start()
    {
        Joints = Root.GetComponentsInChildren<Transform>();
        SJoints = SRoot.GetComponentsInChildren<Transform>();
        JointCount = Joints.Length;
        JacobianA = new LegacyMatrix(3, JointCount * 3);
        InverseA = new LegacyMatrix(JointCount * 3, 3);
        //SelectMat = new LegacyMatrix(JointCount * 3, JointCount * 3);
        initLocalRot = new Quaternion[JointCount];
        initRot = new Quaternion[JointCount];

        //jacobianA의 해당 열(joint)이 non zero인 경우 true
        //isSelect가 true인 joint는 collision avoidance가 적용되어야 하고, false이면 기존 값을 그대로 가져와야 함
        isSelect = new bool[JointCount];
        for (int i = 0; i < JointCount; i++)
        {
            initLocalRot[i] = Joints[i].localRotation;
            initRot[i] = Joints[i].rotation;
        }

        int count = 4;
        testPoints = new GameObject[count];
        for (int i = 0; i < count; i++)
        {
            GameObject sphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            sphere.transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);
            testPoints[i] = sphere;
        }
    }

    // Update is called once per frame
    void Update()
    {
        //console clear
        var assembly = Assembly.GetAssembly(typeof(UnityEditor.Editor));
        var type = assembly.GetType("UnityEditor.LogEntries");
        var method = type.GetMethod("Clear");
        method.Invoke(new object(), null);

        UpdateTargetPos(testPoints[0].transform, SJoints[12]);
        UpdateTargetPos(testPoints[1].transform, SJoints[8]);
        UpdateTargetPos(testPoints[2].transform, SJoints[26]);
        UpdateTargetPos(testPoints[3].transform, SJoints[22]);

        JacobianMatrix(testPoints[0].transform, Joints[11]);
        Debug.Log(testPoints[0].transform.position.ToString());
        Debug.Log(JacobianA.ToString());
        InverseA = (JacobianA.T * JacobianA);
        Debug.Log(JacobianA.T);
        //Debug.Log((InverseA * JacobianA).ToString());
    }

    void UpdateTargetPos(Transform Pos, Transform nextPos)
    {
        Pos.position = Root.position + (nextPos.position - SRoot.position) * 0.7f; //0.7은 임의의 값
    }

    void JacobianMatrix(Transform tee, Transform pee) //pajoint는 pa가 속해있는 joint
    {
        Transform temp = pee;
        //만약 해당 end effector가 해당 joint의 자손일 경우 Jacobian값 채우기
        while (temp != Root.parent)
        {
            for (int j = 0; j < JointCount; j++)
            {
                if (temp.name == Joints[j].name)
                {
                    Vector3 a = Vector3.Cross(Joints[j].right, (tee.position - Joints[j].position));
                    Vector3 b = Vector3.Cross(Joints[j].up, (tee.position - Joints[j].position));
                    Vector3 c = Vector3.Cross(Joints[j].forward, (tee.position - Joints[j].position));

                    //현재 Joint가 아니라 Tpose 기준으로 계산
                    //Vector3 a = Vector3.Cross(TJoints[j].right, (pa.position - TJoints[j].position));
                    //Vector3 b = Vector3.Cross(TJoints[j].up, (pa.position - TJoints[j].position));
                    //Vector3 c = Vector3.Cross(TJoints[j].forward, (pa.position - TJoints[j].position));

                    JacobianA[0, j * 3] = a.x;
                    JacobianA[1, j * 3] = a.y;
                    JacobianA[2, j * 3] = a.z;
                    JacobianA[0, j * 3 + 1] = b.x;
                    JacobianA[1, j * 3 + 1] = b.y;
                    JacobianA[2, j * 3 + 1] = b.z;
                    JacobianA[0, j * 3 + 2] = c.x;
                    JacobianA[1, j * 3 + 2] = c.y;
                    JacobianA[2, j * 3 + 2] = c.z;

                    temp = temp.transform.parent;
                    isSelect[j] = true;
                    break;
                }
            }
        }
    }
}
