using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TemugeTest : MonoBehaviour
{
    private Matrix4x4 Get_R(Vector3 A, Vector3 B)
    {
        var uA = A.normalized;
        var uB = B.normalized;

        var cos_t = Vector3.Dot(uA, uB);
        var sin_t = Vector3.Cross(uA, uB).sqrMagnitude;

        var u = uA;
        var v = (uB - cos_t * uA).normalized;
        var w = Vector3.Cross(uA, uB).normalized;

        var C = new Matrix4x4(new Vector4(u.x, u.y, u.z),
                                new Vector4(v.x, v.y, v.z),
                                new Vector4(w.x, w.y, w.z),
                                new Vector4(0.0f, 0.0f, 0.0f, 1.0f));

        var R_uvw = new Matrix4x4(new Vector4(cos_t, -sin_t, 0.0f),
                                    new Vector4(sin_t, cos_t, 0.0f),
                                    new Vector4(0.0f, 0.0f, 1.0f),
                                    new Vector4(0.0f, 0.0f, 0.0f, 1.0f));

        var R = MatMul(MatMul(C.transpose, R_uvw), C);
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

}
