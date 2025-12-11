using UnityEngine;
using UnityEngine.UI;

public class MusicNoteScript : MonoBehaviour
{
    #region Champs publique
    [Tooltip("Vitesse de la note quand elle se deplace")]                   public float moveSpeed = 5f;            
    [Tooltip("A quelle vitesse la note scale up quand elle doit fade")]     public float fadeScale = 2;             
    [Tooltip("A quelle vitesse la note disparait")]                         public float opacityFadespeed = .1f;    
    [Tooltip("Combien de temps la note doit etre joue")]                    public float durability = 0;
    [Tooltip("Valeur du CanvaScaleFactor")]                                 public float canvaScale = 1;
    [Tooltip("Note")]                                                       public E_NOTE note;
    #endregion

    #region Champs prives
    private float durabiltyTimer = 0;
    private bool m_fadeAway = false;
    private bool m_isWaiting = true;
    private RectTransform m_RectTransform;
    private Image m_Image;
    private AudioSource m_AudioSource;
    #endregion

    public void Update()
    {
        if (m_isWaiting)
            durability += Time.deltaTime;

        if (m_fadeAway)
        {
            durabiltyTimer += Time.deltaTime;

            if (durabiltyTimer > durability)
                StartCoroutine(FadeOutAndDestroy());

            float speed = Time.deltaTime * fadeScale;
            m_RectTransform.localScale = new Vector3(m_RectTransform.localScale.x + speed, m_RectTransform.localScale.y + speed, m_RectTransform.localScale.z + speed);
            Color col = m_Image.color;
            col.a -= opacityFadespeed * Time.deltaTime;
            m_Image.color = col;
        }
        else
        {
            float speed = Time.deltaTime * moveSpeed * canvaScale;
            m_RectTransform.localPosition = new Vector3(m_RectTransform.localPosition.x - speed, m_RectTransform.localPosition.y, 0);
        }
    }
    public void OnTriggerEnter2D(Collider2D collision)
    {
        if(m_fadeAway)
            return;

        if (collision.gameObject.CompareTag("NotePlayer"))
        {
            m_AudioSource.Play();
            collision.gameObject.GetComponent<NotePlayerScript>().Startanimation(note);
            MidiHandler.Instance.removeNoteOnScreenCounter();
            m_fadeAway = true;
        }
    }

    public System.Collections.IEnumerator FadeOutAndDestroy()
    {
        float vol = m_AudioSource.volume;
        float fadeTime = 0.07f;
        float t = 0f;
        while (t < fadeTime)
        {
            m_AudioSource.volume = Mathf.Lerp(vol, 0f, t / fadeTime);
            t += Time.deltaTime;
            yield return null;
        }
        m_AudioSource.Stop();
        Destroy(gameObject);
    }

    public void init()
    {
        m_RectTransform = GetComponent<RectTransform>();
        m_Image = GetComponent<Image>();
        m_AudioSource = GetComponent<AudioSource>();
    }

    public void stopDurability()
    {
        m_isWaiting = false;
    }

    public bool getFadeAway()
    {
        return m_fadeAway;
    }

    public void setAudioClip(AudioClip clip)
    {
        m_AudioSource.clip = clip;
    }

    public void setVolume(float vol)
    {
        m_AudioSource.volume = vol;
    }
}
