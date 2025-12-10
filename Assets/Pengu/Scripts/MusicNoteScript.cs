using System;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;
using static UnityEngine.GraphicsBuffer;

public class MusicNoteScript : MonoBehaviour
{
    private RectTransform m_RectTransform;
    private Image m_Image;
    public AudioSource m_AudioSource;

    public float moveSpeed = 5f;            //Vitesse de la note quand elle se deplace 
    public float fadeScale = 2;             //A quelle vitesse la note scale up quand elle doit fade
    public float opacityFadespeed = .1f;    //A quelle vitesse la note disparait
    public float durability = 0;            //Combien de temps la note doit etre joue
    public float durabiltyTimer = 0;

    public float canvaScale = 1;
    public E_NOTE note;

    private bool m_fadeAway = false;
    private bool m_isWaiting = true;

    private void Awake()
    {
        m_RectTransform = GetComponent<RectTransform>();
        m_Image = GetComponent<Image>();
        m_AudioSource = GetComponent<AudioSource>();
    }

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

    public void stopDurability()
    {
        m_isWaiting = false;
    }

    public bool getFadeAway()
    {
        return m_fadeAway;
    }
}
