namespace KAMBIO.CORE.CORE.Interfaces
{
    public interface IOfertaService
    {
        Task CancelarOfertaAsync(int idOferta, int idUsuario);
    }
}
