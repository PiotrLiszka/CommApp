using System.Diagnostics;
using System.Text;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

using CommunicationsLib;

namespace CommunicationsLib.MsgServices;

public class Receiver
{
    private readonly ConnectionFactory factory;
    private readonly IConnection connection;
    private readonly IModel channel;

    public static Queue<string?> messageQueue = new();

    public Receiver(bool localHost = true)
    {
        if (localHost)
        {
            factory = new ConnectionFactory { HostName = "localhost" };
            connection = factory.CreateConnection();
            channel = connection.CreateModel();

            channel.QueueDeclare(queue: "hello",
                             durable: false,
                             exclusive: false,
                             autoDelete: false,
                             arguments: null);
        }
        else
        {
            //  ip port username i password z pliku 
            using (StreamReader sr = new StreamReader(string.Concat(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
                                                    Path.DirectorySeparatorChar,
                                                    "adresip.txt")))
            {
                factory = new ConnectionFactory { HostName = sr.ReadLine() };

                if (int.TryParse(sr.ReadLine(), out int port))
                    factory.Port = port;
                factory.UserName = sr.ReadLine();
                factory.Password = sr.ReadLine();
                connection = factory.CreateConnection();
                channel = connection.CreateModel();

                channel.QueueDeclare(queue: "hello",
                             durable: false,
                             exclusive: false,
                             autoDelete: false,
                             arguments: null);
            }
        }
    }

    public void StartMessageConsumer()
    {
        var consumer = new EventingBasicConsumer(channel);

        consumer.Received += (model, ea) =>
        {
            var body = ea.Body.ToArray();
            var message = Encoding.UTF8.GetString(body);

            messageQueue.Enqueue(message);
        };

        channel.BasicConsume(queue: "hello",
                             autoAck: true,
                             consumer: consumer);
    }

    ~Receiver()
    {
        channel.Close();
        connection.Close();
    }
}
