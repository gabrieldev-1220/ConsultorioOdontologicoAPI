using System.ComponentModel.DataAnnotations;

namespace ConsultorioOdontologicoAPI.DTOs
{
    public class OdontologoDTO
    {
        public int IdOdontologo { get; set; }

        [Required]
        [StringLength(50)]
        public string Nombre { get; set; }

        [Required]
        [StringLength(50)]
        public string Apellido { get; set; }

        [Required]
        [StringLength(30)]
        public string Matricula { get; set; }

        [Required]
        [StringLength(20)]
        public string Telefono { get; set; }

        [StringLength(100)]
        public string Email { get; set; }

        [StringLength(50)]
        public string Especialidad { get; set; }
    }
}
