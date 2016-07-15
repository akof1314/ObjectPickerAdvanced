using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class EditorGUIUtil
{
    public static string SearchField(Rect position, string text)
    {
        Rect position2 = position;
        position2.width -= 15f;
        text = EditorGUI.TextField(position2, text, new GUIStyle("SearchTextField"));
        Rect position3 = position;
        position3.x += position.width - 15f;
        position3.width = 15f;
        if (GUI.Button(position3, GUIContent.none, string.IsNullOrEmpty(text) ? "SearchCancelButtonEmpty" : "SearchCancelButton"))
        {
            text = string.Empty;
            GUIUtility.keyboardControl = 0;
        }
        return text;
    }
}