using Confluent.Kafka;
using MongoDB.Driver;
using Sample.Consumer.API.Services;
using Sample.SideCar.Monitoring;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.AddSerilogWithElastic("consumer", Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT"));

builder.Services.AddSingleton<IMongoClient, MongoClient>(s =>
    new MongoClient(builder.Configuration.GetConnectionString("MongoDB")));

builder.Services.AddSingleton<IConsumer<string, string>>(provider =>
{
    var config = new ProducerConfig
    {
        BootstrapServers = builder.Configuration["Kafka:BootstrapServers"]
    };
    return new ConsumerBuilder<string, string>(config).Build();
});

builder.Services.AddHostedService<ConsumerService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
