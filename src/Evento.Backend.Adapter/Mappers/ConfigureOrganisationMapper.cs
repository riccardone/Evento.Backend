using System.Text.Json;
using CloudEventData;
using Evento.Backend.Domain.Commands;

namespace Evento.Backend.Adapter.Mappers
{
    public class ConfigureOrganisationMapper
    {
        public Uri Schema => new Uri("configureorganisation/1.0", UriKind.RelativeOrAbsolute);

        private readonly Uri _source = new Uri("evento", UriKind.RelativeOrAbsolute);
        private readonly List<string> _dataContentTypes = new List<string> { "application/json", "application/cloudevents+json" };

        public Command Map(CloudEventRequest request)
        {
            Ensure.NotNull(request, nameof(request));

            if (!_dataContentTypes.Contains(request.DataContentType))
            {
                throw new ArgumentException($"While running Map in '{nameof(ConfigureOrganisationMapper)}' I can't recognize the DataContentType:{request.DataContentType} (DataSchema:{request.DataSchema};Source:{request.Source})");
            }

            if (_source.ToString() != "*" && !request.Source.Equals(_source))
            {
                throw new ArgumentException($"While running Map in '{nameof(ConfigureOrganisationMapper)}' I can't recognize the Source:{request.Source} (DataSchema:{request.DataSchema})");
            }

            if (!request.DataSchema.Equals(Schema))
            {
                throw new ArgumentException($"While running Map in '{nameof(ConfigureOrganisationMapper)}' I can't recognize the DataSchema:{request.DataSchema} (Source:{request.Source})");
            }

            ConfigureOrganisation cmd = JsonSerializer.Deserialize<ConfigureOrganisation>(request.Data.ToString(), new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            cmd.Metadata = new Dictionary<string, string>
            {
                {"$correlationId", cmd.Id},
                {"source", request.Source.ToString()},
                {"$applies", request.Time.ToString("O")},
                {"cloudrequest-id", request.Id},
                {"schema", request.DataSchema.ToString()},
                {"content-type", request.DataContentType}
            };

            return cmd;
        }
    }
}
