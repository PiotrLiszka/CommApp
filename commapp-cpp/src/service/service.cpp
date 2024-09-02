#include "service.h"

namespace service
{
basic_service::basic_service(boost::asio::io_service& io_service,
                             const amqp::connection_details& connection_details)
: _channel(io_service, connection_details)
{
}

void basic_service::init()
{
    _channel.declare_consumers();
}

void basic_service::consume(amqp::consumer_callback callback)
{
    _channel.consume(std::move(callback));
}

void basic_service::publish(const std::string& message)
{
    _channel.publish(message);
}
} // namespace service