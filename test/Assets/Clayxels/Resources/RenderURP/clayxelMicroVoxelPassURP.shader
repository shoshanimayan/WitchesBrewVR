
Shader "Clayxels/ClayxelMicroVoxelPassURP"
{
    Properties
    {
        _backFillDark("Backdrop Darkner", Range(0.0, 1.0)) = 0.0
        _alphaCutout("Splat Cutout", Range(0.0, 1.0)) = 0.0
        _splatSizeMult("Splat Size", Range(0.0, 1.0)) = 0.0
        _roughSize("Rough Size", Range(0.0, 1.0)) = 0.0
        _roughColor("Rough Color", Range(0.0, 1.0)) = 0.0
        _roughPos("Rough Position", Range(0.0, 1.0)) = 0.0
        _roughTwist("Rough Twist", Range(0.0, 10.0)) = 0.0
        _roughOrientX("Rough Orient X", Range(-1.0, 1.0)) = 0.0
        _roughOrientY("Rough Orient Y", Range(-1.0, 1.0)) = 0.0
        _roughOrientZ("Rough Orient Z", Range(-1.0, 1.0)) = 0.0
        _roughOffset("Rough Offset", Range(0.0, 1.0)) = 0.0
        [NoScaleOffset]_MainTex("BigSplat Texture", 2D) = "white" {}
    }

    SubShader
    {
        Pass
        {
            Name "FirstPass"
            Tags { "LightMode" = "FirstPass" }

            ZWrite On
            ZTest LEqual
            Cull Front

            HLSLPROGRAM

            #pragma target 4.5

            #pragma vertex LitPassVertex
            #pragma fragment LitPassFragment

            #pragma shader_feature CLAYXELS_NONE CLAYXELS_URP CLAYXELS_HDRP

            // #if defined(CLAYXELS_NONE)
            //     float4 LitPassVertex():SV_POSITION{return 0.0;}
            //     float4 LitPassFragment():SV_TARGET0{return 0.0;}
            // #endif

            // #if defined(CLAYXELS_URP)
                #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
                #include "Packages/com.unity.shadergraph/ShaderGraphLibrary/ShaderVariablesFunctions.hlsl"
            // #elif defined(CLAYXELS_HDRP)
                // #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Common.hlsl"
                // #include "Packages/com.unity.render-pipelines.high-definition/Runtime/ShaderLibrary/ShaderVariables.hlsl"
            // #endif

            // #if !defined(CLAYXELS_NONE) // this is the first variant that gets compiled, needs to be render-pipe agnostic
            
            #include "../clayxelMicroVoxelUtils.cginc"

            struct Attributes
            {
                float4 positionOS   : POSITION;
                uint vertexID: SV_VertexID;
                float2 uv: TEXCOORD0;
            };

            struct Varyings
            {
                float3 chunkId: TEXCOORD0;
                float3 voxelSpacePos: TEXCOORD1;
                float3 voxelSpaceViewDir: TEXCOORD2;
                float3 viewDirectionWS: TEXCOORD3;
                float3 boundsCenter: TEXCOORD4;
                float3 boundsSize: TEXCOORD5;
                float3 localCamPos: TEXCOORD6;
                float4x4 modelMatrix: TEXCOORD7;
                float4 screenPos: TEXCOORD12;
                float lod: TEXCOORD13;
                float4 positionCS: SV_POSITION;
            };

            struct OutBuffers{
                float4 buffer0: SV_TARGET0;
                float4 buffer1: SV_TARGET1;
                float4 buffer2: SV_TARGET2;
                float4 buffer3: SV_TARGET3;
            };

            Varyings LitPassVertex(Attributes input)
            {
                Varyings output = (Varyings)0;

                output.chunkId = float3(input.vertexID / 8, 0, 0);
                
                float3 camNearPlanePos = _WorldSpaceCameraPos + (UNITY_MATRIX_V._m20_m21_m22 * _ProjectionParams.y);
                output.localCamPos = mul(objectMatrixInv, float4(camNearPlanePos, 1)).xyz;

                float3 vertexPos = 0;
                float3 boundsCenter = 0;
                float3 boundsSize = 0;
                
                float forceFlip = 0;
                float invertFlip = 0;
                float outBoxFlipped = 0;
                clayxels_microVoxels_vert(forceFlip, invertFlip, output.chunkId.x, input.vertexID, input.positionOS.xyz, output.localCamPos, vertexPos, boundsCenter, boundsSize, outBoxFlipped);

                output.boundsCenter = boundsCenter;
                output.boundsSize = boundsSize;

                output.voxelSpacePos = vertexPos;

                output.voxelSpaceViewDir = mul(objectMatrixInv, float4(_WorldSpaceCameraPos.xyz, 1)).xyz;

                output.modelMatrix = mul(UNITY_MATRIX_M, objectMatrix);// hdrp needs UNITY_MATRIX_M, lets pre-multiply it here with the real objectMatrix
                float4 modelPos = mul(output.modelMatrix, float4(vertexPos, 1.0));

                float4 positionCS = mul(UNITY_MATRIX_VP, modelPos);
                output.positionCS = positionCS;

                output.viewDirectionWS = GetWorldSpaceNormalizeViewDir(modelPos.xyz).xyz;

                float scale = length(objectMatrix._m00_m10_m20);
                float cellSize = (chunkSize * rcp(256.0)) * scale;

                float4 cellSizeWS = mul(UNITY_MATRIX_M, float4(UNITY_MATRIX_V._m00_m10_m20,1)) * cellSize;
                float4 cellSizeCS = mul(UNITY_MATRIX_VP, modelPos + cellSizeWS);

                float4 screenPos1 = computeScreenPos(positionCS);
                float4 screenPos2 = computeScreenPos(cellSizeCS);

                float deviceCoords1 = screenPos1.x / screenPos1.w;
                float deviceCoords2 = screenPos2.x / screenPos2.w;
                float cellSizePixels = abs(deviceCoords1 - deviceCoords2) * _ScreenParams.x;

                float splatPixelWidthThreshold = 4.0; // when the pixel width of a splat is smaller than this, splats aren't rendered
                float LOD = saturate(cellSizePixels * rcp(splatPixelWidthThreshold));
                
                output.screenPos = screenPos1;
                output.lod = LOD;
                
                return output;
            }

            OutBuffers LitPassFragment(Varyings input, out float outDepth : SV_Depth){
                int chunkId = round(input.chunkId.x);
                
                float3 positionWS = input.voxelSpacePos;

                float perspIso = unity_OrthoParams.w;// pespective == 0.0 , ortho == 1.0

                float3 viewDirectionWSPersp = input.voxelSpaceViewDir - positionWS;
                float3 viewDirectionWSIso = input.viewDirectionWS + 0.000001; // 0.000001 corrects a division error

                float3 viewDirectionWS = normalize(lerp(viewDirectionWSPersp, viewDirectionWSIso, perspIso));

                float4 hitNormal = 0;
                float3 hitColor = 0;
                float3 hitDepthPoint = 0;
                float3 hitGridData = 0;
                int hitClayObjId = 0;
                
                float lod = input.lod;
                bool hit = false;
                hit = clayxels_microVoxelsMip3SplatSeed_frag(chunkId, positionWS, viewDirectionWS, input.boundsCenter, input.boundsSize, input.localCamPos, lod, hitNormal, hitColor, hitDepthPoint, hitGridData, hitClayObjId);
                
                if(!hit){
                    discard;
                }

                OutBuffers outBuffers = (OutBuffers)0;

                float scale = length(objectMatrix._m00_m10_m20);

                float4 hitDepthPointWS = mul(input.modelMatrix, float4(hitDepthPoint, 1));
                float4 hitDepthPointScreen = mul(UNITY_MATRIX_VP, hitDepthPointWS);
                outDepth = saturate(hitDepthPointScreen.z / hitDepthPointScreen.w);

                int maxChunks = 27;
                float3 containerIdRGB = unpackRgb(((containerId * maxChunks) + chunkId) + 1);
                float3 clayObjIdRGB = unpackRgb(hitClayObjId);
                
                outBuffers.buffer0 = hitNormal;
                outBuffers.buffer1 = float4(hitGridData, clayObjIdRGB.x);
                outBuffers.buffer2 = float4(hitColor, clayObjIdRGB.y);
                outBuffers.buffer3 = float4(containerIdRGB, clayObjIdRGB.z);

                return outBuffers;
            }

            // #endif

            ENDHLSL
        }
    }
}