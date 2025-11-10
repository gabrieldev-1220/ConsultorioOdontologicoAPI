using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ConsultorioOdontologicoAPI.Entities
{
    public class Bitacora
    {
        [Key]
        public int IdBitacora { get; set; }

        [Required]
        public int IdUsuario { get; set; }

        [Required]
        [StringLength(100)]
        public string Accion {  get; set; }

        public DateTime Fecha { get; set; } = DateTime.Now;

        public string Detalles { get; set; }

        // Relaciones.
        [ForeignKey("IdUsuario")]
        public Usuario Usuario { get; set; }
    }
}
