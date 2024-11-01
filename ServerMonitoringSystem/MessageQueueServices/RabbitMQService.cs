using Newtonsoft.Json;
using RabbitMQ.Client;
using System.Text;

namespace ServerMonitoringSystem.MessageQueueServices
{
    public class RabbitMQService : IMessageQueueService
    {
        private readonly IConnection _connection;
        private readonly IModel _channel;
        private readonly string _queueName = "MessageQueues";
        private readonly string _exchangeName = "topic_logs";

        public RabbitMQService()
        {
            var factory = new ConnectionFactory() { HostName = "localhost", Port = 5672 };
            _connection = factory.CreateConnection();
            _channel = _connection.CreateModel();
            Console.WriteLine("Connection and channel established.");

            _channel.ExchangeDeclare(exchange: _exchangeName, type: ExchangeType.Topic);

            _channel.QueueDeclare(queue: _queueName,
                                  durable: false,
                                  exclusive: false,
                                  autoDelete: false,
                                  arguments: null);

            _channel.QueueBind(queue: _queueName,
                               exchange: _exchangeName,
                               routingKey: "#");
        }

        public void Publish(string topic, string message)
        {
            try
            {
                string jsonMessage = JsonConvert.SerializeObject(message);
                var body = Encoding.UTF8.GetBytes(jsonMessage);

                _channel.BasicPublish(exchange: _exchangeName,
                                      routingKey: topic,
                                      basicProperties: null,
                                      body: body);

                Console.WriteLine($"Message published to topic '{topic}': {message}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed to publish message: {ex.Message}");
            }
        }
    }
}
