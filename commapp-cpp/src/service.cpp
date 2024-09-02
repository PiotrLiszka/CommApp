#include "service.h"

service::service(boost::asio::io_service& io_service,
                 const amqp::connection_details& connection_details)
: _io_service(io_service),
  _channel(io_service, connection_details)
{}

void service::consume(amqp::consumer_callback callback)
{
    _channel.consume(std::move(callback));
}

void service::publish(const std::string& message)
{
    _channel.publish(message);
}

void service::run()
{
    _io_service.run();
}

void service::stop()
{
    _io_service.stop();
}

bool service::stopped() const
{
    return _io_service.stopped();
}