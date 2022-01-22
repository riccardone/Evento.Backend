using System.Text;
using CloudEventData;
using Evento.Backend.Adapter.Mappers;
using Evento.Backend.Domain.Commands;
using NLog;

namespace Evento.Backend.Adapter
{
    public class Worker : 
        IHandle<ConfigureOrganisation>
    {
        private readonly IDomainRepository _domainRepository;
        private readonly Dictionary<string, Func<CloudEventRequest, Command>> _deserializers = CreateDeserializersMapping();
        private int _maxLengthForLogs = 255;
        private readonly ILogger _log;

        public Worker(IDomainRepository domainRepository)
        {
            _domainRepository = domainRepository;
            _log = LogManager.GetCurrentClassLogger();
        }

        #region BoilerplateCode
        public void Process(CloudEventRequest cloudRequest)
        {
            var command = BuildCommand(cloudRequest);

            if (command == null)
                throw new Exception(
                    $"I received CloudRequest Type:'{cloudRequest.Type}' Source:'{cloudRequest.Source}' Schema:'{cloudRequest.DataSchema}' but I was unable to deserialize a Command out of it");

            IAggregate aggregate = null;
            try
            {
                switch (command)
                {
                    case ConfigureOrganisation blockTime:
                        aggregate = Handle(blockTime);
                        break;
                }

                // Add here any further command matches
                if (aggregate == null)
                    throw new Exception(
                        $"Received CloudRequest Type:'{cloudRequest.Type}' Source:'{cloudRequest.Source}' Schema:'{cloudRequest.DataSchema}' but I can't find an available handler for it");
            }
            finally
            {
                if (aggregate != null && aggregate.UncommitedEvents().Any())
                {
                    var uncommittedEventsList = aggregate.UncommitedEvents().ToList();
                    _domainRepository.Save(aggregate);

                    var error = new StringBuilder();
                    foreach (var uncommittedEvent in uncommittedEventsList)
                    {
                        _log.Info(
                            $"Handled '{cloudRequest.Type}' CorrelationId:'{aggregate.AggregateId}' [0]Resulted event:'{uncommittedEvent.GetType()}'");

                        if (uncommittedEvent.GetType().ToString().EndsWith("FailedV1"))
                        {
                            error.Append(HandleFailedEvent(uncommittedEvent, command));

                        }
                    }

                    if (error.Length > 0)
                    {
                        throw new BusinessException(error.ToString());
                    }
                }
                else
                    _log.Info(
                        $"Handled CloudRequest Type:'{cloudRequest.Type}' Source:'{cloudRequest.Source}' Schema:'{cloudRequest.DataSchema}' with no events to save");
            }
        }
        private string HandleFailedEvent(Event failedEvent, Command command)
        {
            var errMessage = string.Empty;
            var errForLogging = string.Empty;
            if (failedEvent.GetType().ToString().EndsWith("todo"))
            {
                errMessage = !failedEvent.Metadata.ContainsKey("error")
                   ? $"Error while processing a '{command.Metadata["source"]}' command (no error message has been set in command metadata)"
                   : $"Error while submitting a '{command.Metadata["source"]}' command: {failedEvent.Metadata["error"]}";
                errForLogging = failedEvent.Metadata.ContainsKey("error") ? failedEvent.Metadata["error"] : "undefined";
            }
            var errStack = !failedEvent.Metadata.ContainsKey("error-stack")
                ? string.Empty
                : $"StackTrace: {failedEvent.Metadata["error-stack"]}";
            var err = $"{errMessage} - {errStack}";
            var correlationId = failedEvent.Metadata.ContainsKey("$correlationId")
                ? failedEvent.Metadata["$correlationId"]
                : "undefined";

            var msgToLog = $"CorrelationId:'{correlationId}';{errForLogging}";
            _log.Error(TruncateFieldIfNecessary(msgToLog));
            return err;
        }

        private string TruncateFieldIfNecessary(string field)
        {
            return field.Length > _maxLengthForLogs ? field.Substring(0, _maxLengthForLogs) : field;
        }

        private Command BuildCommand(CloudEventRequest cloudRequest)
        {
            if (!_deserializers.ContainsKey(cloudRequest.DataSchema.ToString()) &&
                !_deserializers.ContainsKey($"{cloudRequest.DataSchema}{cloudRequest.Source}"))
                throw new Exception(
                    $"I can't find a mapper for schema:'{cloudRequest.DataSchema}' source:''{cloudRequest.Source}''");

            var command = _deserializers.ContainsKey(cloudRequest.DataSchema.ToString())
                ? _deserializers[cloudRequest.DataSchema.ToString()](cloudRequest)
                : _deserializers[$"{cloudRequest.DataSchema}{cloudRequest.Source}"](cloudRequest);
            return command;
        }

        private static Dictionary<string, Func<CloudEventRequest, Command>> CreateDeserializersMapping()
        {
            // TODO make this automatic loading all the available mappers using reflection
            var blockTimeMapper = new ConfigureOrganisationMapper();
            var deserialisers = new Dictionary<string, Func<CloudEventRequest, Command>>
            {
                {blockTimeMapper.Schema.ToString(), blockTimeMapper.Map},
            };
            return deserialisers;
        }
        #endregion

        public IAggregate Handle(ConfigureOrganisation command)
        {
            throw new NotImplementedException();
        }
    }
}
