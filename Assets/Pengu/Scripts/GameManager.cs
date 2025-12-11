using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Struct qui definie la condition pour la fin d'un niveau 
/// </summary>
struct ClearNotes
{
    public bool[] m_clearNotes;
}

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }                                            //Instance du GameManager

    //Level
    private int         m_level                 = 1;                                                    //Niveau actuel
    private ClearNotes  m_levelValues           = new ClearNotes();                                     //Conditions de victoire du niveau
    private bool        levelCleared            = false;                                                //Est ce que le niveau a etait complete ?
    private bool        playingEndMusic         = false;                                                //Est ce que la musique de fin de niveau est en train d'etre jouee ?

    //Text
    public Text         textLevel;                                                                      //Texte du titre du niveau
    public Text         textLevelDescription;                                                           //Texte de la description du niveau

    //GameObjects
    public GameObject   noteCardStocker;                                                                //Reference a l'objet contenant toutes les noteCard
    public GameObject   cardEmplacementStocker;                                                         //Reference a l'objet contenant tout les emplacement de carte

    //Curtain
    public GameObject   curtain;                                                                        //Reference au rideau
    private bool        curtainRaised           = false;                                                //Est ce que le rideau est leve ?
    private bool        moveCurtain             = false;                                                //Est ce que le rideau doit bouger ?
    public float        curtainspeed            = 100;                                                  //Vitesse de mouvement du rideau
    public float        beginTimer              = 0.5f;                                                 //Lorce que le rideau se leve, temps qu'il met a descendre puis se leve (effet plus realiste)
    private float       beginCounter            = 0;                                                    //Depuis quand le rideau se baisse

    //Audio
    public string       level1MsuciWin          = "Assets/Pengu/Assets/Midi/BasicTetrisTheme.mid";      //Musique de fin de niveau 1
    public string       level2MsuciWin          = "Assets/Pengu/Assets/Midi/lvl2Music.mid";             //Musique de fin de niveau 1
    public string       level3MsuciWin          = "Assets/Pengu/Assets/Midi/lvl3Music.mid";             //Musique de fin de niveau 1

    private Canvas mainCanva;                                                                           //Reference au canva 


    #region UNITY METHODS
    //Creation de l'instance
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
        createLevel1();                                                                     //Creation du niveau 1
        mainCanva = GameObject.FindGameObjectWithTag("MainCanva").GetComponent<Canvas>();   //Obtention du Canva grace a un tag
    }
    public void Update()
    {
        //Si le rideau doit bouger
        if (moveCurtain)
        {
            if (!curtainRaised)
                raiseCurtain();     //Leve le rideau
            else
                downCurtain();      //Baisse le rideau
        }

        //Si le niveau est clear, qu'qucun fichier est entrain d'etre joue et qu'il n'y a plus de note a l'ecran, faire bouger le rideau
        if (levelCleared && MidiHandler.Instance.getNoteOnScreencounter() == 0 && !MidiHandler.Instance.m_isPlaying)
        {
            if (!playingEndMusic)
            {
                MidiHandler.Instance.StartPlayback();       //Joue la musique de fin de niveau
                playingEndMusic = true;
            }
            else
            {
                moveCurtain = true;
            }

        }
    }
    #endregion

    #region Levels
    void resetLevelValues() 
    {
        for (int i = 0; i < m_levelValues.m_clearNotes.Length; i++)                  //(TODO: !nombre hardcode) 
            m_levelValues.m_clearNotes[i] = false;  //Mets toutes les notes a "faux"
    }

    public void createLevel1() 
    {
        m_levelValues.m_clearNotes = new bool[7];   //Tableau de taille fixe pour chaque note (TODO : !nombre hardcode) 
        resetLevelValues();

        //Mise en place du texte
        textLevel.text = "Level un :";
        textLevelDescription.text = "Relis chaque note a son bon emplacement sur la portée !";

        moveCurtain = true;                         //Leve le rideau
        levelCleared = false;                       //Niveau fini = faux

        //Melange les notes Cards
        shuffleNoteCards();

        MidiHandler.Instance.LoadMidi(level1MsuciWin);  //Charge la musique de fin de niveau
        playingEndMusic = false;
    }

    private void createLevel2()
    {
        resetLevelValues();     //Reinitialise les valeurs du niveau

        //Mise en place du texte
        textLevel.text = "Level deux :";
        textLevelDescription.text = "Te souviens-tu de ou etait place les notes ?";

        moveCurtain = true;                         //Leve le rideau
        levelCleared = false;                       //Niveau fini = faux

        //Reecupere tout les script d'emplacements de carte depuis le stocker
        CareEmplacementScript[] arr = cardEmplacementStocker.GetComponentsInChildren<CareEmplacementScript>();

        //Cree une couleur transparente et l'associe a chaque texte des emplacements de carte afin de ne plus les voirs
        foreach (CareEmplacementScript sc in arr)
        {
            Color transparent = new Color();        
            transparent.a = 0;
            sc.m_name.color = transparent;
            sc.resetCornersParenting();
        }

        //Melange les notes Cards
        shuffleNoteCards();

        MidiHandler.Instance.LoadMidi(level2MsuciWin);  //Charge la musique de fin de niveau
        playingEndMusic = false;
    }

    private void createLevel3()
    {
        resetLevelValues();     //Reinitialise les valeurs du niveau

        //Mise en place du texte
        textLevel.text = "Level trois :";
        textLevelDescription.text = "Plus difficile encore ! As-tu bien appris ?";

        moveCurtain = true;                         //Leve le rideau
        levelCleared = false;                       //Niveau fini = faux

        //Reecupere tout les script d'emplacements de carte depuis le stocker
        CareEmplacementScript[] arr = cardEmplacementStocker.GetComponentsInChildren<CareEmplacementScript>();

        //Cree une couleur transparente et l'associe a chaque texte des emplacements de carte afin de ne plus les voirs
        foreach (CareEmplacementScript sc in arr)
        {
            Color transparent = new Color();
            transparent.a = 0;
            sc.m_name.color = transparent;
            sc.SetCornersColor(transparent);
            sc.resetCornersParenting();
        }

        //Melange les notes Cards
        shuffleNoteCards();

        MidiHandler.Instance.LoadMidi(level3MsuciWin);  //Charge la musique de fin de niveau
        playingEndMusic = false;
    }

    private void createLevel4()
    {
        resetLevelValues();     //Reinitialise les valeurs du niveau

        //Mise en place du texte
        textLevel.text = "Level quatre :";
        textLevelDescription.text = "BRAVO ! Maintenant libre a toi de jouer ce que tu veux, appuie sur les notes afin de les jouer !";

        moveCurtain = true;                         //Leve le rideau
        levelCleared = false;                       //Niveau fini = faux

        //Reecupere tout les script de noteCard depuis le stocker
        NoteCardScript[] arr2 = noteCardStocker.GetComponentsInChildren<NoteCardScript>();

        foreach (NoteCardScript sc in arr2)
            sc.playable = true;
        playingEndMusic = false;
    }

    public void setValidedNote(E_NOTE note)
    {
        switch (m_level)
        {
            case 1:
            case 2:
            case 3:
                m_levelValues.m_clearNotes[(int)note] = true;   //Valide la note

                //Verifie que toues les notes sont valides
                for (int i = 0; i < 7; i++)
                {
                    if (!m_levelValues.m_clearNotes[i])
                        return;
                }

                Debug.Log("LEVEL CLEARED");
                levelCleared = true;                        //Niveau fini
                m_level++;                                  //Niveau suivamt
                break;
        }
    }
    #endregion

    #region NoteCards
    public void shuffleNoteCards()
    {
        //Reecupere tout les script de noteCard depuis le stocker
        NoteCardScript[] arr2 = noteCardStocker.GetComponentsInChildren<NoteCardScript>();

        List<int> posList = new List<int>();

        posList.Add(-119);
        posList.Add(-19);
        posList.Add(79);
        posList.Add(179);
        posList.Add(279);
        posList.Add(379);
        posList.Add(479);

        foreach (NoteCardScript sc in arr2)
        {
            int index = Random.Range(0, posList.Count);

            sc.gameObject.GetComponent<RectTransform>().localPosition = new Vector3(posList[index], 0, 0);
            Image img = sc.gameObject.GetComponent<Image>();
            Color color = img.color;
            color.a = 100;
            img.color = color;
            sc.draggable = true;

            posList.Remove(posList[index]);
        }
    }
    #endregion

    #region Curtain
    private void raiseCurtain()
    {
        //Si le mainCanva n'est pas associe, le retrouver
        if(mainCanva == null)
            mainCanva = GameObject.FindGameObjectWithTag("MainCanva").GetComponent<Canvas>();

        beginCounter += Time.deltaTime;                                         //Incremente le temps passe a baisser le rideau

        float speed = Time.deltaTime * curtainspeed * mainCanva.scaleFactor;    //Calcule de la vitesse de deplacement du rideau

        //Monte ou descend le rideau en fonction du timer
        if (beginCounter < beginTimer) 
            curtain.transform.position = new Vector3(curtain.transform.position.x, curtain.transform.position.y - speed * 2, curtain.transform.position.z);
        else
            curtain.transform.position = new Vector3(curtain.transform.position.x, curtain.transform.position.y + speed, curtain.transform.position.z);

        //Si le rideau est entierement remonte arrete le deplacement 
        if (curtain.transform.position.y > 2030 * mainCanva.scaleFactor)//(TODO: !nombre hardcode) 
        {
            curtainRaised = true;
            moveCurtain = false;
        }
    }

    private void downCurtain()
    {
        float speed = Time.deltaTime * curtainspeed * mainCanva.scaleFactor;        //Calcule de la vitesse de deplacement du rideau

        //Descend le rideau
        curtain.transform.position = new Vector3(curtain.transform.position.x, curtain.transform.position.y - speed, curtain.transform.position.z);

        //Si le rideau est entierement descendu arrete le deplacement 
        if (curtain.transform.position.y < 413 * mainCanva.scaleFactor)//(TODO: !nombre hardcode) 
        {
            curtainRaised = false;
            moveCurtain = false;

            //Si le niveau est clear, creer le niveau suivant
            if (levelCleared) 
            {
                if (m_level == 2)
                    createLevel2();
                if(m_level == 3)
                    createLevel3();
                if (m_level == 4)
                    createLevel4();
            }
        }
    }
    #endregion
}
