using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ConsultorioOdontologicoAPI.Entities
{
    public class HistorialTratamiento
    {
        [Key]
        public int IdHistorialTratamiento { get; set; }

        [Required]
        public int IdHistorial { get; set; }

        [Required]
        public int IdTratamiento { get; set; }

        public int Cantidad { get; set; }

        [Required]
        public decimal PrecioUnitario { get; set; }

        // Relaciones.
        [ForeignKey("IdHistorial")]
        public HistorialClinico HistorialClinico { get; set; }

        [ForeignKey("IdTratamiento")]
        public Tratamiento Tratamiento { get; set; }
    }
}
