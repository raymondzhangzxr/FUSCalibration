Shader "Custom/MirrorStencilShader" {

	
//show values to edit in inspector
Properties{
	[IntRange] _StencilRef("Stencil Reference Value", Range(0,255)) = 1
}

	SubShader{
	//the material is completely non-transparent and is rendered at the same time as the other opaque geometry
	Tags{ "RenderType" = "Opaque" "Queue" = "Geometry-10"}

	//stencil operation
	Stencil{
		Ref[_StencilRef]
		Comp Always
		Pass Replace
	}

	Pass{
			Cull Off
			//don't draw color or depth
			Blend One One
			ZWrite Off

			CGPROGRAM
			

			#pragma vertex vert
			#pragma fragment frag
			#pragma multi_compile_instancing
			
			#include "UnityCG.cginc"


			struct appdata {
				float4 vertex : POSITION;
				UNITY_VERTEX_INPUT_INSTANCE_ID
			};

			struct v2f {
				//float4 position : SV_POSITION;
				float4 vertex : SV_POSITION;
				UNITY_VERTEX_OUTPUT_STEREO
			};

			v2f vert(appdata v) {
				v2f o;
				UNITY_SETUP_INSTANCE_ID(v);
				UNITY_INITIALIZE_OUTPUT(v2f, o);
				UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);
				//calculate the position in clip space to render the object
				o.vertex = UnityObjectToClipPos(v.vertex);
				return o;
			}

			fixed4 frag(v2f i) : SV_TARGET{
				return 0;
			}

			ENDCG
		}
}
	
}