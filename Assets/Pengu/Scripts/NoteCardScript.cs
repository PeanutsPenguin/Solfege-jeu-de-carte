using UnityEngine;
using UnityEngine.EventSystems;

//Utilisation des evenements deja creer par unity afin de gerer le Drag and Drop des carte notes
public class NoteCardScript : MonoBehaviour, IPointerDownHandler, IBeginDragHandler, IEndDragHandler, IDragHandler, IDropHandler
{
    private RectTransform m_RectTransform;
    private CanvasGroup m_CanvasGroup;
    [SerializeField] private Canvas m_Canvas;
    [SerializeField] private E_NOTE note;

    private void Awake()
    {
        m_RectTransform = GetComponent<RectTransform>();
        m_CanvasGroup = GetComponent<CanvasGroup>();
    }

    //Au commencement du drag
    public void OnBeginDrag(PointerEventData eventData)
    {
        //Desactive les evenement sur l'objets et le rend plus opaque 
        m_CanvasGroup.alpha = .5f;
        m_CanvasGroup.blocksRaycasts = false;
    }

    //Pendant le drag
    public void OnDrag(PointerEventData eventData)
    {
        //Prends le delta du drag et bouge l'image 
        m_RectTransform.anchoredPosition += eventData.delta / m_Canvas.scaleFactor;

    }

    //A la fin du drag
    public void OnEndDrag(PointerEventData eventData)
    {
        m_CanvasGroup.alpha = 1;
        m_CanvasGroup.blocksRaycasts = true;
    }

    //Au moment du clic sur l'objet 
    public void OnPointerDown(PointerEventData eventData)
    {
        Debug.Log("Clic on Object");
    }

    //Au moment du drop
    public void OnDrop(PointerEventData eventData)
    {
        throw new System.NotImplementedException();
    }

    public E_NOTE getNote()
    {
        return note;
    }
}
