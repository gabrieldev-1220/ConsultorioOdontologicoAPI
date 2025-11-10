
using ConsultorioOdontologicoAPI.Data;
using ConsultorioOdontologicoAPI.Entities;
using ConsultorioOdontologicoAPI.Interfaces;

namespace ConsultorioOdontologicoAPI.Services
{
    public class BitacoraService : IBitacoraService
    {
        private readonly ConsultorioOdontologicoContext _context;

        // CONSTRUCTOR
        public BitacoraService(ConsultorioOdontologicoContext context)
        {
            _context = context;
        }

        public async Task LogActionAsync(int idUsuario, string accion, string detalles)
        {
            var bitacora = new Bitacora
            {
                IdUsuario = idUsuario,
                Accion = accion,
                Detalles = detalles
            };
            _context.Bitacora.Add(bitacora);
            await _context.SaveChangesAsync();
        }
    }
}
