namespace Evento.Backend.Domain.Events
{
    public class MemberConfiguredV1 : Event
    {
        public MemberConfiguredV1(IDictionary<string, string> metadata)
        {
            Metadata = metadata;
        }
        public IDictionary<string, string> Metadata { get; }
    }
}
