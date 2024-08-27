/**
 *  LibBoostAsio.cpp
 * 
 *  Test program to check AMQP functionality based on Boost's asio io_service.
 * 
 *  @author Gavin Smith <gavin.smith@coralbay.tv>
 *
 *  Compile with g++ -std=c++14 libboostasio.cpp -o boost_test -lpthread -lboost_system -lamqpcpp
 */

/**
 *  Dependencies
 */
#include <boost/asio/io_service.hpp>
#include <boost/asio/strand.hpp>
#include <boost/asio/deadline_timer.hpp>


#include <amqpcpp.h>
#include <amqpcpp/libboostasio.h>

/**
 *  Main program
 *  @return int
 */
int main()
{

    // access to the boost asio handler
    // note: we suggest use of 2 threads - normally one is fin (we are simply demonstrating thread safety).
    boost::asio::io_service service(2);

    // handler for libev
    AMQP::LibBoostAsioHandler handler(service);
    
    // make a connection
    AMQP::TcpConnection connection(&handler, AMQP::Address("amqp://guest:guest@192.168.2.84/"));
    
    // we need a channel too
    AMQP::TcpChannel channel(&connection);
    channel.onReady([](){
        std::cout << "channel ready" << std::endl;
    });
    channel.onError([](const char *message){
        std::cout << "channel build error: " << message << std::endl;
    });
    
    // create a temporary queue
    // channel.declareQueue(AMQP::exclusive).onSuccess([&connection](const std::string &name, uint32_t messagecount, uint32_t consumercount) {
        
    //     // report the name of the temporary queue
    //     std::cout << "declared queue " << name << std::endl;
        
    //     // now we can close the connection
    //     connection.close();
    // });

    channel.declareExchange("my-exchange", AMQP::fanout).onSuccess([](){
        std::cout << "exchange declared" << std::endl;
    }).onError([](const char *message){
        std::cout << "exchange declare error: " << message << std::endl;
    });

    channel.declareQueue("my-queue").onSuccess([](){
        std::cout << "queue declared" << std::endl;
    }).onError([](const char *message){
        std::cout << "queue declare error: " << message << std::endl;
    });

    channel.bindQueue("my-exchange", "my-queue", "my-key").onSuccess([](){
        std::cout << "queue binded" << std::endl;
    }).onError([](const char *message){
        std::cout << "queue bind error: " << message << std::endl;
    });

    channel.startTransaction();
    channel.publish("my-exchange", "my-key", "my first message");
    channel.commitTransaction().onSuccess([](){
        std::cout << "message published" << std::endl;
    }).onError([](const char *message){
        std::cout << "message publish error: " << message << std::endl;
    });


    // callback function that is called when the consume operation starts
    auto startCb = [](const std::string &consumertag) {

        std::cout << "consume operation started" << std::endl;
    };

    // callback function that is called when the consume operation failed
    auto errorCb = [](const char *message) {

        std::cout << "consume operation failed" << std::endl;
    };

    // callback operation when a message was received
    auto messageCb = [&channel](const AMQP::Message &message, uint64_t deliveryTag, bool redelivered) {

        // channel.ack(deliveryTag);
        std::string message_str{message.body(), message.bodySize()};
        std::cout << "message received: \"" << message_str << "\" redelivered: " << redelivered << std::endl;

        // acknowledge the message
    };

    // callback that is called when the consumer is cancelled by RabbitMQ (this only happens in
    // rare situations, for example when someone removes the queue that you are consuming from)
    auto cancelledCb = [](const std::string &consumertag) {

        std::cout << "consume operation cancelled by the RabbitMQ server" << std::endl;
    };

    // start consuming from the queue, and install the callbacks
    channel.consume("my-queue", AMQP::noack)
        .onReceived(messageCb)
        .onSuccess(startCb)
        .onCancelled(cancelledCb)
        .onError(errorCb);
    
    // run the handler
    // a t the moment, one will need SIGINT to stop.  In time, should add signal handling through boost API.
    return service.run();
}
