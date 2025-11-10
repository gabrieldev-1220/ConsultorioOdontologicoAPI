namespace ConsultorioOdontologicoAPI.Interfaces
{
    public interface IBitacoraService
    {
        Task LogActionAsync(int idUsuario, string accion, string detalles);
    }
}
