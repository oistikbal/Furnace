#pragma once
#include "common.h"
#include "..\Utilities\common.h"

namespace furnace::component
{
	struct transform : component
	{
		math::vec4 rotation;
		math::vec3 orientation;
		math::vec3 position;
		math::vec3 scale;
	};
}
