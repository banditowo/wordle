using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class RevealWord : MonoBehaviour
{
    private TMP_Text text;
    private string currentWord = null;
    public FlowManager flowManager;


    private void Start()
    {
        text = GetComponent<TMP_Text>();
    }

    public void CurrentWord()
    {
        currentWord = flowManager.GetWord();
        text.text = "The word was: " + currentWord;
        
    }

    public void ClearText()
    {
        text.text = "";
    }
    
}
