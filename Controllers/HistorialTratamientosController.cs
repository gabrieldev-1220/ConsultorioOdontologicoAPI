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
    public class HistorialTratamientosController : ControllerBase
    {
        private readonly ConsultorioOdontologicoContext _context;

        public HistorialTratamientosController(ConsultorioOdontologicoContext context)
        {
            _context = context;
        }

        [HttpGet]
        [Authorize(Roles = "admin,odontologo")]
        public async Task<ActionResult<IEnumerable<HistorialTratamiento>>> GetHistorialTratamientos()
        {
            return await _context.HistorialTratamientos
                .Include(ht => ht.HistorialClinico)
                .Include(ht => ht.Tratamiento)
                .ToListAsync();
        }

        [HttpGet("{id}")]
        [Authorize(Roles = "admin,odontologo")]
        public async Task<ActionResult<HistorialTratamiento>> GetHistorialTratamiento(int id)
        {
            var historialTratamiento = await _context.HistorialTratamientos
                .Include(ht => ht.HistorialClinico)
                .Include(ht => ht.Tratamiento)
                .FirstOrDefaultAsync(ht => ht.IdHistorialTratamiento == id);
            if (historialTratamiento == null)
                return NotFound();

            return historialTratamiento;
        }

        [HttpPost]
        [Authorize(Roles = "admin,odontologo")]
        public async Task<ActionResult<HistorialTratamiento>> CreateHistorialTratamiento(HistorialTratamiento historialTratamiento)
        {
            _context.HistorialTratamientos.Add(historialTratamiento);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetHistorialTratamiento), new { id = historialTratamiento.IdHistorialTratamiento }, historialTratamiento);
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "admin,odontologo")]
        public async Task<IActionResult> UpdateHistorialTratamiento(int id, HistorialTratamiento historialTratamiento)
        {
            if (id != historialTratamiento.IdHistorialTratamiento)
                return BadRequest();

            _context.Entry(historialTratamiento).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}
