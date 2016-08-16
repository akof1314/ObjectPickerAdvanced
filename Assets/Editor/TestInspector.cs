using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;

[CustomEditor(typeof (Test))]
public class TestInspector : Editor
{
    private int m_ControlPrefabPathId;
    private SerializedObject targetObj;
    private SerializedProperty person;
    private SerializedProperty person2;
    private SerializedProperty person3;
    private SerializedProperty person4;

    private double m_TimeNow = -1;

    public void OnEnable()
    {
        targetObj = new SerializedObject(target);
        person = targetObj.FindProperty("person");
        person2 = targetObj.FindProperty("person2");
        person3 = targetObj.FindProperty("person3");
        person4 = targetObj.FindProperty("person4");
    }

    public override void OnInspectorGUI()
    {
        targetObj.Update();

        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.PropertyField(person);
        EditorGUIUtil.ObjectPickerField(person);
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.PropertyField(person2);
        EditorGUILayout.PropertyField(person3);
        EditorGUILayout.PropertyField(person4);
        if (GUILayout.Button("选择对象"))
        {
            ObjectSelectorWindow.ShowObjectPicker<GameObject>(person.objectReferenceValue, OnObjectPicker1, "Assets/AC");
        }
        if (GUILayout.Button("选择对象2"))
        {
            ObjectSelectorWindow.ShowObjectPicker<Sprite>(person.objectReferenceValue, OnObjectPicker, "Assets");
        }

        if (GUILayout.Button("选择对象3"))
        {
            ObjectSelectorWindow.ShowObjectPicker<AudioClip>(person3.objectReferenceValue, OnObjectPicker3, "Assets/Sound");
        }

        if (GUILayout.Button("选择对象4"))
        {
            EditorGUIUtility.ShowObjectPicker<GameObject>(person.objectReferenceValue, false, String.Empty, 0);
            EditorApplication.update += updaterShow;
        }
        if (GUILayout.Button("选择对象5"))
        {
            GameObject go =
                AssetDatabase.LoadAssetAtPath<GameObject>(
                    "Assets/Standard Assets/Characters/ThirdPersonCharacter/Prefabs/ThirdPersonController.prefab");

            AnimationClip[] clips = AnimationUtility.GetAnimationClips(go);
            List<int> ids = new List<int>();
            foreach (var clip in clips)
            {
                ids.Add(clip.GetInstanceID());
            }
            ObjectSelectorWindow.ShowObjectPicker<AnimationClip>(person4.objectReferenceValue, OnObjectPicker4, "Assets/Standard Assets/Characters/ThirdPersonCharacter/Animation", ids);
        }
        targetObj.ApplyModifiedProperties();
    }

    private void OnObjectPicker1(UnityEngine.Object obj)
    {
        person.objectReferenceValue = obj;
        targetObj.ApplyModifiedProperties();
    }

    private void OnObjectPicker3(UnityEngine.Object obj)
    {
        person3.objectReferenceValue = obj;
        targetObj.ApplyModifiedProperties();
    }

    private void OnObjectPicker4(UnityEngine.Object obj)
    {
        person4.objectReferenceValue = obj;
        targetObj.ApplyModifiedProperties();
    }

    private void OnObjectPicker(UnityEngine.Object obj)
    {
        if (obj)
        {
            Debug.Log("点中了 " + obj.name);
        }
        else
        {
            Debug.Log("点中了空");
        }
    }

    private void updaterShow()
    {
        Debug.Log(1);
        if (m_TimeNow < 0)
        {
            m_TimeNow = EditorApplication.timeSinceStartup + 3f;
        }
        if (m_TimeNow <= EditorApplication.timeSinceStartup)
        {
            EditorApplication.update -= updaterShow;
            Debug.Log(2);
            ShowPicker();
        }
    }

    private void ShowPicker()
    {
        string m_RequiredType = typeof (Sprite).Name;
        Assembly assembly = Assembly.GetAssembly(typeof(EditorGUIUtility));
        Type typeObjectSelector = assembly.GetType("UnityEditor.ObjectSelector");
        PropertyInfo get = typeObjectSelector.GetProperty("get", BindingFlags.Static | BindingFlags.Public | BindingFlags.GetProperty);
        FieldInfo listAreaInfo = typeObjectSelector.GetField("m_ListArea", BindingFlags.GetField | BindingFlags.Instance | BindingFlags.NonPublic);
        PropertyInfo listPositionInfo = typeObjectSelector.GetProperty("listPosition",
            BindingFlags.Instance | BindingFlags.GetProperty | BindingFlags.NonPublic);
        EditorWindow objectSelector = get.GetValue(null, null) as EditorWindow;
        object listArea = listAreaInfo.GetValue(objectSelector);
        object listPosition = listPositionInfo.GetValue(objectSelector, null);

        Type typeSearchFilter = assembly.GetType("UnityEditor.SearchFilter");
        object searchFilter = Activator.CreateInstance(typeSearchFilter);
        MethodInfo searchFieldStringToFilter = typeSearchFilter.GetMethod("SearchFieldStringToFilter",
            BindingFlags.Instance | BindingFlags.NonPublic);
        PropertyInfo classNames = typeSearchFilter.GetProperty("classNames",
            BindingFlags.Instance | BindingFlags.Public | BindingFlags.GetProperty);
        classNames.SetValue(searchFilter, new string[] { m_RequiredType }, null);

        Type typeObjectListArea = assembly.GetType("UnityEditor.ObjectListArea");
        MethodInfo initInfo = typeObjectListArea.GetMethod("Init", BindingFlags.Instance | BindingFlags.Public);
        initInfo.Invoke(listArea, new object[] { listPosition, HierarchyType.Assets, searchFilter, true });
    }
}
