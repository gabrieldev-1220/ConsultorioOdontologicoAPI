using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ConsultorioOdontologicoAPI.Entities
{
    public class Tratamiento
    {
        [Key]
        public int IdTratamiento { get; set; }

        [Required]
        [StringLength(100)]
        public string Nombre { get; set; }

        public string Descripcion { get; set; }

        [Required]
        public decimal Precio { get; set; }

        // Relaciones
        public List<HistorialTratamiento> HistorialTratamientos { get; set; }
    }
}
