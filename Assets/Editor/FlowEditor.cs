using System;
using System.Collections;
using System.Collections.Generic;
using Codice.CM.Common;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(FlowManager))]
public class FlowEditor : Editor
{
    private string m_spoiler = null;
    private FlowManager m_manager = null;

    private void OnEnable()
    {
        m_manager = (FlowManager)target;
        m_manager.Restarted += OnRestarted;
    }

    private void OnDisable()
    {
        m_manager.Restarted -= OnRestarted;
        m_manager = null;
    }

    void OnRestarted()
    {
        m_spoiler = null;
        Repaint();
    }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        if (Application.isPlaying)
        {
            GUILayout.Space(20f);

            if (string.IsNullOrEmpty(m_spoiler))
            {
                if (GUILayout.Button(("Spoiler")))
                    m_spoiler = ((FlowManager)target).GetWord();
            }
            else
            {
                GUILayout.Label(m_spoiler, EditorStyles.boldLabel);
            }
        }
    }
}
