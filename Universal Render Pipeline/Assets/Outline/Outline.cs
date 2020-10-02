using System.Collections;
using System.Linq;
using UnityEngine;

public class Outline : MonoBehaviour
{
    private OutlineFeature m_outlineFeature;
    private float m_defaultSize;
    private float m_targetSize;
    private int m_defaultLayer;
    private int m_outlineLayer;
    private Color m_defaultColor;
    private float m_alpha = 0;

    public string defaultLayerName = "Default";
    public string outlineLayerName = "Outline";
    public float fadeSpeed = 5;
    public float highlightSpeed = 10;
    public float maxSize = 3;    

    private void Awake()
    {
        m_outlineFeature = Resources.FindObjectsOfTypeAll<OutlineFeature>().FirstOrDefault();
        if (!m_outlineFeature)
        {
            Debug.LogError("Outline feature not found.");            
            return;
        }
        
        m_defaultLayer = LayerMask.NameToLayer(defaultLayerName);
        m_outlineLayer = LayerMask.NameToLayer(outlineLayerName);
        gameObject.layer = m_defaultLayer;
    }

    private void OnMouseEnter()
    {
        //Start outline fadein
        m_defaultSize = m_outlineFeature.settings.size;
        m_defaultColor = m_outlineFeature.settings.color;
        gameObject.layer = m_outlineLayer;
        StartCoroutine(FadeInColor());        
    }
    private void OnMouseDown()
    {        
        StartCoroutine(OnClickHighlihgt());
    }

    private void OnMouseExit()
    {
        //Reset outline to defaults
        StopAllCoroutines();
        m_outlineFeature.settings.size = m_defaultSize;
        m_outlineFeature.settings.color = m_defaultColor;
        gameObject.layer = m_defaultLayer;        
    }

    private IEnumerator FadeInColor()
    {       
        m_alpha = 0;
        while (m_alpha < 1)
        {
            m_alpha = Mathf.MoveTowards(m_alpha, 1, fadeSpeed * Time.deltaTime);
            var c = m_outlineFeature.settings.color;
            m_outlineFeature.settings.color = new Color(c.r, c.g, c.b, m_alpha);
            yield return null;
        }        
    }    
    
    private IEnumerator OnClickHighlihgt()
    {        
        m_targetSize = maxSize;
        while (true)
        {
            m_outlineFeature.settings.size = Mathf.MoveTowards(m_outlineFeature.settings.size, m_targetSize, highlightSpeed * Time.deltaTime);
            if (m_outlineFeature.settings.size >= maxSize) m_targetSize = m_defaultSize;
            if (m_outlineFeature.settings.size == m_defaultSize) break;
            yield return null;
        }
    }
}