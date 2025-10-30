using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ConsultorioOdontologicoAPI.Entities
{
    public class HistorialClinico
    {
        [Key]
        public int IdHistorial { get; set; }

        [Required]
        public int IdPaciente { get; set; }

        [Required]
        public int IdOdontologo { get; set; }

        [Required]
        public DateTime Fecha { get; set; }

        public string MotivoConsulta { get; set; }
        public string Diagnostico { get; set; }
        public string Observacion { get; set; }

        // Relaciones
        [ForeignKey("IdPaciente")]
        public Paciente Paciente { get; set; }

        [ForeignKey("IdOdontologo")]
        public Odontologo Odontologo { get; set; }

        public List<HistorialTratamiento> HistorialTratamientos { get; set; }
    }
}