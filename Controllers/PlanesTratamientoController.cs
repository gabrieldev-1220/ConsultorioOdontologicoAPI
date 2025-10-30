using ConsultorioOdontologicoAPI.Data;
using ConsultorioOdontologicoAPI.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;

namespace ConsultorioOdontologicoAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PlanesTratamientoController : ControllerBase
    {
        private readonly ConsultorioOdontologicoContext _context;

        public PlanesTratamientoController(ConsultorioOdontologicoContext context)
        {
            _context = context;
        }

        // POST: api/PlanesTratamiento/5
        [HttpPost("{idPaciente}")]
        public async Task<IActionResult> CrearOActualizarPlan(int idPaciente, [FromBody] PlanTratamientoRequest payload)
        {
            try
            {
                if (payload == null || payload.IdProcedimientos == null || !payload.IdProcedimientos.Any())
                    return BadRequest("Debe seleccionar al menos un procedimiento.");

                var pacienteExists = await _context.Pacientes.AnyAsync(p => p.IdPaciente == idPaciente);
                if (!pacienteExists)
                    return NotFound("Paciente no encontrado.");

                // Validar procedimientos
                var procedimientosValidos = await _context.Procedimientos
                    .Where(p => payload.IdProcedimientos.Contains(p.IdProcedimiento) && p.Activo)
                    .Select(p => p.IdProcedimiento)
                    .ToListAsync();

                if (procedimientosValidos.Count != payload.IdProcedimientos.Count)
                    return BadRequest("Uno o más procedimientos no son válidos o están inactivos.");

                // Buscar plan existente
                var planExistente = await _context.PlanesTratamiento
                    .FirstOrDefaultAsync(pt => pt.IdPaciente == idPaciente);

                var datosPlan = new
                {
                    procedimientos = procedimientosValidos,
                    observaciones = payload.Observaciones ?? ""
                };

                var jsonDatos = JsonSerializer.Serialize(datosPlan, new JsonSerializerOptions { WriteIndented = false });

                if (planExistente == null)
                {
                    // Crear nuevo (usamos un procedimiento dummy, el primero)
                    var primerProc = procedimientosValidos.First();
                    var nuevoPlan = new PlanTratamiento
                    {
                        IdPaciente = idPaciente,
                        IdProcedimiento = primerProc,
                        FechaPlan = DateTime.Now,
                        Estado = "pendiente",
                        Observaciones = jsonDatos
                    };

                    _context.PlanesTratamiento.Add(nuevoPlan);
                }
                else
                {
                    // Actualizar existente
                    planExistente.Observaciones = jsonDatos;
                    planExistente.FechaPlan = DateTime.Now;
                    // No tocamos IdProcedimiento ni Estado
                }

                await _context.SaveChangesAsync();

                return Ok(new { mensaje = "Plan de tratamiento guardado correctamente." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error al guardar el plan de tratamiento: {ex.Message}");
            }
        }

        // GET: api/PlanesTratamiento/5
        [HttpGet("{idPaciente}")]
        public async Task<ActionResult<PlanTratamientoResponse>> GetPorPaciente(int idPaciente)
        {
            try
            {
                var plan = await _context.PlanesTratamiento
                    .Where(pt => pt.IdPaciente == idPaciente)
                    .Select(pt => new
                    {
                        pt.IdPlanTratamiento,
                        pt.Observaciones,
                        pt.Estado,
                        pt.FechaPlan
                    })
                    .FirstOrDefaultAsync();

                if (plan == null)
                    return Ok(new PlanTratamientoResponse
                    {
                        IdProcedimientos = new List<int>(),
                        Observaciones = "",
                        Estado = "pendiente",
                        FechaPlan = null
                    });

                List<int> procedimientos = new();
                string observaciones = "";

                if (!string.IsNullOrWhiteSpace(plan.Observaciones))
                {
                    try
                    {
                        var jsonDoc = JsonDocument.Parse(plan.Observaciones);
                        var root = jsonDoc.RootElement;
                        procedimientos = root.GetProperty("procedimientos").EnumerateArray()
                            .Select(x => x.GetInt32()).ToList();
                        observaciones = root.TryGetProperty("observaciones", out var obs) ? obs.GetString() ?? "" : "";
                    }
                    catch
                    {
                        // Si falla el parse, usar como observación plana
                        observaciones = plan.Observaciones;
                    }
                }

                return Ok(new PlanTratamientoResponse
                {
                    IdProcedimientos = procedimientos,
                    Observaciones = observaciones,
                    Estado = plan.Estado,
                    FechaPlan = plan.FechaPlan
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error al obtener el plan: {ex.Message}");
            }
        }
    }

    // DTOs para entrada y salida
    public class PlanTratamientoRequest
    {
        public List<int> IdProcedimientos { get; set; } = new();
        public string? Observaciones { get; set; }
    }

    public class PlanTratamientoResponse
    {
        public List<int> IdProcedimientos { get; set; } = new();
        public string Observaciones { get; set; } = "";
        public string Estado { get; set; } = "pendiente";
        public DateTime? FechaPlan { get; set; }
    }
}