using NoteValues;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CareEmplacementScript : MonoBehaviour, IDropHandler
{
    #region Champs publique
    [Tooltip("Texte ecrit dans l'emplacement de carte")]					public Text m_name;
	[Tooltip("Particle system joue lors d'une bonne/mauvaise reponse")]		public ParticleSystem particle;                     
	[Tooltip("Audio joue lors d'une bonne reponse")]						public AudioClip goodsoundEffect;                   
	[Tooltip("Audio joue lors d'une mauvaise reponse")]						public AudioClip badsoundEffect;
    #endregion

    #region Champs serialize
    [Tooltip("Image de coin de l'emplacement de carte")][SerializeField]	private Image m_toprightCorner;
    [Tooltip("Image de coin de l'emplacement de carte")][SerializeField]	private Image m_bottomLeftCorner;
    [Tooltip("Note de l'emplacemetn de carte")][SerializeField]				private E_NOTE note;               
    #endregion

    #region Champ prive
    private RectTransform m_RectTransform;              //Transform component de l'image
    #endregion

    #region UNITY Methods
    void Awake()
	{
		//Mise en place des valeurs
		m_RectTransform = GetComponent<RectTransform>();
		m_name.text = NoteValuesHandler.SetNoteText(note);
        SetCornersColor(NoteValuesHandler.setNoteColor(note));
	}
    #endregion

    #region OnDrop
    public void OnDrop(PointerEventData eventData)
	{
		//Verifie que l'objet depose n'est pas nul
	   if(eventData.pointerDrag != null)
		{
			//Place l'objet au centre de celui-ci
			eventData.pointerDrag.GetComponent<RectTransform>().position = m_RectTransform.position;

			//Recupere le script de la noteCard
			NoteCardScript data = eventData.pointerDrag.GetComponent<NoteCardScript>();

			//Place le systeme de particule a l'endroit de l'objet
			Vector3 worldPos = Camera.main.ScreenToWorldPoint(m_RectTransform.position);
			worldPos.z = 0;
			particle.transform.position = worldPos;

			//Met la couleur du systeme de particuile en vert ou rouge en fonction du resultat
			var pfxMain = particle.main;

			//Si la note depose est egal a la note de l'emplacement
			if (data.getNote() == note)
			{
				data.resetvalues();											//Remet en place les valeurs de la noteCard
				data.draggable = false;										//Desactive le systeme de drag de la noteCard
				m_toprightCorner.transform.SetParent(data.transform, false);
				m_bottomLeftCorner.transform.SetParent(data.transform, false);
				pfxMain.startColor = Color.green;							//Mets la couleur du systeme de particule en vert
				MidiHandler.Instance.launchNote(note, 100);					//Lance la demarche pour jouer la note
				GameManager.Instance.setValidedNote(note);					//Valide la bonne reponse aupres du GameManager
				//GetComponent<AudioSource>().PlayOneShot(goodsoundEffect);	//Joue l'audio de bonne reponse
			}
			else
			{
				pfxMain.startColor = Color.red;								//Mets la couleur du systeme de particule en rouge
				GetComponent<AudioSource>().PlayOneShot(badsoundEffect);	//Joue l'audio de mauvaise reponse
			}

			//Lance l'animation du systeme de particule
			particle.Play();
		}
	}
	#endregion

	#region Corners
	public void SetCornersColor(Color color)
	{
        m_toprightCorner.color = color;
        m_bottomLeftCorner.color = color;
    }

	public void resetCornersParenting()
	{
        m_toprightCorner.transform.SetParent(gameObject.transform, false);
        m_bottomLeftCorner.transform.SetParent(gameObject.transform, false);
    }
	#endregion
}
