using System;

namespace ConsultorioOdontologicoAPI.DTOs
{
    public class PagoDTO
    {
        public int IdPago { get; set; }
        public int IdPaciente { get; set; }
        public DateTime FechaPago { get; set; }
        public decimal Monto { get; set; }
        public string MetodoPago { get; set; }
        public string Observaciones { get; set; }
    }
}
