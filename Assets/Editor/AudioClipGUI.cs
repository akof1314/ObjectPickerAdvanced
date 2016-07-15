using System;
using UnityEngine;
using System.Reflection;
using UnityEditor;

public static class AudioClipGUI
{
    private static GUIStyle s_PreButton;
    private static Rect m_wantedRect;
    private static bool m_bAutoPlay;
    private static bool m_bLoop = false;
    private static bool m_bPlayFirst;
    private static GUIContent[] s_PlayIcons = new GUIContent[2];
    private static GUIContent[] s_AutoPlayIcons = new GUIContent[2];
    private static GUIContent[] s_LoopIcons = new GUIContent[2];
    private static MethodInfo m_audioPlayClip;
    private static MethodInfo m_audioStopAllClips;
    private static MethodInfo m_audioLoopClip;
    private static MethodInfo m_audioIsClipPlaying;
    private static AudioClip m_PlayingClip;
    private static bool playing
    {
        get
        {
            return m_PlayingClip != null;
        }
    }

    private static void Init()
    {
        if (s_PreButton != null)
        {
            return;
        }
        s_PreButton = "preButton";
        m_bAutoPlay = EditorPrefs.GetBool("AutoPlayAudio", false);
        m_bLoop = false;
        s_AutoPlayIcons[0] = EditorGUIUtility.IconContent("preAudioAutoPlayOff", "Turn Auto Play on");
        s_AutoPlayIcons[1] = EditorGUIUtility.IconContent("preAudioAutoPlayOn", "Turn Auto Play off");
        s_PlayIcons[0] = EditorGUIUtility.IconContent("preAudioPlayOff", "Play");
        s_PlayIcons[1] = EditorGUIUtility.IconContent("preAudioPlayOn", "Stop");
        s_LoopIcons[0] = EditorGUIUtility.IconContent("preAudioLoopOff", "Loop on");
        s_LoopIcons[1] = EditorGUIUtility.IconContent("preAudioLoopOn", "Loop off");

        Assembly assembly = Assembly.GetAssembly(typeof(EditorGUIUtility));
        Type type = assembly.GetType("UnityEditor.AudioUtil");
        m_audioPlayClip = type.GetMethod("PlayClip", BindingFlags.Static | BindingFlags.Public, null, 
            new Type[] {typeof(AudioClip), typeof(int), typeof(bool) }, new ParameterModifier[] { });
        m_audioStopAllClips = type.GetMethod("StopAllClips", BindingFlags.Static | BindingFlags.Public);
        m_audioLoopClip = type.GetMethod("LoopClip", BindingFlags.Static | BindingFlags.Public);
        m_audioIsClipPlaying = type.GetMethod("IsClipPlaying", BindingFlags.Static | BindingFlags.Public);
    }

    public static void Clear()
    {
        if (s_PreButton == null)
        {
            return;
        }

        if (playing)
        {
            m_audioStopAllClips.Invoke(null, null);
            m_PlayingClip = null;
        }
    }

    public static void OnGUI(Rect rect, AudioClip audioClip)
    {
        Init();
        rect.width = 24f;
        rect.height = 16f;

        if (playing && m_PlayingClip != audioClip)
        {
            m_audioStopAllClips.Invoke(null, null);
            m_PlayingClip = null;
        }
        if (!playing && m_bAutoPlay)
        {
            m_audioPlayClip.Invoke(null, new object[] { audioClip, 0, m_bLoop });
            m_PlayingClip = audioClip;
        }

        bool flag = CycleButton(rect, !playing ? 0 : 1, s_PlayIcons) != 0;
        if (flag != playing)
        {
            if (flag)
            {
                m_audioPlayClip.Invoke(null, new object[] {audioClip, 0, m_bLoop});
                m_PlayingClip = audioClip;
            }
            else
            {
                m_audioStopAllClips.Invoke(null, null);
                m_PlayingClip = null;
            }
        }

        rect.x += 24f;
        bool bLoop = m_bLoop;
        m_bLoop = CycleButton(rect, !m_bLoop ? 0 : 1, s_LoopIcons) != 0;
        if (bLoop != m_bLoop && playing)
        {
            m_audioLoopClip.Invoke(null, new object[] { audioClip, m_bLoop });
        }

        rect.x += 24f;
        m_bAutoPlay = CycleButton(rect, !m_bAutoPlay ? 0 : 1, s_AutoPlayIcons) != 0;

        if (playing && m_PlayingClip == audioClip)
        {
            bool isClipPlaying = (bool)m_audioIsClipPlaying.Invoke(null, new object[] {audioClip});
            if (!isClipPlaying)
            {
                m_PlayingClip = null;
            }
        }
    }

    private static int CycleButton(Rect rect, int selected, GUIContent[] options)
    {
        if (GUI.Button(rect, options[selected], s_PreButton))
        {
            selected++;
            if (selected >= options.Length)
            {
                selected = 0;
            }
        }
        return selected;
    }
}
