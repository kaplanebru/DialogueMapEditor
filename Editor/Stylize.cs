using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Editor
{
    public class Stylize
    {
        public static GUIStyle nodeStyle = new GUIStyle();

        public struct Style
        {
            public string path;
            public int padding;
            public int border;

            public Style(string _path, int _padding, int _border)
            {
                path = _path;
                padding = _padding;
                border = _border;
            }
        }

        public static Style style = new Style();

        public static void StylizeNode()
        {

            style = new Style("node2", 20, 22);

            nodeStyle = new GUIStyle();
            nodeStyle.normal.background = EditorGUIUtility.Load(style.path) as Texture2D;
            nodeStyle.padding = new RectOffset(style.padding, style.padding, style.padding, style.padding);
            nodeStyle.border = new RectOffset(style.border, style.border, style.border, style.border);

        }

        public static void SetNodeStyle(Node node)
        {
            GUILayout.BeginArea(node.rect, nodeStyle);
            EditorGUILayout.LabelField("Main " + node.id, EditorStyles.whiteLargeLabel);
        }

    }

}
