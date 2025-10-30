using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ConsultorioOdontologicoAPI.Entities
{
    public class InformacionMedica
    {
        [Key]
        public int IdInformacion { get; set; }

        [Required]
        public int IdPaciente { get; set; }

        public string Alergias { get; set; }
        public string MedicacionesActuales { get; set; }
        public string AntecedentesMedicos { get; set; }
        public string AntecedentesOdontologicos { get; set; }
        public string Habitos { get; set; }
        public string ObservacionesMedicas { get; set; }

        [ForeignKey("IdPaciente")]
        public Paciente Paciente { get; set; } // AÑADIDO
    }
}