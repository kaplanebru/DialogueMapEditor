using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using UnityEngine.SceneManagement;
using File = System.IO.File;

public abstract class SaveSystem
{
    private static string savePath => Application.dataPath + "/save.txt";
    private static string json;
    private static SaveData saveData = new();
    public int[] values = new int[3];


    public class SaveData
    {
        public int checkPoint;
        public int[] values = new int[3];
        public void Save(int _checkPoint, UpgradeHandler.PlayerStats[] stats) //int currentStatId,
        {
            checkPoint = _checkPoint;
            for (int i = 0; i < values.Length; i++)
            {
                values[i] = stats[i].value;
            }
        }
    }

   
    public static void SaveUpgrade(int checkPoint, UpgradeHandler.PlayerStats[] stats)
    {
        saveData.Save(checkPoint, stats);
        json = JsonUtility.ToJson(saveData);
        File.WriteAllText(savePath, json);
    }

    public static SaveData Loaded()
    {
        if (!File.Exists(savePath)) return null;
        string saveString = File.ReadAllText(savePath);
        SaveData saveObject = JsonUtility.FromJson<SaveData>(saveString);
        return saveObject;
    }

    public static void ResetSaves()
    {
        if (!File.Exists(savePath)) return;
        File.WriteAllText(savePath, null);
    }
    


}
