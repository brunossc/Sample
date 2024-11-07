using CustomerGrpc;
using Elasticsearch.Net;
using Sample.BFF.API.Services;
using Sample.SideCar.Monitoring;
using Serilog;
using Serilog.Sinks.Elasticsearch;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.AddSerilogWithElastic("BFF", Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT"));

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
