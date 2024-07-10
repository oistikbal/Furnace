#pragma once

#include "ComponentsCommon.h"

namespace dx12engine 
{
#define INIT_INFO(COMPONENT) namespace COMPONENT {struct init_info;}
	
INIT_INFO(transform)

#undef INIT_INFO
	namespace gameentity 
	{
		struct entity_info 
		{
			transform::init_info* transform{ nullptr };
		};

		u32 createGameEntity(const entity_info& info);
		void removeGameEntity(entity_id id);
		bool isAlive(entity_id id);
	}
}