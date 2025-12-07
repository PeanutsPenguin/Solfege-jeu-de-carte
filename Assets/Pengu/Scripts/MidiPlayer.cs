using Melanchall.DryWetMidi.Interaction;
using System.Collections;
using System.Linq;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

//Le midi player est simplifie afin de jouer uniquement les notes ABCDEF et sur le meme octave.
//Un scale tres simple de cette clase afin de pouvoir jouer toutes les notes est possible 
public class MidiPlayer : MonoBehaviour
{
    public static MidiPlayer Instance { get; private set; }
    public AudioSource audioSource;

    /// <summary>
    /// Array de note de piano contenant 7 note
    /// (le but de ce jeu est d'apprendre la base des bases du solfege donc pas de complication)
    /// </summary>
    public AudioClip[] pianoNotes;

    /// <summary>
    /// Array d'image pour afficher les 7 notes
    /// </summary>
    public Sprite[] notesImages;

    /// <summary>
    /// Array des notes du fichier midi
    /// </summary>
    private Note[] notes;

    /// <summary>
    /// Tempo map du fichier midi
    /// </summary>
    private TempoMap tempoMap;

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

    public void StartPlaying()
    {
        //Prends les notes et la tempoMap du fichier midi
        notes = MidiReader.Instance.GetNotes();
        tempoMap = MidiReader.Instance.GetTempoMap();

        //Tri les notes par moment auquel elle doivent etre joue
        notes = notes.OrderBy(n => n.TimeAs<MetricTimeSpan>(tempoMap).TotalSeconds).ToArray();

        StartCoroutine(PlayMidi());
    }

    IEnumerator PlayMidi()
    {
        //Prends le temps de l'audio system
        double startDSP = AudioSettings.dspTime;

        //Pour chaque note dans le fichier midi
        foreach (var note in notes)
        {
            //Prends le temps auquel la note doit etre joue
            double noteStart = note.TimeAs<MetricTimeSpan>(tempoMap).TotalSeconds;

            //Attends d'atteindre la bon moment avant de jouer la note
            while (AudioSettings.dspTime < startDSP + noteStart)
                yield return null;

            //Joue la note
            PlayPianoNote(note.NoteNumber);
        }
    }

    void PlayPianoNote(int midiNoteNumber)
    {
        //Simplifie la note
        midiNoteNumber = ReOrderNote(midiNoteNumber);

        //Si la note est hors de portee (ici : != do, re, mi, fa, sol, la, si)
        if (midiNoteNumber < 0 || midiNoteNumber >= pianoNotes.Length)
        {
            Debug.Log(midiNoteNumber);
            Debug.Log("Out of range note");
            return;
        }

        //Arrete l'audio d'avant pour eviter le surplus
        audioSource.Stop(); 

        //Joue la note de piano associer a la note envoyer par le fichier midi
        audioSource.PlayOneShot(pianoNotes[midiNoteNumber]);
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
}
