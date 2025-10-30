using ConsultorioOdontologicoAPI.Data;
using ConsultorioOdontologicoAPI.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace ConsultorioOdontologicoAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class HistorialClinicoController : ControllerBase
    {
        private readonly ConsultorioOdontologicoContext _context;
        private readonly ILogger<HistorialClinicoController> _logger;

        public HistorialClinicoController(ConsultorioOdontologicoContext context, ILogger<HistorialClinicoController> logger)
        {
            _context = context;
            _logger = logger;
        }

        [HttpGet]
        [Authorize(Roles = "admin,odontologo")]
        public async Task<ActionResult<IEnumerable<HistorialClinico>>> GetHistoriales()
        {
            try
            {
                _logger.LogInformation("Solicitud a GetHistoriales");
                var historiales = await _context.HistorialClinico
                    .Include(h => h.Paciente)
                    .Include(h => h.Odontologo)
                    .ToListAsync();
                return Ok(historiales);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener historiales");
                return StatusCode(500, $"Error interno: {ex.Message}");
            }
        }

        [HttpGet("{id}")]
        [Authorize(Roles = "admin,odontologo")]
        public async Task<ActionResult<HistorialClinico>> GetHistorial(int id)
        {
            try
            {
                _logger.LogInformation("Solicitud a GetHistorial con ID {Id}", id);
                var historial = await _context.HistorialClinico
                    .Include(h => h.Paciente)
                    .Include(h => h.Odontologo)
                    .FirstOrDefaultAsync(h => h.IdHistorial == id);

                if (historial == null)
                    return NotFound();

                return Ok(historial);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener historial con ID {Id}", id);
                return StatusCode(500, $"Error interno: {ex.Message}");
            }
        }

        // MÉTODO CORREGIDO: SIN .Include(h => h.Evoluciones)
        [HttpGet("pacientes/{idPaciente}")]
        [Authorize(Roles = "admin,odontologo")]
        public async Task<ActionResult<IEnumerable<HistorialClinico>>> GetHistorialesPorPaciente(int idPaciente)
        {
            try
            {
                _logger.LogInformation("Solicitud a GetHistorialesPorPaciente con ID {IdPaciente}", idPaciente);

                var historiales = await _context.HistorialClinico
                    .Include(h => h.Paciente)
                    .Include(h => h.Odontologo)
                    .Where(h => h.IdPaciente == idPaciente)
                    .OrderByDescending(h => h.Fecha)
                    .ToListAsync();

                return Ok(historiales);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener historiales del paciente {IdPaciente}", idPaciente);
                return StatusCode(500, $"Error interno: {ex.Message}");
            }
        }

        [HttpPost]
        [Authorize(Roles = "admin,odontologo")]
        public async Task<ActionResult<HistorialClinico>> CreateHistorial(HistorialClinico historial)
        {
            try
            {
                _context.HistorialClinico.Add(historial);
                await _context.SaveChangesAsync();
                return CreatedAtAction(nameof(GetHistorial), new { id = historial.IdHistorial }, historial);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al crear historial");
                return StatusCode(500, $"Error interno: {ex.Message}");
            }
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "admin,odontologo")]
        public async Task<IActionResult> UpdateHistorial(int id, HistorialClinico historial)
        {
            try
            {
                if (id != historial.IdHistorial)
                    return BadRequest();

                _context.Entry(historial).State = EntityState.Modified;
                await _context.SaveChangesAsync();
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al actualizar historial con ID {Id}", id);
                return StatusCode(500, $"Error interno: {ex.Message}");
            }
        }
    }
}