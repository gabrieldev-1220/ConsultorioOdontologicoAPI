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
    public class InformacionMedicaController : ControllerBase
    {
        private readonly ConsultorioOdontologicoContext _context;

        public InformacionMedicaController(ConsultorioOdontologicoContext context)
        {
            _context = context;
        }

        [HttpGet("paciente/{idPaciente}")]
        public async Task<ActionResult<InformacionMedica>> GetPorPaciente(int idPaciente)
        {
            var info = await _context.InformacionMedica
                .FirstOrDefaultAsync(i => i.IdPaciente == idPaciente);

            if (info == null)
                return NotFound();

            return Ok(info);
        }

        [HttpPost]
        public async Task<ActionResult<InformacionMedica>> Crear(InformacionMedica info)
        {
            _context.InformacionMedica.Add(info);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetPorPaciente), new { idPaciente = info.IdPaciente }, info);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Actualizar(int id, InformacionMedica info)
        {
            if (id != info.IdInformacion)
                return BadRequest();

            _context.Entry(info).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}