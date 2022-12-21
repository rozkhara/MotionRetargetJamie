using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Reflection;

public class CreateEETarget : MonoBehaviour
{    
    LegacyMatrix JacobianA;
    LegacyMatrix InverseA;
    LegacyMatrix ErrorA;
    LegacyMatrix deltaA;
    LegacyMatrix Identity;
    float lamda = Mathf.Pow(1.4f * 0.05f, 2f);
    Vector3[] rotateAxis;
    int[] EEindex = { (int)Constants.TargetPositionIndex.Cha_HandR, (int)Constants.TargetPositionIndex.Cha_HandL}; // , (int)Constants.TargetPositionIndex.Cha_FootR, (int)Constants.TargetPositionIndex.Cha_FootL

    //LegacyMatrix SelectMat;
    public int JointCount;

    public GameObject[] EESphere;
    public Transform SRoot;
    public Transform[] SJoints;

    public Transform Root; //targetRot�� Cha_Hips
    public Transform[] Joints;

    // Start is called before the first frame update
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

        EESphere = new GameObject[EEindex.Length];
        for (int i = 0; i < EEindex.Length; i++)
        {
            GameObject sphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            sphere.transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);
            EESphere[i] = sphere;
        }
    }

    // Update is called once per frame
    void Update()
    {
        ClearConsole();

        for (int i = 0; i < EEindex.Length; i++)
        {
            UpdateTargetPos(EESphere[i].transform, SJoints[EEindex[i]]);
        }

    }
    private void LateUpdate()
    {
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
        int iteration = 0;

        while (count != 0)
        {
            Vector3 error = new Vector3();

            for (int i = 0; i < EEindex.Length; i++)
            {
                error = EESphere[i].transform.position - Joints[EEindex[i]].position;
                ErrorA[i * 3 + 0, 0] = error.x;
                ErrorA[i * 3 + 1, 0] = error.y;
                ErrorA[i * 3 + 2, 0] = error.z;
            }
            Debug.Log(ErrorA);
            for (int i = 0; i < EEindex.Length; i++)
            {
                JacobianMatrix(EESphere[i].transform, Joints[EEindex[i]]);
            }
            Debug.Log(JacobianA);
            InverseA = JacobianA.T * ((JacobianA * JacobianA.T + lamda * Identity).Inverse());
            deltaA = InverseA * ErrorA;
            count = UpdateRot(error.magnitude * 100);
            iteration++;
        }
        Debug.Log(iteration);

    }

    int UpdateRot(float s) // update�� joint ���� ��ȯ
    {
        int count = 0;
        float threshold = 0.1f;

        for (int j = 0; j < JointCount; j++)
        {
            if (rotateAxis[j] != null && Mathf.Abs(deltaA[j,0]) > threshold)
            {
                Joints[j].RotateAround(Joints[j].position, rotateAxis[j], deltaA[j, 0]*s);
                //Debug.LogFormat("Joint[{0}] axis: {1} angle: {2}", j, rotateAxis[j], deltaA[j,0]);
                count++;
            }
        }

        return count;
    }

    void UpdateTargetPos(Transform Pos, Transform nextPos)
    {
        Pos.position = Root.position + (nextPos.position - SRoot.position) * 0.8f; // ����� ������ ��
    }

    void JacobianMatrix(Transform tee, Transform see) // tee : end-effector�� target ��ġ, see : end-effector�� �ش��ϴ� joint(��, ��)
    {
        Transform temp = see.transform.parent;
        while(temp.childCount <= 1) // temp�� ���� end-effector�� �����ϴ� joint�� �ƴ϶��
        {   
            // Joints �迭���� temp�� index ��ȣ(j) ã��
            for (int j = 0; j < JointCount; j++)
            {
                if (temp.name == Joints[j].name)
                {
                    // delta ���� ����ϱ� ���� ȸ������ tee, see, temp�� �����ϴ� ����� �������� ����
                    Vector3 norm = Vector3.Cross(tee.position - see.position, see.position - temp.position).normalized;
                    rotateAxis[j] = norm;
                    // JacobianA ����� �� element�� ���Ŀ� ���� ���
                    Vector3 elementJ = Vector3.Cross(norm, (see.position - temp.position));
                    JacobianA[0, j] = elementJ.x;
                    JacobianA[1, j] = elementJ.y;
                    JacobianA[2, j] = elementJ.z;

                    temp = temp.transform.parent;
                    break;
                }
            }
        }
    }
}
