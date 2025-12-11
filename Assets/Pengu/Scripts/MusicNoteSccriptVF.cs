using UnityEngine;
using UnityEngine.UI;

public class MusicNoteScriptVF : MonoBehaviour
{
    #region Champs publique
    [Tooltip("Vitesse de la note quand elle se deplace")]public float moveSpeed = 5f;            
    #endregion

    #region Champs prives
    private RectTransform m_RectTransform;
    private float m_canvaScale;
    #endregion

    //Find the component
    public void Awake()
    {
        m_RectTransform = GetComponent<RectTransform>();
    }

    //Move constantly to the right until a certain position
    public void Update()
    {
        if (m_RectTransform.localPosition.x > 1800 * m_canvaScale)
        {
            MidiHandler.Instance.removeNoteOnScreenCounter();
            Destroy(gameObject);
        }

        float speed = Time.deltaTime * moveSpeed * m_canvaScale;
        m_RectTransform.localPosition = new Vector3(m_RectTransform.localPosition.x + speed, m_RectTransform.localPosition.y, 0);
    }

    //Set Right value
    public void setCanvaScale(float scale)
    {
        m_canvaScale = scale;
    }
}
    