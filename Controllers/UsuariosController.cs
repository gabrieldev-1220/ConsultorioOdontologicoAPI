using ConsultorioOdontologicoAPI.Data;
using ConsultorioOdontologicoAPI.DTOs;
using ConsultorioOdontologicoAPI.Entities;
using ConsultorioOdontologicoAPI.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace ConsultorioOdontologicoAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsuariosController : ControllerBase
    {
        private readonly ConsultorioOdontologicoContext _context;
        private readonly IAuthService _authService;
        private readonly ILogger<UsuariosController> _logger;

        public UsuariosController(ConsultorioOdontologicoContext context, IAuthService authService, ILogger<UsuariosController> logger)
        {
            _context = context;
            _authService = authService;
            _logger = logger;
        }

        [HttpPost("login")]
        public async Task<ActionResult<LoginResponseDTO>> Login([FromBody] LoginRequestDTO loginRequest)
        {
            try
            {
                if (string.IsNullOrEmpty(loginRequest.Username) || string.IsNullOrEmpty(loginRequest.Password))
                {
                    return BadRequest(new { message = "Usuario y contraseña son obligatorios" });
                }

                var response = await _authService.LoginAsync(loginRequest);
                return Ok(response);
            }
            catch (UnauthorizedAccessException)
            {
                _logger.LogWarning("Intento de login fallido para usuario: {Username}", loginRequest.Username);
                return Unauthorized(new { message = "Credenciales inválidas" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error en el endpoint de login para usuario: {Username}", loginRequest.Username);
                return StatusCode(500, new { message = "Error interno del servidor" });
            }
        }

        [HttpPost("register")]
        [Authorize(Roles = "admin")]
        public async Task<ActionResult> Register([FromBody] UsuarioDTO usuarioDTO)
        {
            try
            {
                await _authService.RegisterAsync(usuarioDTO);
                return CreatedAtAction(nameof(GetUsuario), new { id = usuarioDTO.Username }, usuarioDTO);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al registrar usuario: {Username}", usuarioDTO.Username);
                return StatusCode(500, new { message = "Error interno al registrar" });
            }
        }

        [HttpGet]
        [Authorize(Roles = "admin")]
        public async Task<ActionResult<IEnumerable<Usuario>>> GetUsuarios()
        {
            try
            {
                return await _context.Usuarios
                    .Include(u => u.Odontologo)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener usuarios");
                return StatusCode(500, new { message = "Error interno al obtener usuarios" });
            }
        }

        [HttpGet("{id}")]
        [Authorize(Roles = "admin")]
        public async Task<ActionResult<Usuario>> GetUsuario(int id)
        {
            try
            {
                var usuario = await _context.Usuarios
                    .Include(u => u.Odontologo)
                    .FirstOrDefaultAsync(u => u.IdUsuario == id);

                if (usuario == null)
                    return NotFound();

                return usuario;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener usuario con ID: {Id}", id);
                return StatusCode(500, new { message = "Error interno al obtener usuario" });
            }
        }
    }
}