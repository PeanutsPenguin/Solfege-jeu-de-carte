using NoteValues;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

//Utilisation des evenements deja creer par unity afin de gerer le Drag and Drop des carte notes
public class NoteCardScript : MonoBehaviour, IPointerDownHandler, IBeginDragHandler, IEndDragHandler, IDragHandler
{
	private RectTransform m_RectTransform;
	private CanvasGroup m_CanvasGroup;
    public Text m_name;
    [SerializeField] private Canvas m_Canvas;
	[SerializeField] private E_NOTE note;

	public bool draggable = true;

	private void Awake()
	{
		m_RectTransform = GetComponent<RectTransform>();
		m_CanvasGroup = GetComponent<CanvasGroup>();
		m_name.text = NoteValuesHandler.SetNoteText(note);
		GetComponent<Image>().color = NoteValuesHandler.setNoteColor(note);
	}

	//Au commencement du drag
	public void OnBeginDrag(PointerEventData eventData)
	{
		if(!draggable)
			return;

		//Desactive les evenement sur l'objets et le rend plus opaque 
		m_CanvasGroup.alpha = .5f;
		m_CanvasGroup.blocksRaycasts = false;
	}

	//Pendant le drag
	public void OnDrag(PointerEventData eventData)
	{
        if (!draggable)
            return;

        //Prends le delta du drag et bouge l'image en fonction du canva
        m_RectTransform.anchoredPosition += eventData.delta / m_Canvas.scaleFactor;

	}

	//A la fin du drag
	public void OnEndDrag(PointerEventData eventData)
	{
        if (!draggable)
            return;

		resetvalues();
	}

	//Au moment du clic sur l'objet 
	public void OnPointerDown(PointerEventData eventData)
	{
		
	}

	public E_NOTE getNote()
	{
		return note;
	}
	
	public void resetvalues()
	{
        //Remet l'image a la normal
        m_CanvasGroup.alpha = 1;
        m_CanvasGroup.blocksRaycasts = true;
    }
}
