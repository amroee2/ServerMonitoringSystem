using Models.StatisticsCollectors;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RabbitMQ.Client;
using System.Text;

namespace Models.MessageQueues
{
    public class RabbitMQService : IMessageQueueService
    {
        private readonly IConnection _connection;
        private readonly IModel _channel;
        private readonly string _queueName = "MessageQueues";
        private readonly string _exchangeName = "topic_logs";

        public RabbitMQService()
        {
            var factory = new ConnectionFactory()
            {
                HostName = Environment.GetEnvironmentVariable("MESSAGE_QUEUE_HOST"),
                Port = int.Parse(Environment.GetEnvironmentVariable("MESSAGE_QUEUE_PORT")),
                UserName = "guest",
                Password = "guest"
            };

            _connection = CreateConnection(factory);
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

        private IConnection CreateConnection(ConnectionFactory factory)
        {
            int retryCount = 30;
            int delay = 2000;

            for (int i = 0; i < retryCount; i++)
            {
                try
                {
                    return factory.CreateConnection();
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Attempt {i + 1} to connect failed: {ex.Message}");
                    Thread.Sleep(delay);
                }
            }

            throw new Exception("Could not establish a connection to RabbitMQ after multiple attempts.");
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
                Console.WriteLine($"published {serverStatistics}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed to publish message: {ex.Message}");
            }
        }

        public ServerStatistics GetMessage()
        {
            ServerStatistics serverStatistics = null;
            try
            {
                var result = _channel.BasicGet(_queueName, autoAck: true);
                if (result != null)
                {
                    var message = Encoding.UTF8.GetString(result.Body.ToArray());
                    try
                    {
                        var jsonObject = JObject.Parse(message);
                        serverStatistics = new ServerStatistics
                        {
                            AvailableMemory = (double)jsonObject["AvailableMemory"],
                            CpuUsage = (double)jsonObject["CpuUsage"],
                            MemoryUsage = (double)jsonObject["MemoryUsage"],
                            ServerIdentifier = (string)jsonObject["ServerIdentifier"],
                            Timestamp = (DateTime)jsonObject["Timestamp"]
                        };
                    }
                    catch (JsonException jsonEx)
                    {
                        Console.WriteLine($"Failed to parse JSON properties: {jsonEx.Message}");
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Unexpected error: {ex.Message}");
                    }
                }
                else
                {
                    Console.WriteLine("No message available in the queue.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed to get message: {ex.Message}");
            }

            return serverStatistics;
        }


    }
}
