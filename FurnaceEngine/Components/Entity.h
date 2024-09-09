#pragma once

#include "ComponentsCommon.h"

namespace dx12engine 
{
#define INIT_INFO(COMPONENT) namespace COMPONENT {struct init_info;}
	
INIT_INFO(transform)

#undef INIT_INFO
	namespace game_entity 
	{
		struct entity_info 
		{
			transform::init_info* transform{ nullptr };
		};

		entity create_game_entity(const entity_info& info);
		void remove_game_entity(entity id);
		bool is_alive(entity id);
	}
}