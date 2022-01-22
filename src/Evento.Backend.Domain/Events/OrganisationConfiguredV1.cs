namespace Evento.Backend.Domain.Events
{
    public class OrganisationConfiguredV1 : Event
    {
        public OrganisationConfiguredV1(IDictionary<string, string> metadata)
        {
            Metadata = metadata;
        }
        public IDictionary<string, string> Metadata { get; }
    }
}
