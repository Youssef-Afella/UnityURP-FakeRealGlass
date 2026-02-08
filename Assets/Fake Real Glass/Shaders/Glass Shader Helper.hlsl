#ifndef SHADERGRAPH_PREVIEW
#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
#endif

#pragma multi_compile_fragment _ _REFLECTION_PROBE_BLENDING
#pragma multi_compile_fragment _ _REFLECTION_PROBE_BOX_PROJECTION
#pragma multi_compile _ _CLUSTER_LIGHT_LOOP

void GetCubemap_float(float3 ReflectedDir, float3 PositionWS, float2 NormalizedScreenSpaceUV, float Roughness, out float3 Cubemap)
{
#ifdef SHADERGRAPH_PREVIEW
    Cubemap = 0;
#else
    Cubemap = GlossyEnvironmentReflection(ReflectedDir, PositionWS, Roughness, 1, NormalizedScreenSpaceUV);
#endif
}

float GetReflectance(float3 i, float3 t, float3 nor, float iora, float iorb)
{
    float cosi = dot(i, nor);
    float cost = dot(t, nor);
    float spr = pow((cosi / iorb - cost / iora) / (cosi / iorb + cost / iora), 2.0);
    float spp = pow((cost / iorb - cosi / iora) / (cost / iorb + cosi / iora), 2.0);
    return (spr + spp) / 2.0;
}

void SplitRay_float(float3 inDir, float3 normal, float iorA, float iorB, out float3 reflected, out float3 refracted, out float reflectance)
{
    float refractRatio = iorA / iorB;
    reflected = reflect(inDir, normal);
    refracted = refract(inDir, normal, refractRatio);
    
    reflectance = saturate(GetReflectance(inDir, refracted, normal, iorA, iorB));
}