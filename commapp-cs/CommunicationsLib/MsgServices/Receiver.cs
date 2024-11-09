using RabbitMQ.Client;
using RabbitMQ.Client.Events;


namespace CommunicationsLib.MsgServices;

public class Receiver
{
    private readonly ConnectionFactory factory;
    private readonly string myUsername;

    private IConnection? connection;
    private IModel? channel;

    public Receiver(string myUserame, bool localHost = true)
    {
        this.myUsername = myUserame;

        if (localHost)
        {
            factory = new ConnectionFactory { HostName = "localhost" };

            InitializeConnection(myUserame);
        }
        else
        {
            //  ip port username and password from file
            string connectionFilePath = string.Concat(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), Path.DirectorySeparatorChar, "adresip.txt");
            using (StreamReader sr = new StreamReader(connectionFilePath))
            {
                factory = new ConnectionFactory { HostName = sr.ReadLine() };

                if (int.TryParse(sr.ReadLine(), out int port))
                    factory.Port = port;

                factory.UserName = sr.ReadLine();
                factory.Password = sr.ReadLine();
                
                InitializeConnection(myUserame);
            }
        }
    }

    private void InitializeConnection(string userName)
    {
        //  TODO: check if connection was established, if not - retry
        //  return bool maybe?
        connection = factory.CreateConnection();
        channel = connection.CreateModel();

        channel.ExchangeDeclare(
            exchange: "commapp-cs",
            type: ExchangeType.Direct,
            durable: true);

        channel.QueueDeclare(queue: $"commapp-{userName}",
                         durable: true,
                         exclusive: false,
                         autoDelete: false,
                         arguments: null);

        channel.QueueBind(
                queue: $"commapp-{userName}",
                exchange: "commapp-cs",
                routingKey: userName);
    }

    public void StartMessageConsumer(EventHandler<BasicDeliverEventArgs> callbackHandler)
    {
        var consumer = new EventingBasicConsumer(channel);
        consumer.Received += callbackHandler;

        channel.BasicConsume(queue: $"commapp-{myUsername}",
            autoAck: true,
            consumer: consumer);
    }

    ~Receiver()
    {
        connection?.Close();
        channel?.Close();
    }
}
