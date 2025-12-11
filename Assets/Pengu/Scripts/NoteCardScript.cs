using NoteValues;
using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class NoteCardScript : MonoBehaviour, IPointerDownHandler, IBeginDragHandler, IEndDragHandler, IDragHandler		//<-"Built-in Class" afin de gerer le drag and drop
{
    #region Champs publique
    [Tooltip("Text afficahnt la note")]											public Text m_name;
    [Tooltip("Est que la carte est draggable ?")]								public bool draggable = true;
    [Tooltip("Est ce que la carte doit jouer la note en appuyant dessus ?")]	public bool playable = false;
    #endregion

    #region Champs Serialize
    [Tooltip("Canva qui contient la carte")][SerializeField]		private Canvas m_Canvas;
    [Tooltip("Note de la carte")][SerializeField]					private E_NOTE note;
    [Tooltip("Image du coin en bas a gauvhe")][SerializeField]		private Image m_bottomLeftCorner;
    [Tooltip("Image du coin en haut a droite")][SerializeField]		private Image m_topRightCorner;
    #endregion

    #region Champs prives
    private RectTransform m_RectTransform;
    private CanvasGroup m_CanvasGroup;
    #endregion

    #region UNITY Methods
    //Defini les bonnes valeurs
    private void Awake()
	{
		m_RectTransform = GetComponent<RectTransform>();
		m_CanvasGroup = GetComponent<CanvasGroup>();
		m_name.text = NoteValuesHandler.SetNoteText(note);
		Color color = NoteValuesHandler.setNoteColor(note);
		m_bottomLeftCorner.color = color;
		m_topRightCorner.color = color;
	}
    #endregion

    #region Drag and Drop
    //Au moment du clic sur l'objet 
    public void OnPointerDown(PointerEventData eventData)
    {
        //Lorsque la note est "playable" joue la note correspondante au clic
        if (playable)
            MidiHandler.Instance.launchNote(note, 100);
    }

    //Au commencement du drag
    public void OnBeginDrag(PointerEventData eventData)
	{
        if (!draggable)
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

        //Remets les valeurs de l'image a la normal
        m_CanvasGroup.alpha = 1;
        m_CanvasGroup.blocksRaycasts = true;
    }
    #endregion

    #region Carte
    public E_NOTE getNote()
	{
		return note;
	}
	
	public void resetvalues()
	{
        //Remet l'image a la normal
        m_CanvasGroup.alpha = 1;
        m_CanvasGroup.blocksRaycasts = true;
        Image img = GetComponent<Image>();
        Color color = img.color;
        color.a = 255;
        img.color = color;
    }
    #endregion
}
