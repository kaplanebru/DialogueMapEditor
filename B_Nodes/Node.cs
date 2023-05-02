using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Unity.Collections;
using UnityEditor;
using UnityEngine;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

[CreateAssetMenu(fileName = "Node", menuName = "Nodes")]
public class Node : ScriptableObject
{
   [Header("Editor")]
   public int id;
   public string mainText;
   public Rect rect;
   
   
   [Serializable]
   public class Option
   {
      [Header("Editor")]
      public int id = 1;
      public string mainText;
      public Node nextNode;

      [Header("Stats")]
      public UpgradeHandler.Stats stat;
      public int value;
      public int price;
   }
   public List<Option> options = new();
   public List<Node> parents = new();
   
   
   [FormerlySerializedAs("mainChildren")] [Header("Map")]
   public List<Node> children = new();
   public MapGenerator.Types type;
   public Vector2 pos;
   public bool unlocked = false;
   public bool unknown = false;
   private Point point;
   
   private void OnEnable()
   {
      Initialize();
   }

   void Initialize()
   {
      GetMainChildren();
      rect.width = 300;
      rect.height = 500;
      children = children.OrderByDescending(c => c.id).ToList();
   }
   public void XPos(Action<List<Node>> setXPos)
   {
      setXPos.Invoke(children);
   }
  
   void GetMainChildren()
   {
      children.Clear();
      if (options.Count == 0) return;
      foreach (var option in options)
      {
         if (!option.nextNode) continue;
         if (children.Contains(option.nextNode)) continue;
         {
            children.Add(option.nextNode);
         }
      }
   }
   
   
   public Point CreatePoint()
   {
      point = Instantiate(GameManager.Instance.samplePoint, pos, Quaternion.identity);
      point.props = this;
      return point;
   }
       
   public void UnlockPoint(bool enable)
   {
      unlocked = enable;
      point.ChangeColor(enable);
   }
   
  
   
   

   




}
