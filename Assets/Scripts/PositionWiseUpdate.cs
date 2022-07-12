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

    private bool powerSwitch = true;


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
        Application.targetFrameRate = -1;
    }

    private void Update()
    {
        if (powerSwitch)
        {
            if (DataProcess.Instance.index == DataProcess.Instance.refreshIndex)
            {
                try
                {
                    DataProcess.Instance.UpdateDataChunk();

                }
                catch (System.Exception e)
                {
                    Debug.Log(e.Message);
                    powerSwitch = false;
                    return;
                }
            }
            try
            {
                DataProcess.Instance.CheckFrameIndex();
            }
            catch (System.Exception e)
            {
                Debug.Log(e.Message);
                powerSwitch = false;
                return;
            }
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
            foreach (Transform child in childNodes)
            {
                if (child.name.Contains("Hips"))
                {
                    child.position = newTransform[0];
                }
                else if (child.name.Contains("Spine"))
                {
                    child.position = newTransform[7];
                }
                else if (child.name.Contains("Chest"))
                {
                    child.position = newTransform[8];
                }
                else if (child.name.Contains("UpperArmL"))
                {
                    child.position = newTransform[11];
                }
                else if (child.name.Contains("UpperArmR"))
                {
                    child.position = newTransform[14];
                }
                else if (child.name.Contains("UpperLegL"))
                {
                    child.position = newTransform[4];
                }
                else if (child.name.Contains("UpperLegR"))
                {
                    child.position = newTransform[1];
                }
                else if (child.name.Contains("LowerArmL"))
                {
                    child.position = newTransform[12];
                }
                else if (child.name.Contains("LowerArmR"))
                {
                    child.position = newTransform[15];
                }
                else if (child.name.Contains("LowerLegL"))
                {
                    child.position = newTransform[5];
                }
                else if (child.name.Contains("LowerLegR"))
                {
                    child.position = newTransform[2];
                }
                else if (child.name.Contains("FootL"))
                {
                    child.position = newTransform[6];
                }
                else if (child.name.Contains("FootR"))
                {
                    child.position = newTransform[3];
                }
                else if (child.name.Contains("HandL"))
                {
                    child.position = newTransform[13];
                }
                else if (child.name.Contains("HandR"))
                {
                    child.position = newTransform[16];
                }
                else if (child.name.Contains("Face"))
                {
                    child.position = newTransform[10];
                }
            }
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
