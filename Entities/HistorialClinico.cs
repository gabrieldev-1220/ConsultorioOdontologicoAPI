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
        public DateTime Fecha { get; set; } = DateTime.Now;

        public string? MotivoConsulta { get; set; }
        public string? Diagnostico { get; set; }
        public string? Observacion { get; set; }

        // NO EXISTE EN BD → NO MAPEAR
        [NotMapped]
        public string? PlanTratamiento { get; set; }

        // NO EXISTE EN BD → NO MAPEAR
        [NotMapped]
        public string? Archivos { get; set; } // JSON: [{"nombre":"rx1.jpg","ruta":"..."}]

        // Relaciones
        [ForeignKey("IdPaciente")]
        public Paciente? Paciente { get; set; }

        [ForeignKey("IdOdontologo")]
        public Odontologo? Odontologo { get; set; }

        public List<HistorialTratamiento> HistorialTratamientos { get; set; } = new List<HistorialTratamiento>();
        public List<EvolucionVisita> Evoluciones { get; set; } = new List<EvolucionVisita>();
    }

    public class EvolucionVisita
    {
        [Key]
        public int IdEvolucion { get; set; }
        public int IdHistorial { get; set; }
        public DateTime Fecha { get; set; } = DateTime.Now;
        public string? Descripcion { get; set; }
        public string? Profesional { get; set; }

        [ForeignKey("IdHistorial")]
        public HistorialClinico? HistorialClinico { get; set; }
    }
}