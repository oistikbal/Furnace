#include "..\Common\common.h"
#include "api.h"

namespace api
{
	FURNACE_API u32 create()
	{
		return furnace::entity::create().id();
	}
	FURNACE_API void destroy(u32 id)
	{
		static_cast<furnace::entity>(id).destroy();
	}
	FURNACE_API bool is_alive(u32 id)
	{
		return static_cast<furnace::entity>(id).is_alive();
	}	
};