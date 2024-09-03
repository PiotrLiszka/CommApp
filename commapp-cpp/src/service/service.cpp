#include "service.h"

namespace service
{
basic_service::basic_service(boost::asio::io_service& io_service,
                             const amqp::connection_details& connection_details)
: _dispatcher(io_service, connection_details)
{
}

void basic_service::consume(amqp::consumer_callback callback)
{
    _dispatcher.consume(std::move(callback));
}

void basic_service::publish(const std::string& message)
{
    _dispatcher.publish(message);
}
} // namespace service