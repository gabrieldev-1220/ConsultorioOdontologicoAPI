using System;

namespace ConsultorioOdontologicoAPI.DTOs
{
    public class BitacoraDTO
    {
        public int IdBitacora { get; set; }
        public int IdUsuario { get; set; }
        public string Accion { get; set; }
        public DateTime Fecha { get; set; }
        public string Detalles { get; set; }
    }
}
