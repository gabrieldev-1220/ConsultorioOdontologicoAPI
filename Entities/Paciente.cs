using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Collections.Generic;

namespace ConsultorioOdontologicoAPI.Entities
{
    public class Paciente
    {
        [Key]
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

        public DateTime FechaRegistro { get; set; } = DateTime.Now;

        public bool Activo { get; set; } = true;

        // Relaciones
        public List<Turno> Turnos { get; set; }
        public List<HistorialClinico> Historiales { get; set; }
        public List<Pago> Pagos { get; set; }
        public List<PlanTratamiento> PlanesTratamiento { get; set; }
    }
}