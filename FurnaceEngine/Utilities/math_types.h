#pragma once
namespace furnace::math {
	constexpr float PI = 3.1415926535897932384626433832795f;
	constexpr float hPI = PI * 0.5f;
	constexpr float TAU = 2.0f * PI;
	constexpr float EPSILON = 1e-10f;
#if defined(_WIN64)
#include <DirectXMath.h>
	using vec2 = DirectX::XMFLOAT2;
	using vec2a = DirectX::XMFLOAT2A;
	using vec3 = DirectX::XMFLOAT3;
	using vec3a = DirectX::XMFLOAT3A;
	using vec4 = DirectX::XMFLOAT4;
	using vec4a = DirectX::XMFLOAT4A;
	using ivec2 = DirectX::XMUINT2;
	using ivec3 = DirectX::XMUINT3;
	using ivec4 = DirectX::XMUINT4;
	using svec2 = DirectX::XMINT2;
	using svec3 = DirectX::XMINT3;
	using svec4 = DirectX::XMINT4;
	using mat3 = DirectX::XMFLOAT3X3;
	using mat4 = DirectX::XMFLOAT4X4;
	using mat4a = DirectX::XMFLOAT4X4A;

#endif
}