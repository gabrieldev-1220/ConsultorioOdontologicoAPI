using ConsultorioOdontologicoAPI.Data;
using ConsultorioOdontologicoAPI.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ConsultorioOdontologicoAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "admin")]
    public class ReportesController : ControllerBase
    {
        private readonly ConsultorioOdontologicoContext _context;

        // CONSTRUCTOR
        public ReportesController(ConsultorioOdontologicoContext context)
        {
            _context = context;
        }

        [HttpGet("pacientes-atendidos")]
        public async Task<ActionResult<IEnumerable<HistorialClinico>>> GetPacientesAtendidos(DateTime startDate, DateTime endDate)
        {
            return await _context.HistorialClinico
                .Where(h => h.Fecha >= startDate && h.Fecha <= endDate)
                .Include(h => h.Paciente)
                .Include(h => h.Odontologo)
                .ToListAsync();
        }

        [HttpGet("turnos-por-odontologo")]
        public async Task<ActionResult<IEnumerable<Turno>>> GetTurnosPorOdontologo(int idOdontologo, string estado)
        {
            var query = _context.Turnos
                .Where(t => t.IdOdontologo == idOdontologo)
                .Include(t => t.Paciente)
                .Include(t => t.Odontologo)
                .AsQueryable();

            if (!string.IsNullOrEmpty(estado))
                query = query.Where(t => t.Estado == estado);

            return await query.ToListAsync();
        }

        [HttpGet("ingresos")]
        public async Task<ActionResult<decimal>> GetIngresos(DateTime startDate, DateTime endDate)
        {
            var ingresos = await _context.Pagos
                .Where(p => p.FechaPago >= startDate && p.FechaPago <= endDate)
                .SumAsync(p => p.Monto);
            return ingresos;
        }

        [HttpGet("deudas-pendientes")]
        public async Task<ActionResult<IEnumerable<object>>> GetDeudasPendientes()
        {
            var deudas = await _context.Pacientes
                .Select(p => new
                {
                    p.IdPaciente,
                    p.Nombre,
                    p.Apellido,
                    SaldoPendiente = _context.HistorialTratamientos
                        .Where(ht => ht.HistorialClinico.IdPaciente == p.IdPaciente)
                        .Sum(ht => ht.Cantidad * ht.PrecioUnitario) - _context.Pagos
                        .Where(pg => pg.IdPaciente == p.IdPaciente)
                        .Sum(pg => pg.Monto)
                })
                .Where(p => p.SaldoPendiente > 0)
                .ToListAsync();
            return deudas;
        }
    }
}
