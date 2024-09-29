#pragma once

#include <vector>
#include <functional>
#include <utility>
#include <mutex>

namespace furnace::utl 
{
    template<typename... Args>
    class action {
    public:
        using callback_type = std::function<void(Args...)>;

        action& operator+=(callback_type callback) {
            subscribe(callback);
            return *this;
        }

        action& operator-=(callback_type callback) {
            unsubscribe(callback);
            return *this;
        }

        void subscribe(callback_type callback) {
            std::lock_guard<std::mutex> lock(m_mutex);
            m_callbacks.push_back(callback);
        }

        void unsubscribe(callback_type callback) {
            std::lock_guard<std::mutex> lock(m_mutex);
            auto it = std::remove_if(m_callbacks.begin(), m_callbacks.end(),
                [&](const callback_type& storedCallback) {
                    return storedCallback.target_type() == callback.target_type();
                });
            if (it != m_callbacks.end()) {
                m_callbacks.erase(it, m_callbacks.end());
            }
        }

        // Invoke all callbacks
        void invoke(Args... args) const { // Make this method const
            for (const auto& callback : m_callbacks) {
                if (callback) {
                    callback(args...);
                }
            }
        }

        // Operator() to invoke like a function call
        void operator()(Args... args) {
            invoke(args...);
        }

    private:
        std::mutex m_mutex;
        std::vector<callback_type> m_callbacks;
    };
}