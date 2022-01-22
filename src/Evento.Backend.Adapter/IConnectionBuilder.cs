namespace Evento.Backend.Adapter
{
    public interface IConnectionBuilder
    {
        object Build(bool openConnection = true);
    }
}
