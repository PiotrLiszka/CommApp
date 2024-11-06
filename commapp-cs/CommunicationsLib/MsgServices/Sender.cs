using System.Text;
using System.Diagnostics;
using RabbitMQ.Client;
using CommunicationsLib.MsgParser;

namespace CommunicationsLib.MsgServices;

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

            channel.ExchangeDeclare(
                    exchange: "comapp-cs",
                    type: ExchangeType.Direct);

            //channel.QueueDeclare(queue: "hello",
            //         durable: false,
            //         exclusive: false,
            //         autoDelete: false,
            //         arguments: null);
        }
        else
        {
            //  ip port username and password from file (security reasons)
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

                //channel.QueueDeclare(queue: "hello",
                //     durable: false,
                //     exclusive: false,
                //     autoDelete: false,
                //     arguments: null);

            }
        }
    }

    public void SendMessage(string messageToSend, string messageMIMEType, string sendTo, string messFrom)
    {
        string jsonMessage = MessageJson.JsonSerializeMessage(messageToSend, messFrom);

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

        //channel.BasicPublish(exchange: string.Empty,
        //                     routingKey: "hello",
        //                     basicProperties: properties,
        //                     body: body);

        channel.BasicPublish(exchange: "commapp-cs",
                             routingKey: sendTo,
                             basicProperties: properties,
                             body: body);

    }

    ~Sender()
    {
        channel.Dispose();
        channel.Close();
        connection.Dispose();
        connection.Close();
    }

}
