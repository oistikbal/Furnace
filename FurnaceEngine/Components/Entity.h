#pragma once

#include "..\..\ThirdParty\entt\single_include\entt\entt.hpp"
#include "..\Common\common.h"

namespace furnace
{
    class entity {
    public:
        entity() = delete;
        entity static create();
        ~entity();
        void destroy();
        template <typename Type, typename... Args>
        inline decltype(auto) add_component(Args&&... args);


    private:
        explicit entity(entt::entity id);
    private:
        entt::entity m_id;
        static inline entt::registry s_registry;

    };

    template <typename Type, typename... Args>
    inline decltype(auto) entity::add_component(Args&&... args)
    {
        return s_registry.emplace<Type>(m_id, std::forward<Args>(args)...);
    }
};
