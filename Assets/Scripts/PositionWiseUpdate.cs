using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Linq;
using System.Text;


public class PositionWiseUpdate : MonoBehaviour
{
    private static readonly int _JOINTCOUNT = Constants.JOINTCOUNT;
    private static readonly int _TARGETFRAME = Constants.TARGETFRAME;
    private static readonly int _SCALEFACTOR = Constants.SCALEFACTOR;

    public Transform rootNode;
    public Transform[] childNodes;
    public string[][] sData = new string[_JOINTCOUNT * _TARGETFRAME][];
    public List<Vector3> newTransform;
    public List<Vector3> firstTransform;

    //private bool powerSwitch = true;


    private void Awake()
    {
        if (rootNode != null)
        {
            if (childNodes == null || childNodes.Length == 0)
            {
                //get all joints to draw
                PopulateChildren();
            }
        }
    }

    private void Update()
    {
        if (DataProcess.Instance.parseFlag)
        {
            sData = DataProcess.Instance.GetSData();
            for (int i = 0; i < _JOINTCOUNT; i++)
            {
                if (newTransform.Count <= i)
                {
                    newTransform.Add(new Vector3(float.Parse(sData[DataProcess.Instance.chunkIndex + i][2]) / _SCALEFACTOR,
                        float.Parse(sData[DataProcess.Instance.chunkIndex + i][4]) / _SCALEFACTOR,
                        float.Parse(sData[DataProcess.Instance.chunkIndex + i][3]) / _SCALEFACTOR));
                    if (i == 0)
                    {
                        newTransform[0] += childNodes[0].position;
                    }
                    else
                    {
                        newTransform[i] = newTransform[0] + newTransform[i];
                    }
                    firstTransform.Add(newTransform[i]);
                }
                else
                {
                    newTransform[i] = new Vector3(float.Parse(sData[DataProcess.Instance.chunkIndex + i][2]) / _SCALEFACTOR,
                        float.Parse(sData[DataProcess.Instance.chunkIndex + i][4]) / _SCALEFACTOR,
                        float.Parse(sData[DataProcess.Instance.chunkIndex + i][3]) / _SCALEFACTOR);
                    if (i == 0)
                    {
                        newTransform[i] += childNodes[0].position;
                    }
                    else
                    {
                        newTransform[i] = newTransform[0] + newTransform[i];
                    }
                }
            }

            childNodes[(int)Constants.TargetPositionIndex.Cha_Hips].position = newTransform[0];
            childNodes[(int)Constants.TargetPositionIndex.Cha_Spine].position = newTransform[7];
            childNodes[(int)Constants.TargetPositionIndex.Cha_Chest].position = newTransform[8];

            childNodes[(int)Constants.TargetPositionIndex.Cha_UpperArmL].position = newTransform[11];
            childNodes[(int)Constants.TargetPositionIndex.Cha_LowerArmL].position = newTransform[12];
            childNodes[(int)Constants.TargetPositionIndex.Cha_HandL].position = newTransform[13];

            childNodes[(int)Constants.TargetPositionIndex.Cha_UpperArmR].position = newTransform[14];
            childNodes[(int)Constants.TargetPositionIndex.Cha_LowerArmR].position = newTransform[15];
            childNodes[(int)Constants.TargetPositionIndex.Cha_HandR].position = newTransform[16];

            childNodes[(int)Constants.TargetPositionIndex.Face].position = newTransform[10];

            childNodes[(int)Constants.TargetPositionIndex.Cha_UpperLegL].position = newTransform[4];
            childNodes[(int)Constants.TargetPositionIndex.Cha_LowerLegL].position = newTransform[5];
            childNodes[(int)Constants.TargetPositionIndex.Cha_FootL].position = newTransform[6];

            childNodes[(int)Constants.TargetPositionIndex.Cha_UpperLegR].position = newTransform[1];
            childNodes[(int)Constants.TargetPositionIndex.Cha_LowerLegR].position = newTransform[2];
            childNodes[(int)Constants.TargetPositionIndex.Cha_FootR].position = newTransform[3];

            DataProcess.Instance.index++;
        }
    }

    //string ReadTxt(string filePath)
    //{
    //    FileInfo fileInfo = new FileInfo(filePath);
    //    string value = "";

    //    if (fileInfo.Exists)
    //    {
    //        StreamReader reader = new StreamReader(filePath);
    //        value = reader.ReadToEnd();
    //        reader.Close();
    //    }

    //    else
    //        value = "파일이 없습니다.";

    //    return value;
    //}


    public void PopulateChildren()
    {
        childNodes = rootNode.GetComponentsInChildren<Transform>();
    }

}
