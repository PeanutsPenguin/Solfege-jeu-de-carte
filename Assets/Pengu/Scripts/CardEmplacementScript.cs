using NoteValues;
using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CardEmplacementScript : MonoBehaviour, IDropHandler
{
    #region Champs publique
    [Tooltip("Texte ecrit dans l'emplacement de carte")]					public Text m_name;
	[Tooltip("Particle system joue lors d'une bonne/mauvaise reponse")]		public ParticleSystem particle;                                    
	[Tooltip("Audio joue lors d'une mauvaise reponse")]						public AudioClip badsoundEffect;
    [Tooltip("Scale in du texte lors de l'animation")]                      public float zoomInScale = 1.8f;
    [Tooltip("Scale out du texte lors de l'animation")]                     public float zoomOutScale = 1f;
    [Tooltip("Temps que le texte va mettre a scale in")]                    public float zoomInTime = 0.15f;
    [Tooltip("Temps que le texte va mettre a scale out")]                   public float zoomOutTime = 0.85f;
    #endregion

    #region Champs serialize
    [Tooltip("Image de coin de l'emplacement de carte")]				[SerializeField]		private Image m_toprightCorner;
    [Tooltip("Image de coin de l'emplacement de carte")]				[SerializeField]		private Image m_bottomLeftCorner;
    [Tooltip("Texte qui jouera une animation lors du jeu de la note")]	[SerializeField]		private Text m_animText;
    [Tooltip("Note de l'emplacemetn de carte")]							[SerializeField]		private E_NOTE note;               
    #endregion

    #region Champ prive
    private RectTransform m_RectTransform;              //Transform component de l'image
    private IEnumerator m_animTextCoroutine;            //Coroutine de l'animation du texte
    #endregion

    #region UNITY Methods
    void Awake()
	{
		//Mise en place des valeurs
		m_RectTransform = GetComponent<RectTransform>();
		string noteText = NoteValuesHandler.SetNoteText(note);
		m_name.text = noteText;
		m_animText.text = noteText;
        m_animText.gameObject.SetActive(false);
        SetCornersColor(NoteValuesHandler.setNoteColor(note));
	}
    #endregion

    #region OnDrop
    public void OnDrop(PointerEventData eventData)
	{
		//Verifie que l'objet depose n'est pas nul
	   if(eventData.pointerDrag != null)
		{
            //Recupere le script de la noteCard
            NoteCardScript data = eventData.pointerDrag.GetComponent<NoteCardScript>();

            if (!data.draggable)
                return;

            //Place l'objet au centre de celui-ci
            eventData.pointerDrag.GetComponent<RectTransform>().position = m_RectTransform.position;

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
				pfxMain.startColor = Color.black;							//Mets la couleur du systeme de particule en vert
				MidiHandler.Instance.launchNote(note, 100);					//Lance la demarche pour jouer la note
				GameManager.Instance.setValidedNote(note);					//Valide la bonne reponse aupres du GameManager
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

    #region AnimateText

    public void StartTextAnimation()
    {
        if(m_animTextCoroutine != null)
        {
            StopCoroutine(m_animTextCoroutine);
            m_animText.rectTransform.localScale = Vector3.one;
        }

        m_animText.gameObject.SetActive(true);
        m_animTextCoroutine = AnimateZoom();
        StartCoroutine(m_animTextCoroutine);
    }

    IEnumerator AnimateZoom()
    {
        // Zoom in
        yield return ScaleTo(zoomInScale, zoomInTime);

        // Zoom out
        yield return ScaleTo(zoomOutScale, zoomOutTime);

        m_animText.gameObject.SetActive(false);
    }

    IEnumerator ScaleTo(float targetScale, float duration)
    {
        Vector3 startScale = m_animText.rectTransform.localScale;
        Vector3 endScale = Vector3.one * targetScale;
        float t = 0f;

        while (t < duration)
        {
            t += Time.deltaTime;
            float normalized = t / duration;
            m_animText.rectTransform.localScale = Vector3.Lerp(startScale, endScale, normalized);
            yield return null;
        }

        m_animText.rectTransform.localScale = endScale;
    }
    #endregion

    #region Note
    public E_NOTE getNote()
    {
        return note;
    }
    #endregion
}
