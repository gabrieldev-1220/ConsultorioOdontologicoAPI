using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ConsultorioOdontologicoAPI.Entities
{
    public class Pago
    {
        [Key]
        public int IdPago { get; set; }

        [Required]
        public int IdPaciente { get; set; }

        [Required]
        public DateTime FechaPago { get; set; }

        [Required]
        public decimal Monto { get; set; }

        [Required]
        [StringLength(20)]
        public string MetodoPago { get; set; }

        public string Observaciones { get; set; }

        // Relaciones.
        [ForeignKey("IdPaciente")]
        public Paciente Paciente { get; set; }
    }
}
