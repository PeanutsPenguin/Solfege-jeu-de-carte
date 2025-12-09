using UnityEditor.Searcher;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CareEmplacementScript : MonoBehaviour, IDropHandler
{
	public ParticleSystem particle;
	private RectTransform m_RectTransform;
	[SerializeField] Text m_name;
	[SerializeField] private E_NOTE note;

	public AudioClip goodsoundEffect;
	public AudioClip badsoundEffect;

	private void Awake()
	{
		m_RectTransform = GetComponent<RectTransform>();
		setText();
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

	void setText()
	{
		switch (note)
		{
			case E_NOTE.E_DO:
				m_name.text = "DO";
				break;
            case E_NOTE.E_RE:
                m_name.text = "RE";
                break;
            case E_NOTE.E_MI:
                m_name.text = "MI";
                break;
            case E_NOTE.E_FA:
                m_name.text = "FA";
                break;
            case E_NOTE.E_SOL:
                m_name.text = "SOL";
                break;
            case E_NOTE.E_LA:
                m_name.text = "LA";
                break;
            case E_NOTE.E_SI:
                m_name.text = "SI";
                break;
        }
	}
}
