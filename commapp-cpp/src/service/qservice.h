#ifndef QSERVICE_H
#define QSERVICE_H

#include <QObject>

#include <amqp/dispatcher.h>

namespace service
{
class qservice : public QObject
{
    Q_OBJECT
public:
    explicit qservice(QObject* parent, const amqp::connection_details& connection_details);
    ~qservice();

    amqp::dispatcher& dispatcher();

signals:

private:
    boost::asio::io_service _io_service;
    amqp::dispatcher _dispatcher;
};
} // namespace service

#endif // QSERVICE_H
