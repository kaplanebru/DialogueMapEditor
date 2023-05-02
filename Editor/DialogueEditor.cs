using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Codice.Client.BaseCommands;
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEngine;
using UnityEngine.Serialization;

namespace Editor
{
    public class DialogueEditor : EditorWindow
    {
        public static DialogueEditor Instance;
        NodeHolder _nodeHolder;
        private Node draggingNode;
        private Vector2 dragOffset;

        private void Awake() => Instance = this;
       
        private void OnEnable()
        {
            Stylize.StylizeNode();
            _nodeHolder = Selection.activeObject as NodeHolder;
        }
        

        /// <WINDOW />

        [OnOpenAsset(1)]
        public static bool OnOpenMap(int instanceID, int line)
        {
            NodeHolder _selection = EditorUtility.InstanceIDToObject(instanceID) as NodeHolder;
           
            if (_selection != null)
            {
                ShowEditorWindow();
                if (_selection.allNodes.Count == 0)
                {
                    //Statics.SaveChanges(_selection, "NewNode", "x");
                    Undo.RecordObject(_selection, "NewNode");
                    _selection.FirstNode();
                }
               
                return true;
            }
            return false;
        }
        
        [MenuItem("Window/DialogueEditor")]
        public static void ShowEditorWindow()
        {
            GetWindow(typeof(DialogueEditor), false, "Dialogue Editor");
        }

        private void OnGUI()
        {
            if (_nodeHolder == null) return;
            ProcessEvents();
            GenerateNodes(); //map.rootNode
           
        }

        /*private void OnDisable()
        {
            EditorUtility.SetDirty(_nodeHolder);
        }*/

        /// <NODES />

        
        private void GenerateNodes()
        {
            for (int i = _nodeHolder.allNodes.Count - 1; i >= 0; i--)
            {
                DrawCurves(_nodeHolder.allNodes[i]);
            }
            for (int i = _nodeHolder.allNodes.Count - 1; i >= 0; i--)
            {
                Draw(_nodeHolder.allNodes[i]);
            }
        }

        void Draw(Node node)
        {
            EditorGUI.BeginChangeCheck();
            Stylize.SetNodeStyle(node);
            string tempTxt = EditorGUILayout.TextField(node.mainText);
            string[] optionTempTxt = new string[node.options.Count];
            bool tempUnknown = EditorGUILayout.Toggle("IsUnknown", node.unknown);
            MapGenerator.Types tempType = (MapGenerator.Types) EditorGUILayout.EnumPopup("Type:", node.type);
            

            OptionsLayout(node, optionTempTxt);
            
            if (EditorGUI.EndChangeCheck()) //temp != node.mainTexts
            {
                SaveUserChanges(node, tempTxt, tempType, tempUnknown, optionTempTxt);
               
            }
            CheckButtons(node);
            GUILayout.EndArea();
        }


        void SaveUserChanges(Node node, string tempTxt, MapGenerator.Types tempType, bool tempUnknown, string[] optionTempTxt)
        {
           
            Undo.RecordObject(node, "NodeMainText" + node.name); //name
            node.mainText = tempTxt;
            
            Undo.RecordObject(node, "NodeType" + node.name); //name
            node.type = tempType;

            Undo.RecordObject(node, "IsUnknown" + node.name); //name
            node.unknown = tempUnknown;
                
                
            for (int i = 0; i < node.options.Count; i++)
            {
                Undo.RecordObject(node, "NodeOption" + node.name);
                node.options[i].mainText = optionTempTxt[i];
            }
                
            EditorUtility.SetDirty(node);
        }
        
        void OptionsLayout(Node node,  string[] optionTempTxt)
        {
            
            for (int i = 0; i < node.options.Count; i++)
            {
                EditorGUILayout.LabelField("Option: " + (i+1) + " : next node " + node.options[i].nextNode.id); //+ " " + node.options[i].nextNode.id
                optionTempTxt[i] = EditorGUILayout.TextField(node.options[i].mainText);
                node.options[i].stat = (UpgradeHandler.Stats) EditorGUILayout.EnumPopup("Stat:", node.options[i].stat);
                node.options[i].value = EditorGUILayout.IntField(node.options[i].stat + " Value: ", node.options[i].value);
                node.options[i].price = EditorGUILayout.IntField("Price: ", node.options[i].price);
            }
        }
        
        private void DrawCurves(Node node)
        {
           
            Vector3 startPos = node.rect.center; 
            foreach (var child in node.children) // foreach (var child in node.children)
            {
                Vector3 endPos = child.rect.center;
                Vector3 tangent = endPos - startPos;
                tangent.y = 0;
                tangent.x *= 0.5f;
                Handles.DrawBezier(startPos, endPos, startPos + tangent, 
                    endPos - tangent, Color.white, null, 4f);
            }
        }
        
        /// <BUTTONS />
        
        private Node connectingNode = null;
        void CheckButtons(Node node)
        {
            if (!_nodeHolder.detacheds.Contains(node))
            {
                if (GUILayout.Button("add"))
                {
                    Undo.RecordObject(_nodeHolder, "NewNode" + "x");
                    _nodeHolder.CreateNode(node);
                    _nodeHolder.UpdateAllIds();
                    EditorUtility.SetDirty(_nodeHolder);
                }
            }
            
            if(node.id > 1)
            {
                if (GUILayout.Button("remove"))
                {
                    _nodeHolder.DeleteNode(node);
                    _nodeHolder.UpdateAllIds();
                    EditorUtility.SetDirty(_nodeHolder);
                  
                }
            }
            
            LinkingNodes(node);
            
           // _nodeHolder.UpdateAllIds();

        }

        void LinkingNodes(Node node)
        {
            if (!connectingNode)
            {
                if (!_nodeHolder.detacheds.Contains(node))
                {
                    if (GUILayout.Button("connect"))
                        connectingNode = node;
                }
            }
            else if (connectingNode == node)
            {
                if (GUILayout.Button("cancel"))
                    connectingNode = null;
                
            }
            else if (connectingNode.children.Contains(node))
            {
                if (GUILayout.Button("disconnect"))
                    Disconnect(node);
            }
            else
            {
                if (node.id < connectingNode.id) return;
                if (connectingNode.parents.Count > 0 && connectingNode.parents.FirstOrDefault(p => p.children.Contains(node))) return;
                if (GUILayout.Button("connectable"))
                    LinkNodes(node);
            
            }
            _nodeHolder.UpdateAllIds();
        }
        
        void Disconnect(Node node)
        {
            Undo.RecordObject(_nodeHolder, "Removed");
            connectingNode.children.Remove(node);
            node.parents.Remove(connectingNode);
            for (var i = connectingNode.options.Count - 1; i >= 0; i--)
            {
                var option = connectingNode.options[i];
                if (option.nextNode == node)
                    connectingNode.options.Remove(option);
            }
            connectingNode = null;

            if(node.parents.Count == 0)
                _nodeHolder.detacheds.Add(node);
            
            
            EditorUtility.SetDirty(_nodeHolder);
            
           
        }

        void LinkNodes(Node node)
        {
            Undo.RecordObject(_nodeHolder, "Linked");
            connectingNode.children.Add(node);
            node.parents.Add(connectingNode);
                    
            var newOption = new Node.Option();
            newOption.nextNode = node;
            connectingNode.options.Add(newOption);
            connectingNode = null;
            if (_nodeHolder.detacheds.Contains(node))
                _nodeHolder.detacheds.Remove(node);
            
            EditorUtility.SetDirty(_nodeHolder);
        }

        


        /// <INPUT />
        private Node GetNodeAtPoint(Vector2 point)
        {
            Node lastNode = null;
            foreach (var node in _nodeHolder.allNodes)
            {
                if (node.rect.Contains(point))
                {
                    lastNode = node;
                }
            }
            return lastNode;
        }

        void ProcessEvents()
        {
            if (Event.current.type == EventType.MouseDown && !draggingNode)
            {
                draggingNode = GetNodeAtPoint(Event.current.mousePosition);
                if(draggingNode)
                    dragOffset = draggingNode.rect.position - Event.current.mousePosition;
            }
            else if (Event.current.type == EventType.MouseDrag && draggingNode)
            {
                draggingNode.rect.position = Event.current.mousePosition + dragOffset;
                GUI.changed = true;
            }
            else if (Event.current.type == EventType.MouseUp && draggingNode)
            {
                draggingNode = null;
             
            }
        }
        
        
        
        
    }
}

