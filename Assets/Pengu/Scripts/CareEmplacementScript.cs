using NoteValues;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CareEmplacementScript : MonoBehaviour, IDropHandler
{

	public ParticleSystem particle;						//Particle system joue lors d'une bonne/mauvaise reponse
	private RectTransform m_RectTransform;				//Transform component de l'image
	public Text m_name;									//Texte ecrit dans l'emplacement de carte
	public Image m_colorImage;							//Image de couleur de l'emplacement de carte
    [SerializeField] private Image m_backGround;		//Image de fond de l'emplacement de carte
    [SerializeField] private E_NOTE note;				//Note de l'emplacemetn de carte

	public AudioClip goodsoundEffect;					//Audio joue lors d'une bonne reponse
	public AudioClip badsoundEffect;					//Audio joue lors d'une mauvaise reponse


	void Awake()
	{
		//Mise en place des valeurs
		m_RectTransform = GetComponent<RectTransform>();
		m_name.text = NoteValuesHandler.SetNoteText(note);
        m_backGround.color = NoteValuesHandler.setNoteColor(note);
    }

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
				pfxMain.startColor = Color.green;							//Mets la couleur du systeme de particule en vert
				MidiHandler.Instance.launchCustomNote(note);					//Lance la demarche pour jouer la note
				GameManager.Instance.setValidedNote(note);					//Valide la bonne reponse aupres du GameManager
				GetComponent<AudioSource>().PlayOneShot(goodsoundEffect);	//Joue l'audio de bonne reponse
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
}
