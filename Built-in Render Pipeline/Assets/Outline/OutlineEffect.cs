using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.PostProcessing;

[Serializable]
[PostProcess(typeof(OutlineRenderer), PostProcessEvent.AfterStack, "Custom/Outline")]
public sealed class OutlineEffect : PostProcessEffectSettings
{
    public ColorParameter color = new ColorParameter { value = Color.red };
    [Range(0f, 5f), Tooltip("Outline size.")]
    public FloatParameter size = new FloatParameter { value = 2f };
    [Range(0f, 1f), Tooltip("Outline softness.")]
    public FloatParameter softness = new FloatParameter { value = 0.5f };    
    public BoolParameter downSample = new BoolParameter { value = false };
    public BoolParameter drawOnTop = new BoolParameter { value = true };
    public ParameterOverride<string> Tag = new ParameterOverride<string> { value = "Outline" };
    
    public override bool IsEnabledAndSupported(PostProcessRenderContext context)
    {
        //TODO: Find a smart way of filtering objects with outline
        bool objectsExist = false;
        try
        {
            objectsExist = GameObject.FindWithTag(Tag);
        }
        catch (Exception) { }
        return enabled.value && objectsExist; 
    }
}

public sealed class OutlineRenderer : PostProcessEffectRenderer<OutlineEffect>
{
    private Material m_mat;
    private Shader m_SeparableBlurShader;
    private Shader m_DilateShader;   
    private Shader m_OutlineShader;
    private int m_objectsID;
    private int m_outlineColorID;
    private int m_ztestID;
    private int m_blurredID;
    private int m_tempID;
    private int m_offsetsID;

    public override void Init()
    {
        m_mat = new Material(Shader.Find("Hidden/Outline/UnlitColor"));
        m_mat.hideFlags = HideFlags.HideAndDontSave;
        m_SeparableBlurShader = Shader.Find("Hidden/Outline/SeparableBlur");
        m_DilateShader = Shader.Find("Hidden/Outline/Dilate");        
        m_OutlineShader = Shader.Find("Hidden/Outline");
        m_objectsID = Shader.PropertyToID("_ObjectsTex");
        m_outlineColorID = Shader.PropertyToID("_OutlineColor");
        m_ztestID = Shader.PropertyToID("_ZTest");
        m_blurredID = Shader.PropertyToID("_BlurredTex");
        m_tempID = Shader.PropertyToID("_Temp");
        m_offsetsID = Shader.PropertyToID("_Offsets");
    }
    public override void Render(PostProcessRenderContext context)
    {
        //TODO: Find a smart way of filtering objects with outline
        var renderers = GameObject.FindGameObjectsWithTag(settings.Tag)
            .Select((g) => g.GetComponent<MeshRenderer>());        
        context.command.GetTemporaryRT(m_objectsID, -1, -1, 24, FilterMode.Bilinear);
        var depthId = context.camera.actualRenderingPath == RenderingPath.Forward 
            ? BuiltinRenderTextureType.Depth : BuiltinRenderTextureType.ResolvedDepth;
        context.command.SetRenderTarget(color: m_objectsID, depth: depthId);
        context.command.ClearRenderTarget(false, true, Color.clear);   
        context.command.SetGlobalColor(m_outlineColorID, settings.color);
                
        int ztest = settings.drawOnTop ? (int)CompareFunction.Always : (int)CompareFunction.LessEqual;
        m_mat.SetInt(m_ztestID, ztest);
        // draw objects
        foreach (var r in renderers)
        {
            context.command.DrawRenderer(r, m_mat);
        }         

        int sample = settings.downSample.value ? 2 : 1;        
        context.command.GetTemporaryRT(m_blurredID, -sample, -sample, 0, FilterMode.Bilinear);
        context.command.GetTemporaryRT(m_tempID, -sample, -sample, 0, FilterMode.Bilinear);
        context.command.Blit(m_objectsID, m_blurredID);

        // horizontal dilate        
        float dilateSize = settings.size * (1 - settings.softness);
        var dilate = context.propertySheets.Get(m_DilateShader);         
        dilate.properties.SetVector(m_offsetsID, new Vector4(dilateSize / context.width, 0, 0, 0));
        context.command.BlitFullscreenTriangle(m_blurredID, m_tempID, dilate, 0);
        // vertical dilate
        dilate.properties.SetVector(m_offsetsID, new Vector4(0, dilateSize / context.height, 0, 0));
        context.command.BlitFullscreenTriangle(m_tempID, m_blurredID, dilate, 0);
        // horizontal blur        
        float blurSize = settings.size - dilateSize;
        var blur = context.propertySheets.Get(m_SeparableBlurShader);
        blur.properties.SetVector(m_offsetsID, new Vector4(blurSize / context.width, 0, 0, 0));
        context.command.BlitFullscreenTriangle(m_blurredID, m_tempID, blur, 0);
        // vertical blur
        blur.properties.SetVector(m_offsetsID, new Vector4(0, blurSize / context.height, 0, 0));
        context.command.BlitFullscreenTriangle(m_tempID, m_blurredID, blur, 0);

        var outline = context.propertySheets.Get(m_OutlineShader);
        context.command.BlitFullscreenTriangle(context.source, context.destination, outline, 0);        
    }
    public override void Release()
    {
        GameObject.DestroyImmediate(m_mat);
        base.Release();
    }
}