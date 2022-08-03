using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OMR : MonoBehaviour
{
    LegacyMatrix Jacobian;
    private int EndEffectorCount = 5;

    public Transform Root;
    public Transform[] Joints;
    public Transform[] EndEffector;

    public Vector3[] PreEndEffector; // 이전 프레임의 EndEffector 좌표
    //public Vector3[] dx1;
    public LegacyMatrix dx1; // EndEffector 좌표의 변화량
    public Vector3[] ee1; // error1의 예측값
    public Vector3[] dsrc; // source Joints 회전의 변화량
    //public Vector3[] dedes;
    public LegacyMatrix dedes; // destination Joints 회전의 변화량의 예측값

    private void Start()
    {
        Joints = Root.GetComponentsInChildren<Transform>();

        EndEffector = new Transform[EndEffectorCount];
        EndEffector[0] = Joints[3];
        EndEffector[1] = Joints[6];
        EndEffector[2] = Joints[10];
        EndEffector[3] = Joints[13];
        EndEffector[4] = Joints[16];

        Jacobian = new LegacyMatrix(EndEffectorCount * 3, Constants.JOINTCOUNT * 3);

        PreEndEffector = new Vector3[EndEffectorCount];
        //dx1 = new Vector3[EndEffectorCount];
        dx1 = new LegacyMatrix(EndEffectorCount * 3, 1);
        ee1 = new Vector3[EndEffectorCount];

        dsrc = new Vector3[Constants.JOINTCOUNT];
        //dedes = new Vector3[Constants.JOINTCOUNT];
        dedes = new LegacyMatrix(Constants.JOINTCOUNT * 3, 1);
    }

    private void Update()
    {        
        for(int i = 0; i < EndEffectorCount; i++)
        {
            dx1[3*i, 0] = EndEffector[i].position.x - PreEndEffector[i].x;
            dx1[3*i+1, 0] = EndEffector[i].position.y - PreEndEffector[i].y;
            dx1[3*i+2, 0] = EndEffector[i].position.z - PreEndEffector[i].z;
            PreEndEffector[i] = EndEffector[i].position;
        }

        dedes = JacobianPseudoInverse(Jacobian) * dx1;

        for (int i = 0; i < Constants.JOINTCOUNT; i++)
        {

        }

        JacobianMatrix();
        Debug.Log(Jacobian.ToString());
    }

    public void JacobianMatrix()
    {   
        for (int i = 0; i < EndEffectorCount; i++)
        {
            for (int j = 0; j < Constants.JOINTCOUNT; j++)
            {
                Vector3 a = Vector3.Cross(Joints[j].right, (EndEffector[i].position - Joints[j].position));
                Vector3 b = Vector3.Cross(Joints[j].up, (EndEffector[i].position - Joints[j].position));
                Vector3 c = Vector3.Cross(Joints[j].forward, (EndEffector[i].position - Joints[j].position));

                Jacobian[i * 3, j * 3] = a.x;
                Jacobian[i * 3 + 1, j * 3] = a.y;
                Jacobian[i * 3 + 2, j * 3] = a.z;
                Jacobian[i * 3, j * 3 + 1] = b.x;
                Jacobian[i * 3 + 1, j * 3 + 1] = b.y;
                Jacobian[i * 3 + 2, j * 3 + 1] = b.z;
                Jacobian[i * 3, j * 3 + 2] = c.x;
                Jacobian[i * 3 + 1, j * 3 + 2] = c.y;
                Jacobian[i * 3 + 2, j * 3 + 2] = c.z;
            }
        }
    }

    public LegacyMatrix JacobianPseudoInverse(LegacyMatrix input)
    {
        LegacyMatrix Temp = Jacobian * Jacobian.T;
        LegacyMatrix JPI = Jacobian.T * Temp.Inverse();

        return JPI;
    }


}
