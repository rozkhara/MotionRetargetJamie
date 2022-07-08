using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class DataProcess : MonoBehaviour
{
    private static DataProcess instance = null;

    private const int JOINTCOUNT = 17;
    private const int TARGETFRAME = 30;

    public int index { get; set; } = 0;
    public TextAsset textA;
    public string[] lineData;
    private string data;
    public string dataChunk;
    private string[] temp = new string[JOINTCOUNT * 30];
    public string[][] sData = new string[JOINTCOUNT][];
    public int frameNum { get; set; }
    public int chunkIndex { get; set; }
    public int currentFrame { get; set; }
    public int loadedFrameCount { get; set; }
    public int refreshIndex { get; set; }
    private int inChunkFrame;



    public string[][] GetSData()
    {
        return sData;
    }

    public void UpdateDataChunk()
    {
        data = data.Remove(0, dataChunk.Length);
        lineData = data.Split("\n", (JOINTCOUNT * TARGETFRAME) + 1);
        if (lineData.Length == 0 || lineData[lineData.Length - 1] == "")
        {
            throw new System.Exception("EOF");
        }
        System.Array.Copy(lineData, temp, lineData.Length - 1);
        dataChunk = string.Join("\n", temp) + "\n";
        loadedFrameCount = temp.Length / JOINTCOUNT;
        refreshIndex = index + loadedFrameCount;
        inChunkFrame = 0;
    }

    public void UpdatePerJointData()
    {
        for (int i = 0; i < JOINTCOUNT; i++)
        {
            lineData[inChunkFrame * JOINTCOUNT + i] = lineData[inChunkFrame * JOINTCOUNT + i].Replace("(", "");
            lineData[inChunkFrame * JOINTCOUNT + i] = lineData[inChunkFrame * JOINTCOUNT + i].Replace(")", "");
            sData[i] = lineData[inChunkFrame * JOINTCOUNT + i].Split(" ");
        }
        string[] firstColArr = GetColumn(sData, 0);
        if (firstColArr.Contains(index.ToString()))
        {
            frameNum = index;
            chunkIndex = firstColArr.ToList().IndexOf(index.ToString());
        }
        else
        {
            if (int.Parse(firstColArr[0]) > index) //if current frame is less than read frame
            {
                throw new System.Exception("Wait until index comes");
            }
            else if (int.Parse(firstColArr[firstColArr.Length - 1]) < index) //if current frame is greater than read frame
            {
                if (inChunkFrame == loadedFrameCount) //if current frame is the last frame of the loaded chunk
                {
                    UpdateDataChunk();
                }
                else
                {
                    inChunkFrame++;
                }
                UpdatePerJointData();
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
}
