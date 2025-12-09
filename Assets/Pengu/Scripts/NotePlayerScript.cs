using System.Collections;
using System.Xml.Linq;
using UnityEngine;
using UnityEngine.UI;

public class NotePlayerScript : MonoBehaviour
{
    public GameObject textObj;
    public RectTransform textTransform;

    public float zoomInScale = 1.6f;
    public float zoomOutScale = 1f;
    public float zoomInTime = 0.2f;
    public float zoomOutTime = 0.6f;
    public void Startanimation(E_NOTE note)
    {
        textObj.SetActive(true);
        setText(note);
        StartCoroutine(AnimateZoom());
    }
    IEnumerator AnimateZoom()
    {
        // Zoom in
        yield return ScaleTo(zoomInScale, zoomInTime);

        // Zoom out
        yield return ScaleTo(zoomOutScale, zoomOutTime);

        textObj.SetActive(false);
    }

    IEnumerator ScaleTo(float targetScale, float duration)
    {
        Vector3 startScale = textTransform.localScale;
        Vector3 endScale = Vector3.one * targetScale;
        float t = 0f;

        while (t < duration)
        {
            t += Time.deltaTime;
            float normalized = t / duration;
            textTransform.localScale = Vector3.Lerp(startScale, endScale, normalized);
            yield return null;
        }

        textTransform.localScale = endScale;
    }

    void setText(E_NOTE note)
    {
        Text noteText = textObj.GetComponent<Text>();

        switch (note)
        {
            case E_NOTE.E_DO:
                noteText.text = "DO";
                break;
            case E_NOTE.E_RE:
                noteText.text = "RE";
                break;
            case E_NOTE.E_MI:
                noteText.text = "MI";
                break;
            case E_NOTE.E_FA:
                noteText.text = "FA";
                break;
            case E_NOTE.E_SOL:
                noteText.text = "SOL";
                break;
            case E_NOTE.E_LA:
                noteText.text = "LA";
                break;
            case E_NOTE.E_SI:
                noteText.text = "SI";
                break;

        }
    }
}
