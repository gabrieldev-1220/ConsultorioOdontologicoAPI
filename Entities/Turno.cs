using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ConsultorioOdontologicoAPI.Entities
{
    public class Turno
    {
        [Key]
        public int IdTurno { get; set; }

        [Required]
        public int IdPaciente { get; set; }

        [Required]
        public int IdOdontologo { get; set; }

        [Required]
        public DateTime FechaHora { get; set; }

        [Required]
        [StringLength(20)]
        public string Estado { get; set; } = "pendiente";

        public string Observaciones { get; set; }

        // Relaciones
        [ForeignKey("IdPaciente")]
        public Paciente Paciente { get; set; }

        [ForeignKey("IdOdontologo")]
        public Odontologo Odontologo { get; set; }
    }
}