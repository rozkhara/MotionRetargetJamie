using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Reflection;

public class CollisionAvoidance : MonoBehaviour
{
    LegacyMatrix JacobianA;
    public int JointCount;
    public Transform test;
    public Quaternion[] initRotation;

    public Transform Root; //targetRot의 Cha_Hips
    public Transform[] Joints;

    // Start is called before the first frame update
    void Start()
    {
        Joints = Root.GetComponentsInChildren<Transform>();
        JointCount = Joints.Length;
        JacobianA = new LegacyMatrix(3, JointCount * 3);
        initRotation = new Quaternion[JointCount];
        for(int i=0; i<JointCount; i++)
        {
            initRotation[i] = Joints[i].localRotation;
        }

        test.position = Vector3.Lerp(Joints[26].position, Joints[25].position, 0.5f);
        //JacobianMatrix(test, Joints[11]);
        //Debug.Log(JacobianA.ToString());

    }

    // Update is called once per frame
    void Update()
    {
        //test.position = Joints[11].position;
        /*
        
        Debug.Log(Joints[i].rotation.eulerAngles.ToString());
        Debug.Log(Joints[i].localRotation.eulerAngles.ToString());
        Debug.Log(Quaternion.Inverse(Joints[i].localRotation).eulerAngles.ToString());
        Debug.Log((Quaternion.Inverse(Joints[i].rotation) * Joints[i].parent.rotation).eulerAngles.ToString());
        Debug.Log((Joints[i].parent.rotation * Quaternion.Inverse(Joints[i].rotation)).eulerAngles.ToString());
         
         */
        int i = 25;
        var assembly = Assembly.GetAssembly(typeof(UnityEditor.Editor));
        var type = assembly.GetType("UnityEditor.LogEntries");
        var method = type.GetMethod("Clear");
        method.Invoke(new object(), null);

        
        calcTpos(test, Joints[i]);
    }

    void calcTpos(Transform pa, Transform pajoint)
    {
        Transform temp = pajoint;
        Quaternion r1, deltaLocalRot, rot;
        int index;
        pa.position = Vector3.Lerp(Joints[26].position, Joints[25].position, 0.5f);
        while (temp != Root.parent)
        {
            for(int i=0; i<JointCount; i++)
            {
                if(Joints[i] == temp)
                {
                    

                    index = i;
                    r1 = temp.rotation;
                    deltaLocalRot = temp.localRotation * Quaternion.Inverse(initRotation[index]); //Tpose 기준으로 회전한 localRotation 회전값 -> 이만큼을 localRotation 기준으로 역회전 시켜주어야 함
                    rot = Quaternion.Inverse(r1) * Quaternion.Inverse(deltaLocalRot) * r1;
                    pa.position = rot * (pa.position - temp.position) + temp.position;

                    Debug.Log(temp.name);
                    Debug.LogFormat("initRotation : {0}", (initRotation[i]).eulerAngles.ToString());
                    Debug.LogFormat("deltaLocalRot : {0}", (deltaLocalRot).eulerAngles.ToString());
                    Debug.LogFormat("in globalRot : {0}", (rot).eulerAngles.ToString());

                    temp = temp.parent;
                    break;
                }
            }
        }

        //Debug.Log(temp.name);
        /*
         
        Debug.Log(temp.name);
        rot = temp.parent.rotation * Quaternion.Inverse(temp.localRotation) * Quaternion.Inverse(temp.parent.rotation);
        pa.position = rot * (pa.position - temp.position) + temp.position;
        temp = temp.parent;
       
        Debug.Log(temp.name);
        rot = Quaternion.Inverse(temp.rotation * Quaternion.Inverse(temp.parent.rotation));
        pa.position = rot * (pa.position - temp.position) + temp.position;
        temp = temp.parent;
        Debug.Log(temp.name);
        rot = Quaternion.Inverse(temp.rotation); // * Quaternion.AngleAxis(90, Vector3.forward)
        pa.position = rot * (pa.position - temp.position) + temp.position;
         */

    }

    void JacobianMatrix(Transform pa, Transform pajoint) //pajoint는 pa가 속해있는 joint
    {
        Transform temp;
        for (int j = JointCount - 1; j >= 0; j--)
        {
            //만약 해당 end effector가 해당 joint의 자손일 경우 Jacobian값 채우기
            temp = pajoint;
            do
            {
                temp = temp.transform.parent;
                if (temp.name == Joints[j].name)
                {
                    Vector3 a = Vector3.Cross(Joints[j].right, (pa.position - Joints[j].position));
                    Vector3 b = Vector3.Cross(Joints[j].up, (pa.position - Joints[j].position));
                    Vector3 c = Vector3.Cross(Joints[j].forward, (pa.position - Joints[j].position));

                    JacobianA[0, j * 3] = a.x;
                    JacobianA[1, j * 3] = a.y;
                    JacobianA[2, j * 3] = a.z;
                    JacobianA[0, j * 3 + 1] = b.x;
                    JacobianA[1, j * 3 + 1] = b.y;
                    JacobianA[2, j * 3 + 1] = b.z;
                    JacobianA[0, j * 3 + 2] = c.x;
                    JacobianA[1, j * 3 + 2] = c.y;
                    JacobianA[2, j * 3 + 2] = c.z;

                }
            } while (temp != Root);
        }
    }
}
