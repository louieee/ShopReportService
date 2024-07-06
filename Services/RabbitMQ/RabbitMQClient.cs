using ReportService.Services.RabbitMQ.Consumers;

namespace ReportApp.Data.Services;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Text;
using Microsoft.Extensions.Configuration;

 public class RabbitMQService
    {
        private readonly IModel? _channel;

        public RabbitMQService(IConfiguration configuration)
        {
            var configuration1 = configuration;
            var factory = new ConnectionFactory();
            factory.UserName = configuration1["RabbitMQ:UserName"];
            factory.Password = configuration1["RabbitMQ:Password"];

            var endpoints = new System.Collections.Generic.List<AmqpTcpEndpoint> {
                new AmqpTcpEndpoint(configuration1["RabbitMQ:HostName"])
            };
            var connection = factory.CreateConnection(endpoints);
            _channel = connection.CreateModel();
            
            _channel?.ExchangeDeclare(exchange: "sales-app", type: ExchangeType.Fanout, durable:true);
            _channel?.QueueDeclare(queue: "report_queue", durable: true, exclusive: false, autoDelete: false,
                new Dictionary<string, object>()
                {
                    ["x-queue-type"] = "quorum"
                });
            _channel?.QueueBind(queue: "report_queue", exchange: "sales-app", routingKey:"");
        }

        /// <summary>
        /// 
        /// </summary>
        public void StartListening()
        {
            Console.WriteLine("Connected to RabbitMQ.....");

            EventingBasicConsumer? consumer = Consumer.CreateConsumer(_channel);
            if (consumer != null)
            {
                _channel.BasicConsume(queue: Consumer.QueueName,
                    autoAck: false,
                    exclusive:false,
                    consumer: consumer);
            }
        }

        public void Publish(string exchange, string queue,  byte[] body)
        {
            var count = _channel.ConsumerCount(queue);
            for (uint val = 0; val <= count; val++)
            {
                _channel.BasicPublish(exchange:exchange, routingKey: "", body: body, mandatory: true);
            }
     
        }
    }
