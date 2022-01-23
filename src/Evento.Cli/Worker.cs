using System.CommandLine;
using Evento.Cli.Commands;
using Microsoft.Extensions.Configuration;

namespace Evento.Cli
{
    internal class Worker
    {
        private readonly IConfiguration configuration;

        public Worker(IConfiguration configuration)
        {
            this.configuration = configuration;
        }

        public void DoWork(string[] args)
        {
            var keyValuePairs = configuration.AsEnumerable().ToList();
            var rootCommand = new RootCommand("Evento.Cli");
            rootCommand.AddCommand(new SendCommand());
            rootCommand.Invoke(args);
        }
    }
}
