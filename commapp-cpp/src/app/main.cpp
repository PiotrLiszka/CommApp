#include "mainwindow.h"

#include <QApplication>
#include <QObject>
#include <QTextBrowser>
#include <QTimer>

#include <amqp/dispatcher.h>
#include <service/context.h>

int main(int argc, char** argv)
{
    QApplication app(argc, argv);

    amqp::connection_details connection_details{
      .address = "amqp://guest:guest@192.168.2.84",
      .exchange_name = "my-exchange",
      .exchange_type = AMQP::ExchangeType::fanout,
      .exchange_flags = AMQP::autodelete,
      .queue_name = "my-queue",
      .routing_key = "my-key"};

    LOG_ERROR("BEFORE INIT");
    context::init(std::move(connection_details));
    LOG_ERROR("AFTER INIT");

    // tick ?

    // QTextBrowser* tb = w.findChild<QTextBrowser*>("textBrowser");

    // service.consume(
    //   [tb](const std::string& message)
    //   {
    //       LOG_ERROR("huan");
    //       tb->append("\n" + QString(message.data()));
    //   });

    // service.consume([tb](const std::string& message) { LOG_ERROR("two"); });

    MainWindow w;
    w.show();
    return app.exec();
}
