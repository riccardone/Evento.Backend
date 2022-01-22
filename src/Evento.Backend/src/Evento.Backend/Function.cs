using Amazon.Lambda.Core;
using System;
using System.IO;
using System.Threading.Tasks;
using Amazon.Lambda.SQSEvents;
using CloudEventData;
using Evento.Backend.Adapter;
using Evento.Repository;
using EventStore.ClientAPI;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;

// Assembly attribute to enable the Lambda function's JSON input to be converted into a .NET class.
[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.SystemTextJson.DefaultLambdaJsonSerializer))]

namespace Evento.Backend
{
    public class Function
    {
        private Worker _worker;

        /// <summary>
        /// This method is called for every Lambda invocation. This method takes in an SQS event object and can be used 
        /// to respond to SQS messages.
        /// </summary>
        /// <param name="input"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        public async Task FunctionHandler(SQSEvent evt, ILambdaContext context)
        {
            var log = NLog.LogManager.LogFactory.GetCurrentClassLogger();
            log.Debug("Triggered");
            var config = BuildConfig();
            var settings = config.Get<AppSettings>();
            log.Debug($"ProcessorLink: {settings.ProcessorLink}");
            var connBuilder = new ConnectionBuilder(new Uri(settings.ProcessorLink),
                ConnectionSettings.Create().DisableServerCertificateValidation(),
                $"blocktimes-function-{Guid.NewGuid().ToString()}");
            _worker = new Worker(new EventStoreDomainRepository(settings.DomainCategory, connBuilder.Build() as IEventStoreConnection));
            foreach (var message in evt.Records)
                await ProcessMessageAsync(message, context);
        }

        private async Task ProcessMessageAsync(SQSEvent.SQSMessage message, ILambdaContext context)
        {
            var cloudRequest = JsonConvert.DeserializeObject<CloudEventRequest>(message.Body);
            context.Logger.LogLine($"Processing message '{message.MessageId}' of type '{cloudRequest.Type}'");
            _worker.Process(cloudRequest);
            await Task.CompletedTask;
        }

        private static IConfigurationRoot BuildConfig()
        {
            var env = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "dev";
            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: false)
                .AddJsonFile($"appsettings.{env}.json", optional: true, reloadOnChange: false)
                .AddEnvironmentVariables();
            return builder.Build();
        }
    }

    public record Casing(string Lower, string Upper);
}
