#pragma once

#include "amqp/dispatcher.h"

#include <boost/asio/deadline_timer.hpp>
#include <boost/asio/io_service.hpp>
#include <boost/asio/signal_set.hpp>
#include <boost/asio/strand.hpp>
#include <boost/system/error_code.hpp>

namespace service
{
class basic_service;

template <typename Service,
          typename = typename std::enable_if<
            std::is_base_of_v<
              basic_service, Service> || std::is_same_v<basic_service, Service>,
            Service>::type>
[[nodiscard]] inline uint64_t
run(Service& service, boost::asio::io_service& io_service)
{
    try
    {
        boost::asio::signal_set io_signals(io_service, SIGINT, SIGTERM);
        io_signals.async_wait(
          [&io_service](const boost::system::error_code& error_code,
                        int /*signal_number*/)
          {
              if(!error_code)
              {
                  LOG_ERROR("Error code received! %s", error_code.what());
                  io_service.stop();
              }
          });

        service.init();

        return io_service.run();
    }
    catch(const std::exception& e)
    {
        LOG_ERROR("Service run failure! %s", e.what());
        return EXIT_FAILURE;
    }

    return EXIT_SUCCESS;
}

class basic_service
{
public:
    basic_service(boost::asio::io_service& io_service,
                  const amqp::connection_details& connection_details);

    void consume(amqp::consumer_callback callback);
    void publish(const std::string& message);

private:
    amqp::dispatcher _dispatcher;
};
} // namespace service
