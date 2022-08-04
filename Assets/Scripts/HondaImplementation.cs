using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MathNet.Numerics.LinearAlgebra;
using MathNet.Numerics.LinearAlgebra.Double;

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

    private Vector3 AngleToPositionFunction(Matrix<Vector3> Theta)
    {
        Quaternion Q = Quaternion.LookRotation(Theta[1, 0], Vector3.forward);
        double s = Q.w;
        double x = Q.x;
        double y = Q.y;
        double z = Q.z;
        Matrix<double> R = DenseMatrix.OfArray(new double[,] {
            {1 - 2*y*y - 2*z*z, 2*x*y - 2*s*z, 2*x*z + 2*s*y, 0},
            {2*x*y + 2*s*z, 1 - 2*x*x - 2*z*z, 2*y*z - 2*s*x, 0},
            {2*x*z - 2*s*y, 2*y*z + 2*s*x, 1 - 2*x*x - 2*y*y, 0},
            {0, 0, 0, 1}});

        return Vector3.zero;
    }
}
