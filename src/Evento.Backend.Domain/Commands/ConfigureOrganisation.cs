namespace Evento.Backend.Domain.Commands
{
    public class ConfigureOrganisation : Command
    {
        public ConfigureOrganisation(string id, string name, IDictionary<string, string> metadata)
        {
            Id = id;
            Name = name;
            Metadata = metadata;
        }

        public string Id { get; }
        public string Name { get; }
        public IDictionary<string, string> Metadata { get; set; }
    }
}
