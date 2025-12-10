using UnityEngine;
using UnityEngine.UI;

struct ClearNotes
{
    public bool[] m_clearNotes;
}

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    private int m_level = 1;
    private ClearNotes m_levelValues;
    private bool levelCleared = false;

    public Text textLevel;
    public Text textLevelDescription;

    private Canvas mainCanva;

    public GameObject noteCardStocker;
    public GameObject cardEmplacementStocker;

    public GameObject curtain;
    private bool curtainRaised = false;
    private bool moveCurtain = false;
    public float curtainspeed = 100;
    public float beginTimer = 0.5f;
    private float beginCounter = 0;
    //public AudioClip curtainSound;

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
        mainCanva = GameObject.FindGameObjectWithTag("MainCanva").GetComponent<Canvas>();
    }

    public void createLevel1() 
    {
        m_levelValues = new ClearNotes();

        m_levelValues.m_clearNotes = new bool[7];
        for(int i = 0; i < 7; i++)
            m_levelValues.m_clearNotes[i] = false;

        textLevel.text = "Level 1 :";
        textLevelDescription.text = "Relis chaque note a son bon emplacement sur la portée !";
        moveCurtain = true;
        //GetComponent<AudioSource>().clip = curtainSound;
        //GetComponent<AudioSource>().Play();
    }

    private void createLevel2()
    {
        m_levelValues = new ClearNotes();

        m_levelValues.m_clearNotes = new bool[7];
        for (int i = 0; i < 7; i++)
            m_levelValues.m_clearNotes[i] = false;

        textLevel.text = "Level 2 :";
        textLevelDescription.text = "Te souviens-tu de ou etait place les notes ?";
        moveCurtain = true;
        levelCleared = false;

        CareEmplacementScript[] arr = cardEmplacementStocker.GetComponentsInChildren<CareEmplacementScript>();

        foreach (CareEmplacementScript sc in arr)
        {
            Color transparent = new Color();
            transparent.a = 0;
           sc.m_name.color = transparent;
        }

        NoteCardScript[] arr2 = noteCardStocker.GetComponentsInChildren<NoteCardScript>();

        foreach (NoteCardScript sc in arr2)
        {
            int posX = Random.Range(-350, 350);
            int posY = Random.Range(-100, 100);

            sc.gameObject.GetComponent<RectTransform>().localPosition = new Vector3(posX, posY, 0);
            sc.draggable = true;
        }
    }

    public void Update()
    {
        if (moveCurtain)
        {
            if (!curtainRaised)
                raiseCurtain();
            else 
                downCurtain();
        }

        if(levelCleared && MidHandler.Instance.getNoteOnScreencounter() == 0 && !MidHandler.Instance.m_isPlaying)
            moveCurtain = true;
    }

    public void setValidedNote(E_NOTE note)
    {
        switch (m_level)
        {
            case 1:
                m_levelValues.m_clearNotes[(int)note] = true;

                for (int i = 0; i < 7; i++)
                {
                    if(!m_levelValues.m_clearNotes[i])
                        return;

                }

                Debug.Log("LEVEL CLEARED");
                levelCleared = true;
                m_level++;
                MidHandler.Instance.StartPlayback();
                break;
        }
    }

    private void raiseCurtain()
    {
        if(mainCanva == null)
            mainCanva = GameObject.FindGameObjectWithTag("MainCanva").GetComponent<Canvas>();

        beginCounter += Time.deltaTime;
        float speed = Time.deltaTime * curtainspeed * mainCanva.scaleFactor;

        if (beginCounter < beginTimer) 
            curtain.transform.position = new Vector3(curtain.transform.position.x, curtain.transform.position.y - speed * 2, curtain.transform.position.z);
        else
            curtain.transform.position = new Vector3(curtain.transform.position.x, curtain.transform.position.y + speed, curtain.transform.position.z);

        if (curtain.transform.position.y > 2030)
        {
            curtainRaised = true;
            moveCurtain = false;
        }

        Debug.Log(curtain.transform.position.y);
    }

    private void downCurtain()
    {
        Debug.Log(curtain.transform.position.y);
        float speed = Time.deltaTime * curtainspeed * mainCanva.scaleFactor;

        curtain.transform.position = new Vector3(curtain.transform.position.x, curtain.transform.position.y - speed, curtain.transform.position.z);

        if (curtain.transform.position.y < 413)
        {
            curtainRaised = false;
            moveCurtain = false;

            if (levelCleared) 
            {
                if (m_level == 2)
                    createLevel2();
            }
        }
    }
}
