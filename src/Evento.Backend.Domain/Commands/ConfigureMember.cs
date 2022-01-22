namespace Evento.Backend.Domain.Commands
{
    public class ConfigureMember : Command
    {
        public ConfigureMember(string id, string organisationId, string name, IDictionary<string, string> metadata)
        {
            Id = id;
            OrganisationId = organisationId;
            Name = name;
            Metadata = metadata;
        }

        public string Id { get; }
        public string OrganisationId { get; }
        public string Name { get; }
        public IDictionary<string, string> Metadata { get; set; }
    }
}
