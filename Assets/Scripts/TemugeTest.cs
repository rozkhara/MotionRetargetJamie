using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TemugeTest : MonoBehaviour
{
    public Dictionary<Constants.SourcePositionIndex, Vector3> frame_pos = new(); //locally defined position
    public Transform parentObjectTransform;     //for object rotation

    private Joint[] joints;     //for parent-child relationship || isnt it possible to just use childnodes? 
    public Joint[] Joints { get { return joints; } }

    public Transform rootNode;
    public Transform[] childNodes;

    private TposeAlignment TPA;
    private bool t_flag = true;

    public class Joint
    {
        public Transform transform = new GameObject().GetComponent<Transform>();

        public List<Joint> child = null;
    }

    private void Awake()
    {
        TPA = GameObject.Find("sourceTpose").GetComponent<TposeAlignment>();
    }
    private void Update()
    {
        if (t_flag)
        {
            if (!TPA.flag)
            {
                PopulateChildren();
                t_flag = false;
            }
        }

        if (DataProcess.Instance.parseFlag)
        {
            int count = 0;
            foreach (Constants.SourcePositionIndex SPI in System.Enum.GetValues(typeof(Constants.SourcePositionIndex)))
            {
                if (count == (int)Constants.SourcePositionIndex.Count)
                {
                    break;
                }
                frame_pos[SPI] = Get_rotated_worldPos(SPI);
                count++;
            }
        }
            
    }

    private Quaternion Get_hips_rot()          //root position is frame_pos[Constants.SourcePositionIndex.bottom_torso]
    {
        Vector3 root_u = (frame_pos[Constants.SourcePositionIndex.left_hip] - frame_pos[Constants.SourcePositionIndex.bottom_torso]).normalized;
        Vector3 root_v = (frame_pos[Constants.SourcePositionIndex.upper_torso] - frame_pos[Constants.SourcePositionIndex.bottom_torso]).normalized;
        Vector3 root_w = Vector3.Cross(root_u, root_v);

        return Quaternion.LookRotation(root_w, root_v);         //because Unity is LH, while input is RH
    }

    private Quaternion Get_parental_joint_rotations(Transform curJoint)
    {
        if (curJoint == rootNode)
        {
            return rootNode.rotation;
        }
        if (curJoint.parent != null)
        {
            return curJoint.rotation * Get_parental_joint_rotations(curJoint.parent);
        }
        throw new System.Exception("Unexpected Input");
    }


    private Matrix4x4 MatMul(Matrix4x4 matA, Matrix4x4 matB)    //not sure if this function returns correct value
    {
        Matrix4x4 matC = new();
        for (int i = 0; i < 4; i++)
        {
            for (int j = 0; j < 4; j++)
            {
                float sum = 0;
                for (int k = 0; k < 4; k++)
                {
                    sum += matA[i, k] * matB[k, j];
                }
                matC[i, j] = sum;
            }
        }
        return matC;
    }

    private Vector3 Get_rotated_worldPos(Constants.SourcePositionIndex index)
    {
        var sData = DataProcess.Instance.GetSData();
        Vector3 returnVal= new Vector3(float.Parse(sData[DataProcess.Instance.chunkIndex + (int)index][2]) / Constants.SCALEFACTOR,
                    float.Parse(sData[DataProcess.Instance.chunkIndex + (int)index][4]) / Constants.SCALEFACTOR,
                    float.Parse(sData[DataProcess.Instance.chunkIndex + (int)index][3]) / Constants.SCALEFACTOR);
        returnVal = parentObjectTransform.rotation * (returnVal - parentObjectTransform.position) + parentObjectTransform.position;
        return returnVal;
    }

    private void Set_Parent()
    {

    }

    private void PopulateChildren()
    {
        childNodes = rootNode.GetComponentsInChildren<Transform>();
    }
}
