using System;

namespace ConsultorioOdontologicoAPI.DTOs
{
    public class HistorialClinicoDTO
    {
        public int IdHistorial { get; set; }
        public int IdPaciente { get; set; }
        public int IdOdontologo { get; set; }
        public DateTime Fecha { get; set; }
        public string MotivoConsulta { get; set; }
        public string Diagnostico { get; set; }
        public string Observaciones { get; set; }
    }
}
