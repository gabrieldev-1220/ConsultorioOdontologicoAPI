namespace ConsultorioOdontologicoAPI.Entities
{
    public class PlanTratamiento
    {
        public int IdPlanTratamiento { get; set; }
        public int IdPaciente { get; set; }
        public int IdProcedimiento { get; set; }
        public DateTime FechaPlan { get; set; }
        public string Estado { get; set; } = "pendiente";
        public string? Observaciones { get; set; }

        // Relaciones
        public Paciente Paciente { get; set; } = null!;
        public Procedimiento Procedimiento { get; set; } = null!;
    }
}