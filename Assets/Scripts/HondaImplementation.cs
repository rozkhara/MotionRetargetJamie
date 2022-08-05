using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MathNet.Numerics.LinearAlgebra;
using MathNet.Numerics.LinearAlgebra.Single;

public class HondaImplementation : MonoBehaviour
{
    public readonly int EECount = 5;

    Matrix<Vector3> DisplacementTable;

    TposeAlignment tpa;

    private void Awake()
    {
        tpa = GameObject.Find("sourceTpose").GetComponent<TposeAlignment>();
    }

    private void Update()
    {
        if (!tpa.flag)
        {
            CreateDisplacementTable();
        }
    }

    private void CreateDisplacementTable()      // need to replace childnodes data to a saved first frame specific data
    {
        foreach (var child in tpa.childNodes)
        {
            DisplacementTable.Add(child.transform.localPosition);
        }
    }

    private Vector3 AngleToPositionFunction(Matrix<Quaternion> Theta)
    {
        for (int i = 0; i < Theta.RowCount; i++)
        {
            for (int j = 0; j < Theta.ColumnCount; j++)
            {
                float s = Theta[i, j].w;
                float x = Theta[i, j].x;
                float y = Theta[i, j].y;
                float z = Theta[i, j].z;
                Matrix<float> R = Matrix.Build.Dense(4, 4);
                R[0, 0] = 1 - 2 * y * y - 2 * z * z;
                R[0, 1] = 2 * x * y - 2 * s * z;
                R[0, 2] = 2 * x * z + 2 * s * y;

                R[1, 0] = 2 * x * y + 2 * s * z;
                R[1, 1] = 1 - 2 * x * x - 2 * z * z;
                R[1, 2] = 2 * y * z - 2 * s * x;

                R[2, 0] = 2 * x * z - 2 * s * y;
                R[2, 1] = 2 * y * z + 2 * s * x;
                R[2, 2] = 1 - 2 * x * x - 2 * y * y;

                R[3, 3] = 1;
            }
        }

        return Vector3.zero;
    }
}
