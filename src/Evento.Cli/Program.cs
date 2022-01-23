using System.CommandLine;
using Evento.Cli.Commands;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Evento.Cli 
{
    internal class Program
    {
        static void Main(string[] args)
        {
            // Setup Host
            var host = CreateDefaultBuilder().Build();

            // Invoke Worker
            using var serviceScope = host.Services.CreateScope();
            var provider = serviceScope.ServiceProvider;
            var workerInstance = provider.GetRequiredService<Worker>();
            workerInstance.DoWork(args);

            host.Run();
        }

        static IHostBuilder CreateDefaultBuilder()
        {
            var env = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "dev";
            return Host.CreateDefaultBuilder()
                .ConfigureAppConfiguration(app =>
                {
                    app.AddJsonFile("appsettings.json");
                    app.AddJsonFile($"appsettings.{env}.json", optional: true, reloadOnChange: false);
                })
                .ConfigureServices(services =>
                {
                    services.AddSingleton<Worker>();
                }); ;
        }
    }
}