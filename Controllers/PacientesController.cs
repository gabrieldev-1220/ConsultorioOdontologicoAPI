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
    public class PacientesController : ControllerBase
    {
        private readonly ConsultorioOdontologicoContext _context;

        public PacientesController(ConsultorioOdontologicoContext context)
        {
            _context = context;
        }

        [HttpGet]
        [Authorize(Roles = "admin,recepcionista,odontologo")]
        public async Task<ActionResult<IEnumerable<Paciente>>> GetPacientes(int? odontologoId = null)
        {
            try
            {
                var query = _context.Pacientes.AsQueryable();
                if (odontologoId.HasValue)
                {
                    query = query.Where(p => _context.Turnos.Any(t => t.IdPaciente == p.IdPaciente && t.IdOdontologo == odontologoId));
                }
                var pacientes = await query
                    .Select(p => new Paciente
                    {
                        IdPaciente = p.IdPaciente,
                        Nombre = p.Nombre,
                        Apellido = p.Apellido,
                        Dni = p.Dni,
                        FechaNacimiento = p.FechaNacimiento,
                        Telefono = p.Telefono,
                        Email = p.Email,
                        Direccion = p.Direccion,
                        FechaRegistro = p.FechaRegistro,
                        Activo = p.Activo,
                        Turnos = p.Turnos.Select(t => new Turno
                        {
                            IdTurno = t.IdTurno,
                            IdPaciente = t.IdPaciente,
                            IdOdontologo = t.IdOdontologo,
                            FechaHora = t.FechaHora,
                            Estado = t.Estado,
                            Observaciones = t.Observaciones
                        }).ToList(),
                        Historiales = p.Historiales.Select(h => new HistorialClinico
                        {
                            IdHistorial = h.IdHistorial,
                            IdPaciente = h.IdPaciente,
                            IdOdontologo = h.IdOdontologo,
                            Fecha = h.Fecha,
                            MotivoConsulta = h.MotivoConsulta,
                            Diagnostico = h.Diagnostico,
                            Observacion = h.Observacion
                        }).ToList(),
                        Pagos = p.Pagos.ToList()
                    })
                    .ToListAsync();
                return Ok(pacientes);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error interno: {ex.Message}");
            }
        }

        [HttpGet("{id}")]
        [Authorize(Roles = "admin,recepcionista,odontologo")]
        public async Task<ActionResult<Paciente>> GetPaciente(int id)
        {
            var paciente = await _context.Pacientes
                .Where(p => p.IdPaciente == id)
                .Select(p => new Paciente
                {
                    IdPaciente = p.IdPaciente,
                    Nombre = p.Nombre,
                    Apellido = p.Apellido,
                    Dni = p.Dni,
                    FechaNacimiento = p.FechaNacimiento,
                    Telefono = p.Telefono,
                    Email = p.Email,
                    Direccion = p.Direccion,
                    FechaRegistro = p.FechaRegistro,
                    Activo = p.Activo,
                    Turnos = p.Turnos.Select(t => new Turno
                    {
                        IdTurno = t.IdTurno,
                        IdPaciente = t.IdPaciente,
                        IdOdontologo = t.IdOdontologo,
                        FechaHora = t.FechaHora,
                        Estado = t.Estado,
                        Observaciones = t.Observaciones
                    }).ToList(),
                    Historiales = p.Historiales.Select(h => new HistorialClinico
                    {
                        IdHistorial = h.IdHistorial,
                        IdPaciente = h.IdPaciente,
                        IdOdontologo = h.IdOdontologo,
                        Fecha = h.Fecha,
                        MotivoConsulta = h.MotivoConsulta,
                        Diagnostico = h.Diagnostico,
                        Observacion = h.Observacion
                    }).ToList(),
                    Pagos = p.Pagos.ToList()
                })
                .FirstOrDefaultAsync();

            if (paciente == null)
            {
                return NotFound();
            }

            return Ok(paciente);
        }

        [HttpGet("search")]
        [Authorize(Roles = "admin,recepcionista")]
        public async Task<ActionResult<IEnumerable<Paciente>>> SearchPacientes(string query)
        {
            var pacientes = await _context.Pacientes
                .Where(p => p.Nombre.Contains(query) || p.Apellido.Contains(query) || p.Dni.Contains(query) || p.Telefono.Contains(query))
                .Select(p => new Paciente
                {
                    IdPaciente = p.IdPaciente,
                    Nombre = p.Nombre,
                    Apellido = p.Apellido,
                    Dni = p.Dni,
                    FechaNacimiento = p.FechaNacimiento,
                    Telefono = p.Telefono,
                    Email = p.Email,
                    Direccion = p.Direccion,
                    FechaRegistro = p.FechaRegistro,
                    Activo = p.Activo
                })
                .ToListAsync();
            return Ok(pacientes);
        }

        [HttpPost]
        [Authorize(Roles = "admin,recepcionista")]
        public async Task<ActionResult<Paciente>> CreatePaciente(Paciente paciente)
        {
            _context.Pacientes.Add(paciente);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetPaciente), new { id = paciente.IdPaciente }, paciente);
        }

        [HttpPut]
        [Authorize(Roles = "admin,recepcionista")]
        public async Task<IActionResult> UpdatePaciente(int id, Paciente paciente)
        {
            if (id != paciente.IdPaciente)
                return BadRequest();

            _context.Entry(paciente).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return NoContent();
        }

        [HttpPut("{id}/toggle-activo")]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> ToggleActivo(int id)
        {
            var paciente = await _context.Pacientes.FindAsync(id);
            if (paciente == null)
                return NotFound();

            paciente.Activo = !paciente.Activo;
            await _context.SaveChangesAsync();
            return NoContent();
        }

        [HttpGet("{id}/historial")]
        [Authorize(Roles = "admin,recepcionista,odontologo")]
        public async Task<ActionResult<IEnumerable<HistorialClinico>>> GetHistorial(int id)
        {
            return await _context.HistorialClinico
                .Where(h => h.IdPaciente == id)
                .Include(h => h.Odontologo)
                .Select(h => new HistorialClinico
                {
                    IdHistorial = h.IdHistorial,
                    IdPaciente = h.IdPaciente,
                    IdOdontologo = h.IdOdontologo,
                    Fecha = h.Fecha,
                    MotivoConsulta = h.MotivoConsulta,
                    Diagnostico = h.Diagnostico,
                    Observacion = h.Observacion
                })
                .OrderBy(h => h.Fecha)
                .ToListAsync();
        }
    }
}