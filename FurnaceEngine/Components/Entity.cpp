#include "entity.h"

furnace::entity furnace::entity::create()
{
    return static_cast<furnace::entity>(s_registry.create());
}

furnace::entity::~entity()
{
}

void furnace::entity::destroy()
{
    s_registry.destroy(m_id);
}
