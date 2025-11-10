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
    public class OdontologoController : ControllerBase
    {
        private readonly ConsultorioOdontologicoContext _context;

        public OdontologoController(ConsultorioOdontologicoContext context)
        {
            _context = context;
        }

        [HttpGet]
        [Authorize(Roles = "admin,recepcionista")]
        public async Task<ActionResult<IEnumerable<Odontologo>>> GetOdontologos()
        {
            return await _context.Odontologos.ToListAsync();
        }

        [HttpGet("{id}")]
        [Authorize(Roles = "admin,recepcionista,odontologo")]
        public async Task<ActionResult<Odontologo>> GetOdontologo(int id)
        {
            var odontologo = await _context.Odontologos.FindAsync(id);
            if (odontologo == null)
                return NotFound();
            return odontologo;
        }

        [HttpGet("search")]
        [Authorize(Roles = "admin,recepcionista")]
        public async Task<ActionResult<IEnumerable<Odontologo>>> SearchOdontologo(string query)
        {
            var odontologos = await _context.Odontologos
                .Where(o => o.Nombre.Contains(query) || o.Apellido.Contains(query) || o.Matricula.Contains(query))
                .ToListAsync();
            return odontologos;
        }

        [HttpPost]
        [Authorize(Roles = "admin")]
        public async Task<ActionResult<Odontologo>> CreateOdontologo(Odontologo odontologo)
        {
            _context.Odontologos.Add(odontologo);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetOdontologo), new { id = odontologo.IdOdontologo }, odontologo);
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> UpdateOdontologo(int id, Odontologo odontologo)
        {
            if (id != odontologo.IdOdontologo)
                return BadRequest();

            _context.Entry(odontologo).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}