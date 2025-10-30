using System;
using System.ComponentModel.DataAnnotations;

namespace ConsultorioOdontologicoAPI.DTOs
{
    public class PacienteDTO
    {
        public int IdPaciente { get; set; }

        [Required]
        [StringLength(50)]
        public string Nombre { get; set; }

        [Required]
        [StringLength(50)]
        public string Apellido { get; set; }

        [Required]
        [StringLength(15)]
        public string Dni { get; set; }

        [Required]
        public DateTime FechaNacimiento { get; set; }

        [Required]
        [StringLength(20)]
        public string Telefono { get; set; }

        [StringLength(100)]
        public string Email { get; set; }

        [StringLength(100)]
        public string Direccion { get; set; }

        public bool Activo { get; set; }
    }
}
