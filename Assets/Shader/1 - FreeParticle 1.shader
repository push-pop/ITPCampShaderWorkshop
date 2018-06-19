
Shader "ITParticles/Mesh"
{
	Properties
	{
		_ColorLow("Color Slow Speed", Color) = (0, 0, 0.5, 0.3)
		_ColorHigh("Color High Speed", Color) = (1, 0, 0, 0.3)
		_HighSpeedValue("High speed Value", Range(0, 50)) = 25
	}

		SubShader
	{
		Pass
		{
		//Blend SrcAlpha one

		CGPROGRAM
		#pragma target 5.0

		#pragma vertex vert
		#pragma fragment frag

		#include "UnityCG.cginc"

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
		float3 norm;
	};

	// Pixel shader input
	struct PS_INPUT
	{
		float4 position : SV_POSITION;
		float4 color : COLOR;
	};

	struct VS_INPUT {
		float4 vertex   : POSITION;
		float4 texcoord : TEXCOORD0;
		uint   id       : SV_VertexID;
	};

	// Particle's data, shared with the compute shader
	StructuredBuffer<Particle> particleBuffer;
	StructuredBuffer<MeshData> meshDataBuffer;

	// Properties variables
	uniform float4 _ColorLow;
	uniform float4 _ColorHigh;
	uniform float _HighSpeedValue;

	int MeshIndexCount;

	// Vertex shader
	PS_INPUT vert(VS_INPUT v)
	{
		uint pIndex = v.id / MeshIndexCount;
		uint mIndex = v.id % MeshIndexCount;

		Particle p = particleBuffer[pIndex];

		float3 vert = meshDataBuffer[meshDataBuffer[mIndex].index].vert;
		float3 position = p.position + vert;
		PS_INPUT o = (PS_INPUT)0;

		// Color
		//o.color = lerp(_ColorLow, _ColorHigh, lerpValue);
		o.color = float4(1, 0, 0, 1);

		// Position
		o.position = UnityObjectToClipPos(float4(position, 1.0f));

		return o;
	}

	// Pixel shader
	float4 frag(PS_INPUT i) : COLOR
	{
		return i.color;
	}

	ENDCG
}
	}

		Fallback Off
}
