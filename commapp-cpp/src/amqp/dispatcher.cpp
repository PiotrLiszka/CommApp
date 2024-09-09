#include <amqp/dispatcher.h>

namespace amqp
{
dispatcher::dispatcher(boost::asio::io_service& io_service,
                       const connection_details& connection_details)
: _connection_details(connection_details),
  _handler(io_service),
  _connection(&_handler, _connection_details.address),
  _consumer_channel(&_connection),
  _publisher_channel(&_connection)
{
    _consumer_channel.onReady(
      [this]()
      {
          LOG_OK("Consumer: AMQP channel ready");

          _consumer_channel
            .declareExchange(_connection_details.exchange_name,
                             _connection_details.exchange_type,
                             _connection_details.exchange_flags)
            .onSuccess([]() { LOG_OK("Consumer: AMQP exchange declared"); })
            .onError(
              [](const char* msg)
              { LOG_ERROR("Consumer: AMQP exchange declare failure!", msg); });

          _consumer_channel.declareQueue(_connection_details.queue_name)
            .onSuccess([]() { LOG_OK("Consumer: AMQP queue declared"); })
            .onError(
              [](const char* msg)
              { LOG_ERROR("Consumer: AMQP queue declare failure!", msg); });

          _consumer_channel
            .bindQueue(_connection_details.exchange_name,
                       _connection_details.queue_name,
                       _connection_details.routing_key)
            .onSuccess([]() { LOG_OK("Consumer: AMQP queue bound!"); })
            .onError(
              [](const char* msg)
              { LOG_ERROR("Consumer: AMQP queue bind failure!", msg); });

          auto onReceived
            = [this](const AMQP::Message& message, uint64_t deliveryTag, bool)
          {
              for(consumer_callback& callback : _consumer_callbacks)
              {
                  callback(std::string(message.body(), message.bodySize()));
              }
              _consumer_channel.ack(deliveryTag);
          };

          _consumer_channel.consume(_connection_details.queue_name)
            .onReceived(onReceived)
            .onCancelled([](auto)
                         { LOG_WARNING("Consumer: Consume cancelled"); })
            .onError([](const char* msg)
                     { LOG_ERROR("Consumer: Consume error! ", msg); });
      });
    _consumer_channel.onError([](const char* msg)
                              { LOG_ERROR("AMQP channel error!", msg); });

    // PUBLISHER CHANNEL
    _publisher_channel.onReady(
      [this]()
      {
          LOG_OK("Publisher: AMQP channel ready");
          _publisher_channel
            .declareExchange(_connection_details.exchange_name,
                             _connection_details.exchange_type,
                             _connection_details.exchange_flags)
            .onSuccess([]() { LOG_OK("Publisher: AMQP exchange declared"); })
            .onError(
              [](const char* msg) {
                  LOG_ERROR("Publisher: AMQP exchange declare failure!", msg);
              });

          // _publisher_channel.declareQueue(_connection_details.queue_name)
          //   .onSuccess([]() { LOG_OK("Publisher: AMQP queue declared"); })
          //   .onError(
          //     [](const char* msg)
          //     { LOG_ERROR("Publisher: AMQP queue declare failure!", msg); });

      });
    _publisher_channel.onError(
      [](const char* msg)
      { LOG_ERROR("Publisher: AMQP channel error!", msg); });
}

dispatcher::~dispatcher()
{
    _consumer_callbacks.clear();
    _publisher_channel.close();
    _consumer_channel.close();
    _connection.close();
}

void dispatcher::consume(consumer_callback callback)
{

    _consumer_callbacks.push_back(std::move(callback));
}

void dispatcher::publish(const std::string& message, const std::string& routing_key)
{
    _publisher_channel.startTransaction();
    _publisher_channel.publish(_connection_details.exchange_name,
                               //_connection_details.routing_key,
                               routing_key,
                               message);
    _publisher_channel.commitTransaction().onError(
      [](const char* msg)
      { LOG_ERROR("Publisher: Message publish error: ", msg); });
}

bool dispatcher::opened() const
{
    return _consumer_channel.ready() && _consumer_channel.usable()
           && _publisher_channel.ready() && _publisher_channel.usable();
}
} // namespace amqp
