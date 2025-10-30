using ConsultorioOdontologicoAPI.Data;
using ConsultorioOdontologicoAPI.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Globalization;

namespace ConsultorioOdontologicoAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class TurnosController : ControllerBase
    {
        private readonly ConsultorioOdontologicoContext _context;
        private readonly ILogger<TurnosController> _logger;

        public TurnosController(ConsultorioOdontologicoContext context, ILogger<TurnosController> logger)
        {
            _context = context;
            _logger = logger;
        }

        [HttpGet]
        [Authorize(Roles = "admin,recepcionista,odontologo")]
        public async Task<ActionResult<IEnumerable<Turno>>> GetTurnos()
        {
            try
            {
                var turnos = await _context.Turnos
                    .Include(t => t.Paciente)
                    .Include(t => t.Odontologo)
                    .ToListAsync();

                return Ok(turnos);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener turnos");
                return StatusCode(500, $"Error interno: {ex.Message}");
            }
        }

        [HttpGet("{id}")]
        [Authorize(Roles = "admin,recepcionista,odontologo")]
        public async Task<ActionResult<Turno>> GetTurno(int id)
        {
            try
            {
                var turno = await _context.Turnos
                    .Include(t => t.Paciente)
                    .Include(t => t.Odontologo)
                    .FirstOrDefaultAsync(t => t.IdTurno == id);

                if (turno == null)
                    return NotFound();

                return turno;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener turno con ID {Id}", id);
                return StatusCode(500, $"Error interno: {ex.Message}");
            }
        }

        public class TurnoAgendaQuery
        {
            [FromQuery(Name = "startDate")]
            public string? StartDate { get; set; }

            [FromQuery(Name = "endDate")]
            public string? EndDate { get; set; }

            [FromQuery(Name = "idOdontologo")]
            public int? IdOdontologo { get; set; }

            [FromQuery(Name = "estado")]
            public string? Estado { get; set; }
        }

        [HttpGet("agenda")]
        [Authorize(Roles = "admin,recepcionista,odontologo")]
        public async Task<ActionResult<IEnumerable<Turno>>> GetAgenda([FromQuery] TurnoAgendaQuery query)
        {
            try
            {
                _logger.LogInformation(
                    "Solicitud a GetAgenda con parámetros recibidos: StartDate={StartDate}, EndDate={EndDate}, IdOdontologo={IdOdontologo}, Estado={Estado}",
                    query.StartDate, query.EndDate, query.IdOdontologo, query.Estado
                );

                DateTime? start = ParseDate(query.StartDate);
                DateTime? end = ParseDate(query.EndDate);

                if (!start.HasValue && !end.HasValue)
                {
                    return BadRequest(new { message = "Se requiere al menos una fecha (startDate o endDate)." });
                }

                var queryable = _context.Turnos
                    .Include(t => t.Paciente)
                    .Include(t => t.Odontologo)
                    .AsQueryable();

                if (start.HasValue)
                    queryable = queryable.Where(t => t.FechaHora >= start.Value);

                if (end.HasValue)
                    queryable = queryable.Where(t => t.FechaHora <= end.Value);

                if (query.IdOdontologo.HasValue)
                    queryable = queryable.Where(t => t.IdOdontologo == query.IdOdontologo.Value);

                if (!string.IsNullOrEmpty(query.Estado))
                    queryable = queryable.Where(t => t.Estado == query.Estado);

                var turnos = await queryable
                    .OrderBy(t => t.FechaHora)
                    .ToListAsync();

                _logger.LogInformation("Consulta ejecutada, retornando {Count} turnos desde GetAgenda", turnos.Count);

                if (!turnos.Any())
                    _logger.LogWarning("No se encontraron turnos para los parámetros proporcionados.");

                return Ok(turnos);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener agenda");
                return StatusCode(500, new { message = $"Error interno: {ex.Message}" });
            }
        }

        private static DateTime? ParseDate(string? dateString)
        {
            if (string.IsNullOrWhiteSpace(dateString)) return null;

            // Acepta tanto "YYYY-MM-DD" como "YYYY-MM-DDTHH:mm:ssZ"
            string[] formats = {
                "yyyy-MM-dd",
                "yyyy-MM-ddTHH:mm:ss",
                "yyyy-MM-ddTHH:mm:ssZ",
                "yyyy-MM-ddTHH:mm:ss.fffZ"
            };

            if (DateTime.TryParseExact(dateString, formats, CultureInfo.InvariantCulture, DateTimeStyles.AssumeUniversal, out DateTime parsed))
            {
                return parsed.ToLocalTime();
            }

            return null;
        }

        [HttpPost]
        [Authorize(Roles = "admin,recepcionista")]
        public async Task<ActionResult<Turno>> CreateTurno(Turno turno)
        {
            try
            {
                var exists = await _context.Turnos
                    .AnyAsync(t => t.IdOdontologo == turno.IdOdontologo && t.FechaHora == turno.FechaHora && t.Estado != "cancelado");

                if (exists)
                    return BadRequest(new { message = "El odontólogo ya tiene un turno asignado en ese horario." });

                _context.Turnos.Add(turno);
                await _context.SaveChangesAsync();

                return CreatedAtAction(nameof(GetTurno), new { id = turno.IdTurno }, turno);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al crear turno");
                return StatusCode(500, new { message = $"Error interno: {ex.Message}" });
            }
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "admin,recepcionista")]
        public async Task<IActionResult> UpdateTurno(int id, Turno turno)
        {
            try
            {
                if (id != turno.IdTurno)
                    return BadRequest(new { message = "El ID del turno no coincide." });

                _context.Entry(turno).State = EntityState.Modified;
                await _context.SaveChangesAsync();

                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al actualizar turno con ID {Id}", id);
                return StatusCode(500, new { message = $"Error interno: {ex.Message}" });
            }
        }

        [HttpPut("{id}/cancelar")]
        [Authorize(Roles = "admin,recepcionista")]
        public async Task<IActionResult> CancelarTurno(int id)
        {
            try
            {
                var turno = await _context.Turnos.FindAsync(id);
                if (turno == null)
                    return NotFound(new { message = "Turno no encontrado." });

                turno.Estado = "cancelado";
                await _context.SaveChangesAsync();

                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al cancelar turno con ID {Id}", id);
                return StatusCode(500, new { message = $"Error interno: {ex.Message}" });
            }
        }
    }
}
