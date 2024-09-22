#pragma once

#include "..\..\ThirdParty\entt\src\entt\entt.hpp"
#include "..\Common\common.h"

namespace furnace
{
    class entity {
    public:
        entity() = delete;
        explicit entity(entt::entity e);
        explicit entity(ENTT_ID_TYPE e);
        ~entity() {};
        void destroy();
        bool is_alive() const;
        ENTT_ID_TYPE id() const;
        bool operator==(entity e) const;

        template <typename Type, typename... Args>
        inline decltype(auto) add_component(Args&&... args);

    private:
        inline static entt::registry s_registry;
        ENTT_ID_TYPE m_id;
    public:
        static entity create();
    };

    template <typename Type, typename... Args>
    inline decltype(auto) entity::add_component(Args&&... args)
    {
        return s_registry.emplace<Type>(m_id, std::forward<Args>(args)...);
    }
};
