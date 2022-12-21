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
    Vector3[] rotateAxis;
    int[] EEindex = { (int)Constants.TargetPositionIndex.Cha_HandR, (int)Constants.TargetPositionIndex.Cha_HandL, (int)Constants.TargetPositionIndex.Cha_FootR, (int)Constants.TargetPositionIndex.Cha_FootL };

    //LegacyMatrix SelectMat;
    public int JointCount;


    public GameObject[] EESphere;
    public Transform SRoot;
    public Transform[] SJoints;

    public Transform Root; //targetRot의 Cha_Hips
    public Transform[] Joints;


    // Start is called before the first frame update
    void Start()
    {
        Joints = Root.GetComponentsInChildren<Transform>();
        SJoints = SRoot.GetComponentsInChildren<Transform>();
        JointCount = Joints.Length;
        JacobianA = new LegacyMatrix(3 * EEindex.Length, JointCount);
        InverseA = new LegacyMatrix(JointCount, 3 * EEindex.Length);
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

        for(int i = 0; i < EEindex.Length; i++)
        {
            UpdateTargetPos(EESphere[i].transform, SJoints[EEindex[i]]);
        }

    }

    void ClearConsole()
    {
        var assembly = Assembly.GetAssembly(typeof(UnityEditor.Editor));
        var type = assembly.GetType("UnityEditor.LogEntries");
        var method = type.GetMethod("Clear");
        method.Invoke(new object(), null);
    }

    private void LateUpdate()
    {
        IterateIK();
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
            for (int i = 0; i < EEindex.Length; i++)
            {
                JacobianMatrix(EESphere[i].transform, Joints[EEindex[i]]);
            }
            InverseA = JacobianA.T * ((JacobianA * JacobianA.T).Inverse());
            deltaA = InverseA * ErrorA;
            count = UpdateRot(error.magnitude * 100);
            iteration++;
        }
        Debug.Log(iteration);

    }

    int UpdateRot(float s) // update한 joint 수를 반환
    {
        int count = 0;
        float threshold = 0.01f;

        for (int j = 0; j < JointCount; j++)
        {
            Debug.Log(rotateAxis[j]);
            if (rotateAxis[j] != null && Mathf.Abs(deltaA[j,0]) > threshold)
            {
                Joints[j].RotateAround(Joints[j].position, rotateAxis[j], deltaA[j, 0] * s);
                //Debug.LogFormat("Joint[{0}] axis: {1} angle: {2}", j, rotateAxis[j], deltaA[j,0]);
                count++;
            }
        }

        return count;
    }

    void UpdateTargetPos(Transform Pos, Transform nextPos)
    {
        Pos.position = Root.position + (nextPos.position - SRoot.position) * 0.7f; //0.7은 임의의 값
    }

    void JacobianMatrix(Transform tee, Transform see)
    {
        Transform temp = see.transform.parent;
        while(temp.childCount <= 1)
        {
            for (int j = 0; j < JointCount; j++)
            {
                if (temp.name == Joints[j].name)
                {
                    Vector3 norm = Vector3.Cross(tee.position - see.position, see.position - temp.position).normalized;
                    rotateAxis[j] = norm;
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
