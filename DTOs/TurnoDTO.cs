using System;

namespace ConsultorioOdontologicoAPI.DTOs
{
    public class TurnoDTO
    {
        public int IdTurno { get; set; }
        public int IdPaciente { get; set; }
        public int IdOdontologo { get; set; }
        public DateTime FechaHora { get; set; }
        public string Estado { get; set; }
        public string Observaciones { get; set; }
    }
}
