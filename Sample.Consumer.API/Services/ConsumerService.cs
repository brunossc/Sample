using Confluent.Kafka;
using MongoDB.Driver;
using Sample.Consumer.API.Model;
using System.Text.Json;

namespace Sample.Consumer.API.Services
{
    public class ConsumerService : BackgroundService
    {
        private readonly IConsumer<Ignore, string> _consumer;
        private readonly IMongoCollection<CustomerModel> _customerCollection;

        public ConsumerService(IMongoClient mongoClient, IConfiguration configuration)
        {
            var database = mongoClient.GetDatabase("ConsumerDB");
            _customerCollection = database.GetCollection<CustomerModel>("Customers");
            var config = new ConsumerConfig { BootstrapServers = configuration["Kafka:BootstrapServers"], GroupId = "customer-group"};
            _consumer = new ConsumerBuilder<Ignore, string>(config).Build();
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _consumer.Subscribe("customer-topic");
            while (!stoppingToken.IsCancellationRequested)
            {
                var result = _consumer.Consume(stoppingToken);
                var customer = JsonSerializer.Deserialize<CustomerModel>(result.Message.Value);
                await _customerCollection.InsertOneAsync(customer);
            }
        }

        public override async Task StopAsync(CancellationToken cancellationToken)
        {
            _consumer.Close();
            await base.StopAsync(cancellationToken);
        }
    }
}
