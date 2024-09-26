#include "mainwindow.h"

#include <QApplication>
#include <QObject>
#include <QTextBrowser>
#include <QTimer>

#include <login/login_dialog.h>

#include <amqp/dispatcher.h>
#include <service/context.h>

int main(int argc, char** argv)
{
    QApplication app(argc, argv);

    MainWindow w;
    w.show();
    w.setEnabled(false);

    login_dialog login_d;
    login_d.show();
    login_d.set_when_accepted([&w](const QString& login){
        amqp::connection_details connection_details{
          .address = "amqp://guest:guest@192.168.2.84",
          .exchange_name = "commapp-exchange",
          .exchange_type = AMQP::ExchangeType::direct,
          .exchange_flags = AMQP::autodelete,
          .queue_name = "commapp-queue-" + login.toStdString(),
          .routing_key = login.toStdString()};

        context::init(std::move(connection_details));

        w.init(login.toStdString());
        w.setEnabled(true);
    });
    login_d.set_when_rejected([&w](){ w.close();});


    return app.exec();
}
