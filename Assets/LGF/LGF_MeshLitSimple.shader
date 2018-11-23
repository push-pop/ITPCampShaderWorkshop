
Shader "LGF/Simple"
{
	Properties
	{
		_ColorLow ("Color Slow Speed", Color) = (0, 0, 0.5, 0.3)
		_ColorHigh ("Color High Speed", Color) = (1, 0, 0, 0.3)
		_HighSpeedValue ("High speed Value", Range(0, 50)) = 25
		_Scale ("Particle Scale", Range(0.001, 3)) = 0.05
		_Angle("Angle", Vector) = (0,0,0,0)
		_Cube("Cubemap", CUBE) = "" {}

	}

	SubShader 
	{

			//Blend SrcAlpha one
			//Cull Off
			CGPROGRAM
			#pragma target 3.5
#pragma surface surf Standard addshadow fullforwardshadows vertex:vert  

#include "RotationMatrix.hlsl"
			#include "UnityCG.cginc"
#include "ColorSpaceConversion.hlsl"
			// Particle's data
			struct Particle
			{
				float3 position;
				float3 velocity;
			};
			
			// Mesh's data
			struct MeshData
			{
				//float4x4 mat;
				float3 gNorm;
				float3	vert;
				float2	uv;
				int		index;
				float3  norm;
			};
			
			// Pixel shader input
			struct Input
			{
				float4 position : SV_POSITION;
				float4 color : COLOR;
				float3 vel : TEXCOORD0;
				float3 normal : NORMAL;
				float3 worldRefl;

			};

			struct appdata {
				float4 vertex   : POSITION;
				float4 texcoord : TEXCOORD0;
				float4 texcoord1 : TEXCOORD1;
				float4 texcoord2 : TEXCOORD2;
				float4 color : COLOR;
				uint   id       : SV_VertexID;
				float3 normal : NORMAL;
				float4 tangent : TANGENT;
			};
			
#ifdef SHADER_API_D3D11

			// Particle's data, shared with the compute shader
			StructuredBuffer<Particle> particleBuffer;
			StructuredBuffer<MeshData> meshDataBuffer;
#endif
			// Properties variables
			uniform float4 _ColorLow;
			uniform float4 _ColorHigh;
			uniform float _HighSpeedValue;
			samplerCUBE _Cube;

			float _Scale;
			float4 _Angle;

			int MeshIndexCount;



			// Vertex shader
			void vert(inout appdata v, out Input o)
			{
				uint pIndex = v.id / MeshIndexCount;
				uint mIndex = v.id % MeshIndexCount;
#ifdef SHADER_API_D3D11

				Particle p = particleBuffer[pIndex];
				MeshData mesh = meshDataBuffer[v.id];// meshDataBuffer[mIndex].index];
				

				float3 vert = mesh.vert;
				float3 norm = mesh.norm;
				float3 gNorm = mesh.gNorm;

				//float3 v1 = _Angle.xyz;// float3(0, 0, 0);
				//float3 v2 = normalize(norm);// p.velocity);
				//float angle = acos(dot(v2, v1));
				//angle = _Angle.w;// _Time.y;
				//float3 axis = normalize(cross(v2, v1));
				float3 axis =  gNorm;// float3(0, 0, 1);
				float3x3 rot = rotationMatrix(axis, _Angle.w);



				//if (angle > 0.001) {
					vert = mul(rot, vert);
					norm = mul((rot), mesh.norm);
				//}
				float3 position = p.position;
				position += gNorm * _Angle.x * sin(_Angle.z*_Time.z+pIndex*.1);
				position += _Angle.y * (mesh.uv.x) * mesh.gNorm;
				position += _Scale * vert;

				o = (Input)0;
				float hue = fmod(10 * _Time.x + v.id / 100 + 0.05, 1.0f);

				float3 rgb = HUEtoRGB(hue);

				// Color
				v.color = lerp(v.color, v.color*.4, mesh.uv.x);
				o.color = mesh.uv.x;// lerp(v.color, float4(0, 0, 0, 1), mesh.uv.x);// float4(position, 1);
				o.vel = p.velocity;
				o.normal = norm;
				// Position
				v.vertex.xyz = position;
				v.normal = norm;
				//o.position = UnityObjectToClipPos(v.vertex);
#endif
			}

			// Pixel shader
			void surf(Input IN, inout SurfaceOutputStandard o)
			{
				

				o.Albedo = IN.color + (IN.color * texCUBE(_Cube, IN.worldRefl).rgb);// lerp(IN.color, fixed4(1, 1, 1, 1), .5);// _ColorLow + fmod(_Time.xyz, 255.0) / 255.0;
				//o.Albedo = IN.color;
				//o.Albedo = IN.vel*10;
				//o.Albedo = max(length(IN.vel.rgb)*float3(.4,.4,1), IN.color.rgb);
				//o.Emission = texCUBE(_Cube, IN.worldRefl).rgb;

				o.Smoothness = 1;
				o.Metallic = .3;
				//o.Emission = fixed4(1, 1, 1, 1);// length(IN.vel)*.6*_ColorHigh;
				o.Alpha = 1;
			}
			
			ENDCG
		}
	

	Fallback "Diffuse"
}
