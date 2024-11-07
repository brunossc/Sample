using Serilog;
using Microsoft.AspNetCore.Builder;
using Serilog.Sinks.Elasticsearch;

namespace Sample.SideCar.Monitoring
{
    public static class Logger
    {
        public static void AddSerilogWithElastic(this WebApplicationBuilder builder, string serviceName, string environment)
        {
            var options = new ElasticsearchSinkOptions(new Uri("http://elasticsearch:9200")) // Specify your Elasticsearch URI
            {
                AutoRegisterTemplate = true,
                IndexFormat = $"{serviceName}_{environment}" + "-{0:yyyy.MM.dd}",
                EmitEventFailure = EmitEventFailureHandling.WriteToSelfLog
            };

            Log.Logger = new LoggerConfiguration()
                .WriteTo.Elasticsearch(options)
                .CreateLogger();

            Log.Information($"This is a test log entry sent to Elasticsearch! - {serviceName}");
            Log.Warning($"This is a test warning log! - {serviceName}");

        }
    }
}
