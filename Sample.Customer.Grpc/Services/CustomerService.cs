using Confluent.Kafka;
using CustomerGrpc;
using Grpc.Core;
using Mapster;
using MongoDB.Driver;
using Sample.Customer.Grpc.Model;
using Sample.SideCar.Dtos;
using StackExchange.Redis;
using System.Text.Json;

namespace GrpcCustomerService.Services
{
    public class CustomerService : CustomerServiceProto.CustomerServiceProtoBase
    {
        private readonly IMongoCollection<CustomerModel> _customerCollection;
        private readonly IProducer<Null, string> _kafkaProducer;
        private readonly ILogger<CustomerService> _logger;
        private readonly IConnectionMultiplexer _redis;

        public CustomerService(IMongoClient mongoClient, IProducer<Null, string> kafkaProducer, IConnectionMultiplexer redis, ILogger<CustomerService> logger)
        {
            var database = mongoClient.GetDatabase("CustomerDB");
            _customerCollection = database.GetCollection<CustomerModel>("Customers");
            _kafkaProducer = kafkaProducer;
            _redis = redis;
            _logger = logger;
        }

        public override async Task<CustomerResponse> CreateCustomerAsync(CreateCustomerRequest request, ServerCallContext context)
        {
            var customer = new CustomerModel
            {
                Name = request.Name,
                Document = request.Document,
                DocumentType = Enum.Parse<DocumentType>(request.DocumentType)
            };

            await _customerCollection.InsertOneAsync(customer);

            var customerJson = JsonSerializer.Serialize(customer);
            await _kafkaProducer.ProduceAsync("customer-topic", new Message<Null, string> { Value = customerJson });
            _logger.LogInformation($"customer {customer.Id} foi postado");


            var redisDb = _redis.GetDatabase();
            await redisDb.StringSetAsync(customer.Id, customerJson);

            return customer.Adapt<CustomerResponse>();
        }

        public override async Task<CustomerResponse> GetCustomerAsync(GetCustomerRequest request, ServerCallContext context)
        {
            var redisDb = _redis.GetDatabase();
            var cachedCustomer = await redisDb.StringGetAsync(request.Id);

            if (cachedCustomer.HasValue)
            {
                return JsonSerializer.Deserialize<CustomerModel>(cachedCustomer).Adapt<CustomerResponse>();
            }

            var customer = await _customerCollection.Find(c => c.Id == request.Id).FirstOrDefaultAsync();
            if (customer != null)
            {
                await redisDb.StringSetAsync(customer.Id, JsonSerializer.Serialize(customer));
            }

            return customer.Adapt<CustomerResponse>();
        }
    }
}