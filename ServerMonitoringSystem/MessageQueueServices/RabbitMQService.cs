using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using ServerMonitoringSystem.DatabaseManagement;
using ServerMonitoringSystem.ServerStatisticsManagement;
using System.Text;

namespace ServerMonitoringSystem.MessageQueueServices
{
    public class RabbitMQService : IMessageQueueService
    {
        private readonly IConnection _connection;
        private readonly IModel _channel;
        private readonly string _queueName = "MessageQueues";
        private readonly string _exchangeName = "topic_logs";
        private readonly IDatabaseRepository _databaseRepository;

        public RabbitMQService(IDatabaseRepository databaseRepository)
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
            _databaseRepository = databaseRepository;
        }

        public void Publish(ServerStatistics serverStatistics)
        {
            try
            {
                string jsonMessage = JsonConvert.SerializeObject(serverStatistics);
                var body = Encoding.UTF8.GetBytes(jsonMessage);

                _channel.BasicPublish(exchange: _exchangeName,
                                      routingKey: serverStatistics.ServerIdentifier,
                                      basicProperties: null,
                                      body: body);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed to publish message: {ex.Message}");
            }
        }

        public ServerStatistics GetMessage()
        {
            var consumer = new EventingBasicConsumer(_channel);
            ServerStatistics serverStatistics = null;
            consumer.Received += (model, ea) =>
            {
                var body = ea.Body.ToArray();
                var message = Encoding.UTF8.GetString(body);
                serverStatistics = JsonConvert.DeserializeObject<ServerStatistics>(message);
            };

            _channel.BasicConsume(queue: _queueName,
                                 autoAck: true,
                                 consumer: consumer);
            return serverStatistics;
        }
        public void Dispose()
        {
            _channel.Close();
            _connection.Close();
        }
    }
}
