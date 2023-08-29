using System.Text;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

 class Program
{
    private static async Task Main(string[] args)
    {
        var factory = new ConnectionFactory { HostName = "localhost" };

        using var connection = factory.CreateConnection();
        var random = new Random();
        var random2 = new Random();
        using var channel = connection.CreateModel();
        using var channel2 = connection.CreateModel();
        Console.WriteLine("Wating for the Producer");
        channel.QueueDeclare(queue: "letterbox", durable: false, exclusive: false, autoDelete: false, arguments: null);
        Test test = new Test(channel, random,"first");
        Test test1 = new Test(channel2, random2,"second");

        //This line helps to send the message to the free consumer rather than sending it in round robin format 
        
        Thread t1 = new Thread(test.firstConsumer);
        t1.Start();
        Thread t2 = new Thread(test1.firstConsumer);
        t2.Start();
       // firstConsumer(channel, random);
        //secondConsumer(channel, random);
        Console.ReadKey();

        //return Task.CompletedTask;
    }

    //private static async void secondConsumer(IModel channel, Random random)
    //{
    //    Thread thread = new Thread(() =>
    //    {

    //        var consumer = new EventingBasicConsumer(channel);
    //        consumer.Received += async (model, ea) =>
    //        {
    //            var processingTime = random.Next(1, 6);

    //            var body = ea.Body.ToArray();

    //            var message = Encoding.UTF8.GetString(body);



    //            Console.WriteLine($"Receving message: {message} and it take {processingTime} to process");
    //            //Task.Delay(TimeSpan.FromSeconds(processingTime)).Wait();
    //            //Thread.Sleep(processingTime * 1000);

    //            channel.BasicAck(deliveryTag: ea.DeliveryTag, multiple: false);

    //        };
    //        channel.BasicConsume(queue: "letterbox", autoAck: false, consumer: consumer);
    //    });

    //    thread.Start();
    //}

   
}
class Test
{
    public IModel channel;
    public Random random;
    string consumer;
    public Test(IModel channnel, Random random,string consumer)
    {
        this.channel = channnel;
        this.random = random;
        this.consumer = consumer;
    }
    public void firstConsumer()
    {
        channel.BasicQos(prefetchSize: 0, prefetchCount: 1, global: false);
        var consumer1 = new EventingBasicConsumer(channel);
        consumer1.Received += (model, ea) =>
        {
            var processingTime = random.Next(1, 20);

            var body = ea.Body.ToArray();

            var message = Encoding.UTF8.GetString(body);



            Console.WriteLine($"Receving message: {message} and it take {processingTime} to process {consumer}");
            Thread.Sleep(processingTime * 1000);

            channel.BasicAck(deliveryTag: ea.DeliveryTag, multiple: false);

        };
        channel.BasicConsume(queue: "letterbox", autoAck: false, consumer: consumer1);

       

    }
}