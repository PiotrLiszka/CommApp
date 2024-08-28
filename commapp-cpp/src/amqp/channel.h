#pragma once

namespace amqp
{

struct connection_details
{
    AMQP::Address address;
    std::string exchange_name;
    AMQP::ExchangeType exchange_type;
    int exchange_flags;
    std::string queue_name;
    std::string routing_key;
};

using consumer_callback = std::function<void(const std::string& message)>;

class channel
{
public:
    channel(boost::asio::io_service& io_service,
            const connection_details& connection_details);
    ~channel();

    void consume(consumer_callback callback);
    void publish(const std::string& message);

private:
    const connection_details _connection_details;
    AMQP::LibBoostAsioHandler _handler;
    AMQP::TcpConnection _connection;
    AMQP::TcpChannel _channel;
};

} // namespace amqp