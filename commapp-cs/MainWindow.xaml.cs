using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

using CommunicationsLib;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace commappcs
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        string mymess = string.Empty;
       

        private static void CreateConnectionRabbit(out IConnection connection, out IModel model)
        {
            var factory = new ConnectionFactory { HostName = "localhost" };
            connection = factory.CreateConnection();
            model = connection.CreateModel();
        }
        public MainWindow()
        {
            InitializeComponent();

            CreateConnectionRabbit(out IConnection connection, out IModel channel);


            channel.QueueDeclare(queue: "hello",
                             durable: false,
                             exclusive: false,
                             autoDelete: false,
                             arguments: null);

            var consumer = new EventingBasicConsumer(channel);
            consumer.Received += (model, ea) =>
            {
                var body = ea.Body.ToArray();
                var message = Encoding.UTF8.GetString(body);
                Console.WriteLine($" [x] Received {message}");
                RecMessBox.Text = message.ToString();
            };

            channel.BasicConsume(queue: "hello",
                             autoAck: true,
                             consumer: consumer);

            Console.WriteLine(" Press [enter] to exit.");


        }

        private void RecMessBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            
        }

        private void SendMessButton_Click(object sender, RoutedEventArgs e)
        {
            RecMessBox.Clear();

            Sender.SendMessage(SendMessBox.Text.ToString());

            //mymess = Receiver.GetMessage();

            //RecMessBox.Text = mymess;
        }
    }
}