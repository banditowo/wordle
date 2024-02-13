using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;


[CustomEditor(typeof(Letter))]
public class LetterEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        if (Application.isPlaying)
        {
            GUILayout.Space(20f);
            if (GUILayout.Button("Enter Letter"))
            {
                ((Letter)target).EnterLetter('C');
            }

            if (GUILayout.Button("Delete Letter"))
            {
                ((Letter)target).DeleteLetter();
            }

            if (GUILayout.Button("Shake"))
            {
                ((Letter)target).Shake();
            }
            
            if (GUILayout.Button("Correct"))
            {
                ((Letter)target).SetState(LetterState.correctPos);
            }
            
            if (GUILayout.Button("Incorrect Letter"))
            {
                ((Letter)target).SetState(LetterState.incorrectLet);
            }
            
            if (GUILayout.Button("Incorrect Position"))
            {
                ((Letter)target).SetState(LetterState.incorrectPos);
            }
        }
    }
}
