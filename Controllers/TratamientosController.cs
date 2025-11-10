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
    public class TratamientosController : ControllerBase
    {
        private readonly ConsultorioOdontologicoContext _context;

        public TratamientosController(ConsultorioOdontologicoContext context)
        {
            _context = context;
        }

        [HttpGet]
        [Authorize(Roles = "admin,recepcionista,odontologo")]
        public async Task<ActionResult<IEnumerable<Tratamiento>>> GetTratamientos()
        {
            return await _context.Tratamientos.ToListAsync();
        }

        [HttpGet("{id}")]
        [Authorize(Roles = "admin,recepcionista,odontologo")]
        public async Task<ActionResult<Tratamiento>> GetTratamiento(int id)
        {
            var tratamiento = await _context.Tratamientos.FindAsync(id);

            if (tratamiento == null)
                return NotFound();

            return tratamiento;
        }

        [HttpPost]
        [Authorize(Roles = "admin")]
        public async Task<ActionResult<Tratamiento>> CreateTratamiento(Tratamiento tratamiento)
        {
            _context.Tratamientos.Add(tratamiento);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetTratamiento), new { id = tratamiento.IdTratamiento }, tratamiento);
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> UpdateTratamiento(int id, Tratamiento tratamiento)
        {
            if (id != tratamiento.IdTratamiento)
                return BadRequest();

            _context.Entry(tratamiento).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}
