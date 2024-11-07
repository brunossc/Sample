using Confluent.Kafka;
using MongoDB.Driver;
using Sample.SideCar.Monitoring;
using Serilog;
using StackExchange.Redis;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddGrpc();
builder.AddSerilogWithElastic("Customer", Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT"));

builder.Services.AddSingleton<IMongoClient, MongoClient>(s =>
    new MongoClient(builder.Configuration.GetConnectionString("MongoDB")));

builder.Services.AddSingleton<IConnectionMultiplexer>(ConnectionMultiplexer.Connect(
    builder.Configuration.GetConnectionString("Redis")));

builder.Services.AddSingleton<IProducer<Null, string>>(sp =>
{
    var config = new ProducerConfig
    {
        BootstrapServers = builder.Configuration["Kafka:BootstrapServers"]
    };
    return new ProducerBuilder<Null, string>(config).Build();
});

var app = builder.Build();

// Configure the HTTP request pipeline.
app.MapGrpcService<GrpcCustomerService.Services.CustomerService>();
app.MapGet("/", () => "Communication with gRPC endpoints must be made through a gRPC client. To learn how to create a client, visit: https://go.microsoft.com/fwlink/?linkid=2086909");

app.Run();
