using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Reflection;

public class JacobianIK : MonoBehaviour
{    
    private LegacyMatrix JacobianA;
    private LegacyMatrix InverseA;
    private LegacyMatrix ErrorA;
    private LegacyMatrix deltaA;
    private LegacyMatrix Identity;
    private float lamda = Mathf.Pow(1.4f * 0.05f, 2f);
    private Vector3[] rotateAxis;
    private int[] EEindex = { (int)Constants.TargetPositionIndex.Cha_HandR, (int)Constants.TargetPositionIndex.Cha_HandL, (int)Constants.TargetPositionIndex.Cha_FootR, (int)Constants.TargetPositionIndex.Cha_FootL }; // 

    private bool IsInitialized = true;
    private float Sarm, arm, Sleg, leg;
    private float smoothParam = 0.6f;

    private int JointCount;
    private Quaternion[] prevRot;

    public GameObject[] EESphere;
    public Transform SRoot;
    public Transform[] SJoints;

    public Transform Root; //targetRot의 Cha_Hips
    public Transform[] Joints;

    void Start()
    {
        Joints = Root.GetComponentsInChildren<Transform>();
        SJoints = SRoot.GetComponentsInChildren<Transform>();
        JointCount = Joints.Length;
        JacobianA = new LegacyMatrix(3 * EEindex.Length, JointCount);
        InverseA = new LegacyMatrix(JointCount, 3 * EEindex.Length);
        Identity = new LegacyMatrix(3 * EEindex.Length, 3 * EEindex.Length);
        for(int i = 0; i < 3 * EEindex.Length; i++)
        {
            Identity[i, i] = 1;
        }
        ErrorA = new LegacyMatrix(3 * EEindex.Length, 1);
        rotateAxis = new Vector3[JointCount];
        prevRot = new Quaternion[JointCount];

        EESphere = new GameObject[EEindex.Length];
        for (int i = 0; i < EEindex.Length; i++)
        {
            GameObject sphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            sphere.transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);
            EESphere[i] = sphere;
        }
    }

    private void LateUpdate()
    {
        ClearConsole();
        for (int i = 0; i < EEindex.Length; i++)
        {
            UpdateTargetPos(EESphere[i].transform, SJoints[EEindex[i]]);
        }
        IterateIK();
    }

    void ClearConsole()
    {
        var assembly = Assembly.GetAssembly(typeof(UnityEditor.Editor));
        var type = assembly.GetType("UnityEditor.LogEntries");
        var method = type.GetMethod("Clear");
        method.Invoke(new object(), null);
    }

    void IterateIK()
    {
        int count = JointCount;
        //int iteration = 0;

        while (count != 0) // update된 joint가 없으면(delta 값이 threshold보다 작으면) iteration 종료
        {
            Vector3 error = new Vector3();
            for (int i = 0; i < EEindex.Length; i++)
            {
                error = EESphere[i].transform.position - Joints[EEindex[i]].position;
                ErrorA[i * 3 + 0, 0] = error.x;
                ErrorA[i * 3 + 1, 0] = error.y;
                ErrorA[i * 3 + 2, 0] = error.z;
            }

            JacobianMatrix();
            //Debug.Log(JacobianA);
            InverseA = JacobianA.T * ((JacobianA * JacobianA.T + lamda * Identity).Inverse());
            deltaA = InverseA * ErrorA;
            count = UpdateRot(error.magnitude * 400);
            //iteration++;
        }
        //Debug.Log(iteration);

        for (int j = 0; j < JointCount; j++)
        {
            if (rotateAxis[j] != Vector3.zero)
            {
                Joints[j].rotation = Quaternion.Slerp(prevRot[j], Joints[j].rotation, smoothParam);
                prevRot[j] = Joints[j].rotation;
            }
        }
    }

    int UpdateRot(float s) // update한 joint 수를 반환
    {
        int count = 0;
        float threshold = 0.05f;

        for (int j = 0; j < JointCount; j++)
        {
            if (rotateAxis[j] != Vector3.zero && Mathf.Abs(deltaA[j,0]) > threshold)
            {
                Joints[j].RotateAround(Joints[j].position, rotateAxis[j], deltaA[j, 0] * s); //
                //Debug.LogFormat("Joint[{0}] axis: {1} angle: {2}", j, rotateAxis[j], deltaA[j,0]);
                count++;
            }
        }
        return count;
    }

    void UpdateTargetPos(Transform eePos, Transform nextPos) // end-effector의 다음 target 위치를 update
    {
        if (IsInitialized)
        {
            Sarm = Vector3.Distance(SJoints[(int)Constants.TargetPositionIndex.Cha_UpperArmR].position, SJoints[(int)Constants.TargetPositionIndex.Cha_HandR].position);
            Sleg = Vector3.Distance(SJoints[(int)Constants.TargetPositionIndex.Cha_UpperLegR].position, SJoints[(int)Constants.TargetPositionIndex.Cha_FootR].position);
            arm = Vector3.Distance(Joints[(int)Constants.TargetPositionIndex.Cha_UpperArmR].position, Joints[(int)Constants.TargetPositionIndex.Cha_HandR].position);
            leg = Vector3.Distance(Joints[(int)Constants.TargetPositionIndex.Cha_UpperLegR].position, Joints[(int)Constants.TargetPositionIndex.Cha_FootR].position);
            IsInitialized = false;
        }

        float scale = 0.7f;
        if (nextPos.name.Contains("Hand"))
        {
            scale = arm / Sarm;
        }
        else if (nextPos.name.Contains("Foot"))
        {
            scale = leg / Sleg;
        }
        eePos.position = Root.position + (nextPos.position - SRoot.position) * scale;
    }

    void JacobianMatrix() // tee : end-effector의 target 위치, see : end-effector에 해당하는 joint(손, 발)
    {
        for (int i = 0; i < EEindex.Length; i++)
        {
            Transform tee = EESphere[i].transform;
            Transform see = Joints[EEindex[i]];
            Transform temp = see.transform.parent;
            while (temp.childCount <= 1) // temp가 여러 end-effector가 공유하는 joint가 아니라면 
            {
                // Joints 배열에서 temp의 index 번호(j) 찾기
                for (int j = 0; j < JointCount; j++)
                {
                    if (temp.name == Joints[j].name)
                    {
                        // delta 각을 계산하기 위한 회전축을 tee, see, temp가 존재하는 평면의 법선으로 설정
                        Vector3 norm = Vector3.Cross(tee.position - see.position, see.position - temp.position).normalized;
                        rotateAxis[j] = norm;
                        // JacobianA 행렬의 각 element를 공식에 따라 계산
                        Vector3 elementJ = Vector3.Cross(norm, (see.position - temp.position));
                        JacobianA[i * 3 + 0, j] = elementJ.x;
                        JacobianA[i * 3 + 1, j] = elementJ.y;
                        JacobianA[i * 3 + 2, j] = elementJ.z;

                        temp = temp.transform.parent;
                        break;
                    }
                }
            }
        }        
    }
}
