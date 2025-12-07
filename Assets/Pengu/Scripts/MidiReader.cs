using Melanchall.DryWetMidi.Core;
using Melanchall.DryWetMidi.Interaction;
using System.Linq;
using UnityEngine;

public class MidiReader : MonoBehaviour
{
	public static MidiReader Instance { get; private set; }

	//Nom du fichier
	public string midiFileName = "Assets/Pengu/Assets/win.mid";

	//Varaiable utiles du fichier midi
	private MidiFile midiFile;
	private Note[] notes;
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

	void Start()
	{
		LoadMidi();
	}

	public void LoadMidi()
	{
		//Lis le fichier et stock les valeurs
		midiFile = MidiFile.Read(midiFileName);
		tempoMap = midiFile.GetTempoMap();
		notes = midiFile.GetNotes().ToArray();

		Debug.Log($"[MidiReader] Loaded '{midiFileName}' with {notes.Length} notes");
	}
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

}
