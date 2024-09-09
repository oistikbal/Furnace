#pragma once
#include "ComponentsCommon.h"

namespace dx12engine::transform {
	DEFINE_TYPE_ID(transform_id);
	struct init_info
	{
		f32 position[3];
		f32 rotation[4];
		f32 scale[3]{ 1.f,1.f,1.f };
	};

	component create_transform(const init_info& info, game_entity::entity e);
	void remove_transform(transform_id id);
}