using System.Text;
using RabbitMQ.Client;

namespace CommunicationsLib;

public class Sender
{ 
    private readonly ConnectionFactory factory;
    private readonly IConnection connection;
    private readonly IModel channel;

    public Sender(bool localHost = true)
    {
        if (localHost)
        {
            factory = new ConnectionFactory { HostName = "localhost" };
            connection = factory.CreateConnection();
            channel = connection.CreateModel();
        }
        else
        {
            //  TODO: ip port username i password z pliku 
        }
    }

    public void SendMessage(string messageToSend)
    {
        channel.QueueDeclare(queue: "hello",
                             durable: false,
                             exclusive: false,
                             autoDelete: false,
                     arguments: null);


        var body = Encoding.UTF8.GetBytes(messageToSend);

        channel.BasicPublish(exchange: string.Empty,
                             routingKey: "hello",
                             basicProperties: null,
                             body: body);

        Console.WriteLine($" [x] Sent {messageToSend}");

        Console.WriteLine(" Press [enter] to exit.");
    }


}
