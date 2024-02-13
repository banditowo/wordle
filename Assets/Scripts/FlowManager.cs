using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;

public class FlowManager : MonoBehaviour
{
    private const int k_wordLength = 5;

    [SerializeField] [Tooltip("Word repository")]
    private WordRepository m_wordRepository = null;
    
    [SerializeField] [Tooltip("Prefab for letter")]
    Letter m_letterPrefab = null;

    [SerializeField] [Tooltip("Amount of rows")]
    private int m_amountOfRows = 5;

    [SerializeField] [Tooltip("Grid Parent")]
    private GridLayoutGroup m_gridLayout = null;

    [SerializeField] [Tooltip("Offset for letter anim")]
    private float m_letterAnimOffsetTime = 0.5f;

    [SerializeField] [Tooltip("Keys on keyboard")]
    private Key[] m_keys = null;

    private List<Letter> m_letters = null;
    private int m_index = 0;
    private int m_currentRow = 0;
    private char?[] m_guess = new char?[k_wordLength];
    private char[] m_word = new char[k_wordLength];


    public PuzzleState PuzzleState { get; private set; } = PuzzleState.InProgress;
    public Action Restarted;

    public RevealWord revealWord;

    private void Awake()
    {
        SetUpGrid();

        foreach (Key key in m_keys)
            key.Pressed += OnKeyPressed;
    }

    void Start()
    {
        SetWord();
    }

    void Update()
    {
        if (Input.anyKeyDown)
           ParseInput(Input.inputString);
    }

    void Restart()
    {
        StopAllCoroutines();
        
        PuzzleState = PuzzleState.InProgress;

        foreach (Letter letter in m_letters)
            letter.Clear();

        foreach (Key key in m_keys)
            key.ResetState();

        m_index = 0;
        m_currentRow = 0;

        for (int i = 0; i < k_wordLength; i++)
            m_guess[i] = null;
        
        SetWord();
        revealWord.ClearText();
        Restarted?.Invoke();

    }

    void OnKeyPressed(KeyCode keyCode)
    {
        if (PuzzleState != PuzzleState.InProgress)
        {
            if(keyCode == KeyCode.Return)
                Restart();

            return;
        }

        if (keyCode == KeyCode.Return)
        {
            GuessWord();
        }
        else if (keyCode == KeyCode.Backspace || keyCode == KeyCode.Delete)
        {
            DeleteLetter();
        }
        else if (keyCode >= KeyCode.A && keyCode <= KeyCode.Z)
        {
            int index = keyCode - KeyCode.A;
            EnterLetter((char)((int)'A' + index));
        }
    }

    public void ParseInput(string value)
    {
        if (PuzzleState != PuzzleState.InProgress)
        {
            foreach (char c in value)
            {
                if ((c == '\n') || (c == '\r'))
                {
                    Restart();
                    return;
                }
            }

            return;
        }

        foreach(char c in value)
        {
            if (c == '\b') //backspace
            {
                DeleteLetter();
            }
            else if ((c == '\n') || (c == '\r')) //enter or return
            {
                GuessWord();
            }
            else
            {
                EnterLetter(c);
            }
                  
        }
        
    }
    
    public void SetUpGrid()
    {
        if (m_letters == null)
            m_letters = new List<Letter>();

        for (int i = 0; i < m_amountOfRows; i++)
        {
            for (int j = 0; j < k_wordLength; j++)
            {
                Letter letter = Instantiate<Letter>(m_letterPrefab);
                letter.transform.SetParent(m_gridLayout.transform);
                m_letters.Add(letter);
            }
        }
    }

    public void EnterLetter(char c)
    {
        if (m_index < k_wordLength)
        {
            c = char.ToUpper(c);
            
            m_letters[(m_currentRow * k_wordLength) + m_index].EnterLetter(c);
            m_guess[m_index] = c;
            m_index++;
        }
    }

    public void DeleteLetter()
    {
        if (m_index > 0)
        {
            m_index--;
            m_letters[(m_currentRow * k_wordLength) + m_index].DeleteLetter();
            m_guess[m_index] = null;
        }
    }

    void Shake()
    {
        for (int i = 0; i < k_wordLength; i++)
        {
            m_letters[(m_currentRow * k_wordLength) + i].Shake();
        }
    }

    void SetWord()
    {
        string word = m_wordRepository.GetRandomWord();
        
        for (int i = 0; i < word.Length; i++)
        {
            m_word[i] = word[i];
        }
    }

    public string GetWord()
    {
        return new string(m_word);
    }

    public void GuessWord()
    {
        if (m_index != k_wordLength)
        {
            Shake();
        }
        else
        {
            StringBuilder word = new StringBuilder();
            
            for (int i = 0; i < k_wordLength; i++)
                word.Append(m_guess[i].Value);
            
            if(m_wordRepository.IfWordExists(word.ToString()))
            {
                bool incorrect = false;

                for (int i = 0; i < k_wordLength; i++)
                {
                    bool correct = m_guess[i] == m_word[i];

                    if (!correct)
                    {
                        incorrect = true;

                        bool letterExistsInWord = false;
                        
                        for (int j = 0; j < k_wordLength; j++)
                        {
                            letterExistsInWord = m_guess[i] == m_word[j];
                            if (letterExistsInWord)
                                break;
                        }

                        StartCoroutine(PlayLetter(i * m_letterAnimOffsetTime, (m_currentRow * k_wordLength) + i,
                            letterExistsInWord ? LetterState.incorrectPos : LetterState.incorrectLet));
                    }
                    else
                    {
                        StartCoroutine(PlayLetter(i * m_letterAnimOffsetTime, (m_currentRow * k_wordLength) + i,
                            LetterState.correctPos));
                    }
                }

                if (incorrect)
                {
                    m_index = 0;
                    m_currentRow++;
                    if (m_currentRow >= m_amountOfRows)
                    {
                        PuzzleState = PuzzleState.Failed;
                        revealWord.CurrentWord();
                    }
                }
                else
                {
                    PuzzleState = PuzzleState.Complete;
                }
            }
        }
    }

    IEnumerator PlayLetter(float offset, int index, LetterState letterState)
    {
        yield return new WaitForSeconds(offset);
        m_letters[index].SetState(letterState);


        int indexOfChar = (int)m_letters[index].Entry.Value - (int)'A';
        KeyCode keyCode = indexOfChar + KeyCode.A;
        
        foreach (Key key in m_keys)
        {
            if(key.KeyCode == keyCode)
            {
                key.SetState(letterState);
                break;
            }
        }
    }
}