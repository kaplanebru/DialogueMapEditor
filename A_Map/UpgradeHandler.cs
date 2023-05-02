using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Collections;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.Windows;
using System.IO;
using File = System.IO.File;

public class UpgradeHandler : MonoBehaviour
{
    public static UpgradeHandler Instance;
    private void Awake() => Instance = this;

    public enum Stats
    {
        Health,
        Stamina,
        Gold,
    }

    public PlayerStats[] stats;
    private TextMeshProUGUI goldText => stats[2].statText;

    private void Start()
    {
        StartPanel();
    }

    [Serializable]
    public class PlayerStats
    {
        public string statName;
        public int value;
        public TextMeshProUGUI statText;
    }
    
    public void UpgradeStat(int id, int amount, int price, int checkPoint)
    {
        stats[id].value += amount;
        stats[2].value -= price;
        SetUpgradeTexts(id);
        SaveSystem.SaveUpgrade(checkPoint, stats);
    }

    public bool HasMoney() => stats[2].value > 0;


    void SetUpgradeTexts(int i)
    {
        stats[i].statText.text = stats[i].value.ToString();
        goldText.text = stats[2].value.ToString();
    }


    void LoadStats()
    {
        SaveSystem.SaveData savedData = SaveSystem.Loaded();
        if (savedData == null) return;
        for (int i = 0; i < stats.Length; i++)
        {
            stats[i].value = savedData.values[i];
        }
    }
     

    void StartPanel()
    {
        LoadStats();
        for (int i = 0; i < stats.Length; i++)
        {
            SetUpgradeTexts(i);
        }
    }
    
}
