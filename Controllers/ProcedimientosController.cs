using ConsultorioOdontologicoAPI.Data;
using ConsultorioOdontologicoAPI.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ConsultorioOdontologicoAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProcedimientosController : ControllerBase
    {
        private readonly ConsultorioOdontologicoContext _context;

        public ProcedimientosController(ConsultorioOdontologicoContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Procedimiento>>> GetAll()
        {
            return Ok(await _context.Procedimientos
                .Where(p => p.Activo)
                .OrderBy(p => p.Categoria)
                .ThenBy(p => p.Nombre)
                .ToListAsync());
        }

        [HttpGet("categorias")]
        public async Task<ActionResult<IEnumerable<string>>> GetCategorias()
        {
            var categorias = await _context.Procedimientos
                .Where(p => p.Activo)
                .Select(p => p.Categoria)
                .Distinct()
                .OrderBy(c => c)
                .ToListAsync();
            return Ok(categorias);
        }
    }
}