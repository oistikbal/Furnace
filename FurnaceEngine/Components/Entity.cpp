#include "entity.h"

furnace::entity furnace::entity::create()
{
    return static_cast<entity>(s_registry.create());
}

furnace::entity::entity(entt::entity id)
{
    m_id = static_cast<ENTT_ID_TYPE>(id);
}

furnace::entity::entity(ENTT_ID_TYPE e)
{
    m_id = e;
}

void furnace::entity::destroy()
{
    assert(is_alive());
    s_registry.destroy(static_cast<entt::entity>(m_id));
}

bool furnace::entity::is_alive() const
{
    return s_registry.valid(static_cast<entt::entity>(m_id));
}

ENTT_ID_TYPE furnace::entity::id() const
{
    return static_cast<ENTT_ID_TYPE>(m_id);
}

bool furnace::entity::operator==(entity e) const
{
    return m_id == e.m_id;
}
