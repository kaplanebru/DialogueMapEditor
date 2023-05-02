using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

public class Statics : MonoBehaviour
{
    /*public static void SaveChanges<T>(T so, string text, string _name) where T : ScriptableObject
    {
        Undo.RecordObject(so, text +" "+ _name);
        EditorUtility.SetDirty(so);
    }*/
    
    public static string GetPath(string fileName)
    {
        string basePath = "Assets/Resources/";
        return Path.Combine(basePath, fileName);
    }
}
