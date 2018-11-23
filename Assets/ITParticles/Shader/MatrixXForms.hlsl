
float4x4 calcRotatefloat4x4(float3 radian) {
	return calcRotatefloat4x4X(radian.x) * calcRotatefloat4x4Y(radian.y) * calcRotatefloat4x4Z(radian.z);
}

float4x4 calcRotatefloat4x4X(float radian) {
	return float4x4(
		1.0, 0.0, 0.0, 0.0,
		0.0, cos(radian), -sin(radian), 0.0,
		0.0, sin(radian), cos(radian), 0.0,
		0.0, 0.0, 0.0, 1.0
		);
}

float4x4 calcRotatefloat4x4Y(float radian) {
	return float4x4(
		cos(radian), 0.0, sin(radian), 0.0,
		0.0, 1.0, 0.0, 0.0,
		-sin(radian), 0.0, cos(radian), 0.0,
		0.0, 0.0, 0.0, 1.0
		);
}

float4x4 calcRotatefloat4x4Z(float radian) {
	return float4x4(
		cos(radian), -sin(radian), 0.0, 0.0,
		sin(radian), cos(radian), 0.0, 0.0,
		0.0, 0.0, 1.0, 0.0,
		0.0, 0.0, 0.0, 1.0
		);
}

float4x4 calcScalefloat4x4(float3 scale) {
	return float4x4(
		scale.x, 0.0, 0.0, 0.0,
		0.0, scale.y, 0.0, 0.0,
		0.0, 0.0, scale.z, 0.0,
		0.0, 0.0, 0.0, 1.0
		);
}

float4x4 calcTranslatefloat4x4(float3 v) {
	return float4x4(
		1.0, 0.0, 0.0, 0.0,
		0.0, 1.0, 0.0, 0.0,
		0.0, 0.0, 1.0, 0.0,
		v.x, v.y, v.z, 1.0
		);
}