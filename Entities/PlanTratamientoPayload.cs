namespace ConsultorioOdontologicoAPI.Entities
{
    public class PlanTratamientoPayload
    {
        public List<int> IdProcedimientos { get; set; } = new List<int>();
        public string? Observaciones { get; set; }
    }
}