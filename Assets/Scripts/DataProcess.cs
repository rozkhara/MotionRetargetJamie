using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class DataProcess : MonoBehaviour
{
    private static DataProcess instance = null;

    private static readonly int _JOINTCOUNT = Constants.JOINTCOUNT;
    private static readonly int _TARGETFRAME = Constants.TARGETFRAME;
    private int[] parent = {0, 0, 1, 2, 0, 4, 5, 0, 7, 8, 9, 8, 11, 12, 8, 14, 15};

    public int index { get; set; } = 0;
    public TextAsset textA;
    public string[] lineData;
    private string data;
    public string dataChunk;   //could be replaced with integer value as the actual string value is never referenced currently
    private string[] temp = new string[_JOINTCOUNT * _TARGETFRAME];
    public string[][] sData = new string[_JOINTCOUNT * _TARGETFRAME][];
    public string[] Local = new string[_JOINTCOUNT * _TARGETFRAME];

    public int chunkIndex { get; set; }
    private int tempChunkIndex;
    public int loadedFrameCount { get; set; }
    public int refreshIndex { get; set; }
    public int inChunkFrame { get; set; }

    

    public string[][] GetSData()
    {
        return sData;
    }

    public void UpdateDataChunk()
    {
        data = data.Remove(0, dataChunk.Length);
        RefreshDataChunk();
    }

    private void RefreshDataChunk()
    {
        System.Array.Clear(lineData, 0, lineData.Length);
        System.Array.Clear(sData, 0, sData.Length);
        lineData = data.Split("\n", (_JOINTCOUNT * _TARGETFRAME) + 1);
        if (lineData.Length == 0 || lineData[lineData.Length - 1] == "")
        {
            dataChunk = "";     //This necessarily frees the last chunk loaded, thus in future the whole determination process needs to be performed per frame, not per 30 frames
            throw new System.Exception("EOF detected");
        }
        System.Array.Copy(lineData, temp, lineData.Length - 1);
        dataChunk = string.Join("\n", temp) + "\n";
        loadedFrameCount = temp.Length / _JOINTCOUNT;
        refreshIndex = index + loadedFrameCount;
        inChunkFrame = 0;
        for (int i = 0; i < temp.Length; i++)
        {
            lineData[i] = lineData[i].Replace("(", "");
            lineData[i] = lineData[i].Replace(")", "");
            sData[i] = lineData[i].Split(" ");
        }
        convertGtoL(sData, Local);
    }

    public void CheckFrameIndex()
    {
        string[] firstColArr = GetColumn(sData, 0);
        if (firstColArr.Contains(index.ToString())) //if current frame exists within the loaded frames
        {
            tempChunkIndex = firstColArr.ToList().IndexOf(index.ToString());
            if (sData[tempChunkIndex][1] == "0" && sData[tempChunkIndex + _JOINTCOUNT - 1][1] == (_JOINTCOUNT - 1).ToString())    //if current frame's data includes every joint
            {
                chunkIndex = tempChunkIndex;
            }
            else
            {
                throw new System.Exception("Loaded data does not include information for every joint");
            }
        }
        else    //if current frame does not exist within the loaded frames
        {
            if (int.Parse(firstColArr[0]) > index)  //if current frame is less than read frame
            {
                RefreshDataChunk();     //wait until current frame matches with the read frame
                return;
                //throw new System.Exception("Wait until index comes");
            }
            else if (int.Parse(firstColArr[firstColArr.Length - 1]) < index)    //if current frame is greater than read frame
            {
                UpdateDataChunk();      //load new chunk
                return;
            }
            throw new System.Exception("The current frame does not exist within the loaded frames");
        }
        inChunkFrame++;
    }

    private void Awake()
    {
        data = textA.text.ToString();
        if (null == instance)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public static DataProcess Instance
    {
        get
        {
            if (null == instance)
            {
                return null;
            }
            return instance;
        }
    }

    private string[] GetColumn(string[][] inputArr, int column) => Enumerable.Range(0, inputArr.Length).Select(x => inputArr[x][column]).ToArray();

    public void convertGtoL(string [][] sData, string [] Local)
    {
        string[] x = new string[_JOINTCOUNT];
        string[] y = new string[_JOINTCOUNT];
        string[] z = new string[_JOINTCOUNT];

        for (int i = 0; i < sData.Length / _JOINTCOUNT; i++)
        {
            for (int j = 0; j < _JOINTCOUNT; j++)
            {
                Debug.LogFormat("sData[{0}] : {1} {2} {3}", i * _JOINTCOUNT + j, sData[i * _JOINTCOUNT + j][2], sData[i * _JOINTCOUNT + j][3], sData[i * _JOINTCOUNT + j][4]);
                x[j] = (float.Parse(sData[i * _JOINTCOUNT + j][2]) - float.Parse(sData[i * _JOINTCOUNT + parent[j]][2])).ToString();
                y[j] = (float.Parse(sData[i * _JOINTCOUNT + j][3]) - float.Parse(sData[i * _JOINTCOUNT + parent[j]][3])).ToString();
                z[j] = (float.Parse(sData[i * _JOINTCOUNT + j][4]) - float.Parse(sData[i * _JOINTCOUNT + parent[j]][4])).ToString();
                Debug.LogFormat("new sData[{0}] : {1} {2} {3}", i * _JOINTCOUNT + j, sData[i * _JOINTCOUNT + j][2], sData[i * _JOINTCOUNT + j][3], sData[i * _JOINTCOUNT + j][4]);
                Local[i * _JOINTCOUNT + j] = x[j] + " " + y[j] + " " + z[j];
                //sData[i * _JOINTCOUNT + j][2] = x[j];
                //sData[i * _JOINTCOUNT + j][3] = y[j];
                //sData[i * _JOINTCOUNT + j][4] = z[j]; 
            }
        }
    }
}
