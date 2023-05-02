using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Collections;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

public class MapGenerator : MonoBehaviour
{
    public Vector2 _distance = new (1, 3);
    public Vector2 randomize = new(0, 2);
    public enum Types
    {
        Merchant, Treasure, Unknown
    }
    
    public NodeHolder nodeHolder;
    [ReadOnly]public List<Point> points = new();
    
    [HideInInspector]
    public List<Node> nodes = new ();

    private void Start()
    {
        if (nodeHolder.allNodes.Count == 0) return;
        ReorderNodeList();
        SetPositions(nodes[0]);
        points.AddRange(CreatePoints());
        LoadCheckPoint();
        
    }
    
    void SetPositions(Node root)
    {
        root.XPos(XPos);
        foreach (var child in root.children)
        {
            child.pos.y = root.pos.y + _distance.y + GetRandomValue();;
            SetPositions(child);
        }
    }
    
    void XPos(List<Node> children)
    {
        if (children.Count <= 1) return;
        int evenDir = 1;
        int oddDir = -1;
        for (int i = 0; i < children.Count; i++)
        {
            switch (children.Count % 2)
            {
                case 0:
                    children[i].pos.x =  evenDir * _distance.x;
                    evenDir *= -1;
                    break;
                default:
                {
                    if (i == 0) continue;
                    children[i].pos.x = oddDir * _distance.x * 1.5f;
                    oddDir *= -1;
                    break;
                }
            }
        }
    }
    
    float GetRandomValue() => Random.Range(randomize.x, randomize.y);
    

    void LoadCheckPoint()
    {
       
        nodes.ForEach(n=>n.UnlockPoint(false));
        if(nodes.Count > 0)
            nodes[0].UnlockPoint(true);
        
        SaveSystem.SaveData savedData = SaveSystem.Loaded();
        if (savedData == null)
        {
            nodes[0].UnlockPoint(true);
        }
        else
        {
            nodes[savedData.checkPoint].UnlockPoint(true);
            EventBus.OnGameStart?.Invoke(points[savedData.checkPoint]);
        }
    }
    
    IEnumerable<Point> CreatePoints()
    {
        for (int i = 0; i < nodes.Count; i++)
        {
            yield return nodes[i].CreatePoint();
        }
    }
    
    void ReorderNodeList()
    {
       
        nodeHolder.UpdateAllIds();
        nodes = nodeHolder.allNodes.OrderBy(n => n.id).ToList();
        nodes.ForEach(n=>n.pos = Vector2.zero);
    }
    
    
}

