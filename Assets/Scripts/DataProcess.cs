using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class DataProcess : MonoBehaviour
{
    private static DataProcess instance = null;

    private static readonly int _JOINTCOUNT = Constants.JOINTCOUNT;
    private static readonly int _TARGETFRAME = Constants.TARGETFRAME;

    public int index { get; set; } = 0;
    public TextAsset textA;
    public string[] lineData;
    private string data;
    private int dataLength = 0;
    public string[][] sData = new string[_JOINTCOUNT * _TARGETFRAME][];
    public int chunkIndex { get; set; }
    private int tempChunkIndex;
    public int loadedFrameCount { get; set; }
    public int refreshIndex { get; set; }
    public int inChunkFrame { get; set; }

    public bool parseFlag { get; set; } = false;
    public bool flag { get; set; } = true;

    public string[][] GetSData()
    {
        return sData;
    }

    public void UpdateDataChunk()
    {
        data = data.Remove(0, dataLength);
        RefreshDataChunk();
    }

    private void RefreshDataChunk()
    {
        System.Array.Clear(lineData, 0, lineData.Length);
        System.Array.Clear(sData, 0, sData.Length);
        lineData = data.Split("\n", (_JOINTCOUNT * _TARGETFRAME) + 1);
        if (lineData.Length == 0 || lineData[^1] == "")
        {
            dataLength = 0;
            throw new System.Exception("EOF detected");
        }
        dataLength = 0;
        for (int i = 0; i < lineData.Length - 1; i++)
        {
            dataLength += lineData[i].Length + 1;
        }
        loadedFrameCount = (lineData.Length - 1) / _JOINTCOUNT;
        refreshIndex = index + loadedFrameCount;
        inChunkFrame = 0;
        for (int i = 0; i < (lineData.Length - 1); i++)
        {
            lineData[i] = lineData[i].Replace("(", "");
            lineData[i] = lineData[i].Replace(")", "");
            sData[i] = lineData[i].Split(" ");
        }
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
            else if (int.Parse(firstColArr[^1]) < index)    //if current frame is greater than read frame
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
        Application.targetFrameRate = 30;

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
    private void Update()
    {
        if (flag)
        {
            if (index == refreshIndex)
            {
                try
                {
                    UpdateDataChunk();
                    parseFlag = true;
                }
                catch (System.Exception e)
                {
                    Debug.Log(e.Message);
                    parseFlag = false;
                    flag = false;
                    return;
                }
            }
            try
            {
                CheckFrameIndex();
                parseFlag = true;
            }
            catch (System.Exception e)
            {
                Debug.Log(e.Message);
                parseFlag = false;
                flag = false;
                return;
            }
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
}
