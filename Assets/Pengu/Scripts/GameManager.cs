using System.Collections;
using UnityEditor;
using UnityEngine;
using UnityEngine.Assertions.Must;
using UnityEngine.UI;
using static UnityEngine.Rendering.DebugUI;

struct Level1
{
    public bool[] m_clearNotes;
}

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    private int m_level = 1;
    private Level1 m_level1;

    public Text textLevel;
    public Text textLevelDescription;

    public GameObject level1Stocker;
    public GameObject level2Stocker;

    bool m_transitionning;

    public float transitionSpeed = 10;

    public Canvas mainCanva;

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

    void Start()
    {
        createLevel1();  
    }

    private void Update()
    {
        if (m_transitionning && !MidHandler.Instance.m_isPlaying)
        {
            switch (m_level)
            {
                case 1:
                    if (level2Stocker.transform.localPosition.x > 0)
                    {
                        float speed = Time.deltaTime * transitionSpeed * mainCanva.scaleFactor;
                        level2Stocker.transform.localPosition = new Vector3(level2Stocker.transform.localPosition.x - speed, level2Stocker.transform.localPosition.y, level2Stocker.transform.localPosition.z);
                        level1Stocker.transform.localPosition = new Vector3(level1Stocker.transform.localPosition.x - speed, level1Stocker.transform.localPosition.y, level1Stocker.transform.localPosition.z);
                    }
                    else
                    {
                        level2Stocker.transform.localPosition = new Vector3(0, 0, 0);
                        level1Stocker.SetActive(false);
                        m_transitionning = false;
                        m_level = 2;
                    }
                break;
            }
        }
    }

    public void createLevel1() 
    {
        m_level1 = new Level1();

        m_level1.m_clearNotes = new bool[7];
        for(int i = 0; i < 7; i++)
            m_level1.m_clearNotes[i] = false;

        textLevel.text = "Level 1 :";
        textLevelDescription.text = "Relis chaque note a son bon emplacement sur la portée !";
    }

    public void setValidedNote(E_NOTE note)
    {
        switch (m_level)
        {
            case 1:
                m_level1.m_clearNotes[(int)note] = true;

                for (int i = 0; i < 7; i++)
                {
                    if(!m_level1.m_clearNotes[i])
                        return;

                }

                Debug.Log("LEVEL CLEARED");
                StartCoroutine(endLevel1());
                break;
        }
    }
    IEnumerator endLevel1()
    {
        yield return new WaitUntil(() => MidHandler.Instance.getNoteOnScreencounter() <= 0);

        MidHandler.Instance.StartPlayback();
        level2Stocker.SetActive(true);
        m_transitionning = true;
    }
}
