using ConsultorioOdontologicoAPI.Data;
using ConsultorioOdontologicoAPI.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ConsultorioOdontologicoAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OdontogramaController : ControllerBase
    {
        private readonly ConsultorioOdontologicoContext _context;

        public OdontogramaController(ConsultorioOdontologicoContext context)
        {
            _context = context;
        }

        // =============================
        // GET: api/Odontograma/{idPaciente}
        // =============================
        [HttpGet("{idPaciente}")]
        public async Task<ActionResult<IEnumerable<PiezaDental>>> GetPorPaciente(int idPaciente)
        {
            try
            {
                var piezas = await _context.Odontograma
                    .Where(p => p.IdPaciente == idPaciente)
                    .ToListAsync();

                // DEVOLVER [] en vez de 404
                return Ok(piezas ?? new List<PiezaDental>());
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error al obtener el odontograma: {ex.Message}");
            }
        }

        // =============================
        // POST: api/Odontograma/{idPaciente}
        // Crea el odontograma inicial (32 piezas) SI NO EXISTE
        // =============================
        [HttpPost("{idPaciente}")]
        public async Task<ActionResult<IEnumerable<PiezaDental>>> CrearInicial(int idPaciente)
        {
            try
            {
                // Verificar si ya existe
                var existentes = await _context.Odontograma
                    .AnyAsync(p => p.IdPaciente == idPaciente);

                if (existentes)
                {
                    // DEVOLVER LAS PIEZAS EXISTENTES (no error)
                    var piezasExistentes = await _context.Odontograma
                        .Where(p => p.IdPaciente == idPaciente)
                        .ToListAsync();
                    return Ok(piezasExistentes);
                }

                // Crear 32 piezas iniciales
                var piezas = new List<PiezaDental>();
                for (int i = 1; i <= 32; i++)
                {
                    piezas.Add(new PiezaDental
                    {
                        IdPaciente = idPaciente,
                        NumeroPieza = i,
                        Estado = "Sano",
                        Color = "#FFFFFF",
                        FechaActualizacion = DateTime.Now
                    });
                }

                await _context.Odontograma.AddRangeAsync(piezas);
                await _context.SaveChangesAsync();

                return Ok(piezas);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error al crear el odontograma inicial: {ex.Message}");
            }
        }

        // =============================
        // PUT: api/Odontograma/{idPaciente}
        // Actualiza todas las piezas del paciente
        // =============================
        [HttpPut("{idPaciente}")]
        public async Task<IActionResult> ActualizarPorPaciente(int idPaciente, [FromBody] List<PiezaDental> piezasActualizadas)
        {
            try
            {
                var piezasExistentes = await _context.Odontograma
                    .Where(p => p.IdPaciente == idPaciente)
                    .ToListAsync();

                if (piezasExistentes == null || piezasExistentes.Count == 0)
                    return NotFound("No se encontraron piezas para actualizar.");

                foreach (var pieza in piezasActualizadas)
                {
                    var existente = piezasExistentes.FirstOrDefault(p => p.NumeroPieza == pieza.NumeroPieza);
                    if (existente != null)
                    {
                        existente.Color = pieza.Color;           // Color oclusal
                        existente.Estado = pieza.Estado;
                        existente.Observaciones = pieza.Observaciones; // JSON con todos los sectores
                        existente.FechaActualizacion = DateTime.Now;
                    }
                }

                await _context.SaveChangesAsync();
                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error al actualizar el odontograma: {ex.Message}");
            }
        }

        // =============================
        // DELETE: api/Odontograma/{idPaciente}
        // Elimina todas las piezas de un paciente
        // =============================
        [HttpDelete("{idPaciente}")]
        public async Task<IActionResult> EliminarPorPaciente(int idPaciente)
        {
            try
            {
                var piezas = await _context.Odontograma
                    .Where(p => p.IdPaciente == idPaciente)
                    .ToListAsync();

                if (piezas == null || piezas.Count == 0)
                    return NotFound("No se encontraron piezas para eliminar.");

                _context.Odontograma.RemoveRange(piezas);
                await _context.SaveChangesAsync();

                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error al eliminar el odontograma: {ex.Message}");
            }
        }
    }
}