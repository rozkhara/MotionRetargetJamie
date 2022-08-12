using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Reflection;

public class CollisionAvoidance : MonoBehaviour
{
    LegacyMatrix JacobianA;
    public int JointCount;
    public GameObject[] testPoints;
    public Quaternion[] initLocalRot;
    public Quaternion[] initRot;

    public Transform Root; //targetRot의 Cha_Hips
    public Transform[] Joints;

    // Start is called before the first frame update
    void Start()
    {
        Joints = Root.GetComponentsInChildren<Transform>();
        JointCount = Joints.Length;
        JacobianA = new LegacyMatrix(3, JointCount * 3);
        initLocalRot = new Quaternion[JointCount];
        initRot = new Quaternion[JointCount];
        for (int i=0; i<JointCount; i++)
        {
            initLocalRot[i] = Joints[i].localRotation;
            initRot[i] = Joints[i].rotation;
        }
        //JacobianMatrix(test, Joints[11]);
        //Debug.Log(JacobianA.ToString());

        int count = 4;
        testPoints = new GameObject[count];
        for(int i = 0; i < count; i++)
        {
            GameObject sphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            sphere.transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);
            testPoints[i] = sphere;
        }
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
        var assembly = Assembly.GetAssembly(typeof(UnityEditor.Editor));
        var type = assembly.GetType("UnityEditor.LogEntries");
        var method = type.GetMethod("Clear");
        method.Invoke(new object(), null);

        
        calcTpos(testPoints[0].transform, Joints[11], Joints[12]);
        calcTpos(testPoints[1].transform, Joints[7], Joints[8]);
        calcTpos(testPoints[2].transform, Joints[25], Joints[26]);
        calcTpos(testPoints[3].transform, Joints[21], Joints[22]);
    }

    void calcTpos(Transform pa, Transform pajoint, Transform joint)
    {
        Transform temp = pajoint;
        Quaternion deltaLocalRot;
        int index;
        pa.position = Vector3.Lerp(pajoint.position, joint.position, 0.5f);

        while (temp != Root.parent)
        {
            for (int i = 0; i < JointCount; i++)
            {
                if (Joints[i] == temp)
                {
                    index = i;
                    deltaLocalRot = temp.localRotation * Quaternion.Inverse(initLocalRot[index]);

                    pa.position = Quaternion.Inverse(temp.parent.rotation) * (pa.position - temp.position) + temp.position; 
                    pa.position = Quaternion.Inverse(deltaLocalRot) * (pa.position - temp.position) + temp.position; 
                    pa.position = temp.parent.rotation * (pa.position - temp.position) + temp.position; 

                    temp = temp.parent;
                    break;
                }
            }

        }

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
