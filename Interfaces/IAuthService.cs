using ConsultorioOdontologicoAPI.DTOs;

namespace ConsultorioOdontologicoAPI.Interfaces
{
    public interface IAuthService
    {
        Task<LoginResponseDTO> LoginAsync(LoginRequestDTO loginRequest);
        Task RegisterAsync(UsuarioDTO usuarioDTO);
    }
}
