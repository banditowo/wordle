using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class WordRepository : MonoBehaviour
{

    [SerializeField] [Tooltip("The text asset")]
    private TextAsset m_wordList = null;

    private List<string> m_words = null;
    // Start is called before the first frame update
    void Awake()
    {
        m_words = new List<string>(m_wordList.text.Split(new char[] { ',', ' ', '\n', '\r' },
            System.StringSplitOptions.RemoveEmptyEntries));
    }

    // Update is called once per frame
    public string GetRandomWord()
    {
        return m_words[Random.Range(0, m_words.Count)];
    }

    public bool IfWordExists(string word)
    {
        return m_words.Contains(word);
    }
}
