using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ConsultorioOdontologicoAPI.Entities
{
    public class Usuario
    {
        [Key]
        public int IdUsuario { get; set; }

        [Required]
        [StringLength(50)]
        public string Username { get; set; }

        [Required]
        [StringLength(255)]
        public string PasswordHash { get; set; }

        [Required]
        [StringLength(20)]
        public string Rol { get; set; }

        public int? IdOdontologo { get; set; }

        [ForeignKey("IdOdontologo")]
        public Odontologo Odontologo { get; set; }

        public List<Bitacora> Bitacoras { get; set; }
    }
}