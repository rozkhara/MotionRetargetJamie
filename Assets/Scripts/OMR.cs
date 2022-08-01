using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OMR : MonoBehaviour
{
    Matrix Jacobian;
    private int EndEffectorCount = 5;

    private void Update()
    {
        Jacobian = new Matrix(Constants.JOINTCOUNT, EndEffectorCount * 3);
        
    }

}
