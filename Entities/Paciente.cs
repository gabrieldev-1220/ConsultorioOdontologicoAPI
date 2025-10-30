using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ConsultorioOdontologicoAPI.Entities
{
    public class Paciente
    {
        [Key]
        public int IdPaciente { get; set; }
        public string Nombre { get; set; }
        public string Apellido { get; set; }
        public string Dni { get; set; }
        public DateTime FechaNacimiento { get; set; }
        public string Telefono { get; set; }
        public string Email { get; set; }
        public string Direccion { get; set; }
        public DateTime FechaRegistro { get; set; }
        public bool Activo { get; set; }

        // Relaciones
        public List<Turno> Turnos { get; set; }
        public List<HistorialClinico> Historiales { get; set; }
        public List<Pago> Pagos { get; set; }
        public List<PlanTratamiento> PlanesTratamiento { get; set; }

        // 1 paciente → muchas informaciones médicas
        public List<InformacionMedica> InformacionMedica { get; set; } = new List<InformacionMedica>();
    }
}