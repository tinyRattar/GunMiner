using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;


[Serializable]
public class SaveData
{
    [SerializeField] List<int> listRewardGot;
    [SerializeField] int deadNum;
    [SerializeField] int highScore;

    public SaveData()
    {
        listRewardGot = new List<int>();
        DeadNum = 0;
        highScore = 0;
    }

    public List<int> ListRewardGot { get => listRewardGot; set => listRewardGot = value; }
    public int DeadNum { get => deadNum; set => deadNum = value; }
    public int HighScore { get => highScore; set => highScore = value; }
}

public class SaveManager : Singleton<SaveManager>
{
    [SerializeField] string saveFile;
    SaveData saveData;

    [Header("ต๗สิ")]
    [SerializeField] bool deleteSaveFile;

    public void SaveRewardGot(int idx)
    {
        if (saveData.ListRewardGot.Contains(idx))
        {
            Debug.LogError("save failed: reward " + idx.ToString() + " already got");
        }
        else
        {
            saveData.ListRewardGot.Add(idx);
            SaveFile();
        }
    }

    public List<int> LoadListRewardGot()
    {
        return saveData.ListRewardGot;
    }
    public void SaveDeadNum(int value)
    {
        saveData.DeadNum = value;
        SaveFile();
    }
    public int LoadDeadNum()
    {
        return saveData.DeadNum;
    }



    public void SaveFile()
    {
        string save_json = JsonUtility.ToJson(saveData, true);
        string path = Application.persistentDataPath + "/" + saveFile;

        File.WriteAllText(path, save_json);
    }

    public void DeleteFile()
    {
        saveData = new SaveData();
        SaveFile();
    }

    public void LoadFile()
    {
        string path = Application.persistentDataPath + "/" + saveFile;
        if (File.Exists(path))
        {
            string save_json = File.ReadAllText(path);
            saveData = JsonUtility.FromJson<SaveData>(save_json);
        }
        else
        {
            saveData = new SaveData();
        }

    }

    public bool TryUpdateHighScore(int newScore)
    {
        if(saveData.HighScore < newScore)
        {
            saveData.HighScore = newScore;
            SaveFile();
            return true;
        }
        return false;
    }
    protected override void Awake()
    {
        LoadFile();
        base.Awake();
        SaveFile();
    }

    private void Update()
    {
        //if (Input.GetKeyDown(KeyCode.Backspace))
        //{
        //    deleteSaveFile = true;
        //}
        if (deleteSaveFile)
        {
            deleteSaveFile = false;
            DeleteFile();
        }
    }

}
