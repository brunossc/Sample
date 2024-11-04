using CustomerGrpc;
using Elasticsearch.Net;
using Sample.BFF.API.Services;
using Serilog;
using Serilog.Sinks.Elasticsearch;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();


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
        IndexFormat = "BFF-{0:yyyy.MM.dd}"
    })
    .CreateLogger();

builder.Host.UseSerilog();

Log.Information("Teste Elasticsearch!");


builder.Services.AddGrpcClient<CustomerServiceProto.CustomerServiceProtoClient>(options =>
{
    options.Address = new Uri("http://grpcservice:8080"); // URL do gRPC service
});
builder.Services.AddScoped<CustomerServiceClient>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

//app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
