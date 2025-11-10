namespace ConsultorioOdontologicoAPI.DTOs
{
    public class LoginResponseDTO
    {
        public string Token { get; set; }
        public string Rol { get; set; }
        public int? IdOdontologo { get; set; }
        public string FullName { get; set; }
        public string Matricula { get; set; }
    }
}