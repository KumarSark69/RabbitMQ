using System.Text;
using RabbitMQ.Client;

//Basic Producer
//var factory = new ConnectionFactory { HostName = "localhost" };

//using var connection = factory.CreateConnection();

//using var channel = connection.CreateModel();

//channel.QueueDeclare(queue: "letterbox", durable: false, exclusive: false, autoDelete: false, arguments: null);

//var message = "This is my first message";

//var encodedMessage = Encoding.UTF8.GetBytes(message);

//channel.BasicPublish("", "letterbox", null, encodedMessage);

//Console.WriteLine($"Published message: {message}");


//Competing Consumer Producer

var factory = new ConnectionFactory { HostName = "localhost" };
var random = new Random();
using var connection = factory.CreateConnection();

using var channel = connection.CreateModel();
var messageId = 1;
channel.QueueDeclare(queue: "letterbox", durable: false, exclusive: false, autoDelete: false, arguments: null);
while (true)
{
    var publishingTime = random.Next(1, 4);
    var message = $"Sending messageId: {messageId} ";

    var encodedMessage = Encoding.UTF8.GetBytes(message);

    channel.BasicPublish("", "letterbox", null, encodedMessage);

    Console.WriteLine($"Published message: {message}");

    Task.Delay(TimeSpan.FromSeconds(publishingTime)).Wait();

    messageId++;


}



