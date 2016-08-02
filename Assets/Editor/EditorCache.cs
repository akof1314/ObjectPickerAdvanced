using System;
using System.Collections.Generic;
using UnityEngine;
namespace UnityEditor
{
    internal enum EditorFeatures
    {
        None = 0,
        PreviewGUI = 1,
        OnSceneDrag = 4
    }

    internal class EditorCache : IDisposable
	{
		private Dictionary<UnityEngine.Object, EditorWrapper> m_EditorCache;
		private Dictionary<UnityEngine.Object, bool> m_UsedEditors;
		private EditorFeatures m_Requirements;
		public EditorWrapper this[UnityEngine.Object o]
		{
			get
			{
				m_UsedEditors[o] = true;
				if (m_EditorCache.ContainsKey(o))
				{
					return m_EditorCache[o];
				}
				EditorWrapper editorWrapper = EditorWrapper.Make(o, m_Requirements);
				EditorWrapper editorWrapper2 = editorWrapper;
				m_EditorCache[o] = editorWrapper2;
				return editorWrapper2;
			}
		}
		public EditorCache() : this(EditorFeatures.None)
		{
		}
		public EditorCache(EditorFeatures requirements)
		{
			m_Requirements = requirements;
			m_EditorCache = new Dictionary<UnityEngine.Object, EditorWrapper>();
			m_UsedEditors = new Dictionary<UnityEngine.Object, bool>();
		}
		public void CleanupUntouchedEditors()
		{
			List<UnityEngine.Object> list = new List<UnityEngine.Object>();
			foreach (UnityEngine.Object current in m_EditorCache.Keys)
			{
				if (!m_UsedEditors.ContainsKey(current))
				{
					list.Add(current);
				}
			}
			if (m_EditorCache != null)
			{
				foreach (UnityEngine.Object current2 in list)
				{
					EditorWrapper editorWrapper = m_EditorCache[current2];
					m_EditorCache.Remove(current2);
					if (editorWrapper != null)
					{
						editorWrapper.Dispose();
					}
				}
			}
			m_UsedEditors.Clear();
		}
		public void CleanupAllEditors()
		{
			m_UsedEditors.Clear();
			CleanupUntouchedEditors();
		}
		public void Dispose()
		{
			CleanupAllEditors();
			GC.SuppressFinalize(this);
		}
		~EditorCache()
		{
			Debug.LogError("Failed to dispose EditorCache.");
		}
	}
}
