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
        EditorGUIUtil.ObjectPickerField(person, null, "Assets/AC");
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.PropertyField(person2);
        if (GUILayout.Button("C", EditorStyles.miniButton, GUILayout.Width(24f)))
        {
            ObjectSelectorWindow.ShowObjectPicker<Sprite>(person.objectReferenceValue, OnObjectPicker, "Assets");
        }
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.PropertyField(person3);
        if (GUILayout.Button("C", EditorStyles.miniButton, GUILayout.Width(24f)))
        {
            ObjectSelectorWindow.ShowObjectPicker<AudioClip>(person3.objectReferenceValue, OnObjectPicker3, "Assets/Sound");
        }
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.PropertyField(person4);
        if (GUILayout.Button("C", EditorStyles.miniButton, GUILayout.Width(24f)))
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
        EditorGUILayout.EndHorizontal();

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
}
