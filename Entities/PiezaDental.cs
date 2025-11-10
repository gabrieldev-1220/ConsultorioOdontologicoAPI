namespace ConsultorioOdontologicoAPI.Entities
{
    public class PiezaDental
    {
        public int IdPieza { get; set; }
        public int IdPaciente { get; set; }
        public int NumeroPieza { get; set; }
        public string? Color { get; set; }
        public string? Estado { get; set; }
        public string? Observaciones { get; set; }
        public DateTime FechaActualizacion { get; set; }
    }
}