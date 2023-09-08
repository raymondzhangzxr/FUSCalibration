//------------------------------------------------------------------------------ -
//MRTK - Quest
//https ://github.com/provencher/MRTK-Quest
//------------------------------------------------------------------------------ -
//
//MIT License
//
//Copyright(c) 2020 Eric Provencher
//
//Permission is hereby granted, free of charge, to any person obtaining a copy
//of this software and associated documentation files(the "Software"), to deal
//in the Software without restriction, including without limitation the rights
//to use, copy, modify, merge, publish, distribute, sublicense, and / or sell
//copies of the Software, and to permit persons to whom the Software is
//furnished to do so, subject to the following conditions :
//
//The above copyright notice and this permission notice shall be included in all
//copies or substantial portions of the Software.
//
//THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
//IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
//FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT.IN NO EVENT SHALL THE
//AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
//LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
//OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
//SOFTWARE.
//------------------------------------------------------------------------------ -

Shader "MyMRTK/StencilRimOutline"
{   
    Properties{
        _Color("Color", Color) = (1,1,1,1)

        _RimColor("Rim Color", Color) = (0.26,0.19,0.16,0.0)
        _PressRimColor("Press Rim Color", Color) = (1,1,1,1)

        _RimPower("Rim Power", Range(0.5,8.0)) = 6.6
        _PressRimPower("Press Rim Power", Range(0.5,8.0)) = 5.0

        _PressIntensity("Press Intensity", Range(0, 1.0)) = 1.0
        [IntRange] _StencilRef("Stencil Reference Value", Range(0,255)) = 1
    }

        SubShader
    {


        Tags{ "RenderType" = "Opaque" "Queue" = "Geometry"}
        LOD 100

        

        Pass
        {

            
            
        //stencil operation
        Stencil{
            Ref[_StencilRef]
            Comp Equal
        }
            
            
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float4 normal : NORMAL;
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            struct v2f
            {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
                float3 viewDir: TEXCOORD1;
                //float3 cameraDir : TEXCOODR2;
                UNITY_VERTEX_OUTPUT_STEREO
            };


            v2f vert(appdata v)
            {
                v2f o;
                UNITY_SETUP_INSTANCE_ID(v); //Insert
                UNITY_INITIALIZE_OUTPUT(v2f, o); //Insert
                UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o); //Insert

                o.vertex = UnityObjectToClipPos(v.vertex);
                o.normal = UnityObjectToWorldNormal(v.normal);
                o.viewDir = UnityWorldSpaceViewDir(mul(UNITY_MATRIX_M, v.vertex));
                //o.cameraDir = mul((float3x3)unity_CameraToWorld, float3(0, 0, 1));

                return o;
            }


            uniform float4 _Color;
            uniform float4 _PressRimColor;
            uniform float4 _RimColor;

            uniform float _RimPower;
            uniform float _PressRimPower;

            uniform float _PressIntensity;

            fixed4 frag(v2f i) : SV_Target
            {

                half rim = 1.0 - abs(dot(i.viewDir, i.normal));

                float rimPower = pow(rim, lerp(_RimPower, _PressRimPower, _PressIntensity));
                float3 rimColor = lerp(_RimColor.rgb, _PressRimColor.rgb, _PressIntensity);
                float3 emission = rimColor * (rimPower);

                float3 color = _Color.rgb;
                float4 output = fixed4(color + emission, 1);
                return output;
            }
        ENDCG
        }
    }
}