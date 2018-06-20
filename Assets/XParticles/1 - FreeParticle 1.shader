// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "XParticle/1 - FreeParticle (Billboard Quad)"
{
	Properties
	{
		_ColorLow("Color Slow Speed", Color) = (0, 0, 0.5, 0.3)
		_ColorHigh("Color High Speed", Color) = (1, 0, 0, 0.3)
		[NoScaleOffset]_AlphaMap("Alpha Map", 2D) = "white" {}
		_HighSpeedValue("High speed Value", Range(0, 50)) = 25
		_Size("Size", Range(0,3)) = 0.5
	}

		SubShader
	{
		Pass
	{
		//Blend SrcAlpha OneMinusSrcAlpha // Traditional transparency
		Blend One One // Additive
		//Blend OneMinusDstColor One // Soft Additive
		
		Cull Off // Draw both sides of quad

		ZWrite Off // Don't clip particles behind

		Tags{ "Queue" = "Transparent+1" "IgnoreProjector" = "True" "ForceNoShadowCasting" = "True" "DisableBatching" = "True" }

		CGPROGRAM
#pragma target 5.0

#pragma vertex vert
#pragma fragment frag
#pragma geometry geo

#include "UnityCG.cginc"

	// Geo Shader Input
	struct GS_INPUT
	{
		float4	pos: POSITION;
		float3	normal	: NORMAL;
		float4 color : COLOR;
	};
	
	// Particle's data
	struct Particle
	{
		float3 position;
		float3 velocity;
	};

	// Pixel shader input
	struct FS_INPUT
	{
		float4 pos : SV_POSITION;
		float4 color : COLOR;
		float2  uv	: TEXCOORD0;

	};

	// Particle's data, shared with the compute shader
	StructuredBuffer<Particle> particleBuffer;

	// Properties variables
	uniform float4 _ColorLow;
	uniform float4 _ColorHigh;
	uniform float _HighSpeedValue;
	half _Size;
	sampler2D _AlphaMap;

	// Vertex shader
	GS_INPUT vert(uint vertex_id : SV_VertexID, uint instance_id : SV_InstanceID)
	{
		GS_INPUT o;
		UNITY_INITIALIZE_OUTPUT(GS_INPUT, o);
		// Color
		float speed = length(particleBuffer[instance_id].velocity);
		float lerpValue = clamp(speed / _HighSpeedValue, 0.0f, 1.0f);
		o.color = lerp(_ColorLow, _ColorHigh, lerpValue);

		// world Position
		o.pos = float4(particleBuffer[instance_id].position, 1.0f);
		
		return o;
	}

	[maxvertexcount(4)]
	void geo(point GS_INPUT p[1], inout TriangleStream<FS_INPUT> triStream)
	{
		float3 up = float3(0, 1, 0);
		float3 look = _WorldSpaceCameraPos - p[0].pos;
		look.y = 0;
		look = normalize(look);
		float3 right = cross(up, look);

		float halfS = 0.5f * _Size;

		float4 v[4];
		v[0] = float4(p[0].pos + halfS * right - halfS * up, 1.0f);
		v[1] = float4(p[0].pos + halfS * right + halfS * up, 1.0f);
		v[2] = float4(p[0].pos - halfS * right - halfS * up, 1.0f);
		v[3] = float4(p[0].pos - halfS * right + halfS * up, 1.0f);

		// Append Quad Vertices
		FS_INPUT pIn;
		pIn.pos = UnityObjectToClipPos(v[0]);
		pIn.uv = float2(1.0, 0.0);
		pIn.color = p[0].color;
		triStream.Append(pIn);

		pIn.pos = UnityObjectToClipPos(v[1]);
		pIn.uv = float2(1.0, 1.0);
		pIn.color = p[0].color;
		triStream.Append(pIn);

		pIn.pos = UnityObjectToClipPos(v[2]);
		pIn.uv = float2(0.0, 0.0);
		pIn.color = p[0].color;
		triStream.Append(pIn);

		pIn.pos = UnityObjectToClipPos(v[3]);
		pIn.uv = float2(0.0, 1.0);
		pIn.color = p[0].color;
		triStream.Append(pIn);
	}

	// Pixel shader
	float4 frag(FS_INPUT i) : COLOR
	{
		float4 color = i.color;
		float4 alpha = tex2D(_AlphaMap, i.uv);

		//Use this for alpha blend
		//color.a *= alpha.a;

		//Use this for additive blend
		i.color.rgb *= alpha.a*color.a;

		return i.color;
	}

		ENDCG
	}
	}

		Fallback Off
}