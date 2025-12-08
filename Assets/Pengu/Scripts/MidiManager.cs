using Melanchall.DryWetMidi.Core;
using Melanchall.DryWetMidi.Interaction;
using System.Collections;
using System.Linq;
using UnityEngine;

[System.Serializable]
public class MidiManager : MonoBehaviour
{
	public static MidiManager Instance { get; private set; }
	//Nom du fichier
	public string midiFileName = "Assets/Pengu/Assets/solKey.mid";

	//Varaiable utiles du fichier midi
	private MidiFile midiFile;
	private Note[] notes;
	private TempoMap tempoMap;

	//Variable pour jouer les notes
	public AudioSource audioSource;
	public AudioClip[] pianoNotes;

    /// <summary>
    /// Y coordinate spawn of the "Do" note
    /// </summary>
    public float YDoNotePosition = -185f;
	public float spaceBetweenNotePlayer = 32.5f;

	public Canvas mainCanva;
	public GameObject notePrefab;

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
		LoadMidi();
		//If you want to see a file playing 
		StartPlaying();
    }
	public void StartPlaying()
	{
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

            //Simplifie la note
            int midiNoteNumber = ReOrderNote(note.NoteNumber);

            //Joue la note
            launchNote((E_NOTE)midiNoteNumber);
		}
	}
	public void PlayPianoNote(int midiNoteNumber)
	{
		//Si la note est hors de portee(ici: != do, re, mi, fa, sol, la, si)
		if (midiNoteNumber < 0 || midiNoteNumber >= pianoNotes.Length)
		{
			Debug.Log(midiNoteNumber);
			Debug.Log("Out of range note");
			return;
		}

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
	public void LoadMidi()
	{
		//Lis le fichier et stock les valeurs
		midiFile = MidiFile.Read(midiFileName);
		tempoMap = midiFile.GetTempoMap();
		notes = midiFile.GetNotes().ToArray();

		//Tri les notes par moment auquel elle doivent etre joue
		notes = notes.OrderBy(n => n.TimeAs<MetricTimeSpan>(tempoMap).TotalSeconds).ToArray();

		Debug.Log($"[MidiReader] Loaded '{midiFileName}' with {notes.Length} notes");
	}

	public void launchNote(E_NOTE note)
	{
		Debug.Log("LaunchNote");
		GameObject noteVisual = GameObject.Instantiate(notePrefab, mainCanva.transform);
        noteVisual.transform.localPosition = Vector3.zero;
        noteVisual.transform.localScale = Vector3.one;

		float Ypos = YDoNotePosition + (spaceBetweenNotePlayer * (int)note);
        noteVisual.GetComponent<RectTransform>().anchoredPosition = new Vector3(500, Ypos, 0);
		noteVisual.GetComponent<MusicNoteScript>().note = note;
        //noteVisual.GetComponent<MusicNoteScript>().canvas = mainCanva;
    }

    #region GETTER 
    public Note[] GetNotes()
	{
		return notes;
	}
	public TempoMap GetTempoMap()
	{
		return tempoMap;
	}
	public MetricTimeSpan GetNoteStartTime(Note note)
	{
		return note.TimeAs<MetricTimeSpan>(tempoMap);
	}
	public MetricTimeSpan GetNoteLength(Note note)
	{
		return note.LengthAs<MetricTimeSpan>(tempoMap);
	}
	#endregion
}
