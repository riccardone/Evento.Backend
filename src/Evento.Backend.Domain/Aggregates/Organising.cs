using Evento.Backend.Domain.Commands;

namespace Evento.Backend.Domain.Aggregates
{
    public class Organising : AggregateBase
    {
        public override string AggregateId => CorrelationId;
        private string CorrelationId { get; set; }

        public Organising()
        {
            
        }

        public static Organising Create()
        {
            return new Organising();
        }

        public void ConfigureOrganisation(ConfigureOrganisation cmd)
        {
            Ensure.NotNull(cmd, nameof(cmd));
            Ensure.NotNullOrEmpty(cmd.Id, nameof(cmd.Id));
            Ensure.NotNullOrEmpty(cmd.Name, nameof(cmd.Name));
            Ensure.NotNull(cmd.Metadata, nameof(cmd.Metadata));
            Ensure.NotNullOrEmpty(cmd.Metadata["$correlationId"], "$correlationId");

            // Idempotent

            throw new NotImplementedException();
        }
    }
}
