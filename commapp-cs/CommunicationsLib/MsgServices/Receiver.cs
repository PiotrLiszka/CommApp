using System.Diagnostics;
using System.Text;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;


namespace CommunicationsLib.MsgServices;

public class Receiver
{
    private readonly ConnectionFactory factory;
    private readonly IConnection connection;
    private readonly IModel channel;
    private readonly string userName;

    public static Queue<string> messageQueue = new();

    public Receiver(string userName, bool localHost = true)
    {
        this.userName = userName;

        if (localHost)
        {
            factory = new ConnectionFactory { HostName = "localhost" };
            connection = factory.CreateConnection();
            channel = connection.CreateModel();

            channel.ExchangeDeclare(
                exchange: "commapp-cs",
                type: ExchangeType.Direct);

            channel.QueueDeclare(queue: $"commapp-{userName}",
                             durable: false,
                             exclusive: false,
                             autoDelete: false,
                             arguments: null);

            channel.QueueBind(
                    queue: $"commapp-{userName}",
                    exchange: "commapp-cs",
                    routingKey: userName);

            //channel.QueueDeclare(queue: "hello",
            //                 durable: false,
            //                 exclusive: false,
            //                 autoDelete: false,
            //                 arguments: null);
        }
        else
        {
            //  ip port username and password from file
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

                channel.ExchangeDeclare(
                    exchange: "commapp-cs",
                    type: ExchangeType.Direct);

                channel.QueueDeclare(queue: $"commapp-{userName}",
                             durable: true,
                             exclusive: false,
                             autoDelete: false,
                             arguments: null);


                //channel.QueueDeclare(queue: "hello",
                //             durable: false,
                //             exclusive: false,
                //             autoDelete: false,
                //             arguments: null);
            }
        }
    }

    public void StartMessageConsumer(EventHandler<BasicDeliverEventArgs> callback)
    {
        var consumer = new EventingBasicConsumer(channel);
        consumer.Received += callback;

        //consumer.Received += (model, ea) =>
        //{
        //    var body = ea.Body.ToArray();
        //    var messageType = ea.BasicProperties.ContentType;
        //    var message = Encoding.UTF8.GetString(body);

        //    var routingKey = ea.RoutingKey;

        //    Debug.WriteLine($"{message}");

        //    messageQueue.Enqueue(message);
        //    messageQueue.Enqueue(messageType);

        //};

        //channel.BasicConsume(queue: "hello",
        //                     autoAck: true,
        //                     consumer: consumer);

        channel.BasicConsume(queue: $"commapp-{userName}",
            autoAck: true,
            consumer: consumer);
    }

    ~Receiver()
    {
        channel.Close();
        connection.Close();
    }
}
