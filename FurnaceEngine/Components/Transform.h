#pragma once
#include "..\Common\common.h"

namespace furnace::component 
{
	struct transform
	{
		f32 position[3];
		f32 rotation[3];
		f32 scale[3];
	};
}