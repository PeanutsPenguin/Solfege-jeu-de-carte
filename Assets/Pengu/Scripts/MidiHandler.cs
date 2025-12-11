using Melanchall.DryWetMidi.Core;
using Melanchall.DryWetMidi.Multimedia;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class MidiHandler : MonoBehaviour
{
    #region Champs Public
    [Tooltip("Instance du MidiHandler")]                                    public static MidiHandler Instance { get; private set; }
    [Tooltip("Canvas principal de l'application")]                          public Canvas mainCanva;
    [Tooltip("Object contenant toutes les notes de musiques")]              public GameObject musicNoteStocker;
    [Tooltip("Array de note de piano (do, re, mi, fa, sol, la, si)")]       public AudioClip[] pianoNotes;
    [Tooltip("Prefab de la note de musique")]                               public GameObject MusicNotePrefab;
    [Tooltip("Base position de chaque note de musique")]                    public GameObject[] MusicNoteBasePosition;
    [Tooltip("Indique si le playback est entrain d'etre joue")]             public bool m_isPlaying = false;
    #endregion

    #region Champs Prive
    private Dictionary<int, AudioSource> activeNotes    = new Dictionary<int, AudioSource>();           //Array de note entrain d'etre jouee
    private Queue<NoteEvent> noteEventQueue                 = new Queue<NoteEvent>();                   //Queue stockant les notes a envoyer sur le main thread
    private readonly object queueLock                       = new object();                             //Object permettant de bloquer l'ecriture de la queue
    private int m_noteOnScreen                              = 0;                                        //Note actuellement sur l'ecran
    private MidiFile midiFile;                                                                          //Resources du fichier midi
    private Playback playback;                                                                          //Resources pour la lecture du fichier midi  
    #endregion

    //Struct pour stocker les informations d'un evenement de note
    struct NoteEvent
    {
        public bool isOn;
        public int midiNote;
        public int velocity;
    }

    #region UNITY Methods
    void Awake()
    {
        //Creation du Singleton
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    void Update()
    {
        //Copie tout les evenement dans une array pour la traiter
        NoteEvent[] eventsToProcess = null;
        lock (queueLock)
        {
            if (noteEventQueue.Count > 0)
            {
                eventsToProcess = noteEventQueue.ToArray();
                noteEventQueue.Clear();
            }
        }

        //Traite chaque evenement de l'array
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
    #endregion

    #region process midi files
    public void LoadMidi(string fileName)
    {
        if(m_isPlaying)
            return;

        midiFile = MidiFile.Read(fileName);
    }
   
    /// <summary>
    /// Commence la lecture du fichier midi
    /// </summary>
    public void StartPlayback()
    {
        playback = midiFile.GetPlayback();
        playback.EventPlayed += OnMidiEvent;
        m_isPlaying = true;
        playback.Start();
    }

    /// <summary>
    /// Foction appele lors d'un evenement dans le fichier midi
    /// </summary>
    void OnMidiEvent(object sender, MidiEventPlayedEventArgs e)
    {
        //Ajoute ou eteint une note en fonction de l'evenement
        if (e.Event is NoteOnEvent noteOn && noteOn.Velocity > 0)       
            EnqueueNoteEvent(true, noteOn.NoteNumber, noteOn.Velocity);
        else if (e.Event is NoteOffEvent noteOff)                   
            EnqueueNoteEvent(false, noteOff.NoteNumber, 0);
        else if (e.Event is NoteOnEvent noteZero && noteZero.Velocity == 0)
            EnqueueNoteEvent(false, noteZero.NoteNumber, 0);
    }

    /// <summary>
    /// Ajoute une action a la queue de note
    /// </summary>
    /// <param name="isOn">Cree ou etiendre une note</param>
    /// <param name="midiNote">do, re, mi, fa, sol, la, si</param>
    /// <param name="velocity">"Volume"</param>
    void EnqueueNoteEvent(bool isOn, int midiNote, int velocity)
    {
        var ne = new NoteEvent { isOn = isOn, midiNote = ReOrderNote(midiNote), velocity = velocity };

        //Si la note est une diese, 
        if (ne.midiNote == -1)
            return;

        lock (queueLock)
            noteEventQueue.Enqueue(ne);
    }
    #endregion

    #region Note
    /// <summary>
    /// Stop durability of the note
    /// </summary>
    /// <param name="midiNote">Note</param>
    void stopNote(int midiNote)
    {
        if (activeNotes.TryGetValue(midiNote, out AudioSource src))
        {
            StartCoroutine(FadeOutAndDestroy(src));
            activeNotes.Remove(midiNote);
        }
    }

    System.Collections.IEnumerator FadeOutAndDestroy(AudioSource src)
    {
        float vol = src.volume;
        float fadeTime = 0.07f;
        float t = 0f;
        while (t < fadeTime)
        {
            src.volume = Mathf.Lerp(vol, 0f, t / fadeTime);
            t += Time.deltaTime;
            yield return null;
        }
        src.Stop();
        Destroy(src);
    }

    /// <summary>
    /// Lance une note a l'ecran
    /// </summary>
    /// <param name="note">Note</param>
    /// <param name="vel">"Volume" (mettre -1 si la note ne provient pas d'un fichier midi)</param>
    public void launchNote(E_NOTE note, float vel)
    {
        //TODO: Verifier comment est-ce possible d'avoir plusieurs fois la MEME note qui se joue en MEME temps (theorie : multithread)
        if (activeNotes.ContainsKey((int)note))
        {
            stopNote((int)note);
        }

        AudioSource src = gameObject.AddComponent<AudioSource>();
        src.clip = pianoNotes[(int)note];
        src.volume = vel / 127f;
        src.Play();
        activeNotes[(int)note] = src;

        //Instancie la note
        GameObject newNote = GameObject.Instantiate(MusicNotePrefab, musicNoteStocker.transform);

        //Place la note au bon endroit
        RectTransform rt = newNote.GetComponent<RectTransform>();
        RectTransform newrt = MusicNoteBasePosition[(int)note].GetComponent<RectTransform>();
        rt.anchorMax = newrt.anchorMax;
        rt.anchorMin = newrt.anchorMin;
        rt.localPosition = newrt.localPosition;
        rt.sizeDelta = newrt.sizeDelta;
        rt.anchoredPosition = newrt.anchoredPosition;

        //Recupere le script de la note
        MusicNoteScriptVF newNoteScript = newNote.GetComponent<MusicNoteScriptVF>();

        //Met en place les bonnes valeurs
        newNoteScript.setCanvaScale(mainCanva.scaleFactor);

        //Increment le compteur de note a l'ecran
        AddNoteOnScreenCounter();
    }

    /// <summary>
    /// Reconverti une note provenant d'un fichier midi en une note jouable (do, re, mi, fa, sol, la, si)
    /// </summary>
    /// <param name="midiNoteNumber">Note</param>
    /// <returns>La note reconverti</returns>
    int ReOrderNote(int midiNoteNumber)
    {
        //Modulo 12 pour ne pas prendre en compte l'octave
        midiNoteNumber = midiNoteNumber % 12;

        //Switch case specific afin d'eviter les dieses
        //(TODO: !nombre hardcode) 
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

    /// <summary>
    /// Incremente le compteur de note a l'ecran
    /// </summary>
    void AddNoteOnScreenCounter()
    {
        m_noteOnScreen++;
    }

    /// <summary>
    /// Decremente le compteur de note a l'ecran
    /// </summary>
    public void removeNoteOnScreenCounter()
    {
        m_noteOnScreen--;

        if(m_noteOnScreen == 0)     //Si il n'y a plus de notes a l'ecran cela signifie que le fichier a fini de jouer
            m_isPlaying = false;
    }

    /// <summary>
    /// Retourne le nombre de note a l'ecran
    /// </summary>
    public int getNoteOnScreencounter()
    { 
        return m_noteOnScreen; 
    }
    #endregion
}
