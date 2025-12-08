using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

//Utilisation des evenements deja creer par unity afin de gerer le Drag and Drop des carte notes
public class NoteCardScript : MonoBehaviour, IPointerDownHandler, IBeginDragHandler, IEndDragHandler, IDragHandler
{
	private RectTransform m_RectTransform;
	private CanvasGroup m_CanvasGroup;
    [SerializeField] Text m_name;
    [SerializeField] private Canvas m_Canvas;
	[SerializeField] private E_NOTE note;

	public bool draggable = true;

	private void Awake()
	{
		m_RectTransform = GetComponent<RectTransform>();
		m_CanvasGroup = GetComponent<CanvasGroup>();
        setText();
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
