
Shader "ITParticles/MeshLitSimple"
{
	Properties
	{
		_ColorLow ("Color Slow Speed", Color) = (0, 0, 0.5, 0.3)
		_ColorHigh ("Color High Speed", Color) = (1, 0, 0, 0.3)
		_HighSpeedValue ("High speed Value", Range(0, 50)) = 25
		_Scale ("Particle Scale", Range(0.001, 1)) = 0.05
		_Angle("Angle", Vector) = (0,0,0)
	}

	SubShader 
	{

			//Blend SrcAlpha one

			CGPROGRAM
			#pragma target 5.0
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
			
			float _Scale;
			float3 _Angle;

			int MeshIndexCount;



			// Vertex shader
			void vert(inout appdata v, out Input o)
			{
				uint pIndex = v.id / MeshIndexCount;
				uint mIndex = v.id % MeshIndexCount;
#ifdef SHADER_API_D3D11

				Particle p = particleBuffer[pIndex];
				MeshData mesh = meshDataBuffer[meshDataBuffer[mIndex].index];
				

				//float3 v1 = float3(0,1,0);
				//float3 v2 = normalize(p.velocity);
				//float angle = acos(dot(v2, v1));
				//angle = _Time.y;
				//float3 axis = normalize(cross(v2, v1));
				//axis = float3(0, 1, 0);
				//float3x3 rot = rotationMatrix(axis, angle);


				float3 vert = mesh.vert;
				float3 norm = mesh.norm;

				//if (angle > 0.001) {
				//	vert = mul(rot, vert);
				//	norm = mul(transpose(rot), mesh.norm);
				//}
				float3 position = p.position;
				position += _Scale * vert;

				o = (Input)0;
				float hue = fmod(10 * _Time.x + v.id / 100 + 0.05, 1.0f);

				float3 rgb = HUEtoRGB(hue);

				// Color
				v.color = float4(rgb,1);
				//o.color = float4(position,1);
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
				

				o.Albedo = _ColorLow + fmod( _Time.xyz, 255.0)/255.0;
				//o.Albedo = IN.color;
				//o.Albedo = IN.vel*10;
				//o.Albedo = max(length(IN.vel.rgb)*float3(.4,.4,1), IN.color.rgb);

				o.Smoothness = 1;
				o.Metallic = .3;
				o.Emission = length(IN.vel)*.6*_ColorHigh;
				o.Alpha = 1;
			}
			
			ENDCG
		}
	

	Fallback "Diffuse"
}
