using ConsultorioOdontologicoAPI.Data;
using ConsultorioOdontologicoAPI.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ConsultorioOdontologicoAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "admin")]
    public class BitacoraController : ControllerBase
    {
        private readonly ConsultorioOdontologicoContext _context;

        // CONSTRUCTOR
        public BitacoraController(ConsultorioOdontologicoContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Bitacora>>> GetBitacora()
        {
            return await _context.Bitacora
                .Include(b => b.Usuario)
                .ToListAsync();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Bitacora>> GetBitacoraEntry(int id)
        {
            var bitacora = await _context.Bitacora
                .Include(b => b.Usuario)
                .FirstOrDefaultAsync(b => b.IdBitacora == id);
            if (bitacora == null)
                return NotFound();

            return bitacora;
        }
    }
}
