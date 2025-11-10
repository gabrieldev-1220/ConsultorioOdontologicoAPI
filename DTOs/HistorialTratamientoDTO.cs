namespace ConsultorioOdontologicoAPI.DTOs
{
    public class HistorialTratamientoDTO
    {
        public int IdHistorialTratamiento { get; set; }
        public int IdHistorial { get; set; }
        public int IdTratamiento { get; set; }
        public int Cantidad { get; set; }
        public decimal PrecioUnitario { get; set; }
    }
}
