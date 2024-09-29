#pragma once
#include "buffers_generated.h"
#include "..\Utilities\action.h"

namespace furnace::mmf
{
	void initialize();
	void send(flatbuffers::FlatBufferBuilder& builder);
	extern furnace::utl::action<const Furnace::Buffers::MessageWrapper&> on_message_received;
}