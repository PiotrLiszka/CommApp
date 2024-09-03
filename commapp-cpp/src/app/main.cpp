#include "mainwindow.h"

#include <QApplication>
#include <QDebug>
#include <QMap>
#include <QObject>
#include <QPainter>
#include <QPainterPath>
#include <QTextBrowser>
#include <QTimer>

#include <amqp/channel.h>
#include <service/service.h>

class MyObject : public QObject
{
    Q_OBJECT

public:
    MyObject(QObject* parent = nullptr) : QObject(parent)
    {
    }

    void tick(boost::asio::io_service& srv)
    {
        auto timer = new QTimer(this->parent());
        connect(timer, &QTimer::timeout, [&srv] { srv.poll(); });
        timer->start();
    }
};

#include "main.moc"

int main(int argc, char** argv)
{
    QApplication app(argc, argv);

    amqp::connection_details details{
      .address = "amqp://guest:guest@192.168.2.84",
      .exchange_name = "my-exchange",
      .exchange_type = AMQP::ExchangeType::fanout,
      .exchange_flags = AMQP::autodelete,
      .queue_name = "my-queue",
      .routing_key = "my-key"};

    boost::asio::io_service io_service(2);
    service::basic_service service(io_service, details);

    MainWindow w(nullptr, service);
    w.show();
    QTextBrowser* tb = w.findChild<QTextBrowser*>("textBrowser");

    service.consume([tb](const std::string& message)
                    { tb->append("\n" + QString(message.data())); });

    std::thread t1(
      [&io_service, &service]()
      {
          std::string s;
          while(!io_service.stopped() && std::getline(std::cin, s))
          {
              if(s == "stop")
              {
                  boost::asio::post(io_service,
                                    [&io_service, s]() { io_service.stop(); });
                  return;
              }
              else
              {
                  boost::asio::post(io_service,
                                    [&service, s]() { service.publish(s); });
              }
          }
      });

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

    // service::run(service, io_service);
    // t1.join();

    // io_service.poll();

    MyObject m;
    m.tick(io_service);

    const int exec_result = app.exec();
    t1.join();
    io_service.stop();

    return exec_result;
}
