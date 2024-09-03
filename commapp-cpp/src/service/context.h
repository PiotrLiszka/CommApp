#pragma once

#include <amqp/dispatcher.h>
#include <service/qservice.h>

class context
{
public:
    static service::qservice& getInstance()
    {
        return getInstanceImpl()._service;
    }

    static void init(amqp::connection_details connection_details)
    {
        getInstanceImpl(&connection_details);
    }

private:
    explicit context(const amqp::connection_details* connection_details)
    : _connection_details(std::move([connection_details](){ assert(connection_details); return *connection_details; }()))
      , _service(nullptr, _connection_details)
    {}

    static context& getInstanceImpl(const amqp::connection_details* connection_details = nullptr)
    {
        static context instance{connection_details};
        return instance;
    }

    context(context const&) = delete;
    void operator=(context const&) = delete;

    amqp::connection_details _connection_details;
    service::qservice _service;
};
