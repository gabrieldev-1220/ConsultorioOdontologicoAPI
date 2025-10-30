using ConsultorioOdontologicoAPI.Data;
using ConsultorioOdontologicoAPI.DTOs;
using ConsultorioOdontologicoAPI.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using BCrypt.Net;
using ConsultorioOdontologicoAPI.Interfaces;

namespace ConsultorioOdontologicoAPI.Services
{
    public class AuthService : IAuthService
    {
        private readonly ConsultorioOdontologicoContext _context;
        private readonly IConfiguration _configuration;

        public AuthService(ConsultorioOdontologicoContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        public async Task<LoginResponseDTO> LoginAsync(LoginRequestDTO loginRequest)
        {
            var usuario = await _context.Usuarios
                .Include(u => u.Odontologo) // Incluir la relación con Odontologo
                .FirstOrDefaultAsync(u => u.Username == loginRequest.Username);

            if (usuario == null || !BCrypt.Net.BCrypt.Verify(loginRequest.Password, usuario.PasswordHash))
            {
                throw new UnauthorizedAccessException("Credenciales inválidas");
            }

            // Obtener datos del odontólogo si existe
            string fullName = "Odontólogo Desconocido";
            string matricula = "N/A";
            if (usuario.IdOdontologo.HasValue && usuario.Odontologo != null)
            {
                fullName = $"{usuario.Odontologo.Nombre} {usuario.Odontologo.Apellido}";
                matricula = usuario.Odontologo.Matricula;
            }

            var token = GenerateJwtToken(usuario);
            return new LoginResponseDTO
            {
                Token = token,
                Rol = usuario.Rol,
                IdOdontologo = usuario.IdOdontologo,
                FullName = fullName, // Añadir nombre completo
                Matricula = matricula // Añadir matrícula
            };
        }

        public async Task RegisterAsync(UsuarioDTO usuarioDTO)
        {
            var usuario = new Usuario
            {
                Username = usuarioDTO.Username,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(usuarioDTO.Password),
                Rol = usuarioDTO.Rol,
                IdOdontologo = usuarioDTO.IdOdontologo
            };

            _context.Usuarios.Add(usuario);
            await _context.SaveChangesAsync();
        }

        private string GenerateJwtToken(Usuario usuario)
        {
            var claims = new[]
            {
                new Claim(ClaimTypes.Name, usuario.Username),
                new Claim(ClaimTypes.Role, usuario.Rol),
                new Claim("IdUsuario", usuario.IdUsuario.ToString()),
                new Claim("IdOdontologo", usuario.IdOdontologo?.ToString() ?? string.Empty)
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken
            (
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                claims: claims,
                expires: DateTime.Now.AddMinutes(double.Parse(_configuration["Jwt:ExpiryInMinutes"])),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}