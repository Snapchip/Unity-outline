using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

[ExecuteInEditMode]
public class OutlineQuickVolume : MonoBehaviour
{
    private PostProcessVolume m_Volume;
    private OutlineEffect m_OutlineEffect;
    public Color color = Color.white;
    [Range(0,5)]
    public float size = 3f;
    [Range(0,1)]
    public float softness = 0.4f;
    public bool drawOnTop = true;
    public bool downSample = false;
    public string Tag = "Outline";

    void OnEnable()
    {
        m_OutlineEffect = ScriptableObject.CreateInstance<OutlineEffect>();
        m_OutlineEffect.enabled.Override(true);
        m_OutlineEffect.color.Override(color);
        m_OutlineEffect.size.Override(size);
        m_OutlineEffect.softness.Override(softness);
        m_OutlineEffect.drawOnTop.Override(drawOnTop);
        m_OutlineEffect.downSample.Override(downSample);
        m_OutlineEffect.Tag.Override(Tag);
        m_Volume = PostProcessManager.instance.QuickVolume(gameObject.layer, 100f, m_OutlineEffect);
    }

    void Update()
    {
        m_OutlineEffect.size.value = size;
        m_OutlineEffect.softness.value = softness;
        m_OutlineEffect.color.value = color;
        m_OutlineEffect.drawOnTop.value = drawOnTop;
        m_OutlineEffect.downSample.value = downSample;
        m_OutlineEffect.Tag.value = Tag;
    }

    void OnDisable()
    {
        RuntimeUtilities.DestroyVolume(m_Volume, true, true);
    }
}