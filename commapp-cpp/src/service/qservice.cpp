#include "qservice.h"

#include <QTimer>

namespace service {
qservice::qservice(QObject* parent, const amqp::connection_details& connection_details)
: QObject{parent}, _io_service(2), _dispatcher(_io_service, connection_details)
{
    boost::asio::signal_set io_signals(_io_service, SIGINT, SIGTERM);
    io_signals.async_wait(
      [this](const boost::system::error_code& error_code,
             int /*signal_number*/)
      {
          if(!error_code)
          {
              LOG_ERROR("Error code received! %s", error_code.what());
              _io_service.stop();
          }
      });

    auto timer = new QTimer(this->parent());
    connect(timer,
            &QTimer::timeout,
            [this]
            {
                _io_service.poll();
            });
    timer->start();
}

qservice::~qservice()
{
    _io_service.stop();
}

amqp::dispatcher& qservice::dispatcher()
{
    return _dispatcher;
}
}
