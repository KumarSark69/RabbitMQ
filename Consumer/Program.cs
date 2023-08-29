using System.Text;
using System.Threading.Tasks;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
//Basic example
//var factory = new ConnectionFactory { HostName = "localhost" };

//using var connection = factory.CreateConnection();

//using var channel = connection.CreateModel();
//Console.WriteLine("Wating for the Producer");
//channel.QueueDeclare(queue: "letterbox", durable: false, exclusive: false, autoDelete: false, arguments: null);
//var consumer = new EventingBasicConsumer(channel);
//consumer.Received += (model, ea) =>
//{
//    var body = ea.Body.ToArray();
//    var message = Encoding.UTF8.GetString(body);
//    Console.WriteLine($"Received message {message}");
//};
//channel.BasicConsume(queue: "letterbox", autoAck: true, consumer: consumer);
//Console.ReadKey();



//competing consumer
var factory = new ConnectionFactory { HostName = "localhost" };

using var connection = factory.CreateConnection();
var random = new Random();
using var channel = connection.CreateModel();
Console.WriteLine("Wating for the Producer");
channel.QueueDeclare(queue: "letterbox", durable: false, exclusive: false, autoDelete: false, arguments: null);


//This line helps to send the message to the free consumer rather than sending it in round robin format 
channel.BasicQos(prefetchSize: 0, prefetchCount: 1, global: false);

var consumer = new EventingBasicConsumer(channel);
consumer.Received += (model, ea) =>
{
    var processingTime = random.Next(1, 6);

    var body = ea.Body.ToArray();

    var message = Encoding.UTF8.GetString(body);



    Console.WriteLine($"Receving message: {message} and it take {processingTime} to process");
    Task.Delay(TimeSpan.FromSeconds(processingTime)).Wait();

    channel.BasicAck(deliveryTag: ea.DeliveryTag, multiple: false);

};
channel.BasicConsume(queue: "letterbox", autoAck: false, consumer: consumer);
Console.ReadKey();
