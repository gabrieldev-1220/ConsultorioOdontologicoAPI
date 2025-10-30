using ConsultorioOdontologicoAPI.Data;
using ConsultorioOdontologicoAPI.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ConsultorioOdontologicoAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class PagosController : ControllerBase
    {
        private readonly ConsultorioOdontologicoContext _context;

        public PagosController(ConsultorioOdontologicoContext context)
        {
            _context = context;
        }

        [HttpGet]
        [Authorize(Roles = "admin,recepcionista,odontologo")] // Añadido 'odontologo'
        public async Task<ActionResult<IEnumerable<Pago>>> GetPagos()
        {
            return await _context.Pagos
                .Include(p => p.Paciente)
                .ToListAsync();
        }

        [HttpGet("{id}")]
        [Authorize(Roles = "admin,recepcionista,odontologo")] // Añadido 'odontologo'
        public async Task<ActionResult<Pago>> GetPago(int id)
        {
            var pago = await _context.Pagos
                .Include(p => p.Paciente)
                .FirstOrDefaultAsync(p => p.IdPago == id);
            if (pago == null)
                return NotFound();

            return pago;
        }

        [HttpGet("paciente/{idPaciente}")]
        [Authorize(Roles = "admin,recepcionista,odontologo")] // Añadido 'odontologo'
        public async Task<ActionResult<IEnumerable<Pago>>> GetPagosPorPaciente(int idPaciente)
        {
            return await _context.Pagos
                .Where(p => p.IdPaciente == idPaciente)
                .Include(p => p.Paciente)
                .ToListAsync();
        }

        [HttpPost]
        [Authorize(Roles = "admin,recepcionista")] // Mantengo restricción original
        public async Task<ActionResult<Pago>> CreatePago(Pago pago)
        {
            _context.Pagos.Add(pago);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetPago), new { id = pago.IdPago }, pago);
        }
    }
}