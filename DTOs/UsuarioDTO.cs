using System.ComponentModel.DataAnnotations;

namespace ConsultorioOdontologicoAPI.DTOs
{
    public class UsuarioDTO
    {
        [Required]
        [StringLength(50)]
        public string Username { get; set; }

        [Required]
        [StringLength(255)]
        public string Password { get; set; }

        [Required]
        [StringLength(20)]
        public string Rol { get; set; }

        public int? IdOdontologo { get; set; }
    }
}