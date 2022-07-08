using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class DataProcess : MonoBehaviour
{
    private static DataProcess instance = null;

    private const int JOINTCOUNT = 17;

    private int index;
    public TextAsset textA;
    public string[] lineData;
    private string data;
    public string dataChunk;
    private string[] temp = new string[JOINTCOUNT * 30];
    public string[][] sData = new string[JOINTCOUNT][];
    private int frameNum = 0;
    private int chunkIndex = 0;



    public int GetIndex()
    {
        return index;
    }
    public void SetIndex(int _index)
    {
        index = _index;
    }
    public string[] GetLineData()
    {
        return lineData;
    }
    public string[][] GetSData()
    {
        return sData;
    }
    public int GetFrameNum()
    {
        return frameNum;
    }
    public int GetChunkIndex()
    {
        return chunkIndex;
    }


    public void UpdateDataChunk()
    {
        data = data.Remove(0, dataChunk.Length);
        lineData = data.Split("\r\n", JOINTCOUNT * 30 + 1);
        System.Array.Copy(lineData, temp, lineData.Length - 1);
        dataChunk = string.Join("\r\n", temp) + "\r\n";
    }

    public void UpdatePerJointData()
    {
        for (int i = 0; i < JOINTCOUNT; i++)
        {
            lineData[index % 30 * JOINTCOUNT + i] = lineData[index % 30 * JOINTCOUNT + i].Replace("(", "");
            lineData[index % 30 * JOINTCOUNT + i] = lineData[index % 30 * JOINTCOUNT + i].Replace(")", "");
            sData[i] = lineData[index % 30 * JOINTCOUNT + i].Split(" ");
        }
        string[] firstColArr = GetFirstColumn(sData);
        if (firstColArr.Contains(index.ToString()))
        {
            frameNum = index;
            chunkIndex = firstColArr.ToList().IndexOf(index.ToString());
        }
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
            if(null == instance)
            {
                return null;
            }
            return instance;
        }
    }

    private string[] GetFirstColumn(string[][] inputArr) => Enumerable.Range(0, inputArr.Length).Select(x => inputArr[x][0]).ToArray();

}
