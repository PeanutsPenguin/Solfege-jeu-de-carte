using UnityEngine;
using UnityEngine.EventSystems;

public class CareEmplacementScript : MonoBehaviour, IDropHandler
{
	public ParticleSystem particle;
	private RectTransform m_RectTransform;
	[SerializeField] private E_NOTE note;

	private void Awake()
	{
		m_RectTransform = GetComponent<RectTransform>();
	}

	public void OnDrop(PointerEventData eventData)
	{
		//Quand un object est drop, le drop exactement par dessus celui ci
	   if(eventData.pointerDrag != null)
		{
			eventData.pointerDrag.GetComponent<RectTransform>().position = m_RectTransform.position;

			//Place le systeme de particule a l'endroit de l'objet
			Vector3 worldPos = Camera.main.ScreenToWorldPoint(m_RectTransform.position);
			worldPos.z = 0;
			particle.transform.position = worldPos;

			//Met la couleur du systeme de particuile en vert ou rouge en fonction du resultat
			var pfxMain = particle.main;
			if (eventData.pointerDrag.GetComponent<NoteCardScript>().getNote() == note)
			{
				pfxMain.startColor = Color.green;
			}
			else
				pfxMain.startColor = Color.red;

			//Lance l'animation du systeme de particule en fonction du resultat
			particle.Play();
		}
	}
}
