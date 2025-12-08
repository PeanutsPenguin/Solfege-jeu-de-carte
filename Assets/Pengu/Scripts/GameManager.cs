using UnityEditor;
using UnityEngine;
using UnityEngine.Assertions.Must;

struct Level1
{
    public bool[] m_clearNotes;
}

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    private int m_level;

    private Level1 m_level1;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public void createLevel1() 
    {
        m_level1.m_clearNotes = new bool[7];
        for(int i = 0; i < 7; i++)
            m_level1.m_clearNotes[i] = false;
    }

    public void setValidedNote(E_NOTE note)
    {
        switch (m_level)
        {
            case 1:
                m_level1.m_clearNotes[(int)note] = true;

                for (int i = 0; i < 7; i++)
                    if(!m_level1.m_clearNotes[i])
                        break;

                Debug.Log("LEVEL CLEARED");
                //LevelCleared !!!
                break;
        }
    }
}
