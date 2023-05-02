using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Unity.Collections;
using UnityEditor;
using UnityEngine;
using UnityEngine.Serialization;

[CreateAssetMenu(fileName = "NodeHolder", menuName = "NodeHolder")]
public class NodeHolder : ScriptableObject
{
    public List<Node> allNodes = new();
    private int newNodeCount = 0;
    
    public void CreateNode(Node parent)
    {
        Node newNode = CreateInstance<Node>();
        newNode.rect.x = parent.rect.x + parent.rect.width; //100 * newNode.id;
        newNode.rect.y = parent.rect.y;
        newNode.parents.Add(parent);
       
        
        CreateOption(newNode, parent);
        AssetDatabase.CreateAsset(newNode, Statics.GetPath("Node " + newNode.id + ".asset"));
        SaveNewNode();
        UpdateAllIds();
     
    }

    void CreateOption(Node newNode, Node parent)
    {
        Node.Option option = new Node.Option();
        option.nextNode = newNode;

        parent.options.Add(option);
        parent.children.Add(newNode);
        allNodes.Add(newNode);
        newNodeCount++;
        newNode.id = newNodeCount;//allNodes.Count; // + 1;
        

    }
    void SaveNewNode()
    {
        EditorUtility.SetDirty(this);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        EditorUtility.FocusProjectWindow();
       
    }

    public void DeleteNode(Node nodeToDelete)
    {
        allNodes.Remove(nodeToDelete);
       
        foreach (var node in allNodes)
        {
            for (int i = node.options.Count - 1; i >= 0; i--)
            {
                if (node.options[i].nextNode == nodeToDelete)
                {
                    
                    node.children.Remove(nodeToDelete);
                    node.options.Remove(node.options[i]);
                    
                    if (!nodeToDelete) continue;
                    File.Delete (Statics.GetPath(nodeToDelete.name + ".asset"));  //adjusted "Node " + nodeToDelete.id + ".asset"
                    File.Delete (Statics.GetPath(nodeToDelete.name + ".asset.meta"));
                    AssetDatabase.Refresh();
                    SaveSystem.ResetSaves();
                }
            }
        }
        
        if (detacheds.Contains(nodeToDelete))
            detacheds.Remove(nodeToDelete);
        
        EditorUtility.SetDirty(this);
    }
    
    [ReadOnly]public int lastId;
    public List<Node> detacheds = new();
    public void UpdateAllIds()
    {
        var root = allNodes[0];
        lastId = 1;
        UpdateId(root);
        foreach (var detached in detacheds)
        {
            lastId++;
            detached.id = lastId;
        }
    }
    void UpdateId(Node root)
    {
        if(!root) return;
        foreach (var child in root.children)
        {
            lastId++;
            child.id = lastId;
        }

        foreach (var child in root.children)
        {
            UpdateId(child);
        }
       
        EditorUtility.SetDirty(root);
        
    }

    public void FirstNode()
    {
        if (allNodes.Count == 0)
        {
            newNodeCount = 0;
            Node newNode = CreateInstance<Node>();
            SaveNewNode();
            
            allNodes.Add(newNode);
            newNodeCount++;
            newNode.id = newNodeCount;
            
            AssetDatabase.CreateAsset(newNode, Statics.GetPath("Node 1.asset"));
            EditorUtility.SetDirty(this);
        }
    }
    
    
    
    private void OnDisable()
    {
        EditorUtility.SetDirty(this);
    }

//#if UNITY_EDITOR
    private void Awake()
    {
       
        
    }
//#endif
    
    
}
