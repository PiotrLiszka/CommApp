using System.Text;
using System.Diagnostics;
using RabbitMQ.Client;
using CommunicationsLib.MsgParser;

namespace CommunicationsLib.MsgServices;

public class Sender
{
    private readonly ConnectionFactory factory;

    private IConnection? connection;
    private IModel? channel;

    public Sender(bool localHost = true)
    {
        if (localHost)
        {
            factory = new ConnectionFactory { HostName = "localhost" };

            InitializeConnection();
        }
        else
        {
            string connectionFilePath = string.Concat(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), Path.DirectorySeparatorChar, "adresip.txt");
            //  ip port username and password from file (security reasons)
            using (StreamReader sr = new StreamReader(connectionFilePath))
            {
                factory = new ConnectionFactory { HostName = sr.ReadLine() };

                if (int.TryParse(sr.ReadLine(), out int port))
                    factory.Port = port;

                factory.UserName = sr.ReadLine();
                factory.Password = sr.ReadLine();
            }
                InitializeConnection();
        }
    }

    private void InitializeConnection()
    {
        //  TODO: check if connection was established, if not - retry
        //  return bool maybe?
        connection = factory.CreateConnection();
        channel = connection.CreateModel();

        channel.ExchangeDeclare(
                exchange: "commapp-cs",
                type: ExchangeType.Direct,
                durable: true);
    }

    public void SendMessage(string messageToSend, string messageMIMEType, string sendTo, string messFrom)
    {
        // TODO: connection check
        if (channel == null)
            return;

        string jsonMessage = MessageJsonSerializer.JsonSerializeMessage(messageToSend, messFrom);

        Debug.WriteLine(jsonMessage);

        var body = Encoding.UTF8.GetBytes(jsonMessage);

        IBasicProperties properties = channel.CreateBasicProperties();

        properties.ContentEncoding = "application/json";

        switch(messageMIMEType)
        {
            case "text":
                properties.ContentType = "text/plain";
                break;
            case ".jpg":
            case ".jpeg":
                properties.ContentType = "image/jpeg";
                break;
            case ".bmp":
                properties.ContentType = "image/bmp";
                break;
            case ".png":
                properties.ContentType = "image/png";
                break;
        }

        channel.BasicPublish(exchange: "commapp-cs",
                             routingKey: sendTo,
                             basicProperties: properties,
                             body: body);
    }

    ~Sender()
    {
        channel?.Dispose();
        channel?.Close();
        connection?.Dispose();
        connection?.Close();
    }

}
