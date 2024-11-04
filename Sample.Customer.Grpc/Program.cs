using Confluent.Kafka;
using Elasticsearch.Net;
using MongoDB.Driver;
using Serilog;
using Serilog.Sinks.Elasticsearch;
using StackExchange.Redis;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddGrpc();


var uri = new Uri("http://elasticsearch:9200");
var connectionConfiguration = new ConnectionConfiguration(uri)
            .RequestTimeout(TimeSpan.FromSeconds(10));

var transport = new Transport<IConnectionConfigurationValues>(connectionConfiguration);

Log.Logger = new LoggerConfiguration()
    .Enrich.FromLogContext()
    .WriteTo.Console()
    .WriteTo.Elasticsearch(new ElasticsearchSinkOptions(uri)
    {
        ModifyConnectionSettings = conn => connectionConfiguration,
        AutoRegisterTemplate = true,
        IndexFormat = "Customer-{0:yyyy.MM.dd}"
    })
    .CreateLogger();

builder.Host.UseSerilog();

Log.Information("Teste Elasticsearch!");

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
