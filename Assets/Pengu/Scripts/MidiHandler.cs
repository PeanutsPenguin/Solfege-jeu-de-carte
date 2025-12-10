using Melanchall.DryWetMidi.Core;
using Melanchall.DryWetMidi.Interaction;
using Melanchall.DryWetMidi.Multimedia;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public class MidiHandler : MonoBehaviour
{
    [Tooltip("Instance du MidiHandler")] public static MidiHandler Instance { get; private set; }

    [Tooltip("Canvas principal de l'application")] public Canvas mainCanva;
    [Tooltip("Canvas principal de l'application")] public GameObject musicNoteStocker;

    private MidiFile midiFile;      //Resources du fichier midi
    private Playback playback;      //Resources pour la lecture du fichier midi    

    [Tooltip("Array de note de piano (do, re, mi, fa, sol, la, si)")] public AudioClip[] pianoNotes;

    private Dictionary<int, MusicNoteScript> activeNotes = new Dictionary<int, MusicNoteScript>();      //Array de note entrain d'etre jouee

    //Queue pour transmettre les note a joue du thread DryWetMidi vers le main thread
    private Queue<NoteEvent> noteEventQueue = new Queue<NoteEvent>();
    private readonly object queueLock = new object();

    private int m_noteOnScreen = 0;     //Note actuellement sur l'ecran
    [Tooltip("Indique si le playback est entrain d'etre joue")] public bool m_isPlaying = false;

    #region Apparition de note
    [Tooltip("Prefab de la note de musique")] public GameObject MusicNotePrefab;
    [Tooltip("Base position de chaque note de musique")] public GameObject[] MusicNoteBasePosition;
    #endregion

    //Struct pour stocker les informations d'un evenement de note
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
        //LoadMidi();
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
    public void LoadMidi(string fileName)
    {
        if(m_isPlaying)
            return;

        midiFile = MidiFile.Read(fileName);
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

        //Si la note est une diese, ByPass
        if (ne.midiNote == -1)
            return;

        lock (queueLock)
        {
            noteEventQueue.Enqueue(ne);
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
    void launchNote(E_NOTE note, float vel)
    {
        Debug.Log(mainCanva.scaleFactor);

        //TODO: Verifier comment est-ce possible d'avoir plusieurs fois la MEME note qui se joue en MEME temps (theorie : multithread)
        if (activeNotes.ContainsKey((int)note))
            return;


        //Instantie la note et reset sa position
        GameObject newNote = GameObject.Instantiate(MusicNotePrefab, musicNoteStocker.transform);
        //newNote.transform.localPosition = Vector3.zero;
        //newNote.transform.localScale = Vector3.one;

        //Place la note au bon endroit
        RectTransform rt = newNote.GetComponent<RectTransform>();
        RectTransform newrt = MusicNoteBasePosition[(int)note].GetComponent<RectTransform>();

        rt.anchorMax = newrt.anchorMax;
        rt.anchorMin = newrt.anchorMin;
        rt.localPosition = newrt.localPosition;
        rt.sizeDelta = newrt.sizeDelta;
        rt.anchoredPosition = newrt.anchoredPosition;

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
        Debug.Log(mainCanva.scaleFactor);

        GameObject newNote = GameObject.Instantiate(MusicNotePrefab, musicNoteStocker.transform);
        //newNote.transform.localPosition = Vector3.zero;
        //newNote.transform.localScale = Vector3.one;

        //Place la note au bon endroit
        RectTransform rt = newNote.GetComponent<RectTransform>();
        RectTransform newrt = MusicNoteBasePosition[(int)note].GetComponent<RectTransform>();

        rt.anchorMax = newrt.anchorMax;
        rt.anchorMin = newrt.anchorMin;
        rt.localPosition = newrt.localPosition;
        rt.sizeDelta = newrt.sizeDelta;
        rt.anchoredPosition = newrt.anchoredPosition;

        //Met les bonnes valeurs
        newNote.GetComponent<MusicNoteScript>().note = note;
        MusicNoteScript newNoteScript = newNote.GetComponent<MusicNoteScript>();
        newNoteScript.canvaScale = mainCanva.scaleFactor;
        newNoteScript.m_AudioSource.clip = pianoNotes[(int)note];
        newNoteScript.durability = 1f;
        newNoteScript.stopDurability();
        AddNoteOnScreenCounter();
    }

    private int ReOrderNote(int midiNoteNumber)
    {
        //Modulo 12 pour ne pas prendre en compte l'octave
        midiNoteNumber = midiNoteNumber % 12;

        //Switch case specific afin d'eviter les dieses
        //Harcoded values : mauvais. TODO : trouver un algorithm permettant de traduire la valeur
        switch (midiNoteNumber)
        {
            case 2:
                midiNoteNumber--;
                break;
            case 4:
                midiNoteNumber = 2;
                break;
            case 5:
                midiNoteNumber = 3;
                break;
            case 7:
                midiNoteNumber = 4;
                break;
            case 9:
                midiNoteNumber = 5;
                break;
            case 11:
                midiNoteNumber = 6;
                break;

            //ByPass les dieses
            case 10:
            case 8:
            case 6:
            case 3:
            case 1:
                midiNoteNumber = -1;
                break;
        }

        return midiNoteNumber;
    }

    public void AddNoteOnScreenCounter()
    {
        m_noteOnScreen++;
    }

    public void removeNoteOnScreenCounter()
    {
        m_noteOnScreen--;
        if(m_noteOnScreen == 0)
            m_isPlaying = false;
    }

    public int getNoteOnScreencounter()
    { 
        return m_noteOnScreen; 
    }
}
