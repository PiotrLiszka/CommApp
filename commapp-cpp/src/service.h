#pragma once

#include "amqp/channel.h"

namespace boost::asio
{
class io_context;
using io_service = io_context;
} // namespace boost::asio

class service
{
public:
    service(boost::asio::io_service& service,
            const amqp::connection_details& connection_details);

    void consume(amqp::consumer_callback callback);
    void publish(const std::string& message);
    void run();
    void stop();
    bool stopped() const;

private:
    boost::asio::io_service& _io_service;
    amqp::channel _channel;
};