namespace AuditoriaRecepcion.DTOs.Auth
{
    public class ResetPasswordResponseDTO
    {
        public bool Success { get; set; }
        public string NuevaContrasena { get; set; }
        public string Mensaje { get; set; }
    }
}