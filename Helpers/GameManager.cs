using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;
using UnityEngine.Subsystems;
using Random = UnityEngine.Random;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    public Point samplePoint;
    public LineRenderer line;
    public Color unlockedColor;
    public Color lockedColor;

    private void Awake() => Instance = this;
    

    public void ResetGame()
    {
        EventBus.OnDialogueEnd?.Invoke();
        SaveSystem.ResetSaves();
        SceneManager.LoadScene(0);
    }
    
}
