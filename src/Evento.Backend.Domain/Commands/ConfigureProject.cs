namespace Evento.Backend.Domain.Commands
{
    public class ConfigureProject : Command
    {
        public ConfigureProject(string id, string organisationId, string memberId, string identifier,
            string description, Uri link, IDictionary<string, string> metadata)
        {
            Id = id;
            OrganisationId = organisationId;
            MemberId = memberId;
            Identifier = identifier;
            Description = description;
            Link = link;
            Metadata = metadata;
        }

        public string Id { get; }
        public string OrganisationId { get; }
        public string MemberId { get; }
        public string Identifier { get; }
        public string Description { get; }
        public Uri Link { get; }
        public string Name { get; }
        public IDictionary<string, string> Metadata { get; set; }
    }
}
