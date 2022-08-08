using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OMR : MonoBehaviour
{
    LegacyMatrix JacobianP;
    //LegacyMatrix JacobianO;
    private int EndEffectorCount = 5;
    private int JointCount = 28;
    public int Kfactor = 150;

    public Transform Root; //targetRot의 Cha_Hips
    public Transform[] Joints;
    public Transform[] EndEffector;

    public Vector3[] PreEndEffector; // 이전 프레임의 EndEffector 좌표
    //public Vector3[] dx1;
    public LegacyMatrix deltaP; // EndEffector 좌표의 변화량
    public Vector3[] errorP; // error1의 예측값
    public Vector3[] dsrc; // source Joints 회전의 변화량
    //public Vector3[] dedes;
    public LegacyMatrix dedes; // destination Joints 회전의 변화량의 예측값

    private void Start()
    {
        Joints = Root.GetComponentsInChildren<Transform>();

        EndEffector = new Transform[EndEffectorCount];
        EndEffector[0] = Joints[8];
        EndEffector[1] = Joints[12];
        EndEffector[2] = Joints[16];
        EndEffector[3] = Joints[22];
        EndEffector[4] = Joints[26];

        JacobianP = new LegacyMatrix(EndEffectorCount * 3, JointCount * 3);
        //JacobianO = new LegacyMatrix(EndEffectorCount * 3, JointCount * 3);

        PreEndEffector = new Vector3[EndEffectorCount];
        //dx1 = new Vector3[EndEffectorCount];
        deltaP = new LegacyMatrix(EndEffectorCount * 3, 1);
        errorP = new Vector3[EndEffectorCount];

        dsrc = new Vector3[JointCount];
        //dedes = new Vector3[JointCount];
        dedes = new LegacyMatrix(JointCount * 3, 1);
    }

    private void Update()
    {        
        for(int i = 0; i < EndEffectorCount; i++)
        {
            deltaP[3*i, 0] = EndEffector[i].position.x - PreEndEffector[i].x;
            deltaP[3*i+1, 0] = EndEffector[i].position.y - PreEndEffector[i].y;
            deltaP[3*i+2, 0] = EndEffector[i].position.z - PreEndEffector[i].z;
            PreEndEffector[i] = EndEffector[i].position;
        }
        


        for (int i = 0; i < JointCount; i++)
        {

        }

        JacobianMatrix();
        Debug.Log(JacobianP.ToString());
        //JacobianPseudoInverse(JacobianP);
        Debug.Log(JacobianPseudoInverse(JacobianP).ToString());
    }

    public void JacobianMatrix()
    {
        Transform temp;
        for (int i = 0; i < EndEffectorCount; i++)
        {
            for (int j = JointCount - 1; j >= 0; j--)
            {
                //만약 해당 end effector가 해당 joint의 자손일 경우 Jacobian값 채우기
                temp = EndEffector[i];
                do
                {
                    temp = temp.transform.parent;
                    if(temp.name == Joints[j].name)
                    {
                        Vector3 a = Vector3.Cross(Joints[j].right, (EndEffector[i].position - Joints[j].position));
                        Vector3 b = Vector3.Cross(Joints[j].up, (EndEffector[i].position - Joints[j].position));
                        Vector3 c = Vector3.Cross(Joints[j].forward, (EndEffector[i].position - Joints[j].position));

                        JacobianP[i * 3, j * 3] = a.x;
                        JacobianP[i * 3 + 1, j * 3] = a.y;
                        JacobianP[i * 3 + 2, j * 3] = a.z;
                        JacobianP[i * 3, j * 3 + 1] = b.x;
                        JacobianP[i * 3 + 1, j * 3 + 1] = b.y;
                        JacobianP[i * 3 + 2, j * 3 + 1] = b.z;
                        JacobianP[i * 3, j * 3 + 2] = c.x;
                        JacobianP[i * 3 + 1, j * 3 + 2] = c.y;
                        JacobianP[i * 3 + 2, j * 3 + 2] = c.z;

                    }
                } while (temp != Root);                
            }
        }
    }

    public LegacyMatrix JacobianPseudoInverse(LegacyMatrix input)
    {
        LegacyMatrix Temp = JacobianP * JacobianP.T;
        LegacyMatrix JPI = JacobianP.T * Temp.Inverse();

        return JPI;
    }


}
