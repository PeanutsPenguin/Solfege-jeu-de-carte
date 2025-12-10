using NoteValues;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CareEmplacementScript : MonoBehaviour, IDropHandler
{
	public ParticleSystem particle;
	private RectTransform m_RectTransform;
	public Text m_name;
    [SerializeField] private Image m_backGround;
    [SerializeField] private E_NOTE note;

	public AudioClip goodsoundEffect;
	public AudioClip badsoundEffect;

	private void Awake()
	{
		m_RectTransform = GetComponent<RectTransform>();
		m_name.text = NoteValuesHandler.SetNoteText(note);
        m_backGround.color = NoteValuesHandler.setNoteColor(note);
    }

	public void OnDrop(PointerEventData eventData)
	{
		//Quand un object est drop, le drop exactement par dessus celui ci
	   if(eventData.pointerDrag != null)
		{
			eventData.pointerDrag.GetComponent<RectTransform>().position = m_RectTransform.position;
			NoteCardScript data = eventData.pointerDrag.GetComponent<NoteCardScript>();

            //Place le systeme de particule a l'endroit de l'objet
            Vector3 worldPos = Camera.main.ScreenToWorldPoint(m_RectTransform.position);
			worldPos.z = 0;
			particle.transform.position = worldPos;

			//Met la couleur du systeme de particuile en vert ou rouge en fonction du resultat
			var pfxMain = particle.main;
			if (data.getNote() == note)
			{
				data.resetvalues();
				data.draggable = false;
				pfxMain.startColor = Color.green;
				MidHandler.Instance.launchCustomNote(note);
				GameManager.Instance.setValidedNote(note);
				GetComponent<AudioSource>().PlayOneShot(goodsoundEffect);
			}
			else
			{
				pfxMain.startColor = Color.red;
                GetComponent<AudioSource>().PlayOneShot(badsoundEffect);
            }

			//Lance l'animation du systeme de particule en fonction du resultat
			particle.Play();
		}
	}
}
