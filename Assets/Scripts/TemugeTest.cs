using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TemugeTest : MonoBehaviour
{
    public Dictionary<Constants.SourcePositionIndex, Vector3> frame_pos { get; set; }
    public Transform parentObjectTransform;

    private Joint[] joints;
    public Joint[] Joints { get { return joints; } }

    public class Joint
    {
        public Transform transform = new GameObject().GetComponent<Transform>();

        public List<Joint> child = null;
    }

    private void Update()
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

    private Matrix4x4 Get_R(Vector3 A, Vector3 B)
    {
        Vector3 uA = A.normalized;
        Vector3 uB = B.normalized;

        float cos_t = Vector3.Dot(uA, uB);
        float sin_t = Vector3.Cross(uA, uB).sqrMagnitude;

        Vector3 u = uA;
        Vector3 v = (uB - cos_t * uA).normalized;
        Vector3 w = Vector3.Cross(uA, uB).normalized;

        Matrix4x4 C = new Matrix4x4(new Vector4(u.x, u.y, u.z),
                                new Vector4(v.x, v.y, v.z),
                                new Vector4(w.x, w.y, w.z),
                                new Vector4(0.0f, 0.0f, 0.0f, 1.0f));

        Matrix4x4 R_uvw = new Matrix4x4(new Vector4(cos_t, -sin_t, 0.0f),
                                    new Vector4(sin_t, cos_t, 0.0f),
                                    new Vector4(0.0f, 0.0f, 1.0f),
                                    new Vector4(0.0f, 0.0f, 0.0f, 1.0f));

        Matrix4x4 R = MatMul(MatMul(C.transpose, R_uvw), C);
        return R;
    }

    private void Decompose_R_ZXY(Matrix4x4 R, out float thetaZ, out float thetaY, out float thetaX)
    {
        thetaZ = Mathf.Atan2(-R[0, 1], R[1, 1]);
        thetaY = Mathf.Atan2(-R[2, 0], R[2, 2]);
        thetaX = Mathf.Atan2(R[2, 1], Mathf.Sqrt(R[2, 0] * R[2, 0] + R[2, 2] * R[2, 2]));
    }

    private void Get_hips_pos_rot()
    {
        // insert code here
        Vector3 root_u = (frame_pos[Constants.SourcePositionIndex.left_hip] - frame_pos[Constants.SourcePositionIndex.bottom_torso]).normalized;
        Vector3 root_v = (frame_pos[Constants.SourcePositionIndex.upper_torso] - frame_pos[Constants.SourcePositionIndex.bottom_torso]).normalized;
        Vector3 root_w = Vector3.Cross(root_u, root_v);

        Matrix4x4 C = new Matrix4x4(new Vector4(root_u.x,root_u.y,root_u.z),
                                    new Vector4(root_v.x,root_v.y,root_v.z),
                                    new Vector4(root_w.x,root_w.y,root_w.z),
                                    new Vector4(0.0f, 0.0f, 0.0f, 1.0f)).transpose;
        float thetaZ;
        float thetaY;
        float thetaX;
        Decompose_R_ZXY(C, out thetaZ, out thetaY, out thetaX);
        Matrix4x4 root_rotation = 
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

}
