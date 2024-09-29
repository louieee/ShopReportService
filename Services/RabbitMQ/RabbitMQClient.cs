using ReportService.Services.RabbitMQ.Consumers;

namespace ReportApp.Data.Services;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Text;
using Microsoft.Extensions.Configuration;
using Microsoft.EntityFrameworkCore;



public interface IDataContextFactory
{
    DataContext CreateDataContext();
}

public class DataContextFactory : IDataContextFactory
{
    private readonly string _connectionString;

    public DataContextFactory(string connectionString){
        _connectionString = connectionString;
    }
    public DataContext CreateDataContext()
    {
        return new DataContext(new DbContextOptionsBuilder<DataContext>().UseNpgsql(_connectionString).Options);
    }
}


public class RabbitMQService
    {
        private readonly IModel? _channel;

        private readonly IDataContextFactory _dataContextFactory;

        private readonly DataContext _DbContext;

        private readonly IConfiguration _Configuration;
        

        public RabbitMQService(IConfiguration configuration)
        {
            _Configuration = configuration;
            var connString = _Configuration.GetConnectionString("ReportDB");
            
            _dataContextFactory = new DataContextFactory(connString??"");
            _DbContext = _dataContextFactory.CreateDataContext();
            var factory = new ConnectionFactory();
            factory.UserName = _Configuration["RabbitMQ:UserName"];
            factory.Password = _Configuration["RabbitMQ:Password"];

            var endpoints = new System.Collections.Generic.List<AmqpTcpEndpoint> {
                new AmqpTcpEndpoint(_Configuration["RabbitMQ:HostName"])
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

            EventingBasicConsumer? consumer = Consumer.CreateConsumer(_channel, _DbContext);
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
