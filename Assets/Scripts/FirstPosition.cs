using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class FirstPosition : MonoBehaviour
{
    public Transform rootNode;
    public Transform[] childNodes;
    public TextAsset textA;
    public string data;
    public string[] lineData;
    public string[][] sData = new string[17][];
    public List<Vector3> newTransform;
    private static int index = 0;

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
        data = textA.text.ToString();
        lineData = data.Split("\n");
        for (int i = 0; i < 17; i++)
        {
            lineData[i] = lineData[i].Replace("(", "");
            lineData[i] = lineData[i].Replace(")", "");
            sData[i] = lineData[i].Split(" ");
        }
        newTransform.Add(new Vector3(float.Parse(sData[0][2]) / 1000, float.Parse(sData[0][4]) / 1000, float.Parse(sData[0][3]) / 1000));
        newTransform[0] += childNodes[0].position;
        for (int i = 1; i < 17; i++)
        {
            newTransform.Add(new Vector3(float.Parse(sData[i][2]) / 1000, float.Parse(sData[i][4]) / 1000, float.Parse(sData[i][3]) / 1000));
            newTransform[i] = newTransform[0] + newTransform[i];
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
    }

    private void FixedUpdate()
    {
        if (true)
        {
            if (index < 1963)
            {
                for (int i = 0; i < 17; i++)
                {
                    lineData[index * 17 + i] = lineData[index * 17 + i].Replace("(", "");
                    lineData[index * 17 + i] = lineData[index * 17 + i].Replace(")", "");
                    sData[i] = lineData[index * 17 + i].Split(" ");
                }

                for (int i = 0; i < 17; i++)
                {
                    if (newTransform[i] == null)
                    {
                        newTransform.Add(new Vector3(float.Parse(sData[i][2]) / 1000, float.Parse(sData[i][4]) / 1000, float.Parse(sData[i][3]) / 1000));
                    }
                    else
                    {
                        newTransform[i] = new Vector3(float.Parse(sData[i][2]) / 1000, float.Parse(sData[i][4]) / 1000, float.Parse(sData[i][3]) / 1000);
                        if (i != 0)
                        {
                            newTransform[i] = newTransform[0] + newTransform[i];
                        }
                        else
                        {
                            newTransform[i] += childNodes[0].position;
                        }
                        Debug.Log("newTransform updated");
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
                index++;
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
