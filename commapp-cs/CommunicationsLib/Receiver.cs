using System.Text;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace CommunicationsLib;

public class Receiver
{

    public static Queue<string?> messageQueue = new();
    private readonly ConnectionFactory factory;
    private readonly IConnection connection;
    private readonly IModel channel;
    public Receiver(bool localHost = true)
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

    public void GetMessages()
    {
        channel.QueueDeclare(queue: "hello",
                             durable: false,
                             exclusive: false,
                             autoDelete: false,
                             arguments: null);

        Console.WriteLine(" [*] Waiting for messages.");

        var consumer = new EventingBasicConsumer(channel);
        consumer.Received += (model, ea) =>
        {
            var body = ea.Body.ToArray();
            var message = Encoding.UTF8.GetString(body);
            Console.WriteLine($" [x] Received {message}");
            messageQueue.Enqueue(message);
        };

        channel.BasicConsume(queue: "hello",
                             autoAck: true,
                             consumer: consumer);

        Console.WriteLine(" Press [enter] to exit.");
        Console.ReadLine();
    }

}
