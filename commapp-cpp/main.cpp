#include <src/amqp/channel.h>
#include <src/service.h>

int main()
{
    amqp::connection_details details{
      .address = "amqp://guest:guest@192.168.2.84",
      .exchange_name = "my-exchange",
      .exchange_type = AMQP::ExchangeType::fanout,
      .exchange_flags = AMQP::autodelete,
      .queue_name = "my-queue",
      .routing_key = "my-key"};

    boost::asio::io_service io_service(2);
    service service(io_service, details);

    service.consume(
      [](const std::string& message) { LOG_INFO(message.c_str()); });

    std::thread t1([&io_service, &service]() {
        std::string s;
        while(!service.stopped() && std::getline(std::cin, s))
            {
                if(s == "stop")
                    {
                        boost::asio::post(io_service,
                                          [&service, s]() { service.stop(); });
                        return;
                    }
                else
                    {
                        boost::asio::post(io_service, [&service, s]() {
                            service.publish(s);
                        });
                    }
            }
    });

    service.run();
    t1.join();

    return 0;
}
