namespace ConsultorioOdontologicoAPI.Entities
{
    public class Procedimiento
    {
        public int IdProcedimiento { get; set; }
        public string Nombre { get; set; } = null!;
        public string Categoria { get; set; } = null!;
        public decimal Costo { get; set; }
        public int DuracionMinutos { get; set; }
        public bool Activo { get; set; } = true;
    }
}