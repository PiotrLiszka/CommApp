using System.Text;
using System.Diagnostics;
using System.Text.Json;
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
            //  ip port username i password z pliku
            using (StreamReader sr = new StreamReader(string.Concat(
                Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
                Path.DirectorySeparatorChar,
                "adresip.txt")))
            {
                factory = new ConnectionFactory { HostName = sr.ReadLine() };
                if(int.TryParse(sr.ReadLine(), out int port))
                    factory.Port = port;

                factory.UserName = sr.ReadLine();
                factory.Password = sr.ReadLine();

                connection = factory.CreateConnection();
                channel = connection.CreateModel();

            }
        }
    }

    private Task<string> JsonSerializeMessage(string messageToSerialize, int userID = 1)
    {
        MessageInfo messageInfo = new MessageInfo(userID, messageToSerialize);

        string? jsonMessage = JsonSerializer.Serialize(messageInfo);
   
        return Task.FromResult(jsonMessage);
    }

    public async Task SendMessage(string messageToSend)
    {
        channel.QueueDeclare(queue: "hello",
                             durable: false,
                             exclusive: false,
                             autoDelete: false,
                     arguments: null);

        string jsonMessage = await JsonSerializeMessage(messageToSend);

        var body = Encoding.UTF8.GetBytes(jsonMessage);

        IBasicProperties properties = channel.CreateBasicProperties();

        properties.ContentEncoding = "application/json";
        properties.ContentType = "application/json";

       channel.BasicPublish(exchange: string.Empty,
                             routingKey: "hello",
                             basicProperties: properties,
                             body: body);

        Debug.WriteLine($"Message send.\nMess: {messageToSend}");
        //Console.WriteLine($" [x] Sent {messageToSend}");

        //Console.WriteLine(" Press [enter] to exit.");

        return;
    }


}
