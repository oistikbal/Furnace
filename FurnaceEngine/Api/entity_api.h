#pragma once
#include "..\Common\types.h"
#include "api.h"

namespace api
{
	FURNACE_API u32 create();
	FURNACE_API void destroy(u32 id);
	FURNACE_API bool is_alive(u32 id);
};