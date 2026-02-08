using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.RenderGraphModule;
using UnityEngine.Rendering.Universal;

public class BackFaceNormalsTexture : ScriptableRendererFeature
{
    [SerializeField] LayerMask layerMask;

    BackFaceNormalsTexturePass pass;

    private Material material;

    public override void Create()
    {
        material = (Material)Resources.Load("BackFaceNormalsMat", typeof(Material));

        pass = new BackFaceNormalsTexturePass(layerMask, material);
        pass.renderPassEvent = RenderPassEvent.AfterRenderingOpaques;
    }

    public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
    {
        renderer.EnqueuePass(pass);
    }

    class BackFaceNormalsTexturePass : ScriptableRenderPass
    {
        private LayerMask layerMask;
        private List<ShaderTagId> shaderTagIdList;
        private Material material;

        public BackFaceNormalsTexturePass(LayerMask layerMask, Material material)
        {
            this.layerMask = layerMask;
            this.material = material;

            shaderTagIdList = new List<ShaderTagId>
            {
                new ShaderTagId("UniversalForwardOnly"),
                new ShaderTagId("UniversalForward"),
                new ShaderTagId("SRPDefaultUnlit"),
                new ShaderTagId("LightweightForward")
            };
        }
        private class PassData
        {
            internal RendererListHandle rendererListHandle;
        }
        static void ExecutePass(PassData data, RasterGraphContext context)
        {
            context.cmd.ClearRenderTarget(RTClearFlags.All, new Color(0, 0, 0, 0), 1, 0);
            context.cmd.DrawRendererList(data.rendererListHandle);
        }

        public override void RecordRenderGraph(RenderGraph renderGraph, ContextContainer frameData)
        {
            const string passName = "BackFace Normals Texture Pass";

            using (var builder = renderGraph.AddRasterRenderPass<PassData>(passName, out var passData))
            {
                UniversalResourceData resourceData = frameData.Get<UniversalResourceData>();
                UniversalRenderingData renderingData = frameData.Get<UniversalRenderingData>();
                UniversalCameraData cameraData = frameData.Get<UniversalCameraData>();
                UniversalLightData lightData = frameData.Get<UniversalLightData>();

                var colorDescriptor = resourceData.activeColorTexture.GetDescriptor(renderGraph);
                var depthDescriptor = resourceData.activeDepthTexture.GetDescriptor(renderGraph);
                colorDescriptor.format = UnityEngine.Experimental.Rendering.GraphicsFormat.R16G16B16A16_SFloat;

                TextureHandle colorTex = renderGraph.CreateTexture(colorDescriptor);
                TextureHandle depthTex = renderGraph.CreateTexture(depthDescriptor);

                DrawingSettings drawSettings = RenderingUtils.CreateDrawingSettings(shaderTagIdList, renderingData, cameraData, lightData, cameraData.defaultOpaqueSortFlags);
                drawSettings.overrideMaterial = material;

                var param = new RendererListParams(renderingData.cullResults, drawSettings, new FilteringSettings(RenderQueueRange.all, layerMask));
                passData.rendererListHandle = renderGraph.CreateRendererList(param);

                builder.UseRendererList(passData.rendererListHandle);

                builder.SetRenderAttachment(colorTex, 0);
                builder.SetRenderAttachmentDepth(depthTex, AccessFlags.Write);

                //builder.AllowPassCulling(false);

                builder.SetGlobalTextureAfterPass(colorTex, Shader.PropertyToID("_BackFaceNormalsTex"));

                builder.SetRenderFunc(static (PassData data, RasterGraphContext context) => ExecutePass(data, context));
            }
        }
    }
}
