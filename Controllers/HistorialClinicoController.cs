using ConsultorioOdontologicoAPI.Data;
using ConsultorioOdontologicoAPI.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using System.Text.Json;

namespace ConsultorioOdontologicoAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class HistorialClinicoController : ControllerBase
    {
        private readonly ConsultorioOdontologicoContext _context;
        private readonly ILogger<HistorialClinicoController> _logger;
        private readonly IWebHostEnvironment _env;

        public HistorialClinicoController(ConsultorioOdontologicoContext context, ILogger<HistorialClinicoController> logger, IWebHostEnvironment env)
        {
            _context = context;
            _logger = logger;
            _env = env;
        }

        // AÑADIDO: Endpoint raíz (evita 405)
        [HttpGet]
        [Authorize(Roles = "admin,odontologo")]
        public ActionResult<string> Get()
        {
            return Ok("HistorialClinico API funcionando");
        }

        [HttpGet("{id}")]
        [Authorize(Roles = "admin,odontologo")]
        public async Task<ActionResult<HistorialClinico>> GetHistorial(int id)
        {
            try
            {
                var historial = await _context.HistorialClinico
                    .Include(h => h.Paciente)
                    .Include(h => h.Odontologo)
                    .FirstOrDefaultAsync(h => h.IdHistorial == id);

                if (historial == null)
                    return NotFound();

                return Ok(historial);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener historial con ID {Id}", id);
                return StatusCode(500, $"Error interno: {ex.Message}");
            }
        }

        [HttpGet("pacientes/{idPaciente}")]
        [Authorize(Roles = "admin,odontologo")]
        public async Task<ActionResult<IEnumerable<HistorialClinico>>> GetHistorialesPorPaciente(int idPaciente)
        {
            try
            {
                var historiales = await _context.HistorialClinico
                    .Include(h => h.Paciente)
                    .Include(h => h.Odontologo)
                    .Where(h => h.IdPaciente == idPaciente)
                    .OrderByDescending(h => h.Fecha)
                    .ToListAsync();

                return Ok(historiales);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener historiales del paciente {IdPaciente}", idPaciente);
                return StatusCode(500, $"Error interno: {ex.Message}");
            }
        }

        [HttpPost]
        [Authorize(Roles = "admin,odontologo")]
        public async Task<ActionResult<HistorialClinico>> CreateHistorial([FromForm] HistorialClinicoForm form, IFormFile[]? archivos)
        {
            try
            {
                var historial = new HistorialClinico
                {
                    IdPaciente = form.IdPaciente,
                    IdOdontologo = form.IdOdontologo,
                    Fecha = DateTime.Now,
                    MotivoConsulta = form.MotivoConsulta,
                    Diagnostico = form.Diagnostico,
                    Observacion = form.Observacion,
                    PlanTratamiento = form.PlanTratamiento // ← Guardado en memoria
                };

                // Subida de archivos
                if (archivos != null && archivos.Length > 0)
                {
                    var uploadPath = Path.Combine(_env.WebRootPath, "uploads", "historiales", "temp");
                    Directory.CreateDirectory(uploadPath);

                    var archivosList = new List<object>();
                    foreach (var file in archivos)
                    {
                        var fileName = Guid.NewGuid() + Path.GetExtension(file.FileName);
                        var filePath = Path.Combine(uploadPath, fileName);
                        using (var stream = new FileStream(filePath, FileMode.Create))
                        {
                            await file.CopyToAsync(stream);
                        }
                        archivosList.Add(new { nombre = file.FileName, ruta = $"/uploads/historiales/temp/{fileName}" });
                    }
                    historial.Archivos = JsonSerializer.Serialize(archivosList);
                }

                _context.HistorialClinico.Add(historial);
                await _context.SaveChangesAsync();

                // Mover archivos al ID real
                if (historial.Archivos != null)
                {
                    var tempPath = Path.Combine(_env.WebRootPath, "uploads", "historiales", "temp");
                    var finalPath = Path.Combine(_env.WebRootPath, "uploads", "historiales", historial.IdHistorial.ToString());
                    if (Directory.Exists(tempPath))
                    {
                        Directory.CreateDirectory(finalPath);
                        foreach (var file in Directory.GetFiles(tempPath))
                        {
                            var dest = Path.Combine(finalPath, Path.GetFileName(file));
                            System.IO.File.Move(file, dest);
                        }
                        Directory.Delete(tempPath, true);
                    }

                    var archivosJson = JsonSerializer.Deserialize<List<Dictionary<string, string>>>(historial.Archivos);
                    if (archivosJson != null)
                    {
                        for (int i = 0; i < archivosJson.Count; i++)
                        {
                            var oldRuta = archivosJson[i]["ruta"];
                            var newRuta = oldRuta.Replace("/temp/", $"/{historial.IdHistorial}/");
                            archivosJson[i]["ruta"] = newRuta;
                        }
                        historial.Archivos = JsonSerializer.Serialize(archivosJson);
                        _context.HistorialClinico.Update(historial);
                        await _context.SaveChangesAsync();
                    }
                }

                return CreatedAtAction(nameof(GetHistorial), new { id = historial.IdHistorial }, historial);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al crear historial");
                return StatusCode(500, $"Error interno: {ex.Message}");
            }
        }

        [HttpGet("{id}/pdf")]
        [Authorize(Roles = "admin,odontologo")]
        public async Task<IActionResult> GenerarPdf(int id)
        {
            var historial = await _context.HistorialClinico
                .Include(h => h.Paciente)
                .Include(h => h.Odontologo)
                .FirstOrDefaultAsync(h => h.IdHistorial == id);

            if (historial == null) return NotFound();

            var document = Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Size(PageSizes.A4);
                    page.Margin(2, Unit.Centimetre);
                    page.PageColor(Colors.White);
                    page.DefaultTextStyle(x => x.FontSize(12));

                    page.Header().Text("Historial Clínico - COIN").FontSize(18).Bold().AlignCenter();
                    page.Content().Column(col =>
                    {
                        col.Item().Text($"Paciente: {historial.Paciente?.Nombre} {historial.Paciente?.Apellido}");
                        col.Item().Text($"Odontólogo: Dr. {historial.Odontologo?.Nombre} {historial.Odontologo?.Apellido}");
                        col.Item().Text($"Fecha: {historial.Fecha:dd/MM/yyyy}");
                        col.Item().Text($"Motivo: {historial.MotivoConsulta}");
                        col.Item().Text($"Diagnóstico: {historial.Diagnostico}");
                        col.Item().Text($"Tratamiento: {historial.PlanTratamiento}");
                        col.Item().Text($"Observación: {historial.Observacion}");
                    });

                    page.Footer().AlignCenter().Text(x =>
                    {
                        x.Span("Página ");
                        x.CurrentPageNumber();
                        x.Span(" de ");
                        x.TotalPages();
                    });
                });
            });

            var pdfBytes = document.GeneratePdf();
            return File(pdfBytes, "application/pdf", $"Historial_{id}.pdf");
        }
    }

    public class HistorialClinicoForm
    {
        public int IdPaciente { get; set; }
        public int IdOdontologo { get; set; }
        public string MotivoConsulta { get; set; } = string.Empty;
        public string Diagnostico { get; set; } = string.Empty;
        public string Observacion { get; set; } = string.Empty;
        public string PlanTratamiento { get; set; } = string.Empty;
    }
}