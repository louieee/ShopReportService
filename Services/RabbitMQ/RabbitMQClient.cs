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
            
            _channel?.ExchangeDeclare(exchange: "business", type: ExchangeType.Fanout, durable:true);
            _channel?.QueueDeclare(queue: "task", durable:true, exclusive:false, autoDelete: false);
            _channel?.QueueBind(queue: "task", exchange: "business", routingKey:"");
        }

        /// <summary>
        /// 
        /// </summary>
        public void StartListening()
        {
            Console.WriteLine("Connected to RabbitMQ.....");

            EventingBasicConsumer? userConsumer = UserConsumer.CreateConsumer(_channel);
            if (userConsumer != null)
            {
                Console.WriteLine("My consumer.....");
                _channel.BasicConsume(queue: UserConsumer.QueueName,
                    autoAck: false,
                    exclusive:false,
                    consumer: userConsumer);
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
