using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Linq;
using System.Text;


public class FirstPosition : MonoBehaviour
{
    private const int JOINTCOUNT = 17;

    public Transform rootNode;
    public Transform[] childNodes;
    public string dataChunk;
    public string[][] sData = new string[JOINTCOUNT][];
    public List<Vector3> newTransform;
    private int index = 0;
    private int frameNum = 0;
    private int chunkIndex = 0;
   

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

    private void FixedUpdate()
    {
        if (true)
        {
            index = DataProcess.Instance.GetIndex();
            if (index < 1963)
            {
                if (index % 30 == 0)
                {
                    DataProcess.Instance.UpdateDataChunk();
                }
                DataProcess.Instance.UpdatePerJointData();
                sData = DataProcess.Instance.GetSData();
                chunkIndex = DataProcess.Instance.GetChunkIndex();
                frameNum = DataProcess.Instance.GetFrameNum();
                for (int i = 0; i < JOINTCOUNT; i++)
                {
                    if (newTransform.Count <= i)
                    {
                        newTransform.Add(new Vector3(float.Parse(sData[chunkIndex + i][2]) / 1000, float.Parse(sData[chunkIndex + i][4]) / 1000, float.Parse(sData[chunkIndex + i][3]) / 1000));
                        if (i == 0)
                        {
                            newTransform[0] += childNodes[0].position;
                        }
                        else
                        {
                            newTransform[i] = newTransform[0] + newTransform[i];
                        }
                    }
                    else
                    {
                        newTransform[i] = new Vector3(float.Parse(sData[chunkIndex + i][2]) / 1000, float.Parse(sData[chunkIndex + i][4]) / 1000, float.Parse(sData[chunkIndex + i][3]) / 1000);
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
                        child.position = newTransform[14];
                    }
                    else if (child.name.Contains("UpperArmR"))
                    {
                        child.position = newTransform[11];
                    }
                    else if (child.name.Contains("UpperLegL"))
                    {
                        child.position = newTransform[1];
                    }
                    else if (child.name.Contains("UpperLegR"))
                    {
                        child.position = newTransform[4];
                    }
                    else if (child.name.Contains("LowerArmL"))
                    {
                        child.position = newTransform[15];
                    }
                    else if (child.name.Contains("LowerArmR"))
                    {
                        child.position = newTransform[12];
                    }
                    else if (child.name.Contains("LowerLegL"))
                    {
                        child.position = newTransform[2];
                    }
                    else if (child.name.Contains("LowerLegR"))
                    {
                        child.position = newTransform[5];
                    }
                    else if (child.name.Contains("FootL"))
                    {
                        child.position = newTransform[3];
                    }
                    else if (child.name.Contains("FootR"))
                    {
                        child.position = newTransform[6];
                    }
                    else if (child.name.Contains("HandL"))
                    {
                        child.position = newTransform[16];
                    }
                    else if (child.name.Contains("HandR"))
                    {
                        child.position = newTransform[13];
                    }
                    else if (child.name.Contains("Face"))
                    {
                        child.position = newTransform[10];
                    }
                }
                DataProcess.Instance.SetIndex(index + 1);
            }
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
