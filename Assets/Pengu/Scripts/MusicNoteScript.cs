using UnityEngine;
using UnityEngine.UI;
using static UnityEngine.GraphicsBuffer;

public class MusicNoteScript : MonoBehaviour
{
    private RectTransform m_RectTransform;
    private Image m_Image;

    public float moveSpeed = 5f;
    public float fadeDuration = 1;
    public float fadeScale = 2;
    public float opacityFadespeed = .1f;
    public E_NOTE note;
    public Canvas canvas;

    private bool m_fadeAway = false;
    private float fadeTimer = 0;

    private void Awake()
    {
        m_RectTransform = GetComponent<RectTransform>();
        m_Image = GetComponent<Image>();
    }

    public void Update()
    {
        if (m_fadeAway)
        {
            fadeTimer += Time.deltaTime;
            float speed = Time.deltaTime * fadeScale;
            m_RectTransform.localScale = new Vector3(m_RectTransform.localScale.x + speed, m_RectTransform.localScale.y + speed, m_RectTransform.localScale.z + speed);
            Color col = m_Image.color;
            col.a -= opacityFadespeed * Time.deltaTime;
            m_Image.color = col;

            if (fadeTimer > fadeDuration)
                Destroy(gameObject);
        }
        else
        {
            float speed = Time.deltaTime * moveSpeed * canvas.scaleFactor;
            m_RectTransform.position = new Vector3(m_RectTransform.position.x - speed, m_RectTransform.position.y, m_RectTransform.position.z);
        }
    }
    public void OnTriggerEnter2D(Collider2D collision)
    {
        if(m_fadeAway)
            return;

        if (collision.gameObject.CompareTag("NotePlayer"))
        {
            m_fadeAway = true;
            MidiManager.Instance.PlayPianoNote((int)note);
        }
    }
}
