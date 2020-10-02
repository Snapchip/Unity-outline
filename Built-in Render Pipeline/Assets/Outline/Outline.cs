using System.Collections;
using UnityEngine;

public class Outline : MonoBehaviour
{
    private OutlineQuickVolume m_outlineVolume;
    private float m_defaultSize;
    private float m_targetSize;
    private float m_alpha = 0;
    public float fadeSpeed = 5;
    public float highlightSpeed = 10;
    public float maxSize = 4;
    private void Awake()
    {
        tag = "Untagged";
        m_outlineVolume = FindObjectOfType<OutlineQuickVolume>();
        m_defaultSize = m_outlineVolume.size;        
    }

    private void OnMouseEnter()
    {             
        tag = "Outline";
        StartCoroutine(FadeInColor());
    }
    private void OnMouseDown()
    {        
        StartCoroutine(OnClickHighlihgt());
    }

    private void OnMouseExit()
    {
        StopAllCoroutines();
        tag = "Untagged";
        m_outlineVolume.size = m_defaultSize;
    }

    private IEnumerator FadeInColor()
    {
        m_alpha = 0;
        while (m_alpha < 1)
        {
            m_alpha = Mathf.MoveTowards(m_alpha, 1, fadeSpeed * Time.deltaTime);
            var c = m_outlineVolume.color;
            m_outlineVolume.color = new Color(c.r, c.g, c.b, m_alpha);
            yield return null;
        }
    }

    private IEnumerator OnClickHighlihgt()
    {
        m_targetSize = maxSize;
        while (true)
        {
            m_outlineVolume.size = Mathf.MoveTowards(m_outlineVolume.size, m_targetSize, highlightSpeed * Time.deltaTime);
            if (m_outlineVolume.size >= maxSize) m_targetSize = m_defaultSize;
            if (m_outlineVolume.size == m_defaultSize) break;
            yield return null;
        }        
    }
}
