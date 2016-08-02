using System;
using System.Reflection;
using UnityEngine;
namespace UnityEditor
{
	internal class EditorWrapper : IDisposable
	{
		public delegate void VoidDelegate(SceneView sceneView);
		private Editor editor;
		public EditorWrapper.VoidDelegate OnSceneDrag;
		public string name
		{
			get
			{
				return editor.target.name;
			}
		}
		private EditorWrapper()
		{
		}
        public void OnEnable()
        {
            MethodInfo method = editor.GetType().GetMethod("OnEnable", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
            if (method != null)
            {
                method.Invoke(editor, null);
            }
        }
        public void OnDisable()
        {
            MethodInfo method = editor.GetType().GetMethod("OnDisable", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
            if (method != null)
            {
                method.Invoke(editor, null);
            }
        }
        public bool HasPreviewGUI()
		{
			return editor.HasPreviewGUI();
		}
		public void OnPreviewSettings()
		{
			editor.OnPreviewSettings();
		}
		public void OnPreviewGUI(Rect position, GUIStyle background)
		{
			editor.OnPreviewGUI(position, background);
		}
		public void OnInteractivePreviewGUI(Rect r, GUIStyle background)
		{
			if (editor != null)
			{
				editor.OnInteractivePreviewGUI(r, background);
			}
		}
		public string GetInfoString()
		{
			return editor.GetInfoString();
		}
		public static EditorWrapper Make(UnityEngine.Object obj, EditorFeatures requirements)
		{
			EditorWrapper editorWrapper = new EditorWrapper();
			if (editorWrapper.Init(obj, requirements))
			{
				return editorWrapper;
			}
			editorWrapper.Dispose();
			return null;
		}
		private bool Init(UnityEngine.Object obj, EditorFeatures requirements)
		{
			editor = Editor.CreateEditor(obj);
			if (editor == null)
			{
				return false;
			}
			if ((requirements & EditorFeatures.PreviewGUI) > EditorFeatures.None && !editor.HasPreviewGUI())
			{
				return false;
			}
			Type type = editor.GetType();
			MethodInfo method = type.GetMethod("OnSceneDrag", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
			if (method != null)
			{
				OnSceneDrag = (EditorWrapper.VoidDelegate)Delegate.CreateDelegate(typeof(EditorWrapper.VoidDelegate), editor, method);
			}
			else
			{
				if ((requirements & EditorFeatures.OnSceneDrag) > EditorFeatures.None)
				{
					return false;
				}
				OnSceneDrag = new EditorWrapper.VoidDelegate(DefaultOnSceneDrag);
			}
			return true;
		}
		private void DefaultOnSceneDrag(SceneView sceneView)
		{
		}
		public void Dispose()
		{
			if (editor != null)
			{
				OnSceneDrag = null;
				UnityEngine.Object.DestroyImmediate(editor);
				editor = null;
			}
			GC.SuppressFinalize(this);
		}
		~EditorWrapper()
		{
			Debug.LogError("Failed to dispose EditorWrapper.");
		}
	}
}
