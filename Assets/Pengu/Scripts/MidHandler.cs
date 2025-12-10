using Melanchall.DryWetMidi.Core;
using Melanchall.DryWetMidi.Interaction;
using Melanchall.DryWetMidi.Multimedia;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using UnityEngine;

public class MidHandler : MonoBehaviour
{
    public static MidHandler Instance { get; private set; }

    public Canvas mainCanva;

    //Nom du fichier
    public string midiFileName = "Assets/Pengu/Assets/solKey.mid";

    //Varaiable utiles du fichier midi
    private MidiFile midiFile;
    private Playback playback;
    private Note[] notes;
    private TempoMap tempoMap;

    public AudioClip[] pianoNotes;

    private Dictionary<int, MusicNoteScript> activeNotes = new Dictionary<int, MusicNoteScript>();

    // queue pour transmettre les events du thread DryWetMidi vers le main thread
    private Queue<NoteEvent> noteEventQueue = new Queue<NoteEvent>();
    private readonly object queueLock = new object();

    private int m_noteOnScreen = 0;
    public bool m_isPlaying = false;

    #region Apparition de note
    public GameObject MusicNotePrefab;                  //Prefab de la note
    public float YDoNotePosition = -185f;               //Coordonee Y de la note "do" (la note la plus basse)
    public float spaceBetweenNotePlayer = 32.5f;        //Espace entre chaque note 
    #endregion
    private struct NoteEvent
    {
        public bool isOn;
        public int midiNote;
        public int velocity;
    }
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
    public void Start()
    {
        LoadMidi();
        //StartPlayback();
    }
    void Update()
    {
        // Déqueue et traite tous les événements en attente
        NoteEvent[] eventsToProcess = null;
        lock (queueLock)
        {
            if (noteEventQueue.Count > 0)
            {
                eventsToProcess = noteEventQueue.ToArray();
                noteEventQueue.Clear();
            }
        }

        if (eventsToProcess != null)
        {
            foreach (var ne in eventsToProcess)
            {
                if (ne.isOn)
                    launchNote((E_NOTE)ne.midiNote, ne.velocity);
                else
                    stopNote(ne.midiNote);
            }
        }
    }
    void OnDestroy()
    {
        if (playback != null)
        {
            playback.EventPlayed -= OnMidiEvent;
            playback.Dispose();
        }
    }
    void LoadMidi()
    {
        midiFile = MidiFile.Read(midiFileName);

        tempoMap = midiFile.GetTempoMap();
        notes = midiFile.GetNotes().ToArray();

        Debug.Log($"[MidiReader] Loaded '{midiFileName}' with {notes.Length} notes");
    }
    public void StartPlayback()
    {
        playback = midiFile.GetPlayback();
        playback.EventPlayed += OnMidiEvent;
        m_isPlaying = true;
        playback.Start();
    }
    void OnMidiEvent(object sender, MidiEventPlayedEventArgs e)
    {
        if (e.Event is NoteOnEvent noteOn && noteOn.Velocity > 0)
        {
            EnqueueNoteEvent(true, noteOn.NoteNumber, noteOn.Velocity);
        }
        else if (e.Event is NoteOffEvent noteOff)
        {
            EnqueueNoteEvent(false, noteOff.NoteNumber, 0);
        }
        else if (e.Event is NoteOnEvent noteZero && noteZero.Velocity == 0)
        {
            // NoteOn with velocity 0 is NoteOff
            EnqueueNoteEvent(false, noteZero.NoteNumber, 0);
        }
    }
    void EnqueueNoteEvent(bool isOn, int midiNote, int velocity)
    {
        var ne = new NoteEvent { isOn = isOn, midiNote = ReOrderNote(midiNote), velocity = velocity };
        lock (queueLock)
        {
            noteEventQueue.Enqueue(ne);
        }
    }
    void stopNote(int midiNote)
    {
        if (activeNotes.TryGetValue(midiNote, out MusicNoteScript src))
        {
            src.stopDurability();
            activeNotes.Remove(midiNote);
        }
    }
    private int ReOrderNote(int midiNoteNumber)
    {
        //Modulo 12 pour ne pas prendre en compte l'octave
        midiNoteNumber = midiNoteNumber % 12;

        //Switch case specific afin d'eviter les dieses
        //Harcoded values : mauvais. TODO : trouver un algorithm permettant de traduire la valeur
        switch (midiNoteNumber)
        {
            case 1:
            case 2:
                midiNoteNumber--;
                break;
            case 3:
                midiNoteNumber = 1;
                break;
            case 4:
                midiNoteNumber = 2;
                break;
            case 5:
            case 6:
                midiNoteNumber = 3;
                break;
            case 7:
            case 8:
                midiNoteNumber = 4;
                break;
            case 9:
            case 10:
                midiNoteNumber = 5;
                break;
            case 11:
                midiNoteNumber = 6;
                break;
        }

        return midiNoteNumber;
    }
    void launchNote(E_NOTE note, float vel)
    {
        Debug.Log("LaunchNote");
        //Instantie la note et reset sa position
        GameObject newNote = GameObject.Instantiate(MusicNotePrefab, mainCanva.transform);
        newNote.transform.localPosition = Vector3.zero;
        newNote.transform.localScale = Vector3.one;

        //Place la note au bon endroit
        float Ypos = YDoNotePosition + (spaceBetweenNotePlayer * (int)note);
        newNote.GetComponent<RectTransform>().anchoredPosition = new Vector3(1000, Ypos, 0);

        //Met les bonnes valeurs
        newNote.GetComponent<MusicNoteScript>().note = note;
        MusicNoteScript newNoteScript = newNote.GetComponent<MusicNoteScript>();
        newNoteScript.canvaScale = mainCanva.scaleFactor;
        newNoteScript.m_AudioSource.clip = pianoNotes[(int)note];
        newNoteScript.m_AudioSource.volume = vel / 127f;
        activeNotes[(int)note] = newNoteScript;
        AddNoteOnScreenCounter();
    }
    public void launchCustomNote(E_NOTE note)
    {
        GameObject newNote = GameObject.Instantiate(MusicNotePrefab, mainCanva.transform);
        newNote.transform.localPosition = Vector3.zero;
        newNote.transform.localScale = Vector3.one;

        //Place la note au bon endroit
        float Ypos = YDoNotePosition + (spaceBetweenNotePlayer * (int)note);
        newNote.GetComponent<RectTransform>().anchoredPosition = new Vector3(1000, Ypos, 0);

        //Met les bonnes valeurs
        newNote.GetComponent<MusicNoteScript>().note = note;
        MusicNoteScript newNoteScript = newNote.GetComponent<MusicNoteScript>();
        newNoteScript.canvaScale = mainCanva.scaleFactor;
        newNoteScript.m_AudioSource.clip = pianoNotes[(int)note];
        newNoteScript.durability = 1f;
        newNoteScript.stopDurability();
        AddNoteOnScreenCounter();
    }

    public void AddNoteOnScreenCounter()
    {
        m_noteOnScreen++;
    }

    public void removeNoteOnScreenCounter()
    {
        m_noteOnScreen--;
        Debug.Log(m_noteOnScreen);
        if(m_noteOnScreen == 0)
            m_isPlaying = false;
    }

    public int getNoteOnScreencounter()
    { 
        return m_noteOnScreen; 
    }
}
