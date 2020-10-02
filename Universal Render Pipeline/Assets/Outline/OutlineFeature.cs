using UnityEngine;
using UnityEngine.Rendering.Universal;

public class OutlineFeature : ScriptableRendererFeature
{
    [System.Serializable]
    public class OutlineFeatureSettings
    {
        public string passTag = "OutlineFeature";
        public RenderPassEvent Event = RenderPassEvent.BeforeRenderingPostProcessing;
        public LayerMask layerMask = 0;
        public Color color = Color.white;
        [Range(0, 5)]
        public float size = 2;
        [Range(0, 1)]
        public float softness = .5f;
        public bool drawOnTop = true;
        public bool downsample = false;        
    }   

    private OutlinePass.OutlineMaterials m_outlineMaterials = new OutlinePass.OutlineMaterials();
    private OutlinePass m_OutlinePass;
    public OutlineFeatureSettings settings = new OutlineFeatureSettings();

    public override void Create()
    {           
        m_OutlinePass = new OutlinePass(settings.layerMask, m_outlineMaterials, name);        
        m_OutlinePass.renderPassEvent = settings.Event;          
    }
    public void OnEnable() => CreateMaterials(m_outlineMaterials);  
    
    public void OnDisable() => DestroyMaterials(m_outlineMaterials);    
    
    public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
    {
        m_OutlinePass.settings = new OutlinePass.OutlineSettings
        {
            color = settings.color,
            size = settings.size,
            softness = settings.softness,
            downsample = settings.downsample,
            drawOnTop = settings.drawOnTop,
        };
        m_OutlinePass.cameraTarget = renderer.cameraColorTarget;       
        renderer.EnqueuePass(m_OutlinePass);
    }
    private void CreateMaterials(OutlinePass.OutlineMaterials materials)
    {
        materials.UnlitMaterial = new Material(Shader.Find("Hidden/Outline/UnlitColor"));
        materials.UnlitMaterial.hideFlags = HideFlags.HideAndDontSave;
        materials.BlurMaterial = new Material(Shader.Find("Hidden/Outline/SeparableBlur"));
        materials.BlurMaterial.hideFlags = HideFlags.HideAndDontSave;
        materials.DilateMaterial = new Material(Shader.Find("Hidden/Outline/Dilate"));
        materials.DilateMaterial.hideFlags = HideFlags.HideAndDontSave;
        materials.OutlineMaterial = new Material(Shader.Find("Hidden/Outline"));
        materials.OutlineMaterial.hideFlags = HideFlags.HideAndDontSave;
    }
    private void DestroyMaterials(OutlinePass.OutlineMaterials materials)
    {
        DestroyImmediate(materials.UnlitMaterial);
        DestroyImmediate(materials.BlurMaterial);
        DestroyImmediate(materials.DilateMaterial);
        DestroyImmediate(materials.OutlineMaterial);
    }
}