#include "channel.h"

namespace amqp
{
channel::channel(boost::asio::io_service& io_service,
                 const connection_details& connection_details)
: _connection_details(connection_details),
  _handler(io_service),
  _connection(&_handler, _connection_details.address),
  _channel(&_connection)
{
    _channel.onReady([]() { LOG_OK("AMQP channel ready"); });
    _channel.onError(
      [](const char* msg) { LOG_ERROR("AMQP channel error!", msg); });

    _channel
      .declareExchange(_connection_details.exchange_name,
                       _connection_details.exchange_type,
                       _connection_details.exchange_flags)
      .onSuccess([]() { LOG_OK("AMQP exchange declared"); })
      .onError([](const char* msg) {
          LOG_ERROR("AMQP exchange declare failure!", msg);
      });

    _channel.declareQueue(_connection_details.queue_name)
      .onSuccess([]() { LOG_OK("AMQP queue declared"); })
      .onError([](const char* msg) {
          LOG_ERROR("AMQP queue declare failure!", msg);
      });

    _channel
      .bindQueue(_connection_details.exchange_name,
                 _connection_details.queue_name,
                 _connection_details.routing_key)
      .onSuccess([]() { LOG_OK("AMQP queue bound!"); })
      .onError(
        [](const char* msg) { LOG_ERROR("AMQP queue bind failure!", msg); });
}

channel::~channel()
{
    _channel.close();
    _connection.close();
}

void channel::consume(consumer_callback callback)
{
    auto onReceived
      = [this,
         callback](const AMQP::Message& message, uint64_t deliveryTag, bool) {
            callback(std::string(message.body(), message.bodySize()));
            _channel.ack(deliveryTag);
        };

    _channel.consume(_connection_details.queue_name)
      .onReceived(onReceived)
      .onCancelled([](auto) { LOG_WARNING("Consume success"); })
      .onError(
        [](const char* msg) { LOG_ERROR("Could not consume message!", msg); });
}

void channel::publish(const std::string& message)
{
    _channel.startTransaction();
    _channel.publish(_connection_details.exchange_name,
                     _connection_details.routing_key,
                     message);
    _channel.commitTransaction().onError(
      [](const char* msg) { LOG_ERROR("Message publish error: ", msg); });
}
} // namespace amqp